using System.Collections.Generic;
using System.IO;
using FancyFolders.Editor.Settings;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.FileSystem
{
    public class ProjectFileSystem
    {
        public static ProjectFileSystem Instance { get; } = new ProjectFileSystem();

        private readonly string _root = CanonicalizePath(Directory.GetParent(Application.dataPath).FullName);

        private readonly Dictionary<AssetGuid, DirectoryItem> _directoryCache = new Dictionary<AssetGuid, DirectoryItem>();
        private readonly Dictionary<AssetGuid, FileItem> _fileCache = new Dictionary<AssetGuid, FileItem>();

        private ProjectFileSystem()
        {
        }

        [CanBeNull] public DirectoryItem GetDirectory(AssetGuid guid)
        {
            GetItem(guid, out _, out var dir);
            return dir;
        }

        [CanBeNull] public FileItem GetFile(AssetGuid guid)
        {
            GetItem(guid, out var file, out _);
            return file;
        }

        [CanBeNull] public IFileSystemItem GetItem(AssetGuid guid)
        {
            GetItem(guid, out var file, out var dir);
            return (IFileSystemItem)file ?? dir;
        }

        public void GetItem(AssetGuid guid, [CanBeNull] out FileItem file, [CanBeNull] out DirectoryItem dir)
        {
            //If the asset path is null that means it's an incorrect guid, return null
            var assetPath = AssetDatabase.GUIDToAssetPath(guid.Guid);
            if (string.IsNullOrEmpty(assetPath))
            {
                dir = null;
                file = null;
                return;
            }

            //If the path is in the `Packages` directory ignore it
            if (assetPath.StartsWith("Packages"))
            {
                dir = null;
                file = null;
                return;
            }

            //Search directory cache for this guid
            if (_directoryCache.TryGetValue(guid, out dir))
            {
                file = null;
                return;
            }

            //Search file cache for this guid
            if (_fileCache.TryGetValue(guid, out file))
                return;

            //It wasn't in the caches, so create it and add to cache
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                dir = new DirectoryItem(guid, this, assetPath, FancyFoldersSettings.Instance.GetDirectoryAssetSettings(guid));
                _directoryCache.Add(guid, dir);
            }
            else
            {
                file = new FileItem(guid, this, assetPath, FancyFoldersSettings.Instance.GetFileAssetSettings(guid));
                _fileCache.Add(guid, file);
            }
        }

        #region cache invalidation
        private void InvalidateDirectoryHierarchy([CanBeNull] string assetPath)
        {
            if (!IsValidPath(assetPath)) return;

            //Get the starting directory
            var dirGuid = new AssetGuid(AssetDatabase.AssetPathToGUID(assetPath));
            FileItem file;
            DirectoryItem dir;
            GetItem(dirGuid, out file, out dir);

            //If it was actually a file, skip up to the parent directory of the file
            if (dir == null && file != null)
                dir = file.Parent;

            //Move up to the root of the hierarchy, clearing caches
            while (dir != null)
            {
                dir.ItemModified();
                dir = dir.Parent;
            }
        }

        internal void AssetCreatedOrModified([CanBeNull] string assetPath)
        {
            if (!IsValidPath(assetPath))
                return;

            FileItem file;
            DirectoryItem dir;
            GetItem(new AssetGuid(AssetDatabase.AssetPathToGUID(assetPath)), out file, out dir);

            var directoryInvalidate = assetPath;
            if (file != null)
            {
                file.ItemModified();
                directoryInvalidate = Path.GetDirectoryName(assetPath);
            }
            else if (dir != null)
            {
                //Unity never reports that a directory was moved, just modified. So we'll treat modification as a move.
                dir.ItemMoved();
                dir.ItemModified();
            }

            //Invalidate the cache for the containing folders (all the way up the tree to the root of the project)
            InvalidateDirectoryHierarchy(directoryInvalidate);
        }

        internal void AssetDeleted([CanBeNull] string assetPath)
        {
            if (!IsValidPath(assetPath)) return;

            //If the settings file is deleted smash the entire cache to force it to be reloaded and load new per item settings
            if (assetPath != null && assetPath.Equals(FancyFoldersSettings.SettingsPath))
            {
                _directoryCache.Clear();
                _fileCache.Clear();
                Main.ForceRedraw();
                return;
            }

            //Asset deleted, remove it from the caches. We don't know if it's a file or a folder so try both
            var guid = new AssetGuid(AssetDatabase.AssetPathToGUID(assetPath));
            if (!_directoryCache.Remove(guid))
                _fileCache.Remove(guid);

            //Invalidate the directory which contained the deleted object
            InvalidateDirectoryHierarchy(Path.GetDirectoryName(assetPath));
        }

        internal void AssetMoved([CanBeNull] string fromAssetPath, [CanBeNull] string toAssetPath)
        {
            if (!IsValidPath(fromAssetPath)) return;
            if (!IsValidPath(toAssetPath)) return;

            //This could be a rename or an actual move. If it was a rename the from/to directory paths will be the same
            var fromDir = Path.GetDirectoryName(fromAssetPath);
            var toDir = Path.GetDirectoryName(toAssetPath);
            var moved = fromDir != toDir;

            //No need to invalidate the dir hierachy cache if this item was not moved
            if (moved)
            {
                InvalidateDirectoryHierarchy(Path.GetDirectoryName(fromAssetPath));
                InvalidateDirectoryHierarchy(Path.GetDirectoryName(toAssetPath));
            }

            //Clear cache for the file itself
            var guid = new AssetGuid(AssetDatabase.AssetPathToGUID(toAssetPath));
            FileItem file;
            if (_fileCache.TryGetValue(guid, out file))
                file.ItemMoved();
        }

        private static bool IsValidPath([CanBeNull] string fromAssetPath)
        {
            return fromAssetPath != null && fromAssetPath.StartsWith("assets", true, null);
        }
        #endregion

        [NotNull] private static string CanonicalizePath([NotNull] string path)
        {
            return path
                   .Replace('\\', '/')
                   .TrimStart('/');
        }

        [CanBeNull] public string FullPathToAssetPath([NotNull] string fullPath)
        {
            var p = CanonicalizePath(Path.GetFullPath(fullPath));

            if (!p.StartsWith(_root))
                return null;
            else
                return CanonicalizePath(p.Remove(0, _root.Length));

        }
    }
}
