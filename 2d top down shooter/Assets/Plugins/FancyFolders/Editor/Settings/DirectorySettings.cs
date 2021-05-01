using System;
using FancyFolders.Editor.FileSystem;
using FancyFolders.Editor.Settings.Icons;
using UnityEngine;

namespace FancyFolders.Editor.Settings
{
    [Serializable]
    public class DirectorySettings
        : BaseAssetSettings
    {
        public static readonly Color DefaultBackgroundIconColor = new Color(116f / 255, 132f / 255f, 132f / 255f, 1.0f);
        [SerializeField, UsedImplicitly] private float _r = DefaultBackgroundIconColor.r;
        [SerializeField, UsedImplicitly] private float _g = DefaultBackgroundIconColor.g;
        [SerializeField, UsedImplicitly] private float _b = DefaultBackgroundIconColor.b;
        [SerializeField, UsedImplicitly] private float _a = 1;
        public Color BackgroundIconColor
        {
            get => new Color(_r, _g, _b, _a);
            set
            {
                _r = value.r;
                _g = value.g;
                _b = value.b;
                _a = value.a;

                Dirty();
            }
        }

        [SerializeField, UsedImplicitly] private float _fr = 1;
        [SerializeField, UsedImplicitly] private float _fg = 1;
        [SerializeField, UsedImplicitly] private float _fb = 1;
        [SerializeField, UsedImplicitly] private float _fa = 1;
        public Color ForegroundIconColor
        {
            get => new Color(_fr, _fg, _fb, _fa);
            set
            {
                _fr = value.r;
                _fg = value.g;
                _fb = value.b;
                _fa = value.a;

                Dirty();
            }
        }

        [SerializeField, UsedImplicitly] private IconType _overlayIcon;
        public IconType OverlayIcon
        {
            get => _overlayIcon;
            set
            {
                if (_overlayIcon != value)
                {
                    _overlayIcon = value;
                    Dirty();
                }
            }
        }

        [SerializeField, UsedImplicitly] private string _customOverlayIconId;
        public string CustomIconOverlayId
        {
            get => _customOverlayIconId;
            set
            {
                if (_customOverlayIconId != value)
                {
                    _customOverlayIconId = value;
                    Dirty();
                }
            }
        }

        private DirectorySettings()
        {
            //Used by serialization
        }

        public DirectorySettings(AssetGuid guid)
            : base(guid)
        {
        }

        [NotNull] public static DirectorySettings Create(AssetGuid guid)
        {
            return new DirectorySettings(guid);
        }

        protected override string DefaultFormatString()
        {
            return FancyFoldersSettings.Instance.DefaultDirFormatString;
        }

        public void CopyTo([NotNull] DirectorySettings settings)
        {
            base.CopyTo(settings);

            settings._r = _r;
            settings._g = _g;
            settings._b = _b;
            settings._a = _a;

            settings._fr = _fr;
            settings._fg = _fg;
            settings._fb = _fb;
            settings._fa = _fa;

            settings._overlayIcon = _overlayIcon;
        }
    }
}
