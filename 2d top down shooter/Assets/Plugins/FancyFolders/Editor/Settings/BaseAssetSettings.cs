using System;
using FancyFolders.Editor.FileSystem;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.Settings
{
    [Serializable]
    public abstract class BaseAssetSettings
    {
#pragma warning disable CS0649  // Field unused (it's used by serialization)
        [SerializeField, UsedImplicitly] private string _formatString = "";
#pragma warning restore CS0649
        [NotNull] public string FormatString
        {
            get
            {
                return !string.IsNullOrEmpty(_formatString)
                    ? _formatString
                    : DefaultFormatString() ?? "";
            }
            set
            {
                if (_formatString != value)
                {
                    _formatString = value;
                    _version++;

                    Dirty();
                }
            }
        }

        [NotNull] protected internal string RawFormatString => _formatString;

        [SerializeField, UsedImplicitly] private AssetGuid _guid;
        public AssetGuid Guid => _guid;

        private uint _version;
        public uint Version => unchecked(_version + FancyFoldersSettings.Instance.Version);

        protected BaseAssetSettings()
        {
            //Used by serialization
        }

        protected BaseAssetSettings(AssetGuid guid)
        {
            _guid = guid;
            _version = 1;
        }

        protected abstract string DefaultFormatString();

        protected static void Dirty()
        {
            EditorUtility.SetDirty(FancyFoldersSettings.Instance);
        }

        internal void CopyTo([NotNull] BaseAssetSettings settings)
        {
            settings._formatString = _formatString;
            unchecked { settings._version++; }
        }
    }
}
