using System;
using FancyFolders.Editor.FileSystem;

namespace FancyFolders.Editor.Settings
{
    [Serializable]
    public class FileSettings
        : BaseAssetSettings
    {
        private FileSettings()
        {
            //used by serialization
        }

        public FileSettings(AssetGuid guid)
            : base(guid)
        {
        }

        [NotNull] public static FileSettings Create(AssetGuid guid)
        {
            return new FileSettings(guid);
        }

        protected override string DefaultFormatString()
        {
            return FancyFoldersSettings.Instance.DefaultFileFormatString;
        }

        public void CopyTo([NotNull] FileSettings settings)
        {
            base.CopyTo(settings);
        }
    }
}
