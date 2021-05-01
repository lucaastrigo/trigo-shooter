using System;
using FancyFolders.Editor.FileSystem;
using FancyFolders.Editor.Settings.Icons;
using FancyFolders.Editor.UI;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.Settings
{
    [CustomEditor(typeof(FancyFoldersSettings))]
    public class FancyFoldersSettingsEditor
        : UnityEditor.Editor
    {
        private static string _searchTerm = "";

        [SettingsProvider, NotNull]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Preferences/Fancy Folders", SettingsScope.Project)
            {
                guiHandler = _ => OnGui(FancyFoldersSettings.Instance)
            };
        }

        public override void OnInspectorGUI()
        {
            var settings = (FancyFoldersSettings)target;

            OnGui(settings);

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        private static void OnGui([NotNull] FancyFoldersSettings settings)
        {
            DrawGlobalSettings(settings);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Item Settings", Styles.BoldLabel);

            _searchTerm = EditorGUILayout.TextField(new GUIContent("Search"), _searchTerm);

            void DefaultFile(FileSettings f) => new FileSettings(default(AssetGuid)).CopyTo(f);
            void DefaultFolder(DirectorySettings d) => new DirectorySettings(default(AssetGuid)).CopyTo(d);

            foreach (var setting in settings)
            {
                if (setting is FileSettings fileSettings)
                    DrawSetting(fileSettings, _searchTerm, DefaultFile);
                else if (setting is DirectorySettings directorySettings)
                    DrawSetting(directorySettings, _searchTerm, DefaultFolder);
            }
        }

        private static void DrawGlobalSettings([NotNull] FancyFoldersSettings settings)
        {
            EditorGUILayout.LabelField("Icons", Styles.BoldLabel);
            settings.IconSet = (IconSet)EditorGUILayout.ObjectField("Icon Set", settings.IconSet, typeof(IconSet), false);
            settings.ShowStatusIcon = EditorGUILayout.Toggle("Show Status Icon", settings.ShowStatusIcon);
            settings.MaxIconCount = (int)EditorGUILayout.Slider(new GUIContent("Icon Column Count"), settings.MaxIconCount, 0, 25);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Format Strings", Styles.BoldLabel);
            settings.DefaultDirFormatString = EditorGUILayout.TextField("Default Directory Format", settings.DefaultDirFormatString);
            settings.DefaultFileFormatString = EditorGUILayout.TextField("Default File Format", settings.DefaultFileFormatString);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Detail Text Color", Styles.BoldLabel);
            settings.UseCustomTextColor = EditorGUILayout.Toggle("Use Custom Text Color", settings.UseCustomTextColor);
            if (settings.UseCustomTextColor)
                settings.DetailsTextColor = EditorGUILayout.ColorField(new GUIContent("Custom Text Color"), settings.DetailsTextColor, true, true, false);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Item Separators", Styles.BoldLabel);
            settings.ShowSeparators = EditorGUILayout.Toggle("Show Line Separators", settings.ShowSeparators);
            if (settings.ShowSeparators)
            {
                settings.UseCustomSeparatorColor = EditorGUILayout.Toggle("Use Custom Separator Color", settings.UseCustomSeparatorColor);
                if (settings.UseCustomSeparatorColor)
                    settings.SeparatorColor = EditorGUILayout.ColorField("Separator Color", settings.SeparatorColor);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Item Selection Outline", Styles.BoldLabel);
            settings.ShowSelectionOutline = EditorGUILayout.Toggle("Show Selection Outline", settings.ShowSelectionOutline);
            if (settings.ShowSelectionOutline)
            {
                settings.UseCustomSelectionOutlineColor = EditorGUILayout.Toggle("Use Custom Selection Outline Color", settings.UseCustomSelectionOutlineColor);
                if (settings.UseCustomSelectionOutlineColor)
                    settings.SelectionOutlineColor = EditorGUILayout.ColorField("Selection Outline Color", settings.SelectionOutlineColor);
            }
        }

        private static void DrawSetting<T>([NotNull] T setting, string filter, Action<T> setToDefault)
            where T : BaseAssetSettings
        {
            var path = AssetDatabase.GUIDToAssetPath(setting.Guid.Guid);
            if (string.IsNullOrEmpty(path))
                return;

            if (!string.IsNullOrWhiteSpace(filter) && filter.Length > 0 && path.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) == -1)
                return;

            using (new EditorGUILayout.VerticalScope(Styles.ContentOutline))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Reset", GUILayout.Width(50)))
                    {
                        setToDefault(setting);
                        return;
                    }

                    EditorGUILayout.LabelField(path);
                }

                setting.FormatString = EditorGUILayout.DelayedTextField(setting.RawFormatString);

                var dir = setting as DirectorySettings;
                if (dir != null)
                {
                    dir.BackgroundIconColor = EditorGUILayout.ColorField(dir.BackgroundIconColor);
                    dir.OverlayIcon = (IconType)EditorGUILayout.EnumPopup("Overlay Icon", dir.OverlayIcon);
                }
            }
        }
    }
}
