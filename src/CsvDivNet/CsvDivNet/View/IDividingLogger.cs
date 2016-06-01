using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvDivNet.View
{
    interface IDividingLogger
    {
        /// <summary>
        /// 分割メッセージ
        /// </summary>
        /// <param name="message"></param>
        void DivideStart(string message);
        /// <summary>
        /// 処理中の中間メッセージ
        /// </summary>
        /// <param name="message"></param>
        void AppendMessage(string message);
        /// <summary>
        /// 分割終了メッセージ
        /// </summary>
        /// <param name="message"></param>
        void DivideComplete(string message);
        /// <summary>
        /// 分割失敗メッセージ
        /// </summary>
        /// <param name="message"></param>
        void DivideFail(string message);
    }
}
