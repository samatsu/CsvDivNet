using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace CsvDivNet.Core
{
    /// <summary>
    /// TextFieldParser版ICsvParserの実装
    /// </summary>
    class TextFieldParserAdapter : ICsvParser, IDisposable
    {
        TextFieldParser _parser = null;

        #region プロパティ
        /// <summary>   
        /// コメントトークンを定義します。コメントトークン   
        /// である文字列を先頭の行に置くと、その行がコメント   
        /// であり、パーサーによって無視されることを示します。   
        /// </summary>   
        public string[] CommentTokens
        {
            get { return _parser.CommentTokens; }
            set { _parser.CommentTokens = value; }
        }

        /// <summary>   
        /// 現在の行番号を返します。ストリームから取り出す文字がなくなった   
        /// 場合は負の値を返します。   
        /// </summary>   
        public long LineNumber
        {
            get
            {
                if (_parser.LineNumber < 0) return _parser.LineNumber;
                return _parser.LineNumber - 1;
            }
        }

        /// <summary>   
        /// テキストファイルの区切り記号を定義します。   
        /// </summary>   
        public char[] Delimitors
        {
            get
            {
                return _parser.Delimiters.Select(x => x.ToCharArray()[0]).ToArray();
            }
            set
            {
                if(value == null) throw new ArgumentNullException("Delimitors");

                _parser.SetDelimiters(value.Select(x => x.ToString()).ToArray());
            }
        }

        /// <summary>   
        /// 区切り形式のファイルを解析する際、フィールドが引用符で   
        /// 囲まれているかどうかを示します   
        /// </summary>   
        public bool HasFieldsEnclosedInQuotas
        {
            get { return _parser.HasFieldsEnclosedInQuotes; }
            set { _parser.HasFieldsEnclosedInQuotes = value; }
        }
        /// <summary>   
        /// ファイルの終端か否かを返します。   
        /// 現在のカーソル位置と、ファイルの終端との間に、   
        /// 空行またはコメント行以外のデータが存在しない場合   
        /// Trueを返します。   
        /// </summary>   
        public bool EndOfData
        {
            get { return _parser.EndOfData; }
        }

        public bool TrimWhiteSpace
        {
            get { return _parser.TrimWhiteSpace; }
            set { _parser.TrimWhiteSpace = value; }
        }

        /// 最近発生した MalformedLineException 例外の原因となった行を返します。   
        /// </summary>   
        public string ErrorLine
        {
            get { return _parser.ErrorLine; }
        }
        /// <summary>   
        /// 最近 MalformedLineException 例外が発生した行の番号(0ベース)を返します。   
        /// </summary>   
        public long ErrorLineNumber
        {
            get { return _parser.ErrorLineNumber - 1; }
        }
        /// <summary>   
        /// 解析するテキスト ファイルの各列の幅を表します。   
        /// </summary>   
        public int[] FieldWidths
        {
            get { return _parser.FieldWidths; }
            set { _parser.FieldWidths = value; }
        }
        /// <summary>   
        /// フィールドが区切られているかのフラグ   
        /// </summary>   
        public bool IsFieldTypeDelimited
        {
            get { return _parser.TextFieldType == FieldType.Delimited; }
        }
        /// <summary>   
        /// フィールドが固定長かのフラグ   
        /// </summary>   
        public bool IsFieldTypeFixed
        {
            get { return _parser.TextFieldType == FieldType.FixedWidth; }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// 読み込み対象をstreamとしてCSVReaderを初期化します。
        /// </summary>
        /// <param name="stream">データを読み取るストリーム</param>
        public TextFieldParserAdapter(Stream stream)
        {
            _parser = new TextFieldParser(stream);
            Initialize();
        }
        /// <summary>
        /// 指定されたファイルを読み込むCSVReaderを作成します。
        /// </summary>
        /// <param name="path">読み取るCSVファイル</param>
        public TextFieldParserAdapter(string path)
        {
            _parser = new TextFieldParser(path);
            Initialize();
        }
        /// <summary>
        /// 指定されたストリームから指定されたエンコーディングで
        /// 読み込みを行うようにCSVReaderを初期化します。
        /// </summary>
        /// <param name="stream">読み取るストリーム</param>
        /// <param name="defaultEncoding">ストリームのエンコーディング</param>
        public TextFieldParserAdapter(Stream stream, Encoding defaultEncoding)
        {
            _parser = new TextFieldParser(stream, defaultEncoding);
            Initialize();
        }
        /// <summary>
        /// 指定されたファイルから、指定された円コーディングで
        /// 読み込みを行うようにCSVReaderを初期化します。
        /// </summary>
        /// <param name="path">読み込みファイルのパス</param>
        /// <param name="defaultEncoding">ファイルのエンコーディング</param>
        public TextFieldParserAdapter(string path, Encoding defaultEncoding)
        {
            _parser = new TextFieldParser(path, defaultEncoding);
            Initialize();
        }
        /// <summary>
        /// 指定されたTextReaderからテキストを読み込むように
        /// CSVReaderを初期化します。
        /// </summary>
        /// <param name="reader">読み取り対象のテキストリーダ</param>
        public TextFieldParserAdapter(TextReader reader)
        {
            _parser = new TextFieldParser(reader);
            Initialize();
        }
        #endregion

        #region メソッド
        /// <summary>   
        /// パーサーを閉じる   
        /// </summary>   
        public void Close()
        {
            if (_parser != null) _parser.Close();
        }
        /// <summary>   
        /// ストリームから文字をnumberOfChars文字取りだす。   
        /// 取り出した文字はストリームから削除されません。   
        /// </summary>   
        /// <param name="numberOfChars"></param>   
        /// <returns></returns>   
        public string PeekChars(int numberOfChars)
        {
            return _parser.PeekChars(numberOfChars);
        }
        /// <summary>   
        /// フィールドに分解された1行を取り出します。   
        /// </summary>   
        /// <returns></returns>   
        public string[] ReadFields()
        {
            return _parser.ReadFields();
        }
        /// <summary>   
        /// 1行文字列として読み出します。   
        /// </summary>   
        /// <returns></returns>   
        public string ReadLine()
        {
            return _parser.ReadLine();
        }
        /// <summary>   
        /// 現在のカーソル位置からStreamから全ての文字列を取り出します。   
        /// </summary>   
        /// <returns></returns>   
        public string ReadToEnd()
        {
            return _parser.ReadToEnd();
        }
        /// <summary>   
        /// 区切り文字を設定する   
        /// </summary>   
        /// <param name="delimiters"></param>   
        public void SetDelimiters(string[] delimiters)
        {
            _parser.SetDelimiters(delimiters);
        }
        /// <summary>   
        /// 固定長フィールドの場合のフィールド幅を設定する。   
        /// </summary>   
        /// <param name="fieldWidths"></param>   
        public void SetFieldWidth(int[] fieldWidths)
        {
            _parser.SetFieldWidths(fieldWidths);
        }
        /// <summary>   
        /// フィールドタイプを区切り文字に変更する   
        /// </summary>   
        public void SetFieldTypeDelimited()
        {
            _parser.TextFieldType = FieldType.Delimited;
        }
        /// <summary>   
        /// フィールドタイプを固定幅に変更する   
        /// </summary>   
        public void SetFieldTypeFixed()
        {
            _parser.TextFieldType = FieldType.FixedWidth;
        }
        /// <summary>   
        /// 初期化処理。   
        /// コンストラクタから呼び出され、初期設定を行います。   
        /// ファイルのデリミタをタブ,フィールドタイプを非固定長   
        /// TrimWhiteSpace=Falseとして初期化します。   
        /// </summary>   
        protected virtual void Initialize()
        {
            SetDelimiters(new string[] { "\t" });
            SetFieldTypeDelimited();
            TrimWhiteSpace = false;
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// 終了処理を行います。
        /// </summary>
        public virtual void Dispose()
        {
            Close();
            if (_parser != null) _parser.Dispose();
        }
        void IDisposable.Dispose()
        {
            this.Dispose();
        }

        #endregion
    }
}
