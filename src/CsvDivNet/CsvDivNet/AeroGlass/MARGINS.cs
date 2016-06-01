using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;

namespace CsvDivNet.AeroGlass
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public MARGINS(Thickness t)
        {
            Left = (int)t.Left;
            Right = (int)t.Right;
            Top = (int)t.Top;
            Bottom = (int)t.Bottom;
        }

        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }
}
