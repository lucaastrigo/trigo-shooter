using UnityEngine;

namespace FancyFolders.Editor.Renderer.Textures
{
    public readonly struct ColoredTexture
    {
        [CanBeNull] public readonly Texture2D Texture;
        public readonly Color Color;

        public ColoredTexture(Texture2D texture, Color color)
        {
            Texture = texture;
            Color = color;
        }
    }
}
