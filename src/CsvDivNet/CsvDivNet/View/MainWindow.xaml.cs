using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CsvDivNet.AeroGlass;
using System.Windows.Interop;
using CsvDivNet.View;
using Microsoft.Win32;

namespace CsvDivNet.View
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window, IFileChooser, IWindowCloser, IDividingLogger
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // SourceInitializedイベントより前に呼び出せない
            GlassHelper.ExtendGlassFrame(this, new Thickness(-1));

            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hWnd).AddHook(new HwndSourceHook(WndProc));
        }
        private const int WM_DWMCOMPOSITIONCHANGED = 0x031E;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DWMCOMPOSITIONCHANGED)
            {
                // 再度有効にする
                GlassHelper.ExtendGlassFrame(this, new Thickness(-1));
                handled = true;
            }
            return IntPtr.Zero;
        }

        #region IFileChooser の実装
        string IFileChooser.SelectFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.ShowReadOnly = true;
            dialog.CheckFileExists = true;
            bool? result = dialog.ShowDialog(this);
            if (result.HasValue && result.Value)
            {
                return dialog.FileName;
            }
            return null;
        }

        string IFileChooser.SelectDirectory()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            //System.Windows.Interop.WindowInteropHelper s = new System.Windows.Interop.WindowInteropHelper(this);
            //System.Windows.Interop.HwndSource source = System.Windows.Interop.HwndSource.FromHwnd(s.Handle);
            //dialog.ShowDialog(source);
            var result =  dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            return null;
        }
        #endregion

        #region IWindowCloserの実装
        void IWindowCloser.CloseWindow()
        {
            this.Close();
        }
        #endregion

        #region IDividingLoggerの実装
        void IDividingLogger.DivideStart(string message)
        {
            ClearLog();
            AppendLog(message);
        }

        void IDividingLogger.AppendMessage(string message)
        {
            AppendLog(message);
        }

        void IDividingLogger.DivideComplete(string message)
        {
            AppendLog(message);
            ShowMessage("分割処理が終了しました.", "処理成功", MessageBoxImage.Information);
        }

        void IDividingLogger.DivideFail(string message)
        {
            AppendLog(message);
            ShowMessage("分割処理中にエラー発生.分割ログを確認してください.", "処理失敗", MessageBoxImage.Error);
        }
        void AppendLog(string log)
        {
            log += Environment.NewLine;
            if (txtLog.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                txtLog.Dispatcher.Invoke(new Action<string>(x => { txtLog.AppendText(x); txtLog.ScrollToEnd(); }), log);
            }
            else
            {
                txtLog.AppendText(log);
            }
        }
        void ClearLog()
        {
            if (txtLog.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                txtLog.Dispatcher.Invoke(new Action(() => txtLog.Clear()));
            }
            else
            {
                txtLog.Clear();
            }
        }
        void ShowMessage(string message, string title, MessageBoxImage icon = MessageBoxImage.Information)
        {
            Action act = new Action(() => MessageBox.Show(this, message, title, MessageBoxButton.OK, icon));
            if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                this.Dispatcher.Invoke(act);
            }
            else
            {
                act();
            }
        }
        #endregion
    }
}
