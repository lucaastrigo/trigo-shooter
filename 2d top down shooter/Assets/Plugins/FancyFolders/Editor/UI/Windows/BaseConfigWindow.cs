using System;
using System.Collections.Generic;
using System.Linq;
using FancyFolders.Editor.FileSystem;
using FancyFolders.Editor.Settings;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.UI.Windows
{
    internal abstract class BaseConfigWindow<TSelf, TItem, TSettings>
        : BasePopupWindow<TSelf>
        where TSelf : BaseConfigWindow<TSelf, TItem, TSettings>
        where TItem : class, IFileSystemItem<TSettings>
        where TSettings : BaseAssetSettings
    {
        public IReadOnlyList<TItem> Items { get; private set; }

        private TItem _example;
        protected TItem Example => _example;

        private TSettings _revertToSettings;

        protected virtual bool ShowApply => true;
        protected virtual bool ShowCancel => true;
        protected virtual bool ShowDefault => true;
        protected virtual bool ShowRevert => true;

        private const int WindowWidth = 350;
        private const int WindowHeight = 250;

        protected static void Show(Vector2 position, [NotNull] IReadOnlyList<TItem> items, [CanBeNull] Action<TSelf> configure = null, int width = WindowWidth, int height = WindowHeight)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (items.Count == 0)
                return;

            Show(new Rect(position, new Vector2(width, height)), w => {
                w.Items = items;
                configure?.Invoke(w);
            });
        }

        [NotNull] protected abstract TSettings Create(AssetGuid guid, bool isFile);

        protected abstract void Copy([NotNull] TSettings from, [NotNull] TSettings to);

        protected override void Initialize()
        {
            //Use the first item as the example one
            _example = Items[0];

            //Save a copy of the example settings to revert changes back to
            _revertToSettings = Create(_example.Guid, _example.IsFile);
            Copy(_example.Settings, _revertToSettings);

            base.Initialize();
        }

        protected override void DrawContent(Rect area)
        {
            if (Items == null || Items.Count == 0)
            {
                Close();
                return;
            }

            using (var changed = new EditorGUI.ChangeCheckScope())
            {
                base.DrawContent(area);

                //Draw bottom bar
                const int bottomBarHeight = 25;
                var bottomBar = new Rect(area.xMin, area.yMax - bottomBarHeight, area.width, bottomBarHeight);
                EditorGUI.DrawRect(bottomBar, new Color(0, 0, 0, 0.1f));

                var x = bottomBar.xMax - 1;

                if (ShowApply)
                {
                    x -= 56;
                    if (GUI.Button(new Rect(x, bottomBar.yMin + 2, 55, bottomBar.height - 4), new GUIContent("Apply", "Apply changes to all selected items")))
                        Apply();
                }

                if (ShowDefault)
                {
                    x -= 56;
                    if (GUI.Button(new Rect(x, bottomBar.yMin + 2, 55, bottomBar.height - 4), new GUIContent("Default", "Change all settings to their default values")))
                        Default();
                }

                if (ShowRevert)
                {
                    x -= 56;
                    if (GUI.Button(new Rect(x, bottomBar.yMin + 2, 55, bottomBar.height - 4), new GUIContent("Revert", "Revert all changed settings")))
                        Revert();
                }

                if (ShowCancel)
                {
                    x -= 56;
                    if (GUI.Button(new Rect(x, bottomBar.yMin + 2, 55, bottomBar.height - 4), new GUIContent("Cancel", "Close this window")))
                        Cancel();
                }

                BottomBar(new Rect(bottomBar.xMin, bottomBar.yMin, x, bottomBar.height));

                const int margin = 3;
                DrawConfigContent(new Rect(area.xMin + margin, area.yMin + margin, area.width - margin * 2, area.height - bottomBarHeight - margin * 2));

                if (changed.changed)
                    Main.ForceRedraw();
            }
        }

        protected virtual void Cancel()
        {
            //Revert changes before cancelling
            Copy(_revertToSettings, _example.Settings);

            //Redraw the entire project window to reflect changes
            Main.ForceRedraw();

            Close();
        }

        protected virtual void Revert()
        {
            //Revert changes
            Copy(_revertToSettings, _example.Settings);

            //Text in a textbox does not update if the textbox is currently focused. It is likely we want to reset the values in text boxes when the changes are reverted so they reflect the new values.
            //Defocus all controls to force the text box to update.
            DefocusControls();

            //Redraw the entire project window to reflect changes
            Main.ForceRedraw();
        }

        protected virtual void Default()
        {
            //Create a new settings object (in the default state)
            var defaultSettings = Create(_example.Guid, _example.IsFile);

            //Copy those default settings into the example
            Copy(defaultSettings, _example.Settings);

            //Text in a textbox does not update if the textbox is currently focused. It is likely we want to reset the values in text boxes when the settings are set to default so they reflect the new values.
            //Defocus all controls to force the text box to update.
            DefocusControls();

            //Redraw the entire project window to reflect changes
            Main.ForceRedraw();
        }

        protected virtual void Apply()
        {
            //Copy settings into each itwm
            var s = _example.Settings;
            foreach (var fileItem in Items.Skip(1))
                Copy(s, fileItem.Settings);

            //Redraw the entire project window to reflect changes
            Main.ForceRedraw();

            //Close this window now that we're done
            Close();
        }

        private void DefocusControls()
        {
            //Focus nothing (a random GUID) to ensure that all the text boxes update properly
            GUI.FocusControl("E8B35128-E524-4750-8066-8E3E678377AE");
        }

        protected abstract void DrawConfigContent(Rect area);

        protected void FormatButton(string placeholder, string description)
        {
            if (GUILayout.Button($"{{{placeholder}}}: {description}", GUI.skin.label))
                Example.Settings.FormatString += $"{{{placeholder}}}";
        }

        protected virtual void BottomBar(Rect area)
        {
        }
    }
}
