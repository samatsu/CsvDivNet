using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace CsvDivNet.Core
{
    public class CSVWriter : IDisposable
    {
        /// <summary>出力ストリーム</summary>   
        StreamWriter _writer = null;

        #region コンストラクタ
        /// <summary>   
        /// 指定されたストリームにデフォルトのエンコーディングで   
        /// 出力を行います。   
        /// </summary>   
        /// <param name="stream"></param>   
        public CSVWriter(Stream stream)
        {
            _writer = new StreamWriter(stream, Encoding.Default);
        }
        /// <summary>   
        /// 指定されたストリームに指定されたエンコーディングで   
        /// 出力を行います。   
        /// </summary>   
        /// <param name="stream"></param>   
        /// <param name="encoding"></param>   
        public CSVWriter(Stream stream, Encoding encoding)
        {
            _writer = new StreamWriter(stream, encoding);
        }
        /// <summary>   
        /// 指定されたパスにデフォルトエンコーディングで   
        /// 出力を行います。   
        /// </summary>   
        /// <param name="path"></param>   
        public CSVWriter(String path)
        {
            _writer = new StreamWriter(path, false, Encoding.Default);
        }
        /// <summary>   
        /// 指定されたパスに指定されたエンコーディングで   
        /// 出力を行います。   
        /// </summary>   
        /// <param name="path"></param>   
        /// <param name="encoding"></param>   
        public CSVWriter(String path, Encoding encoding)
        {
            _writer = new StreamWriter(path, false, encoding);
        }
        #endregion

        #region プロパティ
        private bool _isTokenEnclosedWithDQuotas = false;
        /// <summary>   
        /// フィールドがダブルクオーテーションで囲まれるかのプロパティ   
        /// trueの場合ダブルクオーテーションで囲まれる   
        /// </summary>   
        public bool IsTokenEnclosedWithDQuotas
        {
            get { return _isTokenEnclosedWithDQuotas; }
            set { _isTokenEnclosedWithDQuotas = value; }
        }

        private string _separator = "\t";
        /// <summary>   
        /// 項目間の区切り文字   
        /// </summary>   
        public string Separator
        {
            get { return _separator; }
            set { _separator = value; }
        }

        #endregion

        #region メソッド
        /// <summary>   
        /// CSV出力ストリームに1行分出力する   
        /// </summary>   
        /// <param name="tokens"></param>   
        public virtual void WriteLine(string[] tokens)
        {
            for (int i = 0; i < tokens.Length; ++i)
            {
                _writer.Write(DQuote(tokens[i]));
                if (i != tokens.Length - 1)
                {
                    _writer.Write(Separator);
                }
            }
            _writer.Write(Microsoft.VisualBasic.Constants.vbCrLf);
            _writer.Flush();
        }
        /// <summary>   
        /// トークンを必要に応じてダブルクオーテーションで加工する。   
        /// </summary>   
        /// <param name="token"></param>   
        /// <returns></returns>   
        protected virtual string DQuote(string token)
        {
            bool needDQuote = IsTokenEnclosedWithDQuotas;
            if (token.Contains(Separator) || token.Contains("\r") || token.Contains("\n"))
            {
                needDQuote = true;
            }
            if (needDQuote)
            {
                token = token.Replace("\"", "\"\"");
                token = "\"" + token + "\"";
            }
            return token;
        }
        public void Flush()
        {
            _writer.Flush();
        }
        public void Close()
        {
            if (_writer != null) _writer.Close();
        }
        /// <summary>   
        /// sourceに指定されている内容でCSVファイルを   
        /// 出力します。列はsourceの列順に出されます。   
        /// </summary>   
        /// <param name="source"></param>   
        public virtual void WriteOnce(DataTable source)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < source.Columns.Count; ++i)
            {
                list.Add(i);
            }

            WriteOnce(source, list);
        }
        /// <summary>   
        /// sourceに指定されている内容でCSVファイルを出力します。   
        /// 出力する列はcolumnOrderの順に出力されます。   
        /// </summary>   
        /// <param name="source"></param>   
        /// <param name="columnOrder"></param>   
        public virtual void WriteOnce(DataTable source, IList<int> columnOrder)
        {
            foreach (DataRow row in source.Rows)
            {
                List<string> list = new List<string>();
                foreach (int colIdx in columnOrder)
                {
                    list.Add(row[colIdx].ToString());
                }
                WriteLine(list.ToArray());
            }
        }
        #endregion

        #region IDisposable Members
        public virtual void Dispose()
        {
            Close();
            if (_writer != null) _writer.Dispose();
        }
        void IDisposable.Dispose()
        {
            this.Dispose();
        }

        #endregion

    }
}
