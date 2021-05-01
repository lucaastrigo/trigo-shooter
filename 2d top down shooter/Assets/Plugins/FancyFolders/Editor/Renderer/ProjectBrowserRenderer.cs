using System;
using System.Collections.Generic;
using FancyFolders.Editor.FileSystem;
using FancyFolders.Editor.UI;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.Renderer
{
    public class ProjectBrowserRenderer
    {
        private readonly ProjectFileSystem _fs;
        private readonly HashSet<string> _selectionSet = new HashSet<string>();

        public ProjectBrowserRenderer(ProjectFileSystem fs)
        {
            _fs = fs;
        }

        public void SelectionChanged()
        {
            //This only runs when an item in the right column is selected, not the left column!
            _selectionSet.Clear();
            _selectionSet.UnionWith(Selection.assetGUIDs);
        }

        private bool IsSelected(AssetGuid guid)
        {
            return _selectionSet.Contains(guid.Guid) || Array.IndexOf(Selection.assetGUIDs, guid.Guid) != -1;
        }

        public void DrawDetailedItemInfo(string guid, Rect rect)
        {
            try
            {
                //Get the item from the file system cache
                _fs.GetItem(new AssetGuid(guid), out var fileItem, out var dirItem);
                var item = (IFileSystemItem)fileItem ?? dirItem;
                if (item == null)
                    return;

                //Switch rendering based on what type of thing is being rendered
                var type = DetermineType(rect, item);
                switch (type)
                {
                    case ItemType.LargeIconRightColumn:
                        DrawItemLarge(rect, item);
                        break;

                    case ItemType.BasicFileItemRightColumn:
                        DrawItemDetails(rect, UiConstants.RightColumn.Icon.Width, UiConstants.RightColumn.Icon.ExtraHeight, UiConstants.RightColumn.Icon.Offset, item.NamePixelWidth, true, UiConstants.RightColumn.TextOffset, item);
                        break;

                    case ItemType.BasicFileItemLeftColumn:
                        DrawItemDetails(rect, UiConstants.LeftColumn.Icon.Width, UiConstants.LeftColumn.Icon.ExtraHeight, UiConstants.LeftColumn.Icon.Offset, item.NamePixelWidth, true, UiConstants.LeftColumn.TextOffset, item);
                        break;

                    case ItemType.DirectoryItemLeftColumn:
                        DrawItemDetails(rect, UiConstants.LeftColumn.Icon.Width, UiConstants.LeftColumn.Icon.ExtraHeight, UiConstants.LeftColumn.Icon.Offset, item.NamePixelWidth, false, UiConstants.LeftColumn.TextOffset, item);
                        break;

                    case ItemType.SubFileItemRightColumn:
                        break;

                    case ItemType.SubFileItemLeftColumn:
                        break;

                    case ItemType.Unknown:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Unhandled Project Browser Error: " + e);
                Debug.LogError($"Disabling {Main.AssetName} Asset due to unhandled error. Please report this error at: https://github.com/Placeholder-Software/FancyFolders/issues");

                Main.AssetDisabled = true;
            }
        }

        private void DrawItemLarge(Rect all, [NotNull] IFileSystemItem item)
        {
            item.DrawLarge(all, IsSelected(item.Guid));
        }

        private void DrawItemDetails(Rect all, float leftIconWidth, float iconSizeYOffset, Vector2 iconPosOffset, float titleWidth, bool highlight, Vector2 textOffset, [NotNull] IFileSystemItem item)
        {
            // Pick style based on if the item is selected, and if it's the root `Assets` item (which has it's own style in Unity)
            var style = GetStyle(item, highlight);

            // Work out where Unity has drawn the icon
            var unityFileIcon = new Rect(
                all.x + iconPosOffset.x,
                all.y + iconPosOffset.y,
                leftIconWidth,
                all.height + iconSizeYOffset
            );

            // Work out exactly where Unity has put it's text
            var unityFileText = new Rect(
                unityFileIcon.xMax + textOffset.x,
                all.y + textOffset.y,
                titleWidth,
                all.height
            );

            item.DrawDetails(all, unityFileText, unityFileIcon, style);
        }

        private GUIStyle GetStyle([NotNull] IFileSystemItem item, bool highlight)
        {
            //Pick style based on if the item is selected, and if it's the root `Assets` item (which has it's own style in Unity)
            var selected = highlight && IsSelected(item.Guid);
            var isRoot = item.AssetPath == "Assets";
            if (isRoot)
                return selected ? Styles.RootItemSelectedTextLabel : Styles.RootItemTextLabel;
            else
                return selected ? Styles.ProjectItemSelectedTextLabel : Styles.ProjectItemTextLabel;
        }

        #if UNITY_2019_3_OR_NEWER
        private static ItemType DetermineType(Rect rectangle, IFileSystemItem item)
        {
            if (rectangle.width <= rectangle.height)
                return ItemType.LargeIconRightColumn;

            if (Mathf.RoundToInt(rectangle.xMin) == 14)
                return ItemType.BasicFileItemRightColumn;

            if (Mathf.RoundToInt(rectangle.xMin) == 28)
                return ItemType.SubFileItemRightColumn;

            //Work out how many levels of indentation the item has
            var indentationF = (rectangle.xMin - 16) / 14;
            var indentation = Mathf.RoundToInt(indentationF);

            //If it isn't indented by the correct factor we don't know what this is
            if (Math.Abs(indentationF - indentation) > float.Epsilon)
                return ItemType.Unknown;

            //Determine indentation of an item in either:
            // - left column of two column view
            // - single volumn view
            var file = item as FileItem;
            if (file == null)
                return ItemType.DirectoryItemLeftColumn;

            //Work out the depth of the item in the file system, ignoring:
            // - `Assets` folder (doen't have an indent)
            // - File name (doesn't contribute to indent)
            var depth = file.AssetPath.Split('/').Length - 1;

            if (depth != indentation)
                return ItemType.SubFileItemLeftColumn;

            return ItemType.BasicFileItemLeftColumn;
        }
        #else
        private static ItemType DetermineType(Rect rectangle, IFileSystemItem item)
        {
            //not wider than height => right column large icon (i.e. not zoomed to max zoom)
            //13 => file item in right column
            //28 => file sub-item (e.g. inner parts of DLL or image) in right column
            //16 + x * 14 => item in tree view with x levels of indentation

            if (rectangle.width <= rectangle.height)
                return ItemType.LargeIconRightColumn;

            if (Mathf.RoundToInt(rectangle.xMin) == 13)
                return ItemType.BasicFileItemRightColumn;

            if (Mathf.RoundToInt(rectangle.xMin) == 28)
                return ItemType.SubFileItemRightColumn;

            //Work out how many levels of indentation the item has
            var indentationF = (rectangle.xMin - 16) / 14;
            var indentation = Mathf.RoundToInt(indentationF);

            //If it isn't indented by the correct factor we don't know what this is
            if (Math.Abs(indentationF - indentation) > float.Epsilon)
                return ItemType.Unknown;

            //Determine indentation of an item in either:
            // - left column of two column view
            // - single volumn view
            var file = item as FileItem;
            if (file == null)
                return ItemType.DirectoryItemLeftColumn;

            //Work out the depth of the item in the file system, ignoring:
            // - `Assets` folder (doen't have an indent)
            // - File name (doesn't contribute to indent)
            var depth = file.AssetPath.Split('/').Length - 1;

            if (depth != indentation)
                return ItemType.SubFileItemLeftColumn;

            return ItemType.BasicFileItemLeftColumn;
        }
        #endif

        private enum ItemType
        {
            Unknown,

            BasicFileItemRightColumn,
            SubFileItemRightColumn,
            LargeIconRightColumn,

            DirectoryItemLeftColumn,
            BasicFileItemLeftColumn,
            SubFileItemLeftColumn,
            
        }
    }
}
