using System.Text;
using EQuery.Sql;

namespace EQuery.Core
{
    class SqlWriter
    {
        public const char IndentChar = '\t';

        private readonly StringBuilder _sbuf = new StringBuilder();
        private int _indent = 0;

        private bool _hasIndent = false;

        public string GetResult()
        {
            return _sbuf.ToString();
        }
        
        public void Append(params string[] args)
        {
            if (!_hasIndent)
            {
                WriteIndent();
                _hasIndent = true;
            }

            foreach (var s in args)
            {
                if (_sbuf.Length > 0 && !char.IsWhiteSpace(_sbuf[_sbuf.Length - 1]))
                    _sbuf.Append(' ');                

                _sbuf.Append(s);
            }
        }

        public void AppendLine(params string[] args)
        {
            if (_sbuf.Length > 0 && !char.IsWhiteSpace(_sbuf[_sbuf.Length - 1]))
            {
                _sbuf.Append('\n');
                _hasIndent = false;
            }

            Append(args);
        }

        public void Append(ISqlNode sqlNode)
        {
            if (sqlNode == null)
                return;

            sqlNode.Render(this);
        }

        public void AppendLine(ISqlNode sqlNode)
        {
            if (sqlNode == null)
                return;

            AppendLine();
            sqlNode.Render(this);
        }

        public void Indent()
        {
            _indent++;
        }

        public void Unindent()
        {
            if (_indent > 0)
                _indent--;
        }

        private void WriteIndent()
        {
            for (var i = 0; i < _indent; i++)
            {
                _sbuf.Append(IndentChar);
            }
        }
    }
}