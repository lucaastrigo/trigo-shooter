using UnityEngine;

namespace FancyFolders.Editor.Renderer.Textures
{
    internal readonly struct CachedTexture
    {
        public readonly string Path;
        private readonly TextureResources.TextureResource _textureResource;

        [CanBeNull] public Texture2D Texture => _textureResource?.Texture;

        public CachedTexture(string path)
        {
            Path = path;
            _textureResource = TextureResources.Instance.GetResource(path);
        }
    }
}
