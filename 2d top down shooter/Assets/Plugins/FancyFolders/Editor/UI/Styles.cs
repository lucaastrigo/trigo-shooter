using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.UI
{
    internal static class Styles
    {
        public static readonly GUIStyle ContentOutline = new GUIStyle(EditorStyles.helpBox) {
            padding = new RectOffset(0, 0, 0, 1),
            margin = new RectOffset(0, 0, 0, 3)
        };
        public static readonly GUIStyle PaddedContentOutline = new GUIStyle(ContentOutline) {
            padding = new RectOffset(3, 3, 3, 3)
        };
        public static readonly GUIStyle FoldoutContentOutline = new GUIStyle(ContentOutline) {
            padding = new RectOffset(14, 0, 0, 1),
        };

        public static readonly GUIStyle BoldLabel = EditorStyles.boldLabel;
        public static readonly GUIStyle BoldFoldout = new GUIStyle(EditorStyles.foldout) {
            fontStyle = FontStyle.Bold
        };

        public static readonly GUIStyle ToolbarBreadcrumbLeft = new GUIStyle("GUIEditor.BreadcrumbLeft");
        public static readonly GUIStyle ToolbarBreadcrumbMid = new GUIStyle("GUIEditor.BreadcrumbMid");

        public static readonly GUIStyle ToolbarBreadcrumbLeftToggleOn = new GUIStyle("GUIEditor.BreadcrumbLeft");
        public static readonly GUIStyle ToolbarBreadcrumbMidToggleOn = new GUIStyle("GUIEditor.BreadcrumbMid");

        public static readonly GUIStyle ToolbarButtonToggleOn = new GUIStyle(EditorStyles.toolbarButton);
        public static readonly GUIStyle ToolbarButtonToggleOff = new GUIStyle(EditorStyles.toolbarButton);

        public static readonly GUIStyle RootItemTextLabel = EditorStyles.boldLabel;
        public static readonly GUIStyle RootItemSelectedTextLabel = new GUIStyle(RootItemTextLabel) {
            normal = {
                textColor = Color.white
            }
        };

        public static readonly GUIStyle ProjectItemTextLabel = EditorStyles.label;
        public static readonly GUIStyle ProjectItemSelectedTextLabel = new GUIStyle(ProjectItemTextLabel) {
            normal = {
                textColor = Color.white
            }
        };

        static Styles()
        {
            ToolbarButtonToggleOn.normal = ToolbarButtonToggleOn.active;
            ToolbarBreadcrumbLeftToggleOn.normal = ToolbarBreadcrumbLeftToggleOn.active;
            ToolbarBreadcrumbMidToggleOn.normal = ToolbarBreadcrumbMidToggleOn.active;
        }
    }
}
