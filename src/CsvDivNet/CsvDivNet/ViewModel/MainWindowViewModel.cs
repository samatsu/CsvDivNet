using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsvDivNet.MvvmSlim;
using System.Windows.Input;
using System.IO;
using CsvDivNet.View;
using CsvDivNet.Core;
using System.Windows;

namespace CsvDivNet.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region コンストラクタ
        /// <summary>
        /// 実行ファイルのパスと同じディレクトリ内の
        /// 設定ファイルを読み取るコンストラクタ。
        /// 設定ファイルがない場合はデフォルト値が
        /// 使用される
        /// </summary>
        public MainWindowViewModel()
        {
            _config = CsvDivConfig.LoadOrDefault(_configPath);
        }
        /// <summary>
        /// デザイン用もしくは、設定値が外部から渡されるコンストラクタ.
        /// </summary>
        /// <param name="config"></param>
        public MainWindowViewModel(CsvDivConfig config)
        {
            _config = config;
        }
        #endregion

        /// <summary>
        /// 設定ファイルの保存先
        /// System.AppDomain.CurrentDomain.BaseDirectory でも取得可能
        /// </summary>
        private static string _configPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "config.xml");

        private CsvDivConfig _config = null;
        /// <summary>
        /// CSV分割ツールのコンフィグレーション
        /// </summary>
        public CsvDivConfig Config
        {
            get { return _config; }
            set
            {
                if (_config == value) return;

                _config = value;
                RaisePropertyChanged(() => this.Config);
            }
        }

        private bool _isDividing = false;
        /// <summary>
        /// 分割中かのフラグ
        /// </summary>
        public bool IsDividing
        {
            get { return _isDividing; }
            set
            {
                if (_isDividing == value) return;

                _isDividing = value;
                RaisePropertyChanged(() => this.IsDividing);
            }
        }
        #region コマンド
        private ICommand _inputFileChangedCommand = null;
        public ICommand InputFileChangedCommand
        {
            get
            {
                if (_inputFileChangedCommand == null)
                {
                    _inputFileChangedCommand = new RelayCommand(new Action<object>(x => ProcessInputFileChangedCommand()));
                }
                return _inputFileChangedCommand;
            }
        }
        private ICommand _outputInputFileDirectoryCommand = null;
        public ICommand OutputInputFileDirectoryCommand
        {
            get
            {
                if (_outputInputFileDirectoryCommand == null)
                {
                    _outputInputFileDirectoryCommand = new RelayCommand(new Action<object>(x => ProcessOutputInputFileDirectoryCommand()));
                }
                return _outputInputFileDirectoryCommand;
            }
        }
        private ICommand _selectInputFileCommand = null;
        public ICommand SelectInputFileCommand
        {
            get
            {
                if (_selectInputFileCommand == null)
                {
                    _selectInputFileCommand = new RelayCommand(new Action<object>(x =>
                            ProcessSelectFileNameCommand(x, f =>
                            {
                                this.Config.InputFileName = f;
                                ProcessInputFileChangedCommand();
                            })));
                }
                return _selectInputFileCommand;
            }
        }
        private ICommand _selectHeaderFileCommand = null;
        public ICommand SelectHeaderFileCommand
        {
            get
            {
                if (_selectHeaderFileCommand == null)
                {
                    _selectHeaderFileCommand = new RelayCommand(new Action<object>(x =>
                        ProcessSelectFileNameCommand(x, f =>
                        {
                            this.Config.HeaderFileName = f;
                        })), new Predicate<object>(x => this.Config.HeaderMode == HeaderMode.ExternalFile));
                }
                return _selectHeaderFileCommand;
            }
        }
        private ICommand _selectOutputDirectoryCommand = null;
        public ICommand SelectOutputDirectoryCommand
        {
            get
            {
                if (_selectOutputDirectoryCommand == null)
                {
                    _selectOutputDirectoryCommand = new RelayCommand(new Action<object>(x =>
                        ProcessSelectDirectoryNameCommand(x, f =>
                        {
                            this.Config.OutputDirectoryName = f;
                        })));

                }
                return _selectOutputDirectoryCommand;
            }
        }
        private ICommand _closeCommand = null;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(o => this.ProcessCloseCommand(o));
                }
                return _closeCommand;
            }
        }
        private ICommand _divideCommand = null;
        public ICommand DivideCommand
        {
            get
            {
                if (_divideCommand == null)
                {
                    _divideCommand = new RelayCommand(o => ProcessDivideCommand(o), x => this.Config.IsValid());
                }
                return _divideCommand;
            }
        }
        #endregion

        #region コマンドメソッド
        private void ProcessInputFileChangedCommand()
        {
            if (System.IO.File.Exists(Config.InputFileName))
            {
                Config.OutputFileBase = Path.GetFileNameWithoutExtension(Config.InputFileName);
                Config.OutputFileExtention = Path.GetExtension(Config.InputFileName);
                if (Config.OutputInputFileDirectory)
                {
                    Config.OutputDirectoryName = Path.GetDirectoryName(Config.InputFileName);
                }
            }
        }
        private void ProcessOutputInputFileDirectoryCommand()
        {
            if (Config.OutputInputFileDirectory)
            {
                if (System.IO.File.Exists(Config.InputFileName))
                {
                    Config.OutputDirectoryName = Path.GetDirectoryName(Config.InputFileName);
                }
            }
        }
        private void ProcessSelectFileNameCommand(object o, Action<string> func)
        {
            IFileChooser chooser = o as IFileChooser;
            if (o == null) return;

            string file = chooser.SelectFile();
            if (!string.IsNullOrWhiteSpace(file))
            {
                func(file);
            }
        }
        private void ProcessSelectDirectoryNameCommand(object o, Action<string> func)
        {
            IFileChooser chooser = o as IFileChooser;
            if (o == null) return;

            string file = chooser.SelectDirectory();
            if (!string.IsNullOrWhiteSpace(file))
            {
                func(file);
            }
        }
        private void ProcessCloseCommand(object o)
        {
            IWindowCloser closer = o as IWindowCloser;
            if (closer == null) return;

            closer.CloseWindow();
        }
        private void ProcessDivideCommand(object o)
        {
            IDividingLogger logger = o as IDividingLogger;
            if (logger == null) return;

            // セーブ
            Config.SaveTo(_configPath);

            IsDividing = true;
            // クリアログ
            CsvDivider divider = new CsvDivider(this.Config);
            divider.FileDivideStarted += (sender, e) =>
            {
                string msg = string.Format("分割処理を開始します。(開始時間:{0})", DateTime.Now.ToLongTimeString());
                logger.DivideStart(msg);
            };
            divider.FileDivideCompleted += (sender, e) => 
            {
                string msg = string.Format("分割処理が完了しました。(終了時間:{0})", DateTime.Now.ToLongTimeString());
                logger.DivideComplete(msg);
            };
            divider.FileDivideFailed += (sender, e) =>
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("分割処理時エラー発生：{0}\n", e.Message);
                if (e.Error != null)
                {
                    builder.AppendLine("--例外情報---");
                    builder.AppendLine(e.Error.Message);
                    builder.AppendLine(e.Error.StackTrace);
                }
                logger.DivideFail(builder.ToString());
            };
            divider.UnitFileDivided += (sender, e) =>
            {
                string fmt = "分割ファイル {0} への出力が完了しました。";
                logger.AppendMessage(string.Format(fmt, Path.GetFileName(e.FileName)));
            };
            divider.UnitFileDividing += (sender, e) =>
            {
                string fmt = "分割ファイル {0} への出力を開始します。";
                logger.AppendMessage(string.Format(fmt, Path.GetFileName(e.FileName)));
            };
            divider.DivideAsync((x) => IsDividing = false);

        }
        #endregion
    }
}
