using System.Collections.Generic;
using FancyFolders.Editor.Renderer.Textures;
using FancyFolders.Editor.UI;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.Settings.Icons
{
    [CustomEditor(typeof(IconSet))]
    public class IconSetEditor
        : UnityEditor.Editor
    {
        private readonly Dictionary<string, bool> _foldState = new Dictionary<string, bool>();

        public override void OnInspectorGUI()
        {
            var set = (IconSet)target;

            OnGui(set);

            if (GUI.changed)
                EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }

        private void OnGui([NotNull] IconSet set)
        {
            using (new EditorGUILayout.VerticalScope(Styles.FoldoutContentOutline))
            {
                EditorGUILayout.LabelField("Large Overlay Icons", Styles.BoldLabel);

                LargeIcon("Android");
                LargeIcon("Animation");
                LargeIcon("Audio");
                LargeIcon("Extension");
                LargeIcon("Font");
                LargeIcon("iOS", "IOS");
                LargeIcon("Linux");
                LargeIcon("Lock");
                LargeIcon("MacOS");
                LargeIcon("Material");
                LargeIcon("Mesh");
                LargeIcon("Music");
                LargeIcon("Plugin");
                LargeIcon("Prefab");
                LargeIcon("Resource");
                LargeIcon("Scene");
                LargeIcon("Script");
                LargeIcon("Steam");
                LargeIcon("Text");
                LargeIcon("Texture");
                LargeIcon("Trash");
                LargeIcon("Unlock");
                LargeIcon("WebGL");
                LargeIcon("Windows");

                EditorGUILayout.Separator();

                var list = serializedObject.FindProperty("_customIcons");
                EditorGUILayout.PropertyField(list, true);
            }

            using (new EditorGUILayout.VerticalScope(Styles.FoldoutContentOutline))
            {
                EditorGUILayout.LabelField("Mini Icons", Styles.BoldLabel);

                MiniIcon("Error");
                MiniIcon("Warning");

                MiniIcon("Vcs Conflict");
                MiniIcon("Vcs Modified");
                MiniIcon("Vcs Added");

                MiniIcon("Audio Clip");
                MiniIcon("Script");
                MiniIcon("Mesh");
                MiniIcon("Prefab");
                MiniIcon("Texture");
                MiniIcon("Scene");
                MiniIcon("Shader");
                MiniIcon("Text");
                MiniIcon("Material");
            }
        }

        private void LargeIcon([NotNull] string key, [CanBeNull] string root = null)
        {
            if (Fold(key))
                EditorGUILayout.PropertyField(serializedObject.FindProperty(root ?? key));
        }

        private void MiniIcon(string key)
        {
            var fold = Fold(key + " (mini)", key);
            if (fold)
            {
                var root = "Mini" + key.Replace(" ", "");

                var icon = (MiniIcon)typeof(IconSet).GetField(root).GetValue(target);

                var hiDpi = serializedObject.FindProperty(root + "._hiDpi");
                var loDpi = serializedObject.FindProperty(root + "._loDpi");

                using (new EditorGUILayout.VerticalScope(Styles.ContentOutline))
                {
                    DrawTextureSelect("High DPI", hiDpi, icon.HighDpiNoFallback);
                    DrawTextureSelect("Low DPI", loDpi, icon.LowDpiNoFallback);
                }
            }
        }

        private bool Fold([NotNull] string key, [CanBeNull] string title = null)
        {
            if (!_foldState.TryGetValue(key, out var state))
                _foldState.Add(key, false);

            var fold = EditorGUILayout.Foldout(state, title ?? key, true, EditorStyles.foldout);
            if (fold != state)
                _foldState[key] = fold;

            return fold;
        }

        public static void DrawTextureSelect(string label, SerializedProperty pathProp, Texture2D texture)
        {
            var style = new GUIStyle(GUI.skin.label) { fixedWidth = 90 };

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(label, style);

                using (var c = new EditorGUI.ChangeCheckScope())
                {
                    var selected = EditorGUILayout.ObjectField(texture, typeof(Texture2D), false);
                    if (c.changed)
                        pathProp.stringValue = selected == null ? null : AssetDatabase.GetAssetPath(selected.GetInstanceID());
                }
            }
        }

        public static void DrawLargeFolder(Rect area, Texture2D texture, [NotNull] SerializedProperty pathProp)
        {
            var icon = TextureResources.Instance.GetTexture("Assets/Plugins/FancyFolders/Editor/EditorResources/Unity Icons/Folder Icons/Icon.png");
            DrawFolder(area, icon, texture);
        }

        public static void DrawSmallFolder(Rect area, Texture2D texture, [NotNull] SerializedProperty pathProp)
        {
            var icon = TextureResources.Instance.GetTexture("Assets/Plugins/FancyFolders/Editor/EditorResources/Unity Icons/Folder Icons/Small Icon.png");
            DrawFolder(area, icon, texture);
        }

        public static void DrawFolder(Rect area, [NotNull] Texture2D bg, [CanBeNull] Texture2D fg)
        {
            GUI.DrawTexture(area, bg);
            if (fg != null)
                GUI.DrawTexture(area, fg);
        }
    }
}
