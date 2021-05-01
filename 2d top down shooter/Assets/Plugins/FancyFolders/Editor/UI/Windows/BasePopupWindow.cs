using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.UI.Windows
{
    internal abstract class BasePopupWindow<TSelf>
        : BaseWindow
        where TSelf: BasePopupWindow<TSelf>
    {
        private Vector2 _offset;

        protected static void Show(Rect position, [CanBeNull] Action<TSelf> configure = null)
        {
            //Close all windows
            CloseAll();

            //Get or create the window
            var window = Resources.FindObjectsOfTypeAll<TSelf>().FirstOrDefault() ?? CreateInstance<TSelf>();

            window.minSize = position.size;
            window.position = position;
            window.Focus();

            configure?.Invoke(window);
            window.Initialize();

            window.ShowPopup();
        }

        protected virtual void Initialize()
        {
        }

        // ReSharper disable once InconsistentNaming
        public virtual void OnGUI()
        {
            var windowSize = new Vector2(position.width, position.height);
            var windowRect = new Rect(0, 0, windowSize.x, windowSize.y);
            var contentRect = new Rect(Vector2.one, windowSize - new Vector2(2f, 2f));

            using (new GUILayout.AreaScope(windowRect))
            {
                //Draw background in the color of the border
                var bg = EditorGUIUtility.isProSkin ? new Color(0.83f, 0.83f, 0.83f) : new Color(0.18f, 0.18f, 0.18f);           
                EditorGUI.DrawRect(windowRect, bg);

                //Draw foreground covering whole window except small border
                var fg = EditorGUIUtility.isProSkin ? new Color(0.18f, 0.18f, 0.18f) : new Color(0.83f, 0.83f, 0.83f);
                EditorGUI.DrawRect(contentRect, fg);

                //Draw content inside the foreground box
                using (new GUILayout.AreaScope(contentRect))
                    DrawContent(new Rect(Vector2.zero, contentRect.size));
            }
        }

        protected virtual void DrawContent(Rect area)
        {
        }
    }
}