using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvDivNet.Core
{
    /// <summary>
    /// 1分割ファイルの書き込み完了直後に呼び出されるイベント引数です。
    /// </summary>
    public class DividedEventArgs : EventArgs
    {
        /// <summary>分割先ファイル名のフルパス</summary>
        public string FileName
        {
            get;
            private set;
        }
        #region コンストラクタ
        public DividedEventArgs(string filename)
        {
            this.FileName = filename;
        }
        #endregion
    }
}
