using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace CsvDivNet.View
{
    public class NumericValidation : ValidationRule
    {
        private bool _allowEmpty = false;
        public bool AllowEmpty
        {
            get { return _allowEmpty; }
            set { _allowEmpty = value; }
        }
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int val;
            if (!string.IsNullOrEmpty((string)value))
            {
                if (!int.TryParse(value.ToString(), out val))
                {
                    return new ValidationResult(false, "整数を入力してください。");
                }
            }
            else if (!this.AllowEmpty)
            {
                return new ValidationResult(false, "整数を入力してください。");
            }
            return new ValidationResult(true, null);
        }

    }
}
