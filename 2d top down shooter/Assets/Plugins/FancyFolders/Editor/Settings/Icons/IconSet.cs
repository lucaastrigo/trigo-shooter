using System;
using UnityEngine;
using System.Collections.Generic;
using FancyFolders.Editor.Serialization;

namespace FancyFolders.Editor.Settings.Icons
{
    [Serializable]
    public class IconSet
        : ScriptableObject
    {
        public const string DefaultPath = "Assets/Plugins/FancyFolders/Default Icon Set.asset";

        [SerializeField, UsedImplicitly] public Icon Android;
        [SerializeField, UsedImplicitly] public Icon Animation;
        [SerializeField, UsedImplicitly] public Icon Audio;
        [SerializeField, UsedImplicitly] public Icon Script;
        [SerializeField, UsedImplicitly] public Icon Font;
        [SerializeField, UsedImplicitly] public Icon IOS;
        [SerializeField, UsedImplicitly] public Icon Linux;
        [SerializeField, UsedImplicitly] public Icon Lock;
        [SerializeField, UsedImplicitly] public Icon MacOS;
        [SerializeField, UsedImplicitly] public Icon Material;
        [SerializeField, UsedImplicitly] public Icon Mesh;
        [SerializeField, UsedImplicitly] public Icon Music;
        [SerializeField, UsedImplicitly] public Icon Texture;
        [SerializeField, UsedImplicitly] public Icon Plugin;
        [SerializeField, UsedImplicitly] public Icon Prefab;
        [SerializeField, UsedImplicitly] public Icon Resource;
        [SerializeField, UsedImplicitly] public Icon Extension;
        [SerializeField, UsedImplicitly] public Icon Steam;
        [SerializeField, UsedImplicitly] public Icon Text;
        [SerializeField, UsedImplicitly] public Icon Trash;
        [SerializeField, UsedImplicitly] public Icon Scene;
        [SerializeField, UsedImplicitly] public Icon Unlock;
        [SerializeField, UsedImplicitly] public Icon WebGL;
        [SerializeField, UsedImplicitly] public Icon Windows;

        [SerializeField, UsedImplicitly] private List<SerializableStringIconPair> _customIcons = new List<SerializableStringIconPair>();
        public int CustomIconCount
        {
            get { return _customIcons.Count; }
        }

        [SerializeField, UsedImplicitly] public MiniIcon MiniError;
        [SerializeField, UsedImplicitly] public MiniIcon MiniWarning;
        [SerializeField, UsedImplicitly] public MiniIcon MiniVcsConflict;
        [SerializeField, UsedImplicitly] public MiniIcon MiniVcsModified;
        [SerializeField, UsedImplicitly] public MiniIcon MiniVcsAdded;
        //[SerializeField, UsedImplicitly] public MiniIcon MiniVcsLockedOther;
        //[SerializeField, UsedImplicitly] public MiniIcon MiniVcsLockedSelf;
        [SerializeField, UsedImplicitly] public MiniIcon MiniAudioClip;
        [SerializeField, UsedImplicitly] public MiniIcon MiniScript;
        [SerializeField, UsedImplicitly] public MiniIcon MiniMesh;
        [SerializeField, UsedImplicitly] public MiniIcon MiniPrefab;
        [SerializeField, UsedImplicitly] public MiniIcon MiniTexture;
        [SerializeField, UsedImplicitly] public MiniIcon MiniScene;
        [SerializeField, UsedImplicitly] public MiniIcon MiniShader;
        [SerializeField, UsedImplicitly] public MiniIcon MiniText;
        [SerializeField, UsedImplicitly] public MiniIcon MiniMaterial;

        [CanBeNull] public Icon GetIcon(IconType icon, string customId)
        {
            switch (icon)
            {
                case IconType.Android: return Android;
                case IconType.Animation: return Animation;
                case IconType.Audio: return Audio;
                case IconType.Script: return Script;
                case IconType.Font: return Font;
                case IconType.iOS: return IOS;
                case IconType.Linux: return Linux;
                case IconType.Lock: return Lock;
                case IconType.MacOS: return MacOS;
                case IconType.Material: return Material;
                case IconType.Mesh: return Mesh;
                case IconType.Music: return Music;
                case IconType.Texture: return Texture;
                case IconType.Plugin: return Plugin;
                case IconType.Prefab: return Prefab;
                case IconType.Resource: return Resource;
                case IconType.Extension: return Extension;
                case IconType.Steam: return Steam;
                case IconType.Text: return Text;
                case IconType.Trash: return Trash;
                case IconType.Unlock: return Unlock;
                case IconType.Scene: return Scene;
                case IconType.WebGL: return WebGL;
                case IconType.Windows: return Windows;

                case IconType.Custom:
                    if (string.IsNullOrWhiteSpace(customId))
                        return null;
                    else
                        return GetCustomIcon(customId);

                case IconType.None:
                default: return null;
            }
        }

        [CanBeNull] public Icon GetCustomIcon(string id)
        {
            if (_customIcons == null || _customIcons.Count == 0)
                return null;

            var index = FindCustomIcon(id);
            if (index < 0)
                return null;
            else
                return _customIcons[index].Icon;
        }

        [NotNull] public Icon GetOrAddCustomIcon(string id)
        {
            var index = FindCustomIcon(id);
            if (index < 0)
            {
                index = _customIcons.Count;
                _customIcons.Add(new SerializableStringIconPair(id, new Icon()));
            }

            return _customIcons[index].Icon;
        }

        private int FindCustomIcon(string id)
        {
            if (_customIcons == null)
                _customIcons = new List<SerializableStringIconPair>();

            for (var i = 0; i < _customIcons.Count; i++)
                if (_customIcons[i].Name == id)
                    return i;

            return -1;
        }

        [CanBeNull] public MiniIcon GetMiniIcon(MiniIconType icon)
        {
            switch (icon)
            {
                
                case MiniIconType.Error: return MiniError;
                case MiniIconType.Warning: return MiniWarning;

                case MiniIconType.VcsConflict: return MiniVcsConflict;
                case MiniIconType.VcsModified: return MiniVcsModified;
                case MiniIconType.VcsAdded: return MiniVcsAdded;
                //case MiniIconType.VcsLockedOther: return MiniVcsLockedOther;
                //case MiniIconType.VcsLockedSelf: return MiniVcsLockedSelf;

                case MiniIconType.Audioclip: return MiniAudioClip;
                case MiniIconType.Script: return MiniScript;
                case MiniIconType.Mesh: return MiniMesh;
                case MiniIconType.Prefab: return MiniPrefab;
                case MiniIconType.Texture: return MiniTexture;
                case MiniIconType.Scene: return MiniScene;
                case MiniIconType.Shader: return MiniShader;
                case MiniIconType.Text: return MiniText;
                case MiniIconType.Material: return MiniMaterial;
                default: return null;
            }
        }
    }
}
