using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CsvDivNet.View
{
    public class SampleOutputFileValueConverter : IMultiValueConverter
    {
        /// <summary>
        /// values[0] - ベース名
        /// values[1] - 枝番桁数
        /// values[2] - 拡張子
        /// values[3] - ファイル形式(|SEQ|は枝番桁数で置換される)
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[3] == null || values[1] == null) return null;

            object basename = values[0] ?? string.Empty;
            string format = (values[3] as string).Replace("|SEQ|", values[1].ToString());
            return string.Format(format, basename, 1, values[2]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
