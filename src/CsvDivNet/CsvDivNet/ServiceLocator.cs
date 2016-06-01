using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsvDivNet.ViewModel;
using System.ComponentModel;

namespace CsvDivNet
{
    /// <summary>
    /// WPFアプリケーション用のBootStrapperクラス
    /// </summary>
    public static class ServiceLocator
    {
        private static MainWindowViewModel _model = null;
        public static MainWindowViewModel MainWindowViewModel
        {
            get
            {
                if (_model == null)
                {
                    if (IsInDesignModel)
                    {
                        _model = new MainWindowViewModel(new Core.CsvDivConfig());
                    }
                    else
                    {
                        _model = new MainWindowViewModel();
                    }
                }
                return _model;
            }
            set
            {
                _model = value;
            }
        }

        private static bool IsInDesignModel
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());
            }
        }
    }
}
