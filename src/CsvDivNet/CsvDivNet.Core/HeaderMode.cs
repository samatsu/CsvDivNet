using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvDivNet.Core
{
    /// <summary>
    /// ヘッダ挿入の設定
    /// </summary>
    public enum HeaderMode
    {
        /// <summary>
        /// ヘッダなし.ファイルはCSVに行分割される
        /// </summary>
        None = 0,
        /// <summary>
        /// 入力ファイルの1行目をヘッダとする。分割ファイルの1行目にヘッダ
        /// が出力される
        /// </summary>
        FirstRow = 1,
        /// <summary>
        /// 外部ファイルのデータをヘッダとする。分割ファイルの先頭にヘッダ
        /// が出力される
        /// </summary>
        ExternalFile = 2,
    }
}
