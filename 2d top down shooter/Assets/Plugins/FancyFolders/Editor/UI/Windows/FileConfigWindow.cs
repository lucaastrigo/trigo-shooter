using System.Collections.Generic;
using FancyFolders.Editor.FileSystem;
using FancyFolders.Editor.Renderer.StringFormatting;
using FancyFolders.Editor.Settings;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.UI.Windows
{
    internal class FileConfigWindow
        : BaseConfigWindow<FileConfigWindow, FileItem, FileSettings>
    {
        private const int Height = 185;

        public static void Create([NotNull] IReadOnlyList<FileItem> items, Vector2 mousePosition)
        {
            Show(mousePosition, items, height: Height);
        }

        protected override FileSettings Create(AssetGuid guid, bool isFile)
        {
            return new FileSettings(guid);
        }

        protected override void Copy(FileSettings from, FileSettings to)
        {
            from.CopyTo(to);
        }

        protected override void DrawConfigContent(Rect area)
        {
            using (new GUILayout.AreaScope(area))
            {
                //Title
                EditorGUI.LabelField(new Rect(0, 0, area.width, 17), Items.Count == 1 ? $"Editing \"{Items[0].Name}.{Items[0].Extension}\"" : $"Editing {Items.Count} Files", Styles.BoldLabel);

                //Format field
                const int labelWidth = 93;
                EditorGUI.PrefixLabel(new Rect(0, 19, labelWidth, 17), new GUIContent("Format String:"));
                Example.Settings.FormatString = EditorGUI.TextField(new Rect(labelWidth, 19, area.width - labelWidth, 17), Example.Settings.FormatString);

                using (new GUILayout.AreaScope(new Rect(0, 39, area.width, area.height)))
                {
                    using (new EditorGUILayout.VerticalScope(Styles.ContentOutline))
                    {
                        var message = new FileInfoFormatter(Example.Settings.FormatString, Example).ToString();
                        EditorGUILayout.LabelField(Example.Name + message);
                    }

                    using (new EditorGUILayout.VerticalScope(Styles.ContentOutline))
                    {
                        FormatButton("name", "Show the name of the file");
                        FormatButton("size", "Show the size of the file");
                        FormatButton("ext", "Show the file extension");
                        FormatButton("guid", "Show the Unity file guid");
                        FormatButton("status", "Show the version control status of the file");
                    }
                }
            }
        }
    }
}
