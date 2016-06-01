using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.ComponentModel.DataAnnotations;
using CsvDivNet.Core.Infrastructure;

namespace CsvDivNet.Core
{
    /// <summary>
    /// 分割設定クラス
    /// </summary>
    [XmlRoot("csvconfig", Namespace = "urn:CsvDivNet")]
    public class CsvDivConfig : NotificationObject, IDataErrorInfo
    {
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CsvDivConfig()
        {
            _headerMode = HeaderMode.FirstRow;
            _headerFileName = null;
            _delimitor = ',';
            _isDQuoted = true;
            _maxRowCount = 10000;
            _seqNoDigits = 3;
            _inputFileEncodingName = "shift_jis";
            _inputFileName = null;
            _outputDirectoryName = Environment.CurrentDirectory;
            _outputFileBase = null;
            _outputFileExtention = null;
            _outputFileEncodingName = "shift_jis";
            _outputFileFormat = "{0}_{1:D|SEQ|}.{2}";
            _outputInputFileDirectory = false;
            _userRegacyCsvParser = false;
        }
        #endregion

        #region シリアライズ

        /// <summary>
        /// ファイルに保存されたコンフィグから設定クラスのインスタンスを作成する
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static CsvDivConfig LoadOrDefault(string filepath)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filepath))
                {
                    XmlSerializer x = new XmlSerializer(typeof(CsvDivConfig));
                    return x.Deserialize(reader) as CsvDivConfig;
                }
            }
            catch
            {
                return new CsvDivConfig();
            }
        }
        /// <summary>
        /// ファイルに設定クラスのインスタンスをシリアライズする。
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static void SaveTo(string filepath, CsvDivConfig config)
        {
            using (XmlWriter writer = XmlWriter.Create(filepath))
            {
                XmlSerializer x = new XmlSerializer(typeof(CsvDivConfig));
                x.Serialize(writer, config);
            }
        }
        /// <summary>
        /// ファイルに設定クラスのインスタンスをシリアライズする。
        /// </summary>
        /// <param name="filepath"></param>
        public void SaveTo(string filepath)
        {
            CsvDivConfig.SaveTo(filepath, this);
        }
        #endregion
        private HeaderMode _headerMode;
        /// <summary>
        /// 入力ファイルのヘッダを分割ファイルに埋め込むかのフラグ.
        /// trueの場合、1ファイルの最大行数にヘッダ行は含めない
        /// </summary>
        public HeaderMode HeaderMode
        {
            get { return _headerMode; }
            set
            {
                if (_headerMode != value)
                {
                    _headerMode = value;
                    RaisePropertyChanged(() => HeaderMode);
                    RaisePropertyChanged(() => HeaderFileName);
                }
            }
        }
        private char _delimitor;
        /// <summary>
        /// 入力ファイルのデータ区切文字
        /// </summary>
        [Required(ErrorMessage = "データ区切り文字は必須項目です。")]
        public char Delimitor
        {
            get { return _delimitor; }
            set
            {
                if (_delimitor != value)
                {
                    _delimitor = value;
                    RaisePropertyChanged(() => Delimitor);
                }
            }
        }
        private bool _isDQuoted;
        /// <summary>
        /// 入力ファイルのデータがダブルクォートで囲まれる場合trueを指定
        /// </summary>
        public bool IsDQuoted
        {
            get { return _isDQuoted; }
            set
            {
                if (_isDQuoted != value)
                {
                    _isDQuoted = value;
                    RaisePropertyChanged(() => IsDQuoted);
                }
            }
        }
        private long _maxRowCount;
        /// <summary>
        /// 1ファイル辺りの最大行数
        /// </summary>
        [Range(1, Int32.MaxValue)]
        [Required(ErrorMessage = "分割行数単位は必須項目です")]
        public long MaxRowCount
        {
            get { return _maxRowCount; }
            set
            {
                if (_maxRowCount != value)
                {
                    _maxRowCount = value;
                    RaisePropertyChanged(() => MaxRowCount);
                }
            }
        }
        private int _seqNoDigits;
        /// <summary>
        /// ファイル枝番桁数
        /// </summary>
        [Range(1, 10)]
        public int SeqNoDigits
        {
            get { return _seqNoDigits; }
            set
            {
                if (_seqNoDigits != value)
                {
                    _seqNoDigits = value;
                    RaisePropertyChanged(() => SeqNoDigits);
                }
            }
        }
        private string _inputFileEncodingName;
        /// <summary>
        /// 入力ファイルのエンコーディング名
        /// </summary>
        [Required(ErrorMessage = "入力ファイルの文字コードは必須項目です。")]
        public string InputFileEncodingName
        {
            get { return _inputFileEncodingName; }
            set
            {
                if (_inputFileEncodingName != value)
                {
                    _inputFileEncodingName = value;
                    RaisePropertyChanged(() => InputFileEncodingName);
                }
            }
        }
        /// <summary>
        /// 入力ファイルのエンコーディングを取得する
        /// </summary>
        /// <returns></returns>
        public Encoding GetInputFileEncoding()
        {
            return Encoding.GetEncoding(InputFileEncodingName);
        }
        private bool _outputInputFileDirectory;
        /// <summary>
        /// 入力ファイルと同じディレクトリに出力
        /// するかのフラグ
        /// </summary>
        public bool OutputInputFileDirectory
        {
            get { return _outputInputFileDirectory; }
            set
            {
                if (_outputInputFileDirectory != value)
                {
                    _outputInputFileDirectory = value;
                    RaisePropertyChanged(() => OutputInputFileDirectory);
                }
            }
        }
        private string _outputDirectoryName;
        /// <summary>
        /// 出力先ディレクトリのフルパス
        /// </summary>
        [CustomValidation(typeof(CsvDivConfig), "ValidateOutputDirectoryRequired", ErrorMessage = "出力先ディレクトリを指定してください")]
        [CustomValidation(typeof(CsvDivConfig), "ValidateDirectoryExistance", ErrorMessage = "出力先ディレクトリが存在しません")]
        public string OutputDirectoryName
        {
            get { return _outputDirectoryName; }
            set
            {
                if (_outputDirectoryName != value)
                {
                    _outputDirectoryName = value;
                    RaisePropertyChanged(() => OutputDirectoryName);
                }
            }
        }
        /// <summary>
        /// 出力先ディレクトリを取得する
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo GetOutputDirectory()
        {
            if (OutputInputFileDirectory)
            {
                return new DirectoryInfo(Path.GetDirectoryName(this.InputFileName));
            }
            else
            {
                return new DirectoryInfo(OutputDirectoryName);
            }
        }
        private string _headerFileName;
        /// <summary>
        /// ヘッダファイルのフルパス
        /// </summary>
        [CustomValidation(typeof(CsvDivConfig), "ValidateFileExistance")]
        [CustomValidation(typeof(CsvDivConfig), "ValidateHeaderFileRequired")]
        public string HeaderFileName
        {
            get { return _headerFileName; }
            set
            {
                if (_headerFileName != value)
                {
                    _headerFileName = value;
                    RaisePropertyChanged(() => HeaderFileName);
                }
            }
        }
        /// <summary>
        /// ヘッダファイルを取得する
        /// </summary>
        /// <returns></returns>
        public FileInfo GetHeaderFile()
        {
            return new FileInfo(HeaderFileName);
        }
        private string _inputFileName;
        /// <summary>
        /// 入力ファイルのフルパス
        /// </summary>
        [Required(ErrorMessage = "分割対象ファイルは必須項目です")]
        [CustomValidation(typeof(CsvDivConfig), "ValidateFileExistance")]
        public string InputFileName
        {
            get { return _inputFileName; }
            set
            {
                if (_inputFileName != value)
                {
                    _inputFileName = value;
                    RaisePropertyChanged(() => InputFileName);
                }
            }
        }
        /// <summary>
        /// 入力ファイルを取得する
        /// </summary>
        public FileInfo GetInputFile()
        {
            return new FileInfo(InputFileName);
        }
        private string _outputFileEncodingName;
        /// <summary>
        /// 出力ファイルのエンコーディング名
        /// </summary>
        [Required(ErrorMessage = "出力ファイルのエンコーディング名は必須項目です")]
        public string OutputFileEncodingName
        {
            get { return _outputFileEncodingName; }
            set
            {
                if (_outputFileEncodingName != value)
                {
                    _outputFileEncodingName = value;
                    RaisePropertyChanged(() => OutputFileEncodingName);
                }
            }
        }
        /// <summary>
        /// 出力ファイルエンコーディングを取得する
        /// </summary>
        public Encoding GetOutputFileEncoding()
        {
            return Encoding.GetEncoding(OutputFileEncodingName);
        }
        private string _outputFileBase;
        /// <summary>
        /// 出力ファイルのベース名
        /// </summary>
        [Required(ErrorMessage = "出力ファイルベース名は必須項目です")]
        public string OutputFileBase
        {
            get { return _outputFileBase; }
            set
            {
                if (_outputFileBase != value)
                {
                    _outputFileBase = value;
                    RaisePropertyChanged(() => OutputFileBase);
                }
            }
        }
        private string _outputFileExtention;
        /// <summary>
        /// 出力ファイルの拡張子
        /// </summary>
        [Required(ErrorMessage = "出力ファイルの拡張子は必須項目です")]
        public string OutputFileExtention
        {
            get { return _outputFileExtention; }
            set
            {
                if (value != null && value.StartsWith(".")) value = value.Substring(1);

                if (_outputFileExtention != value)
                {
                    _outputFileExtention = value;
                    RaisePropertyChanged(() => OutputFileExtention);
                }
            }
        }
        private string _outputFileFormat;
        /// <summary>
        /// 出力ファイル名形式
        /// </summary>
        [Required(ErrorMessage = "出力ファイル名の書式は必須項目です")]
        public string OutputFileFormat
        {
            get { return _outputFileFormat; }
            set
            {
                if (_outputFileFormat != value)
                {
                    _outputFileFormat = value;
                    RaisePropertyChanged(() => OutputFileFormat);
                }
            }
        }

        /// <summary>
        /// 出力ファイル名を取得する
        /// </summary>
        /// <param name="fileIdx">分割ファイルのインデックス</param>
        /// <returns></returns>
        public string GetOutputFilePath(int fileIdx)
        {
            if (!IsValid()) return null;

            string format = OutputFileFormat.Replace("|SEQ|", SeqNoDigits.ToString());
            string filename = string.Format(format, OutputFileBase, fileIdx, OutputFileExtention);

            return Path.Combine(GetOutputDirectory().FullName, filename);
        }
        private bool _userRegacyCsvParser = false;
        /// <summary>
        /// TextFieldParser 版 CSVParserを使用するかのフラグ
        /// </summary>
        public bool UseRegacyCsvParser
        {
            get { return _userRegacyCsvParser; }
            set
            {
                if (_userRegacyCsvParser != value)
                {
                    _userRegacyCsvParser = value;
                    RaisePropertyChanged(() => UseRegacyCsvParser);
                }
            }
        }
        #region

        /// <summary>
        /// インスタンスを検証する
        /// </summary>
        /// <returns>true エラーなし</returns>
        public bool IsValid()
        {
            try
            {
                Validator.ValidateObject(this, new ValidationContext(this, null, null), true);
            }
            catch (ValidationException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// インスタンスを検証する
        /// </summary>
        /// <returns></returns>
        public ValidationResult Valid()
        {
            try
            {
                Validator.ValidateObject(this, new ValidationContext(this, null, null), true);
            }
            catch (ValidationException ve)
            {
                return ve.ValidationResult;
            }
            return ValidationResult.Success;
        }
        #endregion

        #region カスタムヴァリデーション
        public static ValidationResult ValidateDirectoryExistance(string path, ValidationContext context)
        {
            if (string.IsNullOrEmpty(path)) return ValidationResult.Success;

            if (!Directory.Exists(path))
            {
                return new ValidationResult("ディレクトリが存在しません");
            }

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateOutputDirectoryRequired(string path, ValidationContext context)
        {
            CsvDivConfig config = context.ObjectInstance as CsvDivConfig;
            if (config == null) return ValidationResult.Success;

            if (!config.OutputInputFileDirectory && string.IsNullOrEmpty(path))
            {
                return new ValidationResult("出力ディレクトリを指定してください");
            }
            return ValidationResult.Success;
        }
        public static ValidationResult ValidateFileExistance(string path, ValidationContext context)
        {
            if (string.IsNullOrEmpty(path)) return ValidationResult.Success;

            if (!File.Exists(path))
            {
                return new ValidationResult("ファイルが存在しません");
            }
            return ValidationResult.Success;
        }
        public static ValidationResult ValidateHeaderFileRequired(string path, ValidationContext context)
        {
            CsvDivConfig config = context.ObjectInstance as CsvDivConfig;
            if (config == null) return ValidationResult.Success;

            if (config.HeaderMode == HeaderMode.ExternalFile && string.IsNullOrEmpty(path))
            {
                return new ValidationResult("ヘッダファイルを指定してください");
            }
            return ValidationResult.Success;
        }
        #endregion

        #region IDataErrorInfo の実装
        string IDataErrorInfo.Error
        {
            get { return String.Empty; }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get { return DataErrorInfoHelper.ValidateProperty(columnName, this); }
        }
        #endregion
    }
}
