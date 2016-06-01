using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using CsvDivNet.Core.Infrastructure;

namespace CsvDivNet.MvvmSlim
{
    /// <summary>
    /// ViewModelの基本クラス
    /// </summary>
    public class ViewModelBase : NotificationObject, IDataErrorInfo
    {
        #region IDataErrorInfoの実装
        /// <summary>
        /// 本プロパティは WPF インフラストラクチャ内では未使用
        /// </summary>
        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                return ValidateProperty(columnName);
            }
        }

        /// <summary>
        /// カラムを検証する
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns>エラーの場合エラーを表す文字列。エラーがない場合null</returns>
        protected virtual string ValidateProperty(string columnName)
        {
            return DataErrorInfoHelper.ValidateProperty(columnName, this);
        }
        #endregion

        protected virtual void RaiseRequerySuggested()
        {
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

    }
}
