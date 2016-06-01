using CsvDivNet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvDivNet.View
{
    /// <summary>
    /// 定数を表すクラス
    /// </summary>
    class Constants
    {
        public static IList<Tuple<string,HeaderMode>> HeaderModes = CreateHeaderModes();
        public static IList<Tuple<string, char>> SupportedDelimiters = CreateSupportedDelimiters();
        public static IList<Tuple<string, string>> SupportedEncodings = CreateSupportedEncodings();
        public static IList<int> EdabanDigits = CreateEdabanDigits();
        public static IList<Tuple<string,string>> SupportedOutputFormats = CreateOutputFileFormats();

        private static IList<Tuple<string, HeaderMode>> CreateHeaderModes()
        {
            List<Tuple<string, HeaderMode>> list = new List<Tuple<string, HeaderMode>>();
            list.Add(new Tuple<string, HeaderMode>("入力ファイルの一行目", HeaderMode.FirstRow));
            list.Add(new Tuple<string, HeaderMode>("ヘッダなし", HeaderMode.None));
            list.Add(new Tuple<string, HeaderMode>("外部ファイルから差込",HeaderMode.ExternalFile));

            return list;
        }

        private static IList<Tuple<string, char>> CreateSupportedDelimiters()
        {
            List<Tuple<string, char>> list = new List<Tuple<string, char>>();
            list.Add(new Tuple<string, char>("カンマ(,)", ','));
            list.Add(new Tuple<string, char>("タブ(\\t)", '\t'));

            return list;
        }

        private static IList<Tuple<string, string>> CreateSupportedEncodings()
        {
            List<Tuple<string, string>> list = new List<Tuple<string, string>>();
            list.Add(new Tuple<string, string>("Shift_JIS", "shift_jis"));
            list.Add(new Tuple<string, string>("UTF8", Encoding.UTF8.BodyName));
            list.Add(new Tuple<string, string>("Unicode", Encoding.Unicode.BodyName));
            list.Add(new Tuple<string, string>("ASCII", Encoding.ASCII.BodyName));
            list.Add(new Tuple<string, string>("euc-jp", "euc-jp"));

            return list;
        }

        private static IList<int> CreateEdabanDigits()
        {
            List<int> list = new List<int>();
            foreach (int i in Enumerable.Range(1, 10))
            {
                list.Add(i);
            }

            return list;
        }

        private static IList<Tuple<string, string>> CreateOutputFileFormats()
        {
            List<Tuple<string, string>> list = new List<Tuple<string, string>>();
            list.Add(new Tuple<string, string>("<ベース>_<枝番>.<拡張子>", "{0}_{1:D|SEQ|}.{2}"));
            list.Add(new Tuple<string, string>("<ベース>-<枝番>.<拡張子>", "{0}-{1:D|SEQ|}.{2}"));
            list.Add(new Tuple<string, string>("<ベース>.<枝番>.<拡張子>", "{0}.{1:D|SEQ|}.{2}"));
            list.Add(new Tuple<string, string>("<ベース>.<拡張子>.<枝番>", "{0}.{2}.{1:D|SEQ|}"));

            return list;
        }

    }
}
