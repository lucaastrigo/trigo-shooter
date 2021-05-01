using System.Collections.Generic;
using System.Linq;
using FancyFolders.Editor.FileSystem;
using FancyFolders.Editor.Renderer.StringFormatting;
using FancyFolders.Editor.Renderer.Textures;
using FancyFolders.Editor.Settings;
using FancyFolders.Editor.Settings.Icons;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.UI.Windows
{
    internal class DirectoryConfigWindow
        : BaseConfigWindow<DirectoryConfigWindow, DirectoryItem, DirectorySettings>
    {
        private Texture2D _background;
        private Texture2D _mid;

        private bool _recursive;

        private const int WindowHeight = 270;
        private const int WindowWidth = 360;

        public static void Create([NotNull] IReadOnlyList<DirectoryItem> items, Vector2 mousePosition)
        {
            Show(mousePosition, items, height: WindowHeight, width: WindowWidth);
        }

        protected override DirectorySettings Create(AssetGuid guid, bool isFile)
        {
            return new DirectorySettings(guid);
        }

        protected override void Initialize()
        {
            _background = TextureResources.Instance.FolderLarge;

            _mid = TextureResources.Instance.FolderLarge;

            base.Initialize();
        }

        protected override void Copy(DirectorySettings from, DirectorySettings to)
        {
            from.CopyTo(to);
        }

        protected override void DrawConfigContent(Rect area)
        {
            using (new GUILayout.AreaScope(area))
            {
                //Title
                EditorGUI.LabelField(new Rect(0, 0, area.width, 17), Items.Count == 1 ? $"Editing \"{Items[0].Name}\"" : $"Editing {Items.Count} Directories", Styles.BoldLabel);

                using (new GUILayout.AreaScope(new Rect(0, 19, area.width, 135), "", Styles.ContentOutline))
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        //Format field
                        const int labelWidth = 77;
                        EditorGUI.PrefixLabel(new Rect(2, 2, labelWidth, 16), new GUIContent("Detail Text:"));
                        Example.Settings.FormatString = EditorGUI.TextField(new Rect(labelWidth, 2, area.width - labelWidth - 2, 16), Example.Settings.FormatString);

                        using (new GUILayout.AreaScope(new Rect(2, 20, area.width - 4, 135 - 20)))
                        {
                            using (new EditorGUILayout.VerticalScope(Styles.ContentOutline))
                            {
                                var message = new DirectoryInfoFormatter(Example.Settings.FormatString, Example).ToString();
                                EditorGUILayout.LabelField(Example.Name + message);
                            }

                            FormatButton("name", "Show the name of the directory");
                            FormatButton("size", "Show the size of the directory");
                            FormatButton("count", "Show the number of items in the directory tree");
                            FormatButton("guid", "Show the Unity directory guid");
                            FormatButton("status", "Show the version control status of the directory");
                        }
                    }
                }

                using (new GUILayout.AreaScope(new Rect(0, 158, area.width, 78), "", Styles.ContentOutline))
                {
                    using (new GUILayout.AreaScope(new Rect(3, 3, area.width - 62, 72)))
                    {
                        using (new EditorGUILayout.VerticalScope())
                        {
                            Example.Settings.BackgroundIconColor = EditorGUILayout.ColorField(new GUIContent("Background Icon Colour"), Example.Settings.BackgroundIconColor, true, false, false);
                            Example.Settings.ForegroundIconColor = EditorGUILayout.ColorField(new GUIContent("Foreground Icon Colour"), Example.Settings.ForegroundIconColor, true, true, false);
                            Example.Settings.OverlayIcon = (IconType)EditorGUILayout.EnumPopup("Overlay Icon", Example.Settings.OverlayIcon);

                            if (Example.Settings.OverlayIcon == IconType.Custom)
                                Example.Settings.CustomIconOverlayId = EditorGUILayout.TextField(new GUIContent("Custom Icon ID", "ID of custom icon in icon Set"), Example.Settings.CustomIconOverlayId);
                        }
                    }

                    using (new GUILayout.AreaScope(new Rect(area.width - 57, 3, 54, 54), "", Styles.ContentOutline))
                    {
                        var icon = FancyFoldersSettings.Instance.IconSet?.GetIcon(Example.Settings.OverlayIcon, Example.Settings.CustomIconOverlayId);

                        DrawFolder(new Rect(4, -4, 50, 50), _background, new ColoredTexture(_mid, Example.Settings.BackgroundIconColor), new ColoredTexture(icon?.Large, Example.Settings.ForegroundIconColor));

                        const int smallSize = 20;
                        DrawFolder(new Rect(2, 54 - smallSize - 2 + 2, 16, 16), _background, new ColoredTexture(_mid, Example.Settings.BackgroundIconColor), new ColoredTexture(icon?.Small, Example.Settings.ForegroundIconColor));
                    }
                }
            }
        }

        private static void DrawFolder(Rect area, [CanBeNull] Texture2D bg, ColoredTexture mid, ColoredTexture fg)
        {
            if (bg != null)
                GUI.DrawTexture(area, bg);
            if (mid.Texture != null)
                GUI.DrawTexture(area, mid.Texture, ScaleMode.StretchToFill, true, 0, mid.Color, 0, 0);
            if (fg.Texture != null)
                GUI.DrawTexture(area, fg.Texture, ScaleMode.StretchToFill, true, 0, fg.Color, 0, 0);
        }

        protected override void Apply()
        {
            IEnumerable<DirectoryItem> Children(DirectoryItem root)
            {
                var todo = new Queue<DirectoryItem>();
                todo.Enqueue(root);

                while (todo.Count > 0)
                {
                    var item = todo.Dequeue();
                    if (item != root)
                        yield return item;

                    foreach (var child in item.Children)
                        todo.Enqueue(child);
                }
            }

            if (_recursive)
            {
                //Apply to all child items
                foreach (var child in Items.SelectMany(Children))
                    Copy(Example.Settings, child.Settings);
            }

            base.Apply();
        }

        protected override void BottomBar(Rect area)
        {
            const float height = 18;
            var toggleBox = new Rect(4, area.y + area.height / 2 - height / 2, 100, height);
            _recursive = GUI.Toggle(toggleBox, _recursive, new GUIContent("Recursive", "Apply changes to all child directories"));
        }
    }
}
