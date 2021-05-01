using System;
using System.IO;
using FancyFolders.Editor.Compiler;
using FancyFolders.Editor.Renderer.Textures;
using FancyFolders.Editor.Settings;
using FancyFolders.Editor.VersionControl;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.FileSystem
{
    public abstract class BaseFileSystemItem<TSettings, TFileSystemInfo>
        : IFileSystemItem<TSettings>
        where TSettings : BaseAssetSettings
        where TFileSystemInfo : FileSystemInfo
    {
        private static readonly ColoredTexture[] IconsTemp = new ColoredTexture[128];

        public abstract bool IsFile { get; }

        public AssetGuid Guid { get; }

        private FsInfoExt _info;
        private bool _fsInfoDueRefresh;
        protected TFileSystemInfo FileSystemInfo
        {
            get
            {
                //Create a new fileinfo object if the path has changed, else refresh it if necessary
                if (_info.OriginalPath != AssetPath)
                    _info = new FsInfoExt(AssetPath, GetFileSystemInfo());
                else if (_fsInfoDueRefresh)
                    _info.Info.Refresh();
                _fsInfoDueRefresh = true;

                return _info.Info;
            }
        }

        public string FullPath => FileSystemInfo.FullName;

        private string _assetPath;
        public string AssetPath
        {
            get
            {
                if (_assetPath == null)
                    _assetPath = AssetDatabase.GUIDToAssetPath(Guid.Guid);
                return _assetPath;
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                if (_name == null)
                    _name = GetNameFromPath(AssetPath);
                Debug.Assert(_name != null);
                return _name;
            }
        }

        private bool _hasError;
        private uint _errorEpoch;
        public bool HasCompilerError
        {
            get
            {
                var cm = CompilerMonitor.Instance;
                if (_errorEpoch != cm.Epoch)
                {
                    _hasError = cm.HasError(Guid);
                    _errorEpoch = cm.Epoch;
                }

                return _hasError;
            }
        }

        private VcsStatusHandle _vcsStatus;
        public VcsStatus VcsStatus
        {
            get
            {
                if (_vcsStatus == null)
                    _vcsStatus = VcsManager.Instance.GetStatusHandle(FullPath);
                return _vcsStatus.Status;
            }
        }

        private DirectoryItem _parent;
        public DirectoryItem Parent
        {
            get
            {
                if (_parent == null)
                {
                    var path = Path.GetDirectoryName(AssetPath);
                    var guid = new AssetGuid(AssetDatabase.AssetPathToGUID(path));

                    FileItem _;
                    FileSystem.GetItem(guid, out _, out _parent);
                }
                return _parent;
            }
        }

        private float? _namePixelWidth;
        public float NamePixelWidth
        {
            get
            {
                if (!_namePixelWidth.HasValue)
                {
                    var labelStyle = AssetPath == "Assets" ? EditorStyles.boldLabel : EditorStyles.label;
                    _namePixelWidth = labelStyle.CalcSize(new GUIContent(Name)).x;
                }
                return _namePixelWidth.Value;
            }
        }

        protected ProjectFileSystem FileSystem { get; }

        public TSettings Settings { get; }

        private uint _settingsVersion;
        private string _toString;

        protected BaseFileSystemItem(AssetGuid guid, [NotNull] ProjectFileSystem fs, TSettings settings, [CanBeNull] string assetPath = null)
        {
            _assetPath = assetPath ?? throw new ArgumentNullException(nameof(assetPath));
            FileSystem = fs ?? throw new ArgumentNullException(nameof(fs));

            Guid = guid;
            Settings = settings;
        }

        [NotNull] protected abstract string GetNameFromPath(string path);

        [NotNull] protected abstract TFileSystemInfo GetFileSystemInfo();

        /// <summary>
        /// This item was changed
        /// </summary>
        public virtual void ItemModified()
        {
            _toString = null;

            _fsInfoDueRefresh = true;
        }

        /// <summary>
        /// The path of this item changed
        /// </summary>
        public virtual void ItemMoved()
        {
            _assetPath = null;
            _name = null;
            _parent = null;
            _namePixelWidth = null;
            _toString = null;

            _fsInfoDueRefresh = true;
        }

        /// <summary>
        /// Clear caches of data generated from the settings iff the version has changed
        /// </summary>
        private void CheckSettingsCaches()
        {
            if (_settingsVersion == Settings.Version)
                return;

            _toString = null;
            _settingsVersion = 0;
        }

        [NotNull] protected abstract string RenderToString();

        public override string ToString()
        {
            CheckSettingsCaches();

            if (_toString == null)
                _toString = RenderToString();
            return _toString;
        }

        public virtual void DrawDetails(Rect allArea, Rect unityTextArea, Rect unityIconArea, GUIStyle baseStyle)
        {
            //Draw lines above and below item
            if (FancyFoldersSettings.Instance.ShowSeparators)
            {
                var separatorColor = FancyFoldersSettings.Instance.SeparatorColor;
                GUI.DrawTexture(new Rect(allArea.xMin, allArea.yMin, allArea.width, 1), TextureResources.Instance.WhitePixel, ScaleMode.StretchToFill, false, 0, separatorColor, 0, 0);
                GUI.DrawTexture(new Rect(allArea.xMin, allArea.yMax, allArea.width, 1), TextureResources.Instance.WhitePixel, ScaleMode.StretchToFill, false, 0, separatorColor, 0, 0);
            }

            //Draw background icon
            var bgIconOffset = TextureResources.IsLowDpi ? new Vector2(0, 0f) : new Vector2(0, 1f);
            var bgIconArea = new Rect(unityIconArea.position + bgIconOffset, unityIconArea.size);
            DrawBackgroundIcon(false, bgIconArea);

            //Place detail text to the right of the Unity text. We clip back into the Unity text area by some amount to get closer the existing text
            var detailsArea = new Rect(
                unityTextArea.xMax,
                unityTextArea.yMin,
                allArea.xMax - unityTextArea.xMax,
                unityTextArea.height
            );

            //Modify base style with colour
            var style = new GUIStyle(baseStyle) {
                normal = {
                    textColor = FancyFoldersSettings.Instance.DetailsTextColor
                }
            };

            //Draw text string over the top of the existing string
            GUI.Label(detailsArea, ToString(), style);

            //Draw icons to the right of the text area
            var iconCount = Math.Min(FancyFoldersSettings.Instance.MaxIconCount, GetIcons(IconsTemp));
            var iconArea = new Rect(allArea.xMax - allArea.height * iconCount, allArea.yMin, allArea.height * iconCount, allArea.height);
            DrawIcons(iconArea, new ArraySegment<ColoredTexture>(IconsTemp, 0, iconCount));
        }

        public virtual void DrawLarge(Rect allArea, bool isSelected)
        {
            //Draw over the main background icon
            var bgSize = Math.Min(allArea.width, allArea.height);
            var bg = new Rect(allArea.xMin, allArea.yMin, bgSize, bgSize);
            DrawBackgroundIcon(true, bg);

            //Calculate how many icons we can possibly show show based on min size (15px)
            var maxCount = FancyFoldersSettings.Instance.MaxIconCount;
            const int minSize = 10;
            var maxIconCount = Math.Min(maxCount, (int)(allArea.width / minSize));
            var iconSize = allArea.width / maxIconCount;

            //Draw an outline box around selected items
            if (isSelected && FancyFoldersSettings.Instance.ShowSelectionOutline)
            {
                var selectionOutlineColor = FancyFoldersSettings.Instance.SelectionOutlineColor;
                var margin = new Vector2(6, 6);
                var offset = new Vector2(-1, 0);
                GUI.DrawTexture(new Rect(allArea.min - margin + offset, allArea.size + margin * 2), TextureResources.Instance.Box96x110, ScaleMode.StretchToFill, true, 0, selectionOutlineColor, 0, 0);
            }

            //Find out how many icons we want to show
            var availableIconCount = GetIcons(IconsTemp);

            //Draw as many icons as we can
            var iconCount = Math.Min(availableIconCount, maxIconCount);
            if (iconCount < 0)
                return;
            var iconStrip = new Rect(allArea.xMax + iconSize * 0.25f - iconSize * iconCount, allArea.yMin - iconSize * 0.25f, iconSize * iconCount, iconSize);
            DrawIcons(iconStrip, new ArraySegment<ColoredTexture>(IconsTemp, 0, iconCount));
        }

        private void DrawBackgroundIcon(bool large, Rect backgroundIconArea)
        {
            GetBackgroundIcon(large, out var bg, out var fg);

            if (bg != null && bg.Value.Texture != null)
                GUI.DrawTexture(backgroundIconArea, bg.Value.Texture, ScaleMode.ScaleToFit, true, 0, bg.Value.Color, 0, 0);

            if (fg != null && fg.Value.Texture != null)
                GUI.DrawTexture(backgroundIconArea, fg.Value.Texture, ScaleMode.ScaleToFit, true, 0, fg.Value.Color, 0, 0);
        }

        private void DrawIcons(Rect area, ArraySegment<ColoredTexture> icons)
        {
            if (icons.Array == null)
                return;

            var width = area.width / icons.Count;

            //Draw icons from right to left
            for (var i = 0; i < icons.Count; i++)
            {
                var icon = icons.Array[i + icons.Offset];
                if (icon.Texture == null || icon.Color.a <= 0)
                    continue;

                var iconBox = new Rect(area.xMax - width * (i + 1), area.yMin, width, area.height);
                GUI.DrawTexture(iconBox, icon.Texture, ScaleMode.ScaleToFit, true, 0, icon.Color, 0, 0);
            }
        }

        protected virtual void GetBackgroundIcon(bool large, out ColoredTexture? bg, out ColoredTexture? fg)
        {
            bg = null;
            fg = null;
        }

        protected virtual int GetIcons([NotNull] ColoredTexture[] icons)
        {
            return 0;
        }

        protected ColoredTexture StatusIcon()
        {
            if (HasCompilerError)
                return new ColoredTexture(FancyFoldersSettings.Instance.IconSet?.GetMiniIcon(Editor.Settings.Icons.MiniIconType.Error)?.Icon, Color.white);
            
            if (VcsStatus.HasFlag(VcsStatus.Conflicted))
                return new ColoredTexture(FancyFoldersSettings.Instance.IconSet?.GetMiniIcon(Editor.Settings.Icons.MiniIconType.VcsConflict)?.Icon, Color.red);

            if (VcsStatus.HasFlag(VcsStatus.Modified) || VcsStatus.HasFlag(VcsStatus.Renamed))
                return new ColoredTexture(FancyFoldersSettings.Instance.IconSet?.GetMiniIcon(Editor.Settings.Icons.MiniIconType.VcsModified)?.Icon, Color.yellow);

            if (VcsStatus.HasFlag(VcsStatus.Added))
                return new ColoredTexture(FancyFoldersSettings.Instance.IconSet?.GetMiniIcon(Editor.Settings.Icons.MiniIconType.VcsAdded)?.Icon, Color.green);

            return new ColoredTexture(TextureResources.Instance.ErrorGrey, new Color(1, 1, 1, 0.1f));
        }

        private struct FsInfoExt
        {
            public readonly TFileSystemInfo Info;
            public readonly string OriginalPath;

            public FsInfoExt([NotNull] string path, TFileSystemInfo info)
            {
                OriginalPath = path;
                Info = info;
            }
        }
    }
}
