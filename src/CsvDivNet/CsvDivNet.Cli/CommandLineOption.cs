using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CsvDivNet.Core;

namespace CsvDivNet
{
    /// <summary>
    /// コマンドラインオプションクラス
    /// </summary>
    class CommandLineOption
    {
        string[] _args = null;
        public CommandLineOption(string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");

            _args = args;
        }

        #region オプション一覧
        /// <summary>
        /// ヘッダーモードオプション
        /// </summary>
        public static string HeaderModeOption
        {
            get { return "/headermode:"; }
        }
        /// <summary>
        /// ヘッダファイル指定オプション
        /// </summary>
        public static string HeaderFileOption
        {
            get { return "/headerfile:"; }
        }
        /// <summary>
        /// 項目デリミタ指定オプション
        /// </summary>
        public static string DelimitorOption
        {
            get { return "/delimitor:"; }
        }
        /// <summary>
        /// 入力ファイルが2重引用符で項目を修飾しているかの指定オプション
        /// </summary>
        public static string DQuotedOption
        {
            get { return "/dquoted:"; }
        }
        /// <summary>
        /// 分割するレコード単位指定オプション
        /// </summary>
        public static string MaxRowCountOption
        {
            get { return "/maxrowcount:"; }
        }
        /// <summary>
        /// 最大想定分割ファイル数指定オプション。
        /// </summary>
        public static string SeqNoDigitsOption
        {
            get { return "/seqnodigits:"; }
        }
        /// <summary>
        /// 入力ファイルエンコーディング指定オプション
        /// </summary>
        public static string InputEncodingOption
        {
            get { return "/inputencoding:"; }
        }
        /// <summary>
        /// 入力ファイル指定オプション
        /// </summary>
        public static string InputFileNameOption
        {
            get { return "/inputfile:"; }
        }
        /// <summary>
        /// 出力ディレクトリ指定オプション
        /// </summary>
        public static string OutputDirectoryOption
        {
            get { return "/outputdir:"; }
        }
        /// <summary>
        /// 出力ファイルの基本名指定オプション
        /// </summary>
        public static string OutputFileBaseOption
        {
            get { return "/outputbase:"; }
        }
        /// <summary>
        /// 出力ファイルの拡張指定オプション
        /// </summary>
        public static string OutputExtOption
        {
            get { return "/outputext:"; }
        }
        /// <summary>
        /// 出力ファイルエンコーディング指定オプション
        /// </summary>
        public static string OutputEncodingOption
        {
            get { return "/outputencoding:"; }
        }
        /// <summary>
        /// 出力ファイルフォーマット
        /// </summary>
        public static string OutputFileFormatOption
        {
            get { return "/outputfileformat:"; }
        }
        /// <summary>
        /// 設定ファイルオプション
        /// </summary>
        public static string ConfigOption
        {
            get { return "/config:"; }
        }
        /// <summary>
        /// 入力ファイルと同じフォルダにファイルを出力
        /// するオプション
        /// </summary>
        public static string OutputInputFileDirectoryOption
        {
            get { return "/outputinputfiledirectory:"; }
        }
        /// <summary>
        /// TextFieldParserを使用してCSVを解析する
        /// モードスイッチ
        /// </summary>
        public static string UseRegacyCsvParserSwitch
        {
            get { return "/useregacycsvparser"; }
        }
        /// <summary>
        /// コンソールモードスイッチ
        /// </summary>
        public static string ConsoleSwitch
        {
            get { return "/console"; }
        }
        /// <summary>
        /// ヘルプスイッチ.
        /// </summary>
        public static string HelpSwitch
        {
            get { return "/help"; }
        }
        #endregion

