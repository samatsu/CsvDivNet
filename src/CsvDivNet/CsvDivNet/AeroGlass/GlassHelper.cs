using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace CsvDivNet.AeroGlass
{
    public class GlassHelper
    {
        [DllImport("dwmapi.dll", PreserveSig=false)]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [DllImport("dwmapi.dll", PreserveSig=false)]
        static extern bool DwmIsCompositionEnabled();

        public static bool ExtendGlassFrame(Window window, Thickness margin)
        {
            if (!DwmIsCompositionEnabled())
            {
                return false;
            }
            if (!IsWin7OrVista())
            {
                return false;
            }
            IntPtr hWnd = new WindowInteropHelper(window).Handle;
            if (hWnd == IntPtr.Zero)
            {
                throw new InvalidOperationException("Windowのハンドルが無効です");
            }

            // Win32, WPFの両方の側面の背景を透明にする
            window.Background = Brushes.Transparent;
            HwndSource.FromHwnd(hWnd).CompositionTarget.BackgroundColor = Colors.Transparent;

            MARGINS margins = new MARGINS(margin);
            DwmExtendFrameIntoClientArea(hWnd, ref margins);
            
            return true;
        }
        private static bool IsWin7OrVista()
        {
            return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor <= 1;
        }
    }
}
