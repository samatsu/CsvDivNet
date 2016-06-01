using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvDivNet.Core
{
    /// <summary>
    /// CSV分割処理 例外イベント引数
    /// </summary>
    public class DivideExceptionEventArgs : EventArgs
    {
        public String Message { get; private set; }
        public Exception Error { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">エラーテキスト</param>
        /// <param name="error">例外</param>
        public DivideExceptionEventArgs(string message, Exception error)
        {
            this.Message = message;
            this.Error = error;
        }
    }
}
