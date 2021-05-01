using FancyFolders.Editor.Settings;
using UnityEngine;

namespace FancyFolders.Editor.FileSystem
{
    /// <summary>
    /// Represents a node in the file system (e.g. file or directory)
    /// </summary>
    public interface IFileSystemItem
    {
        /// <summary>
        /// Get a value indicating if this is a file (if not, it must be a directory)
        /// </summary>
        bool IsFile { get; }

        /// <summary>
        /// The Unity GUID for this item
        /// </summary>
        AssetGuid Guid { get; }

        /// <summary>
        /// Get the name of this item
        /// </summary>
        [NotNull] string Name { get; }

        /// <summary>
        /// The full path to this item relative to the drive root
        /// </summary>
        /// <example>C:\Unity\ProjectName\Assets\Filename.txt</example>
        [NotNull] string FullPath { get; }

        /// <summary>
        /// The asset path to this item relative to the project root
        /// </summary>
        /// <example>Assets\Filename.txt</example>
        [NotNull] string AssetPath { get; }

        /// <summary>
        /// Get the parent directory which contains this item. May be null if this is the root directory
        /// </summary>
        [CanBeNull] DirectoryItem Parent { get; }

        /// <summary>
        /// Get the width of the name of this asset in pixels
        /// </summary>
        float NamePixelWidth { get; }

        /// <summary>
        /// Draw this item into the UI in detail mode (i.e. maximum zoom)
        /// </summary>
        /// <param name="unityIconArea">Where Unity has drawn the icon for this item</param>
        /// <param name="baseStyle">The style to use for text</param>
        /// <param name="allArea">The entire area being rendered into</param>
        /// <param name="unityTextArea">Where Unity has drawn the text for this item</param>
        void DrawDetails(Rect allArea, Rect unityTextArea, Rect unityIconArea, GUIStyle baseStyle);

        /// <summary>
        /// Draw this item into the UI in large mode (i.e. any non-max zoom on the right side)
        /// </summary>
        /// <param name="allArea"></param>
        /// <param name="isSelected"></param>
        void DrawLarge(Rect allArea, bool isSelected);
    }

    public interface IFileSystemItem<out TSettings>
        : ISettingsProvider<TSettings>, IFileSystemItem
        where TSettings : BaseAssetSettings
    {
    }
}
