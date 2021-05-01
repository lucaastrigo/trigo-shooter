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
    ///  - Large Hi -> Large Lo -> Small Hi -> Small Lo
    ///  - Large Lo -> Large Hi -> Small Hi -> Small Lo
    ///  - Small Hi -> Large Lo -> Small Lo -> Large Hi
    ///  - Small Lo -> Small Hi -> Large Lo -> Large Hi
    /// </remarks>
    [Serializable]
    public class Icon
    {
#pragma warning disable IDE0044 // Add readonly modifier (cannot add readonly modifier, this is used by Unity serialization)
#pragma warning disable CS0649  // Field unused (it's used by serialization)
        [SerializeField, UsedImplicitly] private string _largeHiDpi;
        [SerializeField, UsedImplicitly] private string _smallHiDpi;

        [SerializeField, UsedImplicitly] private string _largeLoDpi;
        [SerializeField, UsedImplicitly] private string _smallLoDpi;
#pragma warning restore CS0649
#pragma warning restore IDE0044

        private CachedTexture _largeHighDpiCache;
        [CanBeNull] internal Texture2D LargeHighDpiNoFallback
        {
            get
            {
                if (_largeHighDpiCache.Path != _largeHiDpi)
                    _largeHighDpiCache = new CachedTexture(_largeHiDpi);

                return _largeHighDpiCache.Texture;
            }
        }

        private CachedTexture _largeLowDpiCache;
        [CanBeNull] internal Texture2D LargeLowDpiNoFallback
        {
            get
            {
                if (_largeLowDpiCache.Path != _largeLoDpi)
                    _largeLowDpiCache = new CachedTexture(_largeLoDpi);

                return _largeLowDpiCache.Texture;
            }
        }

        private CachedTexture _smallHighDpiCache;
        [CanBeNull] internal Texture2D SmallHighDpiNoFallback
        {
            get
            {
                if (_smallHighDpiCache.Path != _smallHiDpi)
                    _smallHighDpiCache = new CachedTexture(_smallHiDpi);

                return _smallHighDpiCache.Texture;
            }
        }

        private CachedTexture _smallLowDpiCache;
        [CanBeNull] internal Texture2D SmallLowDpiNoFallback
        {
            get
            {
                if (_smallLowDpiCache.Path != _smallLoDpi)
                    _smallLowDpiCache = new CachedTexture(_smallLoDpi);

                return _smallLowDpiCache.Texture;
            }
        }

        [CanBeNull] internal Texture2D LargeHighDpi
        {
            get { return LargeHighDpiNoFallback ?? LargeLowDpiNoFallback ?? SmallHighDpiNoFallback ?? SmallLowDpiNoFallback; }
        }

        [CanBeNull] internal Texture2D LargeLowDpi
        {
            get { return LargeLowDpiNoFallback ?? LargeHighDpiNoFallback ?? SmallHighDpiNoFallback ?? SmallLowDpiNoFallback; }
        }

        [CanBeNull] internal Texture2D SmallHighDpi
        {
            get { return SmallHighDpiNoFallback ?? LargeLowDpiNoFallback ?? SmallLowDpiNoFallback ?? LargeHighDpiNoFallback; }
        }

        [CanBeNull] internal Texture2D SmallLowDpi
        {
            get { return SmallLowDpiNoFallback ?? SmallHighDpiNoFallback ?? LargeLowDpiNoFallback ?? LargeHighDpiNoFallback; }
        }

        [CanBeNull] public Texture2D Large
        {
            get { return TextureResources.IsLowDpi ? LargeLowDpi : LargeHighDpi; }
        }

        [CanBeNull] public Texture2D Small
        {
            get { return TextureResources.IsLowDpi ? SmallLowDpi : SmallHighDpi; }
        }
    }
}
