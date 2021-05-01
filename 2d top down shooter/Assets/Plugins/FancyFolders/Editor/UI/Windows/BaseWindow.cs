using System;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.UI.Windows
{
    internal class BaseWindow
        : EditorWindow
    {
        protected static void CloseAll()
        {
            foreach (var baseWindow in Resources.FindObjectsOfTypeAll<BaseWindow>())
            {
                try
                {
                    if (baseWindow)
                        baseWindow.Close();
                }
                catch (Exception)
                {
                    //Unity sometimes fails to close windows when it gets into a confused state (across recompiles?). Swallow these errors.
                }
            }
        }
    }
}
