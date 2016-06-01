using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CsvDivNet.Core.Infrastructure
{
    public class DataErrorInfoHelper
    {
        /// <summary>
        /// 検証対象のプロパティを検証する
        /// </summary>
        /// <param name="columnName">対象のプロパティ名</param>
        /// <returns>エラーのある場合非NULL文字列を返す.エラーがない場合はNULLを返す</returns>
        public static string ValidateProperty(string propName, object instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propName == null) throw new ArgumentNullException("propName");

            object v = GetValue(propName, instance);
            ICollection<ValidationResult> results = new List<ValidationResult>();
            Validator.TryValidateProperty(v, new ValidationContext(instance, null, null) { MemberName = propName }, results);
            foreach (var item in results)
            {
                return item.ErrorMessage;
            }
            return null;
        }

        /// <summary>
        /// modelインスタンスのpropNameプロパティの値を取得する
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static object GetValue(string propName, object instance)
        {
            System.Reflection.PropertyInfo prop = instance.GetType().GetProperty(propName,
                System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            return prop.GetValue(instance, null);
        }
    }
}
