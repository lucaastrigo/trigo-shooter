using System.IO;
using FancyFolders.Editor.Renderer.StringFormatting;
using FancyFolders.Editor.Renderer.Textures;
using FancyFolders.Editor.Settings;

namespace FancyFolders.Editor.FileSystem
{
    public class FileItem
        : BaseFileSystemItem<FileSettings, FileInfo>
    {
        public override bool IsFile => true;

        private FileType? _type;
        public FileType Type
        {
            get
            {
                if (!_type.HasValue)
                    _type = Extension.FileTypeFromExtension();

                return _type.Value;
            }
        }

        private string _extension;
        [NotNull] public string Extension
        {
            get
            {
                if (_extension == null)
                    _extension = string.IsNullOrEmpty(FileSystemInfo.Extension) ? "" : FileSystemInfo.Extension.Substring(1);
                return _extension;
            }
        }

        public long Size
        {
            get { return FileSystemInfo.Length; }
        }

        public FileItem(AssetGuid guid, [NotNull] ProjectFileSystem fs, [NotNull] string assetPath, [NotNull] FileSettings settings)
            : base(guid, fs, settings, assetPath)
        {
        }

        public override void ItemMoved()
        {
            base.ItemMoved();

            _extension = null;
            _type = null;
        }

        protected override FileInfo GetFileSystemInfo()
        {
            return new FileInfo(AssetPath);
        }

        protected override string RenderToString()
        {
            return new FileInfoFormatter(Settings.FormatString, this).ToString();
        }

        protected override string GetNameFromPath(string path)
        {
            return Path.GetFileNameWithoutExtension(path) ?? "";
        }

        protected override int GetIcons(ColoredTexture[] icons)
        {
            if (icons.Length < 1)
                return 0;

            if (!FancyFoldersSettings.Instance.ShowStatusIcon)
                return 0;

            icons[0] = StatusIcon();
            return 1;
        }
    }
}