        #region 引数の処理
        public bool HasHelpSwitch()
        {
            return !string.IsNullOrEmpty(GetArgument(HelpSwitch));
        }
        public CsvDivConfig CreateConfig()
        {
            // 設定ファイルが指定された場合、設定ファイルをロード
            string option = GetArgument(ConfigOption);
            string path = string.Empty;
            if (!string.IsNullOrEmpty(option))
            {
                path = option.Substring(ConfigOption.Length);
                if (!System.IO.File.Exists(path))
                {
                    throw new ApplicationException("設定ファイルのパスが不正です.");
                }
            }
            CsvDivConfig config = CsvDivConfig.LoadOrDefault(path);

            #region 詳細オプションの設定
            option = GetArgument(HeaderModeOption);
            if (!string.IsNullOrEmpty(option))
            {
                config.HeaderMode = (HeaderMode)Enum.Parse(typeof(HeaderMode), option.Substring(HeaderModeOption.Length));
            }
            option = GetArgument(HeaderFileOption);
            if (!string.IsNullOrEmpty(option))
            {
                config.HeaderFileName = option.Substring(HeaderFileOption.Length);
            }
            option = GetArgument(DelimitorOption);
            if (!string.IsNullOrEmpty(option))
            {
                string c = option.Substring(DelimitorOption.Length);
                if(c.Length != 1) throw new ApplicationException("フィールド区切り文字は1文字のみサポートしています。");
                config.Delimitor = c[0];
            }
            option = GetArgument(DQuotedOption);
            if (!string.IsNullOrEmpty(option))
            {
                config.IsDQuoted = bool.Parse(option.Substring(DQuotedOption.Length));
            }
            option = GetArgument(MaxRowCountOption);
            if (!string.IsNullOrEmpty(option))
            {
                config.MaxRowCount = long.Parse(option.Substring(MaxRowCountOption.Length));
            }
            option = GetArgument(SeqNoDigitsOption);
            if (!string.IsNullOrEmpty(option))
            {
                config.SeqNoDigits = int.Parse(option.Substring(SeqNoDigitsOption.Length));
            }
            option = GetArgument(InputEncodingOption);
            if (!string.IsNullOrEmpty(option))
            {
                config.InputFileEncodingName = option.Substring(InputEncodingOption.Length);
            }
            option = GetArgument(InputFileNameOption);
            if (!string.IsNullOrEmpty(option))
            {
                config.InputFileName = option.Substring(InputFileNameOption.Length);
            }
            option = GetArgument(OutputDirectoryOption);
            if (!string.IsNullOrEmpty(option))
            {
                config.OutputDirectoryName = option.Substring(OutputDirectoryOption.Length);
            }
            option = GetArgument(OutputFileBaseOption);
            if (!string.IsNullOrEmpty(option))
            {
                config.OutputFileBase = option.Substring(OutputFileBaseOption.Length);
            }
            option = GetArgument(OutputExtOption);
            if (!string.IsNullOrEmpty(option))
            {
                config.OutputFileExtention = option.Substring(OutputExtOption.Length);
            }
            option = GetArgument(OutputEncodingOption);
            if (!string.IsNullOrEmpty(option))
            {
                config.OutputFileEncodingName = option.Substring(OutputEncodingOption.Length);
            }
            option = GetArgument(OutputInputFileDirectoryOption);
            if (!string.IsNullOrEmpty(option))
            {
                config.OutputInputFileDirectory = bool.Parse(option.Substring(OutputInputFileDirectoryOption.Length));
            }
            if (!string.IsNullOrEmpty(GetArgument(UseRegacyCsvParserSwitch)))
            {
                config.UseRegacyCsvParser = true;
            }
            option = GetArgument(OutputFileFormatOption);
            if (!string.IsNullOrEmpty(option))
            {
                config.OutputFileFormat = option.Substring(OutputFileFormatOption.Length);
            }
            #endregion

            return config;
        }
        /// <summary>
        /// コマンドライン引数を取得する
        /// </summary>
        /// <param name="optionOrSwitch"></param>
        /// <returns>見つかった場合、該当引数。見つからない場合null</returns>
        public string GetArgument(string optionOrSwitch)
        {
            foreach (string s in _args)
            {
                if (s.StartsWith(optionOrSwitch)) return s;
            }
            return null;
        }
        #endregion

