using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvDivNet.Core
{
    /// <summary>
    /// CsvTokenizer で使用される
    /// Csvの1データフィールドを表すクラス
    /// </summary>
    internal class CsvField
    {
        public bool IsLastToken { get; private set; }
        public string Data { get; private set; }

        public CsvField(string data, bool isLastToken)
        {
            this.IsLastToken = isLastToken;
            this.Data = data;
        }
        public CsvField(string data) : this(data, false) { }
    }
}
