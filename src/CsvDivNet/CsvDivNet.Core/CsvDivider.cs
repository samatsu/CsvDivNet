using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvDivNet.Core
{
    /// <summary>
    /// CSVの分割を行うクラス
    /// </summary>
    public class CsvDivider
    {
        delegate void DivideAsyncDelegate();

        #region イベント
        /// <summary>
        /// ファイル分割処理が開始のイベントです。
        /// </summary>
        public event EventHandler<EventArgs> FileDivideStarted;
        protected virtual void OnFileDivideStarted()
        {
            if (FileDivideStarted != null)
                FileDivideStarted(this, EventArgs.Empty);
        }
        /// <summary>
        /// ファイル分割処理完了のイベントです。
        /// </summary>
        public event EventHandler<EventArgs> FileDivideCompleted;
        protected virtual void OnFileDivideCompleted()
        {
            if (FileDivideCompleted != null)
                FileDivideCompleted(this, EventArgs.Empty);
        }
        /// <summary>
        /// ファイル分割処理失敗のイベントです。
        /// </summary>
        public event EventHandler<DivideExceptionEventArgs> FileDivideFailed;
        protected virtual void OnFileDivideFailed(DivideExceptionEventArgs args)
        {
            if (FileDivideFailed != null)
                FileDivideFailed(this, args);
        }
        /// <summary>
        /// 1分割ファイルの処理を開始のイベントです。
        /// </summary>
        public event EventHandler<DividingEventArgs> UnitFileDividing;
        protected virtual void OnUnitFileDividing(DividingEventArgs args)
        {
            if (UnitFileDividing != null)
                UnitFileDividing(this, args);
        }
        /// <summary>
        /// 1分割ファイルの処理が完了のイベントです。
        /// </summary>
        public event EventHandler<DividedEventArgs> UnitFileDivided;
        protected virtual void OnUnitFileDivided(DividedEventArgs args)
        {
            if (UnitFileDivided != null)
                UnitFileDivided(this, args);
        }
        #endregion
        /// <summary>
        /// 分割設定
        /// </summary>
        public CsvDivConfig DivSetting { get; private set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="divsetting"></param>
        public CsvDivider(CsvDivConfig divsetting)
        {
            if (divsetting == null) throw new ArgumentNullException("divsetting", "null");

            if (!divsetting.IsValid())
            {
                throw new ApplicationException("無効な分割設定です");
            }

            this.DivSetting = divsetting;
        }
        /// <summary>
        /// ファイル分割処理
        /// </summary>
        public void Divide()
        {
            try
            {
                OnFileDivideStarted();
                OnFileDivide();
                OnFileDivideCompleted();
            }
            catch (Exception ex)
            {
                OnFileDivideFailed(new DivideExceptionEventArgs(ex.Message, ex));
            }
        }
        /// <summary>
        /// 非同期ファイル分割処理
        /// </summary>
        public IAsyncResult DivideAsync(AsyncCallback callback)
        {
            DivideAsyncDelegate _divideAsync = new DivideAsyncDelegate(this.Divide);
            return _divideAsync.BeginInvoke(callback, null);
        }
        /// <summary>
        /// CsvParserを作成する
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        protected virtual ICsvParser CreateCsvParser(string path, Encoding encoding)
        {
            if (!DivSetting.UseRegacyCsvParser)
            {
                CsvTokenizer tokenizer = new CsvTokenizer(path, encoding);
                tokenizer.Delimitors = new char[] { DivSetting.Delimitor };
                tokenizer.HasFieldsEnclosedInQuotas = DivSetting.IsDQuoted;

                return tokenizer;
            }
            else
            {
                TextFieldParserAdapter textFieldParserAdapter = new TextFieldParserAdapter(path, encoding);
                textFieldParserAdapter.Delimitors = new char[] { DivSetting.Delimitor };
                textFieldParserAdapter.HasFieldsEnclosedInQuotas = DivSetting.IsDQuoted;

                return textFieldParserAdapter;
            }
        }

        /// <summary>
        /// 分割処理を行います。
        /// </summary>
        protected virtual void OnFileDivide()
        {
            using (ICsvParser reader = CreateCsvParser(DivSetting.GetInputFile().FullName, DivSetting.GetInputFileEncoding()))
            {
                string[] header = null;
                // ヘッダをセット
                if (DivSetting.HeaderMode == HeaderMode.FirstRow)
                {
                    if (reader.EndOfData) throw new ApplicationException("ヘッダが存在しません");
                    header = reader.ReadFields();
                }
                else if (DivSetting.HeaderMode == HeaderMode.ExternalFile)
                {
                    using (ICsvParser hreader = CreateCsvParser(DivSetting.GetHeaderFile().FullName, DivSetting.GetInputFileEncoding()))
                    {
                        if (hreader.EndOfData) throw new ApplicationException("ヘッダが存在しません");
                        header = hreader.ReadFields();
                    }
                }

                int fileidx = 0;
                while (!reader.EndOfData)
                {
                    fileidx++;
                    string file = DivSetting.GetOutputFilePath(fileidx);
                    OnUnitFileDividing(new DividingEventArgs(file));
                    OnUnitFileDivide(reader, file, header);
                    OnUnitFileDivided(new DividedEventArgs(file));
                }
            }
        }
        /// <summary>
        /// 分割ファイル1ファイルを出力する
        /// </summary>
        /// <param name="fileIdx"></param>
        /// <param name="header">ヘッダを出力しない場合はnull</param>
        protected virtual void OnUnitFileDivide(ICsvParser reader, string file, string[] header)
        {
            using (CSVWriter writer = new CSVWriter(file, DivSetting.GetOutputFileEncoding()))
            {
                writer.Separator = DivSetting.Delimitor.ToString();
                writer.IsTokenEnclosedWithDQuotas = DivSetting.IsDQuoted;

                // ヘッダ出力
                if (header != null)
                {
                    writer.WriteLine(header);
                }

                int linenumber = 0;
                while (!reader.EndOfData && linenumber < DivSetting.MaxRowCount)
                {
                    string[] datas = reader.ReadFields();
                    writer.WriteLine(datas);
                    linenumber++;
                }
            }
        }
    }
}
