using System;
using System.Reflection;
using FancyFolders.Editor.Settings.Icons;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.UI.Properties
{
    [CustomPropertyDrawer(typeof(Icon))]
    public class IconDrawer
        : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 16 * 4 + 64;
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property == null)
                return;

            // By default `position` ignores indent, offset to respect indent.
            var indent = EditorGUI.indentLevel * 16;
            position.x += indent;
            position.width -= indent;

            GUI.Box(position, "");

            // Compute the size of each step up (each is 1.5874 times larger than the last).
            // This is derived from knowing the start point (16px) and the end point (64px) and working out a smooth curve between them:
            //   16 * (x ^ 3) = 64 solve for x
            const int sl = 16;
            const int sh = 25;
            const int ll = 40;
            const int lh = 64;

            var icon = (Icon)GetTargetObjectOfProperty(property);

            var largeHi = property.FindPropertyRelative("_largeHiDpi");
            var smallHi = property.FindPropertyRelative("_smallHiDpi");
            var largeLo = property.FindPropertyRelative("_largeLoDpi");
            var smallLo = property.FindPropertyRelative("_smallLoDpi");

            DrawTextureSelect(new Rect(position.x, position.y + 16 * 0, position.width, 16), "Large High DPI", largeHi, icon.LargeHighDpiNoFallback);
            DrawTextureSelect(new Rect(position.x, position.y + 16 * 1, position.width, 16), "Large Low DPI", largeLo, icon.LargeLowDpiNoFallback);
            DrawTextureSelect(new Rect(position.x, position.y + 16 * 2, position.width, 16), "Small High DPI", smallHi, icon.SmallHighDpiNoFallback);
            DrawTextureSelect(new Rect(position.x, position.y + 16 * 3, position.width, 16), "Small Low DPI", smallLo, icon.SmallLowDpiNoFallback);

            var availableWidth = position.width;
            const int requiredWidth = sl + sh + ll + lh;
            var spareWidth = availableWidth - requiredWidth;
            var space = spareWidth / 5;

            var x = position.x + space;
            var y = position.y + 16 * 4;

            IconSetEditor.DrawLargeFolder(new Rect(x, y, lh, lh), icon.LargeHighDpi, largeHi);
            x += lh + space;
            IconSetEditor.DrawLargeFolder(new Rect(x, y, ll, ll), icon.LargeLowDpi, largeLo);
            x += ll + space;
            IconSetEditor.DrawSmallFolder(new Rect(x, y, sh, sh), icon.SmallHighDpi, smallHi);
            x += sh + space;
            IconSetEditor.DrawSmallFolder(new Rect(x, y, sl, sl), icon.SmallLowDpi, smallLo);
        }

        private static void DrawTextureSelect(Rect area, string label, SerializedProperty pathProp, Texture2D texture)
        {
            GUI.Label(new Rect(area.x, area.y, 100, area.height), label);

            using (var c = new EditorGUI.ChangeCheckScope())
            {
                var selected = EditorGUI.ObjectField(new Rect(area.x + 100, area.y, area.width - 100, area.height), texture, typeof(Texture2D), false);
                if (c.changed)
                    pathProp.stringValue = selected == null ? null : AssetDatabase.GetAssetPath(selected.GetInstanceID());
            }
        }

        /// <summary>
        /// Gets the object the property represents.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        private static object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            if (prop == null)
                return null;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal)).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null)
                return null;

            var enm = enumerable.GetEnumerator();
            for (var i = 0; i <= index; i++)
                if (!enm.MoveNext())
                    return null;

            return enm.Current;
        }
    }
}
