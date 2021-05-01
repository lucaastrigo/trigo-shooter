using FancyFolders.Editor.Settings.Icons;

namespace FancyFolders.Editor.FileSystem
{
    internal static class StringTypeExtensions
    {
        public static FileType FileTypeFromExtension(this string ext)
        {
            if (string.IsNullOrEmpty(ext))
                return FileType.None;

            if (ext[0] == '.')
                ext = ext.Substring(1);

            switch (ext.ToLowerInvariant())
            {
                default:
                    return FileType.None;

                case "cs":
                    return FileType.Script;

                case "shader":
                case "compute":
                case "cginc":
                    return FileType.Shader;

                case "txt":
                case "md":
                case "doc":
                    return FileType.Text;

                case "mp3":
                case "ogg":
                case "wav":
                case "aif":
                case "aiff":
                case "mod":
                case "it":
                case "s3m":
                case "xm":
                case "mixer":
                    return FileType.Audio;

                case "bmp":
                case "exr":
                case "gif":
                case "hdr":
                case "iff":
                case "jpg":
                case "pict":
                case "png":
                case "psd":
                case "tga":
                case "tiff":
                    return FileType.Texture;

                case "fbx":
                case "dae":
                case "3ds":
                case "dxf":
                case "obj":
                case "skp":
                    return FileType.Mesh;

                case "mat":
                    return FileType.Material;

                case "prefab":
                    return FileType.Prefab;

                case "unity":
                    return FileType.Scene;
            }
        }
    }

    public static class FileTypeExtensions
    {
        public static MiniIconType IconType(this FileType file)
        {
            switch (file)
            {
                case FileType.Script: return MiniIconType.Script;
                case FileType.Shader: return MiniIconType.Shader;
                case FileType.Audio: return MiniIconType.Audioclip;
                case FileType.Texture: return MiniIconType.Texture;
                case FileType.Mesh: return MiniIconType.Mesh;
                case FileType.Prefab: return MiniIconType.Prefab;
                case FileType.Scene: return MiniIconType.Scene;
                case FileType.Text: return MiniIconType.Text;
                case FileType.Material: return MiniIconType.Material;

                case FileType.None:
                default:
                    return MiniIconType.None;
            }
        }
    }

    public enum FileType
    {
        /// <summary>
        /// Unknown extension, or no extension
        /// </summary>
        None,

        Script,
        Shader,
        Audio,
        Texture,
        Mesh,
        Prefab,
        Scene,
        Text,
        Material,
    }
}
