using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace CsvDivNet.Core
{
    public class CsvTokenizer : ICsvParser, IDisposable
    {
        #region プロパティ
        TextReader _stream = null;

        private long _lineNumber = -1;
        public long LineNumber
        {
            get { return _lineNumber; }
        }

        private char[] _delimitors = new char[] { ',' };
        public char[] Delimitors
        {
            get { return _delimitors; }
            set
            {
                _delimitors = value;
            }
        }

        private bool _hasFieldsEnclosedInQuotas = true;
        public bool HasFieldsEnclosedInQuotas
        {
            get { return _hasFieldsEnclosedInQuotas; }
            set
            {
                _hasFieldsEnclosedInQuotas = value;
            }
        }

        public bool EndOfData
        {
            get
            {
                return _stream.Peek() == -1;
            }
        }
        private bool _trimWhiteSpace = false;
        public bool TrimWhiteSpace
        {
            get { return _trimWhiteSpace; }
            set { _trimWhiteSpace = value; }
        }

        private const Char DQuote = '\"';
        private const Char CR = '\r';
        private const Char LF = '\n';
        #endregion

        #region コンストラクタ
        public CsvTokenizer(Stream stream) : this(new StreamReader(stream)) { }
        public CsvTokenizer(string path) : this(new StreamReader(path)) { }
        public CsvTokenizer(Stream stream, Encoding encoding) : this(new StreamReader(stream, encoding)) { }
        public CsvTokenizer(string path, Encoding encoding) : this(new StreamReader(path, encoding)) { }
        public CsvTokenizer(TextReader reader)
        {
            _stream = reader;
        }
        #endregion

        /// <summary>
        /// CSV1行分のフィールドを取得する
        /// ストリームの末尾の場合は null 
        /// を返す
        /// </summary>
        /// <returns></returns>
        public string[] ReadFields()
        {
            return TrimWhiteSpaceIfRequired(InternalReadFields());
        }

        private string[] InternalReadFields()
        {
            // Streamが終了している場合は null を返す
            if (this.EndOfData)
            {
                return null;
            }
            _lineNumber++; // インクリメント
            if (HasFieldsEnclosedInQuotas)
            {
                return InternalParseReadFields();
            }
            else
            {
                return InternalNoParseReadFields();
            }
        }
        /// <summary>
        /// "によるフィールド修飾を考慮せず、
        /// 1行分のフィールドデータを取得する
        /// </summary>
        /// <returns></returns>
        private string[] InternalNoParseReadFields()
        {
            string line = _stream.ReadLine();
            return line.Split(Delimitors);
        }
        /// <summary>
        /// "によるフィールド修飾を考慮し、フィールドの解析を
        /// 行いながら1行分のフィールドデータを取得する
        /// </summary>
        /// <returns></returns>
        private string[] InternalParseReadFields()
        {
            List<string> fields = new List<string>();
            CsvField s = null;
            while ((s = ReadField()) != null)
            {
                fields.Add(s.Data);
                if (s.IsLastToken) break;
            }

            return fields.ToArray();
        }
        /// <summary>
        /// CSVのフィールドを1つ読みとる
        /// 読み込もうとしたストリームが行区切り or ストリームの
        /// 末尾の場合は null を返す
        /// </summary>
        /// <returns></returns>
        private CsvField ReadField()
        {
            // EOSは行セパレータと同義で扱う
            if (EndOfData) return ReadNakedToken();

            // 先頭文字検査
            int first = _stream.Peek();
            Char c = Convert.ToChar(first);
            if (c == DQuote)
            {
                // 修飾文字パターン
                return ReadQuotedToken();
            }
            else
            {
                return ReadNakedToken();
            }
        }
        /// <summary>
        /// 修飾されているフィールドを抽出する
        /// </summary>
        /// <returns></returns>
        private CsvField ReadQuotedToken()
        {
            bool isLastToken = false;
            // Skip Quote
            _stream.Read();

            List<Char> list = new List<char>();
            while (true)
            {
                // 終端に来ていたら終了
                if (EndOfData)
                {
                    isLastToken = true;
                    break;
                }

                int current = _stream.Peek();
                Char c = Convert.ToChar(current);
                if (c == DQuote)
                {
                    _stream.Read(); // Seek Next Char
                    if (EndOfData)
                    {
                        // ストリーム終端の場合フィールド区切り到達
                        isLastToken = true;
                        break;
                    }
                    // 次の文字を読み込んで "でエスケープされているかを判定

                    int next = _stream.Peek();
                    Char c2 = Convert.ToChar(next);
                    if (c2 == DQuote)
                    {
                        _stream.Read();
                        // ""でエスケープされているので"として扱う
                        list.Add(c2);
                    }
                    else if (IsRowSeparator(c2))
                    {
                        // フィールド区切り到達
                        _stream.ReadLine();
                        isLastToken = true;
                        break;
                    }
                    else if (IsFieldSeparator(c2))
                    {
                        // フィールド区切り到達
                        _stream.Read();
                        break;
                    }
                    else
                    {
                        string errorMessage = string.Format("CSV行番号{0}に\"がエスケープされていないフィールドがあります。", LineNumber);
                        throw new MalformedLineException(errorMessage, LineNumber);
                    }
                }
                else
                {
                    _stream.Read();
                    list.Add(c);
                }
            }
            if (list.Count == 0) return new CsvField(string.Empty, isLastToken);

            StringBuilder builder = new StringBuilder();
            builder.Append(list.ToArray());

            return new CsvField(builder.ToString(), isLastToken);
            // not work
            //return new string(list.ToArray());
        }
        private CsvField ReadNakedToken()
        {
            bool isLastToken = false;
            List<Char> list = new List<char>();
            while (true)
            {
                // 終端に来ていたら終了
                if (EndOfData)
                {
                    isLastToken = true;
                    break;
                }
                int current = _stream.Peek();

                Char c = Convert.ToChar(current);
                if (IsRowSeparator(c))
                {
                    // フィールド区切り到達
                    isLastToken = true;
                    _stream.ReadLine();
                    break;
                }
                else if (IsFieldSeparator(c))
                {
                    // フィールド区切り到達
                    _stream.Read();
                    break;
                }
                else
                {
                    _stream.Read();
                    list.Add(c);
                }
            }
            if (list.Count == 0) return new CsvField(string.Empty, isLastToken);

            StringBuilder builder = new StringBuilder();
            builder.Append(list.ToArray());

            return new CsvField(builder.ToString(), isLastToken);
            // not work !!
            //return new string(list.ToArray());

        }

        /// <summary>
        /// TrimWhiteSpace が true の場合に、各文字列の前後の空白を削除する
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private string[] TrimWhiteSpaceIfRequired(string[] fields)
        {
            // null の場合は何もしない
            if (fields == null) return fields;

            if (TrimWhiteSpace)
            {
                return fields.Select(x => x.Trim()).ToArray();
            }
            else
            {
                return fields;
            }

        }
        /// <summary>
        /// 行セパレーターか判定
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool IsRowSeparator(Char c)
        {
            return c == CR || c == LF;
        }
        /// <summary>
        /// フィールドセパレーターか判定
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool IsFieldSeparator(Char c)
        {
            foreach (char d in Delimitors)
            {
                if (d.Equals(c)) return true;
            }
            return false;
        }

        #region IDisposable の実装
        public void Dispose()
        {
            if (_stream != null) _stream.Dispose();
        }
        void IDisposable.Dispose()
        {
            this.Dispose();
        }
        #endregion
    }
}