        #region ヘルプメッセージ表示
        /// <summary>
        /// ヘルプメッセージ
        /// </summary>
        public static string GetHelpMessage()
        {
            // 0:オプション, 1:説明
            string optionMessage = "{0} - {1}";
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Usage:");
            builder.Append(Process.GetCurrentProcess().ProcessName);
            builder.Append(" " + CommandLineOption.ConfigOption + "configfile");
            builder.Append(" " + CommandLineOption.HeaderModeOption + "headermode");
            builder.Append(" " + CommandLineOption.HeaderFileOption + "headerfile");
            builder.Append(" " + CommandLineOption.DelimitorOption + "delimitor");
            builder.Append(" " + CommandLineOption.DQuotedOption + "true/false");
            builder.Append(" " + CommandLineOption.MaxRowCountOption + "maxrowcount");
            builder.Append(" " + CommandLineOption.SeqNoDigitsOption + "seqnodigits");
            builder.Append(" " + CommandLineOption.InputEncodingOption + "inputencoding");
            builder.Append(" " + CommandLineOption.InputFileNameOption + "inputfile");
            builder.Append(" " + CommandLineOption.OutputDirectoryOption + "outputdirectory");
            builder.Append(" " + CommandLineOption.OutputFileBaseOption + "outputfilebase");
            builder.Append(" " + CommandLineOption.OutputExtOption + "outputext");
            builder.Append(" " + CommandLineOption.OutputEncodingOption + "outputencoding");
            builder.Append(" " + CommandLineOption.OutputFileFormatOption + "outputfileformat");
            builder.Append(" " + CommandLineOption.OutputInputFileDirectoryOption + "true/false");
            builder.Append(" " + CommandLineOption.UseRegacyCsvParserSwitch);
            builder.Append(" " + CommandLineOption.HelpSwitch);
            builder.AppendLine(string.Empty);

            builder.AppendLine("オプション説明(各オプションはコンソールモードの場合のみ有効)");
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.ConsoleSwitch, "互換性のために残されています。このスイッチは無視されます。"));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.ConfigOption, "CSV分割設定ファイルを指定します.ウィンドウモードの場合無視されます."));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.HeaderModeOption, "ヘッダモード。次のいづれかを指定None,FirstRow,ExternalFile"));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.HeaderFileOption, "外部ヘッダファイルを指定。ヘッダモードで、ExternalFileを指定した場合のみ有効です."));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.DelimitorOption, "入力ファイルのデータ区切り文字を指定."));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.DQuotedOption, "入力ファイルが二重引用符で修飾されているかをtrue or false で指定します."));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.MaxRowCountOption, "1分割ファイルあたりの行数を指定します.ヘッダ行は含まれません."));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.SeqNoDigitsOption, "分割ファイルの枝番の桁数.桁数に応じて分割ファイルの枝版の数字の桁が変化します."));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.InputEncodingOption, "入力ファイルの文字エンコードを指定します.shift_jis, UTF8, Unicode 等を指定できます."));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.InputFileNameOption, "入力ファイルを指定します."));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.OutputDirectoryOption, "出力ディレクトリを指定します."));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.OutputFileBaseOption, "分割ファイルの基本名を指定します.分割ファイル名は基本名に枝番、拡張子が付与された文字列になります."));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.OutputExtOption, "分割ファイルの拡張子を指定します."));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.OutputEncodingOption, "出力ファイルの文字エンコードを指定します。shift_jis, UTF8, Unicode 等を指定できます."));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.OutputFileFormatOption, "出力ファイルの名前形式です(例：{0}_{1:D|SEQ|}.{2}).{0}はベースファイル名,{1:D|SEQ|}は枝番,{2}は拡張子です。"));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.OutputInputFileDirectoryOption, "入力ファイルと同じディレクトリにファイルを出力するか true or false で指定します."));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.UseRegacyCsvParserSwitch, "TextFieldParserを使用してCSVファイルの解析を行います."));
            builder.AppendLine(string.Format(optionMessage, CommandLineOption.HelpSwitch, "このメッセージを表示します."));

            builder.AppendLine("補足説明");
            builder.AppendLine(CommandLineOption.ConfigOption + "が指定されると、コンソールモードで起動します。指定されない場合ウィンドウモードで起動します。");
            builder.AppendLine("コンソールモードでのみ、各オプションが有効になります。");
            builder.AppendLine(CommandLineOption.ConfigOption + "を使用することで、CSV分割の設定ファイルを指定でき多くのオプションの入力を省略できます.");
            builder.AppendLine("設定ファイルの書式は、実行ファイルと同じパスのconfig.xmlファイルです。ウィンドウモードで分割を行ったことがない場合、このファイルは存在しない可能性があります。");
            builder.AppendLine("config.xmlにはウィンドウモードで直近で分割を行ったときの設定が記録されています.");
            builder.AppendLine("設定ファイルを指定しかつ、その他の各オプションが指定された場合、指定されたオプションが優先されます。");
            builder.AppendLine("そのため、基本設定のみ設定ファイルで指定し、動的に変化する項目だけコマンドラインオプションで指定する使い方ができます。");

            return builder.ToString();
        }
        #endregion
    }
}
