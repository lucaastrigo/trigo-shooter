using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FancyFolders.Editor.Renderer.Textures
{
    public class TextureResources
    {
        private const string Root = "Assets/Plugins/FancyFolders/Editor/EditorResources/";

        public static readonly TextureResources Instance = new TextureResources();

        internal static bool IsLowDpi => Screen.dpi <= 100;

        [NotNull] public Texture2D WhitePixel { get; } = GetCriticalTexture(Root + "WhitePixel.bmp");

        public Texture2D FolderLarge { get; } = GetCriticalTexture(Root + "Unity Icons/Folder Icons/Icon.png");

        public Texture2D FolderLargeEmpty { get; } = GetCriticalTexture(Root + "Unity Icons/Folder Icons/Icon Empty.png");

        public Texture2D Git { get; } = GetCriticalTexture(Root + "Mini Icons/icon_git_tiny.png");

        public Texture2D Audio { get; } = GetCriticalTexture(Root + "Unity Icons/AudioClip Icon.png");

        public Texture2D Script { get; } = GetCriticalTexture(Root + "Unity Icons/cs Script Icon.png");

        public Texture2D Text { get; } = GetCriticalTexture(Root + "Unity Icons/TextAsset Icon.png");

        public Texture2D Mesh { get; } = GetCriticalTexture(Root + "Unity Icons/Mesh Icon.png");

        public Texture2D Texture { get; } = GetCriticalTexture(Root + "Unity Icons/RenderTexture Icon.png");

        public Texture2D Shader { get; } = GetCriticalTexture(Root + "Unity Icons/Shader Icon.png");

        public Texture2D Prefab { get; } = GetCriticalTexture(Root + "Unity Icons/Prefab Icon.png");

        public Texture2D Unity { get; } = GetCriticalTexture(Root + "Unity Icons/SceneAsset Icon.png");

        public Texture2D Error { get; } = GetCriticalTexture(Root + "Unity Icons/console.erroricon.sml.png");

        public Texture2D ErrorGrey { get; } = GetCriticalTexture(Root + "Unity Icons/console.erroricon.sml.grey.png");

        public Texture2D Warning { get; } = GetCriticalTexture(Root + "Unity Icons/console.warnicon.sml.png");

        // ReSharper disable once InconsistentNaming
        public Texture2D Box96x110 { get; } = GetCriticalTexture(Root + "Box_96x110.png");

        private readonly Dictionary<string, TextureResource> _textureCache = new Dictionary<string, TextureResource>();

        [NotNull] private static Texture2D GetCriticalTexture([NotNull] string path)
        {
            var result = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (result == null)
            {
                Debug.LogError($"{Main.AssetName} is missing a texture resource, please reinstall from the asset store. `{path}`");
                Main.AssetDisabled = true;
            }

            // ReSharper disable once AssignNullToNotNullAttribute (we've disabled the asset, so this null texture won't cause issues)
            return result;
        }

        [CanBeNull] public Texture2D GetTexture([CanBeNull] string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            return GetResource(path).Texture;
        }

        [NotNull] internal TextureResource GetResource([CanBeNull] string path)
        {
            if (string.IsNullOrEmpty(path))
                return new TextureResource(null);

            if (!_textureCache.TryGetValue(path, out var texture))
            {
                texture = new TextureResource(path);
                _textureCache.Add(path, texture);
            }

            return texture;
        }

        internal class TextureResource
        {
            private readonly string _path;
            private Texture2D _texture;
            [CanBeNull] public Texture2D Texture
            {
                get
                {
                    if (_path == null)
                        return null;
                    if (_texture == null)
                        _texture = AssetDatabase.LoadAssetAtPath<Texture2D>(_path);

                    return _texture;
                }
            }

            public TextureResource(string path)
            {
                _path = path;
                _texture = null;
            }
        }
    }
}
