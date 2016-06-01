using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace CsvDivNet.Core.Infrastructure
{
    /// <summary>
    /// INotifyPropertyChanged を実装する
    /// 基本クラス
    /// </summary>
    public class NotificationObject : INotifyPropertyChanged
    {
        #region コンストラクタ
        public NotificationObject() { }
        #endregion

        #region INotifyPropertyChanged の実装
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
                PropertyChanged(sender, args);
        }
        #endregion

        #region Helperメソッド
        /// <summary>
        /// タイプセーフなRaisePropertyChanged メソッド
        /// プロパティを propertyExpression () => this.プロパティ名
        /// で指定するのでプロパティ名変更によるRaisePropertyChanged 
        /// で指定する文字列の抜けを防ぐことができる。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = ExtractPropertyName(propertyExpression);
            this.RaisePropertyChanged(propertyName);
        }



        /// <summary>
        /// Expressionツリーからプロパティ名を取得する 
        /// Expressionは、 () => this.Property のように
        /// 定義されている前提
        /// PrismLibrary の Microsoft.Practices.Prism.ViewModel.PropertySupportクラスを参考
        /// </summary>
        /// <typeparam name="T">prpertryExpressionの結果の型</typeparam>
        /// <param name="propertyExpression">プロパティを参照する式ツリー</param>
        /// <returns>プロパティ名</returns>
        private string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("プロパティアクセスを行う式を指定してください。");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("プロパティアクセスのみですサポートされています。");
            }

            var getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
            {
                throw new ArgumentException("static プロパティアクセスはサポートされていません。");
            }

            return memberExpression.Member.Name;
        }
        #endregion
    }
}
