using System.Collections.Generic;
using System.Linq;
using FancyFolders.Editor.Compiler;
using FancyFolders.Editor.FileSystem;
using FancyFolders.Editor.Renderer;
using FancyFolders.Editor.Settings;
using FancyFolders.Editor.UI.Windows;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace FancyFolders.Editor
{
    [InitializeOnLoad]
    public class Main
    {
        public const string AssetName = "Fancy Folders";
        public static bool AssetDisabled;

        private static bool _initRequired = true;
        private static bool _redrawRequested;
        private static bool _saveRequested = false;

        private static int _startupDelay = 5;

        static Main()
        {
            EditorApplication.update += Update;
        }

        private static void Initialize()
        {
            //Preload settings from asset file
            if (FancyFoldersSettings.Reload() == null)
                return;

            // Trim non existant files/folders from settings
            if (FancyFoldersSettings.Instance.Trim())
                _saveRequested = true;

            //Begin monitoring for modification of project files to invalidate the FS cache
            var fs = ProjectFileSystem.Instance;
            AssetModificationMonitor.AssetCreatedOrModified += fs.AssetCreatedOrModified;
            AssetModificationMonitor.AssetDeleted += fs.AssetDeleted;
            AssetModificationMonitor.AssetMoved += fs.AssetMoved;

            //Watch for deletion of the settings asset to invalidate settings cache
            AssetModificationMonitor.AssetDeleted += path =>
            {
                if (path.Equals(FancyFoldersSettings.SettingsPath))
                {
                    FancyFoldersSettings.Reload();
                    ForceRedraw();
                }
            };

            //Mark files with compiler errors
            CompilationPipeline.assemblyCompilationFinished += (_, msgs) =>
            {
                CompilerMonitor.Instance.CompileComplete(msgs);
                ForceRedraw();
            };

            //Render UI items when the editor draws an item
            var renderer = new ProjectBrowserRenderer(ProjectFileSystem.Instance);
            Selection.selectionChanged += renderer.SelectionChanged;
            EditorApplication.projectWindowItemOnGUI += (g, r) => {
                if (!AssetDisabled)
                    renderer.DrawDetailedItemInfo(g, r);
            };

            //Detect Alt+Click events on files and folders
            EditorApplication.projectWindowItemOnGUI += (g, r) => {
                if (!AssetDisabled)
                    AltClickPopup(g, r);
            };

            _initRequired = false;
            ForceRedraw();
        }

        private static void AltClickPopup(string drawGuid, Rect area)
        {
            //Ignore all event that are not alt+click
            if (Event.current.type != EventType.MouseUp || !Event.current.alt)
                return;

            //If this current item does not contain the mouse ignore it, the containing item will handle the event
            if (!area.Contains(Event.current.mousePosition))
                return;

            //Ignore events to do with items in `Packages` directory
            var drawPath = AssetDatabase.GUIDToAssetPath(drawGuid);
            if (drawPath.StartsWith("Packages"))
                return;

            //Get set of guids Unity thinks are selected (excluding `Packages` paths)
            var unitySelectionGuids = (from g in Selection.assetGUIDs
                                       let p = AssetDatabase.GUIDToAssetPath(g)
                                       where !p.StartsWith("Packages")
                                       select new AssetGuid(g)).ToArray();

            //Chose which items we are configuring
            // - If unity selection is empty, choose the item that was clicked on
            // - If unity selection does not contain the item that was clicked on, choose the item that was clicked on
            // - Otherwise configure all the items Unity thinks are selected
            var guidsToConfigure = new List<AssetGuid>();
            if (unitySelectionGuids.Length == 0 || !unitySelectionGuids.Contains(new AssetGuid(drawGuid)))
                guidsToConfigure.Add(new AssetGuid(drawGuid));
            else
                guidsToConfigure.AddRange(unitySelectionGuids);

            //Get the file system items from the set of guids
            var itemsToConfigure = (from g in guidsToConfigure
                                    let i = ProjectFileSystem.Instance.GetItem(g)
                                    where i != null
                                    select i).ToArray();

            //Choose where to place the window, closest point on the edge of the rectangle aligned with the mouse
            var mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            var posAcross = GUIUtility.GUIToScreenPoint(new Vector2(area.xMax, Event.current.mousePosition.y));
            var posDown = GUIUtility.GUIToScreenPoint(new Vector2(Event.current.mousePosition.x, area.yMax));
            var pos = Vector2.Distance(mousePos, posAcross) < Vector2.Distance(mousePos, posDown) ? posAcross : posDown;

            //Open a window to configure these items
            var allFiles = itemsToConfigure.All(i => i.IsFile);
            var allDirs = itemsToConfigure.All(i => !i.IsFile);
            if (allFiles)
                FileConfigWindow.Create(itemsToConfigure.Cast<FileItem>().ToArray(), pos);
            else if (allDirs)
                DirectoryConfigWindow.Create(itemsToConfigure.Cast<DirectoryItem>().ToArray(), pos);
            else
                MixedConfigWindow.Create(itemsToConfigure, pos);
        }

        private static void Update()
        {
            if (AssetDisabled)
                return;

            if (_startupDelay > 0)
            {
                _startupDelay--;
                return;
            }

            if (_initRequired)
                Initialize();

            if (_saveRequested)
            {
                AssetDatabase.SaveAssets();
                _saveRequested = false;
            }

            if (!_initRequired && _redrawRequested)
            {
                _redrawRequested = false;
                var browser = GetWindowByName<EditorWindow>("ProjectBrowser");
                if (browser != null)
                    browser.Repaint();
            }
        }

        [CanBeNull] private static T GetWindowByName<T>(string windowTypeName)
            where T : EditorWindow
        {
            return (from window in Resources.FindObjectsOfTypeAll<T>()
                    let name = window.GetType().Name
                    where name == windowTypeName
                    select window
                ).FirstOrDefault();
        }

        public static void ForceRedraw(bool saveAssets = false)
        {
            _redrawRequested = true;
            _saveRequested = saveAssets;
        }
    }
}

