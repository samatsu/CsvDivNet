using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsvDivNet.Core;

namespace CsvDivNet
{
    /// <summary>
    /// コンソールでCSV分割を実行するクラス
    /// </summary>
    class ConsoleExecutor
    {
        public static void Execute(CsvDivConfig config)
        {
            CsvDivider divider = new CsvDivider(config);
            divider.FileDivideCompleted += new EventHandler<EventArgs>(divider_FileDivideCompleted);
            divider.FileDivideStarted += new EventHandler<EventArgs>(divider_FileDivideStarted);
            divider.UnitFileDivided += new EventHandler<DividedEventArgs>(divider_UnitFileDivided);
            divider.UnitFileDividing += new EventHandler<DividingEventArgs>(divider_UnitFileDividing);
            divider.FileDivideFailed += new EventHandler<DivideExceptionEventArgs>(divider_FileDivideFailed);
            divider.Divide();
        }


        static void divider_UnitFileDividing(object sender, DividingEventArgs e)
        {
            WriteMessage(string.Format("{0}への出力を開始します。", e.FileName));
        }

        static void divider_UnitFileDivided(object sender, DividedEventArgs e)
        {
            WriteMessage(string.Format("{0}への出力が完了しました。", e.FileName));
        }

        static void divider_FileDivideStarted(object sender, EventArgs e)
        {
            WriteMessage("分割処理を開始します。");
        }

        static void divider_FileDivideCompleted(object sender, EventArgs e)
        {
            WriteMessage("分割処理が完了しました。");
        }
        static void divider_FileDivideFailed(object sender, DivideExceptionEventArgs e)
        {
            WriteMessage(string.Format("分割時にエラー発生：{0}", e.Message));
            if (e.Error != null)
            {
                WriteMessage("--例外情報---");
                WriteMessage(e.Error.Message);
                WriteMessage(e.Error.StackTrace);
            }
        }

        static void WriteMessage(string log)
        {
            Console.WriteLine(log);
        }
    }
}
