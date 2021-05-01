using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FancyFolders.Editor.Renderer.StringFormatting;
using FancyFolders.Editor.Renderer.Textures;
using FancyFolders.Editor.Settings;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.FileSystem
{
    /// <summary>
    /// Represents a directory in the project.
    /// Caches various information about the item for faster rendering of extra stats
    /// </summary>
    public class DirectoryItem
        : BaseFileSystemItem<DirectorySettings, DirectoryInfo>
    {
        public override bool IsFile => false;

        private long? _size;
        public long Size
        {
            get
            {
                if (!_size.HasValue)
                    RefreshHierarchyData();
                // ReSharper disable once PossibleInvalidOperationException (Justification: set to not null by refresh call)
                return _size.Value;
            }
        }

        private int? _itemsCount;
        public int ItemsCount
        {
            get
            {
                if (!_itemsCount.HasValue)
                    RefreshHierarchyData();
                // ReSharper disable once PossibleInvalidOperationException (Justification: set to not null by refresh call)
                return _itemsCount.Value;
            }
        }

        private int? _directFileCount;
        public int DirectFileCount
        {
            get
            {
                if (!_directFileCount.HasValue)
                    RefreshHierarchyData();
                // ReSharper disable once PossibleInvalidOperationException (Justification: set to not null by refresh call)
                return _directFileCount.Value;
            }
        }

        private bool _typeCountsInitialized;
        private readonly int[] _typeCounts = new int[Enum.GetNames(typeof(FileType)).Length];
        private FileType[] _orderedFileTypes;
        public FileTypeCountAccessor FileTypeCount => new FileTypeCountAccessor(this);

        /// <summary>
        /// Get the immediate children of this node
        /// </summary>
        [NotNull, ItemNotNull] internal IEnumerable<DirectoryItem> Children =>
            from subpath in AssetDatabase.GetSubFolders(AssetPath)
            let subguid = new AssetGuid(AssetDatabase.AssetPathToGUID(subpath))
            let subdir = FileSystem.GetDirectory(subguid)
            where subdir != null
            select subdir;

        public DirectoryItem(AssetGuid guid, [NotNull] ProjectFileSystem fs, [CanBeNull] string assetPath, [NotNull] DirectorySettings settings)
            : base(guid, fs, settings, assetPath)
        {
        }

        public override void ItemModified()
        {
            base.ItemModified();

            _size = null;
            _itemsCount = null;
        }

        protected override DirectoryInfo GetFileSystemInfo()
        {
            return new DirectoryInfo(AssetPath);
        }

        protected override string RenderToString()
        {
            return new DirectoryInfoFormatter(Settings.FormatString, this).ToString();
        }

        protected override string GetNameFromPath(string path)
        {
            return Path.GetFileName(path) ?? "";
        }

        private void RefreshHierarchyData()
        {
            _size = 0;
            _itemsCount = 0;
            _directFileCount = 0;
            Array.Clear(_typeCounts, 0, _typeCounts.Length);

            //Accumulate info from child files
            try
            {
                foreach (var file in Directory.GetFiles(FullPath))
                {
                    //Try to convert the full path to an in-project asset path
                    var assetPath = FileSystem.FullPathToAssetPath(file);
                    if (assetPath == null)
                        continue;

                    //File is in the project fle system, but it may not be in the project itself. Try to convert to GUID to check
                    var guid = AssetDatabase.AssetPathToGUID(assetPath);
                    if (guid == null)
                        continue;

                    //Get the item from the cache
                    var fileItem = FileSystem.GetFile(new AssetGuid(guid));
                    if (fileItem == null)
                        continue;

                    //Increase the counts now that we've found the item
                    _directFileCount++;
                    _itemsCount++;
                    _size += fileItem.Size;

                    //Increase type count
                    var i = (int)fileItem.Type;
                    if (i >= 0 && i <= _typeCounts.Length)
                        _typeCounts[i]++;
                }

                //Accumulate info from child directories
                foreach (var directory in Directory.GetDirectories(FullPath))
                {
                    //Try to convert the full path to an in-project asset path
                    var assetPath = FileSystem.FullPathToAssetPath(directory);
                    if (assetPath == null)
                        continue;

                    //File is in the project fle system, but it may not be in the project itself. Try to convert to GUID to check
                    var guid = AssetDatabase.AssetPathToGUID(assetPath);
                    if (guid == null)
                        continue;

                    //Get the item from the cache
                    var dirItem = FileSystem.GetDirectory(new AssetGuid(guid));
                    if (dirItem == null)
                        continue;

                    //Increase the counts now that we've found the item
                    _itemsCount += (dirItem.ItemsCount + 1);
                    _size += dirItem.Size;

                    //Increase type counts
                    if (!dirItem._typeCountsInitialized)
                        dirItem.RefreshHierarchyData();
                    for (var j = 0; j < dirItem._typeCounts.Length; j++)
                        _typeCounts[j] += dirItem._typeCounts[j];
                }
            }
            catch (DirectoryNotFoundException)
            {
                //This can happen if a directory is moved but this item hasn't been deleted yet
            }

            //We've initialized counts of file types in subtree
            _typeCountsInitialized = true;

            //Calculate icons
            _orderedFileTypes = _typeCounts
                .Select((count, index) => new {type = (FileType)index, count})
                .Where(a => a.count > 0 && a.type != FileType.None)
                .OrderByDescending(a => a.count)
                .Select(a => a.type)
                .ToArray();
        }

        private int GetTypeCount(FileType type)
        {
            var index = (int)type;
            if (index < 0 || index > _typeCounts.Length)
                return 0;

            if (!_typeCountsInitialized)
                RefreshHierarchyData();

            return _typeCounts[index];
        }

        protected override void GetBackgroundIcon(bool large, out ColoredTexture? bg, out ColoredTexture? fg)
        {
            // If no icon set is configured early exit
            var iconSet = FancyFoldersSettings.Instance.IconSet;
            if (iconSet == null)
            {
                fg = null;
                bg = null;
                return;
            }

            // Try to return custom icon (if it exists)
            var maybeFg = iconSet.GetIcon(Settings.OverlayIcon, Settings.CustomIconOverlayId);
            var fgIcon = maybeFg == null ? null : (large ? maybeFg.Large : maybeFg.Small);
            fg = new ColoredTexture(fgIcon, Settings.ForegroundIconColor);

            // Work out which background icon to use.
            //todo: make sure this doesn't affect large icons
            if (DirectFileCount == 0 && !large)
                bg = new ColoredTexture(TextureResources.Instance.FolderLargeEmpty, Settings.BackgroundIconColor);
            else
                bg = new ColoredTexture(TextureResources.Instance.FolderLarge, Settings.BackgroundIconColor);

        }

        protected override int GetIcons(ColoredTexture[] icons)
        {
            if (!_typeCountsInitialized)
                RefreshHierarchyData();

            // If no icon set is configured early exit
            var iconSet = FancyFoldersSettings.Instance.IconSet;
            if (iconSet == null)
                return 0;

            var iconsUsed = 0;

            //Right most icon is error/status symbol
            if (FancyFoldersSettings.Instance.ShowStatusIcon)
            {
                icons[0] = StatusIcon();
                iconsUsed++;
            }

            //Fill rest up with file type icons
            for (var j = 0; j < _orderedFileTypes.Length && iconsUsed < icons.Length; j++, iconsUsed++)
                icons[iconsUsed] = new ColoredTexture(iconSet.GetMiniIcon(_orderedFileTypes[j].IconType())?.Icon, Color.white);

            return iconsUsed;
        }

        public readonly struct FileTypeCountAccessor
        {
            private readonly DirectoryItem _item;

            internal FileTypeCountAccessor(DirectoryItem item)
            {
                _item = item;
            }

            public int this[FileType type] => _item.GetTypeCount(type);
        }
    }
}
