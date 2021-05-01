namespace FancyFolders.Editor.Renderer.StringFormatting
{
    public struct ByteSize
    {
        private readonly long _bytes;

        public ByteSize(long bytes)
        {
            _bytes = bytes;
        }

        public override string ToString()
        {
            const long kb = 1000;
            const long mb = kb * 1000;
            const long gb = mb * 1000;
            const long tb = gb * 1000;

            string suffix;
            var bytes = (double)_bytes;

            if (bytes > tb)
            {
                bytes /= tb;
                suffix = "TB";
            }
            else if (bytes >= gb)
            {
                bytes /= gb;
                suffix = "GB";
            }
            else if (bytes >= mb)
            {
                bytes /= mb;
                suffix = "MB";
            }
            else if (bytes >= kb)
            {
                bytes /= kb;
                suffix = "KB";
            }
            else
                suffix = "B";

            return $"{bytes:0.0}{suffix}";
        }
    }
}
