using System;
using System.Text;
using FancyFolders.Editor.FileSystem;

namespace FancyFolders.Editor.Renderer.StringFormatting
{
    public struct FileInfoFormatter
    {
        private readonly string _formatString;
        private readonly FileItem _source;

        public FileInfoFormatter([NotNull] string formatString, [NotNull] FileItem source)
        {
            _formatString = formatString ?? throw new ArgumentNullException(nameof(formatString));
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public override string ToString()
        {
            var buffer = new StringBuilder(_formatString.Length * 5);

            var formatter = new StringFormatter(buffer, _formatString);
            while (formatter.MoveNext())
            {
                if (formatter.IsToken("{name}"))
                    formatter.ReplaceToken(_source.Name);
                else if (formatter.IsToken("{size}"))
                    formatter.ReplaceToken(new ByteSize(_source.Size));
                else if (formatter.IsToken("{ext}"))
                    formatter.ReplaceToken(_source.Extension);
                else if (formatter.IsToken("{guid}"))
                    formatter.ReplaceToken(_source.Guid);
                else if (formatter.IsToken("{status}"))
                    formatter.ReplaceToken(_source.VcsStatus.ToString());
                else
                    formatter.PassToken();
            }

            return buffer.ToString();
        }
    }
}
