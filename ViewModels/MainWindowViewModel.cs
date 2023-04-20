using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomHotKey.Models;
using CustomHotKey.Properties;
using CustomHotKey.ViewModels.HotKeyCommands;
using CustomHotKey.Views.Dialog;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace CustomHotKey.ViewModels
{
    /// <summary>
    /// <see cref="MainWindow"/>的ViewModel
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// <see cref="MainWindow"/>的主题
        /// </summary>
        public string Theme
        {
            get
            {
                return Settings.Default.Theme;
            }
            set
            {
                Settings.Default.Theme = value;
                Settings.Default.Save();

                try
                {
                    if (!DesignerProperties.GetIsInDesignMode(App.Current.MainWindow))
                    {
                        // 更新App.xaml内的主题路径，实现动态更新
                        App.Current.Resources.MergedDictionaries[0].Source =
                        new Uri("\\Resources\\Color\\" + value + "Theme.xaml", UriKind.Relative);
                    }
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// 主题名称的<see cref="string[]"/>
        /// </summary>
        public string[] ThemeNames { get; } = new string[]
        {
            "Dark",
            "Light"
        };

        /// <summary>
        /// 包装<see cref="Language.Lang"/>
        /// </summary>
        public Language.LanguageJSON Lang
        {
            get
            {
                return Language.Lang;
            }
            set
            {
                Language.Lang = value;
            }
        }

        /// <summary>
        /// 选中的语言名称
        /// </summary>
        public string SelectedLanguageName
        {
            get
            {
                return Language.SelectedLanguageName;
            }
            set
            {
                Language.SelectedLanguageName = value;

                // 通知View Lang 和 HotKeyCommandTypeNames 改变
                OnPropertyChanged("Lang");
                OnPropertyChanged("HotKeyCommandTypeNames");
            }
        }

        /// <summary>
        /// 语言名称的<see cref="List{String}"/>
        /// </summary>
        public List<string> LanguageNames
        {
            get
            {
                return Language.LanguageNames;
            }
        }

        /// <summary>
        /// 将<see cref="HotKeyCommand.CommandTypes"/>中所有元素的Name属性转换成多语言文本作为集合暴露给View
        /// </summary>
        public string[] HotKeyCommandTypeNames
        {
            get
            {
                string[] temp = new string[HotKeyCommand.CommandTypes.Length];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = (string)typeof(Language.LanguageJSON).GetProperty(
                        "command_" + HotKeyCommand.CommandTypes[i].Name.ToLower()
                    ).GetValue(Lang);
                }
                return temp;
            }
        }

        /// <summary>
        /// 添加文件的命令
        /// </summary>
        public RelayCommand AddFile { get; set; }

        /// <summary>
        /// 添加文件夹的命令
        /// </summary>
        public RelayCommand AddFolder { get; set; }

        /// <summary>
        /// 删除文件视图选中的<see cref="AppFileManager.FileItem"/>
        /// </summary>
        public RelayCommand DeleteFileItem { get; set; }

        /// <summary>
        /// 重命名文件视图选中的<see cref="AppFileManager.FileItem"/>
        /// </summary>
        public RelayCommand RenameFileItem { get; set; }

        /// <summary>
        /// 更改文件视图的根目录
        /// </summary>
        public RelayCommand SelectFileViewPath { get; set; }

        /// <summary>
        /// 改变录制热键的状态
        /// </summary>
        public RelayCommand ChangeRecordHotKeyState { get; set; }

        /// <summary>
        /// 包装<see cref="AppFileManager.FileViewPath"/>
        /// </summary>
        public string FileViewPath
        {
            get
            {
                return AppFileManager.FileViewPath;
            }

            set { AppFileManager.FileViewPath = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 包装<see cref="AppFileManager.Files"/>
        /// </summary>
        public ObservableCollection<AppFileManager.FileItem> Files
        {
            get { return AppFileManager.Files; }
            set { AppFileManager.Files = value; }
        }
        
        private AppFileManager.FileItem selectedFileItem;

        /// <summary>
        /// 选中的文件项
        /// </summary>
        public AppFileManager.FileItem SelectedFileItem
        {
            get { return selectedFileItem; }
            set
            {
                selectedFileItem = value; OnPropertyChanged();
                if (SelectedFileItem != null &&
                    selectedFileItem.Type == AppFileManager.FileItem.FileType.File)
                {
                    // 如果选中文件项的类型是文件(热键文件)，就将 SelectedHotKey 设为包装当前文件项包装的HotKey的HotKeyViewModel
                    SelectedHotKey = new HotKeyViewModel(selectedFileItem.HotKey);
                }
                else
                {
                    SelectedHotKey = new HotKeyViewModel();
                }
            }
        }

        private HotKeyViewModel selectedHotKey;

        /// <summary>
        /// 选中的热键文件
        /// </summary>
        public HotKeyViewModel SelectedHotKey
        {
            get { return selectedHotKey; }
            set
            {
                selectedHotKey = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            init();
            
            //初始化命令
            SelectFileViewPath = new RelayCommand(() =>
            {
                // 弹出文件夹浏览器
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.SelectedPath = AppFileManager.FileViewPath;
                DialogResult dr = fbd.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    FileViewPath = fbd.SelectedPath;
                }
            });
            ChangeRecordHotKeyState = new RelayCommand(() =>
            {
                if (this.SelectedHotKey != null)
                    this.SelectedHotKey.RecordHotKeyState = !this.SelectedHotKey.RecordHotKeyState;
            });
            AddFile = new RelayCommand(() =>
            {
                // 通过DialogMessage向对话框发出消息
                int id = DialogMessage.SendMessage(this, "Input", Lang.text_input_file_name);

                // 获取对话框与用户交互的结果，调用AppFileManager.AddFile
                DialogMessage.Return += (object sender, ReceiveEventArgs e) =>
                {
                    if (e.MessageID == id && e.Args.Length >= 1 && e.Args[0].ToString() != "")
                    {
                        if (
                        this.selectedFileItem != null &&
                        this.selectedFileItem.Type == AppFileManager.FileItem.FileType.Folder
                        )
                        {
                            AppFileManager.AddFile(selectedFileItem.Path + "\\", e.Args[0].ToString());
                        }
                        else
                        {
                            AppFileManager.AddFile(FileViewPath + "\\", e.Args[0].ToString());
                        }
                    }
                };
            });
            AddFolder = new RelayCommand(() =>
            {
                int id = DialogMessage.SendMessage(this, "Input", Lang.text_input_folder_name);

                DialogMessage.Return += (object sender, ReceiveEventArgs e) =>
                {
                    if (e.MessageID == id && e.Args.Length >= 1 && e.Args[0].ToString() != "")
                    {
                        if (
                        this.selectedFileItem != null &&
                        this.selectedFileItem.Type == AppFileManager.FileItem.FileType.Folder
                        )
                        {
                            AppFileManager.AddFolder(selectedFileItem.Path + "\\", e.Args[0].ToString());
                        }
                        else
                        {
                            AppFileManager.AddFolder(FileViewPath + "\\", e.Args[0].ToString());
                        }
                    }
                };
            });
            DeleteFileItem = new RelayCommand(() =>
            {
                if (selectedFileItem != null)
                {
                    int id = DialogMessage.SendMessage(this, "Message", 
                        $"\"{selectedFileItem.Name}\"" + Lang.text_will_permanently_delete, "warning");

                    DialogMessage.Return += (object sender, ReceiveEventArgs e) =>
                    {
                        if (e.MessageID == id && ((bool)e.Args[0]) == true)
                        {
                            AppFileManager.DeleteFileItem(SelectedFileItem);
                            SelectedFileItem = null;

                        }
                    };
                }
                OnPropertyChanged("SelectedHotKey");
            });
            RenameFileItem = new RelayCommand(() =>
            {
                if (this.selectedFileItem != null)
                {
                    int id = DialogMessage.SendMessage(this, "Input", Lang.text_input_name);

                    DialogMessage.Return += (object sender, ReceiveEventArgs e) =>
                    {
                        if (e.MessageID == id && e.Args.Length >= 1 && e.Args[0].ToString() != "")
                        {
                            AppFileManager.RenameFileItem(this.selectedFileItem, e.Args[0].ToString());
                            selectedFileItem = null;
                        }

                    };
                }
                OnPropertyChanged("SelectedHotKey");
            });

            // 初始化主题
            Theme = Settings.Default.Theme;

            // 调用自动保存JSON数据的方法
            AutoSaveJSONData();

            // 初始化
            Initialization.Initialize();
        }

        private void init() {
            // 判断程序是否运行在桌面
            if (Directory.GetCurrentDirectory().Split('\\').Last() == "Desktop") {

                System.Windows.MessageBox
                .Show("程序不能在桌面上运行，请将程序移动到别处",
                "错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

                // 强制退出
                Environment.Exit(0);
            }
        }

        private async void AutoSaveJSONData()
        {
            while (true)
            {
                // 每隔一秒就保存所有热键的JSONData
                await Task.Delay(1000);
                foreach (HotKey item in HotKey.AllHotKey)
                {
                    item.SaveJSONData();
                }
            }
        }
    }
}
