using System;
using System.Collections;
using System.Collections.Generic;
using FancyFolders.Editor.FileSystem;
using FancyFolders.Editor.Settings.Icons;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.Settings
{
    [Serializable]
    public class FancyFoldersSettings
        : ScriptableObject, IEnumerable<BaseAssetSettings>
    {
        internal const string SettingsPath = "Assets/Plugins/FancyFolders/Fancy Folders Settings.asset";

        private static FancyFoldersSettings _instance;
        [NotNull] public static FancyFoldersSettings Instance
        {
            get
            {
                //If the asset settings file is missing, create a new instance now
                if (AssetDatabase.AssetPathToGUID(SettingsPath) == null || _instance == null)
                    Reload();

                if (_instance == null)
                    throw new InvalidOperationException("Failed to load Settings file");

                return _instance;
            }
        }

        public string DefaultFileFormatString
        {
            get => _defaultFileFormatString;
            set => SetValueClass(value, ref _defaultFileFormatString);
        }

        public string DefaultDirFormatString
        {
            get => _defaultDirFormatString;
            set => SetValueClass(value, ref _defaultDirFormatString);
        }

        public Color DetailsTextColor
        {
            get => _useCustomTextColor ? _detailsTextColor : (EditorGUIUtility.isProSkin ? new Color(0.67f, 0.67f, 0.67f, 0.5f) : new Color(0.33f, 0.33f, 0.33f, 0.5f));
            set => SetValueStruct(value, ref _detailsTextColor);
        }
        public bool UseCustomTextColor
        {
            get => _useCustomTextColor;
            set => SetValueStruct(value, ref _useCustomTextColor);
        }

        public bool ShowSeparators
        {
            get => !_hideSeparators;
            set => SetValueStruct(!value, ref _hideSeparators);
        }
        public Color SeparatorColor
        {
            get => _useCustomSeparatorColor ? _separatorColor : (EditorGUIUtility.isProSkin ? new Color(0.27f, 0.27f, 0.27f) : new Color(0.73f, 0.73f, 0.73f));
            set => SetValueStruct(value, ref _separatorColor);
        }
        public bool UseCustomSeparatorColor
        {
            get => _useCustomSeparatorColor;
            set => SetValueStruct(value, ref _useCustomSeparatorColor);
        }

        public int MaxIconCount
        {
            get => _maxIconCount;
            set => SetValueStruct(value, ref _maxIconCount);
        }

        public bool ShowStatusIcon
        {
            get => _showStatusIcon;
            set => SetValueStruct(value, ref _showStatusIcon);
        }

        public bool ShowSelectionOutline
        {
            get => !_selectionOutline;
            set => SetValueStruct(!value, ref _selectionOutline);
        }
        public Color SelectionOutlineColor
        {
            get => _useCustomSelectionOutlineColor ? _selectionOutlineColor : (EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.1f) : new Color(0f, 0f, 0f, 0.1f));
            set => SetValueStruct(value, ref _selectionOutlineColor);
        }
        public bool UseCustomSelectionOutlineColor
        {
            get => _useCustomSelectionOutlineColor;
            set => SetValueStruct(value, ref _useCustomSelectionOutlineColor);
        }

        [CanBeNull] public IconSet IconSet
        {
            get
            {
                if (_icons == null)
                    _icons = AssetDatabase.LoadAssetAtPath<IconSet>(IconSet.DefaultPath);
                return _icons;
            }
            set => SetValueClass(value, ref _icons);
        }

        internal uint Version { get; private set; }

        // ReSharper disable FieldCanBeMadeReadOnly.Local
        [SerializeField] private IconSet _icons;

        [SerializeField] private bool _hideSeparators;
        [SerializeField] private bool _useCustomSeparatorColor;
        [SerializeField] private Color _separatorColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        [SerializeField] private bool _useCustomTextColor;
        [SerializeField] private Color _detailsTextColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        [SerializeField] private string _defaultFileFormatString = "";
        [SerializeField] private string _defaultDirFormatString = "";

        [SerializeField] private List<FileSettings> _fileSettings = new List<FileSettings>();
        [SerializeField] private List<DirectorySettings> _dirSettings = new List<DirectorySettings>();

        [SerializeField] private int _maxIconCount = 7;

        [SerializeField] private bool _showStatusIcon = true;

        [SerializeField] private bool _selectionOutline = true;
        [SerializeField] private bool _useCustomSelectionOutlineColor;
        [SerializeField] private Color _selectionOutlineColor = new Color(1f, 1f, 15f, 0.1f);
        // ReSharper restore FieldCanBeMadeReadOnly.Local

        internal static FancyFoldersSettings Reload()
        {
            var asset = AssetDatabase.LoadAssetAtPath<FancyFoldersSettings>(SettingsPath);
            if (asset == null)
            {
                asset = CreateInstance<FancyFoldersSettings>();
                AssetDatabase.CreateAsset(asset, SettingsPath);
                AssetDatabase.SaveAssets();
            }

            _instance = asset;

            return _instance;
        }

        internal bool Trim()
        {
            var count = _fileSettings.Count + _dirSettings.Count;

            // Remove settings for files which no longer exist
            for (var i = _fileSettings.Count - 1; i >= 0; i--)
                if (string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(_fileSettings[i].Guid.Guid)))
                    _fileSettings.RemoveAt(i);

            // Remove settings for directories which no longer exist
            for (var i = _dirSettings.Count - 1; i >= 0; i--)
                if (string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(_dirSettings[i].Guid.Guid)))
                    _dirSettings.RemoveAt(i);

            return _fileSettings.Count + _dirSettings.Count != count;
        }

        private void SetValueStruct<T>(T value, ref T field)
            where T : struct, IEquatable<T>
        {
            if (!value.Equals(field))
            {
                field = value;
                unchecked { Version++; }
                Main.ForceRedraw(true);
            }
        }

        private void SetValueClass<T>(T value, ref T field, IEqualityComparer<T> comparer = null)
            where T : class
        {
            comparer = comparer ?? EqualityComparer<T>.Default;

            if (!comparer.Equals(value, field))
            {
                field = value;
                unchecked { Version++; }
                Main.ForceRedraw(true);
            }
        }

        private T GetOrCreate<T>(AssetGuid guid, [NotNull] IList<T> storage, Func<AssetGuid, T> create)
            where T : BaseAssetSettings
        {
            //Find the item
            T item;
            var index = FindByGuid(storage, guid);
            if (index < 0)
            {
                item = create(guid);
                storage.Insert(~index, item);
                EditorUtility.SetDirty(Instance);
            }
            else
            {
                item = storage[index];
            }

            return item;
        }

        private static int FindByGuid<T>([NotNull] IList<T> items, AssetGuid guid)
            where T : BaseAssetSettings
        {
            var maxIndex = items.Count - 1;
            var minIndex = 0;
            while (maxIndex >= minIndex)
            {
                var mid = minIndex + ((maxIndex - minIndex) / 2);
                var c = string.Compare(guid.Guid, items[mid].Guid.Guid, StringComparison.Ordinal);

                //Found it!
                if (c == 0)
                    return mid;

                if (c > 0)
                    minIndex = mid + 1;
                else
                    maxIndex = mid - 1;
            }

            return ~minIndex;
        }

        [NotNull] public FileSettings GetFileAssetSettings(AssetGuid guid)
        {
            return GetOrCreate(guid, _fileSettings, FileSettings.Create);
        }

        [NotNull] public DirectorySettings GetDirectoryAssetSettings(AssetGuid guid)
        {
            return GetOrCreate(guid, _dirSettings, DirectorySettings.Create);
        }

        public IEnumerator<BaseAssetSettings> GetEnumerator()
        {
            foreach (var dir in _dirSettings)
                yield return dir;
            foreach (var file in _fileSettings)
                yield return file;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
