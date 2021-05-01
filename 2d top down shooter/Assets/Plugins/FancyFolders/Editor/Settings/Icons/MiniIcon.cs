using System;
using FancyFolders.Editor.Renderer.Textures;
using UnityEngine;

namespace FancyFolders.Editor.Settings.Icons
{
    /// <summary>
    /// A single icon represented in different ways for different sizes and DPI
    /// </summary>
    /// <remarks>
    /// Fallbacks:
    ///  - High DPI -> Low DPI
    ///  - Low DPI -> High DPI
    /// </remarks>
    [Serializable]
    public class MiniIcon
    {
#pragma warning disable IDE0044 // Add readonly modifier (cannot add readonly modifier, this is used by Unity serialization)
#pragma warning disable CS0649  // Field unused (it's used by serialization)
        [SerializeField, UsedImplicitly] private string _hiDpi;
        [SerializeField, UsedImplicitly] private string _loDpi;
#pragma warning restore CS0649
#pragma warning restore IDE0044

        private CachedTexture _highDpiCache;
        [CanBeNull] internal Texture2D HighDpiNoFallback
        {
            get
            {
                if (_highDpiCache.Path != _hiDpi)
                    _highDpiCache = new CachedTexture(_hiDpi);

                return _highDpiCache.Texture;
            }
        }

        private CachedTexture _lowDpiCache;
        [CanBeNull] internal Texture2D LowDpiNoFallback
        {
            get
            {
                if (_lowDpiCache.Path != _loDpi)
                    _lowDpiCache = new CachedTexture(_loDpi);

                return _lowDpiCache.Texture;
            }
        }

        [CanBeNull] private Texture2D HighDpi
        {
            get { return HighDpiNoFallback ?? LowDpiNoFallback; }
        }

        [CanBeNull] private Texture2D LowDpi
        {
            get { return LowDpiNoFallback ?? HighDpiNoFallback; }
        }

        [CanBeNull] public Texture2D Icon
        {
            get { return TextureResources.IsLowDpi ? LowDpi : HighDpi; }
        }
    }
}
