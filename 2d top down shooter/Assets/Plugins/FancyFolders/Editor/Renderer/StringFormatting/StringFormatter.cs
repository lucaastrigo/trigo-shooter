using System.Text;

namespace FancyFolders.Editor.Renderer.StringFormatting
{
    public struct StringFormatter
    {
        private readonly StringBuilder _builder;
        private readonly string _formatString;

        private int _formatStringConsumed;

        private bool _hasToken;
        private int _tokenLength;

        public StringFormatter(StringBuilder builder, string formatString)
        {
            _builder = builder;
            _formatString = formatString;

            _hasToken = false;
            _tokenLength = -1;

            _formatStringConsumed = 0;
        }

        /// <summary>
        /// Find the next token in the format string, copying everything else into the string buffer
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            //Find start of token, copying everything we pass over into the output buffer
            while (_formatStringConsumed < _formatString.Length)
            {
                var c = _formatString[_formatStringConsumed];
                if (c == '{')
                    break;

                _builder.Append(c);
                _formatStringConsumed++;
            }

            //If we've reached the end of the format string exit now
            if (_formatStringConsumed >= _formatString.Length)
                return false;

            //Find the end of the token
            // [_formatStringConsumed + 1] == '{'
            var i = _formatStringConsumed;
            _tokenLength = 1;
            while (i < _formatString.Length - 1)
            {
                i++;
                _tokenLength++;

                var c = _formatString[i];
                if (c == '}')
                {
                    _hasToken = true;
                    return true;
                }
            }

            //If we get here then we didn't find the end of the token, rewind and copy everything we passed over into the buffer
            _builder.Append(_formatString, _formatStringConsumed, _formatString.Length - _formatStringConsumed);
            return false;
        }

        /// <summary>
        /// Check if the current token is the given string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsToken(string name)
        {
            if (!_hasToken)
                return false;

            if (name.Length != _tokenLength)
                return false;

            for (var i = 0; i < _tokenLength; i++)
            {
                var tok = _formatString[i + _formatStringConsumed];
                if (tok != name[i])
                    return false;
            }

            return true;
        }

        public void ReplaceToken(string str)
        {
            _builder.Append(str);

            _formatStringConsumed += _tokenLength;
            _hasToken = false;
        }

        public void ReplaceToken<T>([NotNull] T item)
        {
            ReplaceToken(item.ToString());
        }

        /// <summary>
        /// Append the current token to the string buffer unchanged
        /// </summary>
        public void PassToken()
        {
            _builder.Append(_formatString, _formatStringConsumed, _tokenLength);

            _formatStringConsumed += _tokenLength;
            _hasToken = false;
            _tokenLength = 0;
        }
    }
}
