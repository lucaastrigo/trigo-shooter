using System.Collections.Generic;
using System.Linq;
using FancyFolders.Editor.FileSystem;
using FancyFolders.Editor.Renderer.StringFormatting;
using FancyFolders.Editor.Renderer.Textures;
using FancyFolders.Editor.Settings;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.UI.Windows
{
    internal class MixedConfigWindow
        : BaseConfigWindow<MixedConfigWindow, IFileSystemItem<BaseAssetSettings>, BaseAssetSettings>
    {
        private const int Height = 169;

        private int _fileCount;
        private int _dirCount;

        /// <summary>
        /// Defaults for files and folders are different, so don't show the default button
        /// </summary>
        protected override bool ShowDefault => false;

        public static void Create([NotNull] IReadOnlyList<IFileSystemItem> items, Vector2 mousePosition)
        {
            Show(mousePosition, items.Cast<IFileSystemItem<BaseAssetSettings>>().ToArray(), height: Height);
        }

        protected override void Initialize()
        {
            _fileCount = Items.Count(a => a.IsFile);
            _dirCount = Items.Count(a => !a.IsFile);

            base.Initialize();
        }

        protected override BaseAssetSettings Create(AssetGuid guid, bool isFile)
        {
            if (isFile)
                return new FileSettings(guid);
            else
                return new DirectorySettings(guid);
        }

        protected override void Copy(BaseAssetSettings from, BaseAssetSettings to)
        {
            from.CopyTo(to);
        }

        protected override void DrawConfigContent(Rect area)
        {
            using (new GUILayout.AreaScope(area))
            {
                //Title
                EditorGUI.LabelField(new Rect(0, 0, area.width, 17), $"Editing {Items.Count} Items ({_fileCount} file{(_fileCount > 1 ? "s" : "")}, {_dirCount} directorie{(_dirCount > 1 ? "s" : "")})", Styles.BoldLabel);

                //Format field
                const int labelWidth = 93;
                EditorGUI.PrefixLabel(new Rect(0, 19, labelWidth, 17), new GUIContent("Format String:"));
                Example.Settings.FormatString = EditorGUI.TextField(new Rect(labelWidth, 19, area.width - labelWidth, 17), Example.Settings.FormatString);

                using (new GUILayout.AreaScope(new Rect(0, 39, area.width, area.height)))
                {
                    using (new EditorGUILayout.VerticalScope(Styles.ContentOutline))
                    {
                        var message = Example.IsFile
                            ? new FileInfoFormatter(Example.Settings.FormatString, (FileItem)Example).ToString()
                            : new DirectoryInfoFormatter(Example.Settings.FormatString, (DirectoryItem)Example).ToString();
                        EditorGUILayout.LabelField(Example.Name + message);
                    }

                    using (new EditorGUILayout.VerticalScope(Styles.ContentOutline))
                    {
                        FormatButton("name", "Show the name of the item");
                        FormatButton("size", "Show the size of the item");
                        FormatButton("guid", "Show the Unity item guid");
                        FormatButton("status", "Show the version control status of the item");
                    }
                }
            }
        }

        protected override void BottomBar(Rect area)
        {
            using (new GUILayout.AreaScope(area))
            {
                var size = area.height - 4;
                if (GUI.Button(new Rect(2, 2, size, size), new GUIContent(TextureResources.Instance.Text, "Edit Files")))
                    FileConfigWindow.Create(Items.OfType<FileItem>().ToArray(), position.position);
                if (GUI.Button(new Rect(2 + 2 + size, 2, size, size), new GUIContent(TextureResources.Instance.FolderLarge, "Edit Directories")))
                    DirectoryConfigWindow.Create(Items.OfType<DirectoryItem>().ToArray(), position.position);
            }
        }
    }
}
