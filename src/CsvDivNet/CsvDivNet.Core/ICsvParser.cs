using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvDivNet.Core
{
    public interface ICsvParser : IDisposable
    {
        /// <summary>
        /// CSVのフィールドを読み込む
        /// </summary>
        /// <returns></returns>
        string[] ReadFields();
        /// <summary>
        /// CSVの現在の行番号
        /// ストリームの最後の場合-1
        /// </summary>
        long LineNumber { get; }
        /// <summary>
        /// フィールドセパレータ文字列
        /// </summary>
        Char[] Delimitors { get; set; }
        /// <summary>
        /// フィールドが修飾文字で修飾されているか
        /// trueの場合、""で囲まれていることを想定
        /// して処理を行います。
        /// </summary>
        bool HasFieldsEnclosedInQuotas { get; }
        /// <summary>
        /// ストリームの最後かどうか
        /// </summary>
        bool EndOfData { get; }
        /// <summary>
        /// フィールドの前後空白を削除するかどうか
        /// </summary>
        bool TrimWhiteSpace { get; set; }

    }
}
