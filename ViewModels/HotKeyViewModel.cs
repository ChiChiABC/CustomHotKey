using CommunityToolkit.Mvvm.ComponentModel;
using CustomHotKey.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using CustomHotKey.ViewModels.HotKeyCommands;
using CommunityToolkit.Mvvm.Messaging;
using CustomHotKey.Properties;
using CommunityToolkit.Mvvm.Input;
using CustomHotKey.Views.Dialog;
using System.Collections.Generic;

namespace CustomHotKey.ViewModels
{
    public partial class MainWindowViewModel
    {

        /// <summary>
        /// 将<see cref="HotKey"/>的属性暴露给View
        /// </summary>
        public class HotKeyViewModel : ObservableObject
        {

            public RelayCommand DeleteCommandItem { get; set; }
            public RelayCommand AddCommandItem { get; set; }


            private HotKeyCommandItem selectedCommandItem;

            /// <summary>
            /// 当前选中的<see cref="HotKeyCommandItem"/>
            /// </summary>
            public HotKeyCommandItem SelectedCommandItem
            {
                get { return selectedCommandItem; }
                set { 
                    selectedCommandItem = value;
                    if (selectedCommandItem == null) return;
                    Console.WriteLine(selectedCommandItem.Command);
                    OnPropertyChanged("SelectedCommandItem");
                }
            }

            /// <summary>
            /// 记录热键状态
            /// </summary>
            public bool RecordHotKeyState
            {
                get
                {
                    return hotKey.recordHotKeyState;
                }
                set
                {
                    hotKey.recordHotKeyState = value;
                    // 通知Binding
                    OnPropertyChanged();
                    hotKey.SaveJSONData();
                }
            }

            /// <summary>
            /// 包装的<see cref="HotKey"/>对象
            /// </summary>
            protected HotKey hotKey;

            /// <summary>
            /// 包装<see cref="HotKey.Name"/>
            /// </summary>
            public string Name
            {
                get { return hotKey.Name; }
                set { hotKey.Name = value; /*通知Binding*/ OnPropertyChanged(); }
            }

            /// <summary>
            /// 包装<see cref="HotKey.Name"/>
            /// </summary>
            public bool Open
            {
                get { return hotKey.JSONData.Open; }
                set { hotKey.JSONData.Open = value; OnPropertyChanged(); hotKey.SaveJSONData(); }
            }

            /// <summary>
            /// 包装<see cref="HotKey.HotKeyJSON.DistinguishLR"/>
            /// </summary>
            public bool DistinguishLR 
            { 
                get
                {
                    return hotKey.JSONData.DistinguishLR;
                }
                set
                {
                    hotKey.JSONData.DistinguishLR = value;
                    // 通知Binding
                    OnPropertyChanged();
                    OnPropertyChanged("Keys");
                }
            }

            /// <summary>
            /// 包装<see cref="HotKey.HotKeyJSON.Commands"/>
            /// </summary>
            public ObservableCollection<HotKeyCommandItem> Commands 
            { 
                get {
                    return hotKey.JSONData.Commands;
                }
                set {
                    hotKey.JSONData.Commands = value;
                }
            }

            /// <summary>
            /// 包装<see cref="HotKey.HotKeyJSON.Keys"/>
            /// </summary>
            public ObservableCollection<string> Keys
            {
                get
                {
                    var tempList = new ObservableCollection<string>();
                    if (DistinguishLR)
                    {
                        for (int i = 0; i < hotKey.JSONData.Keys.Count; i++)
                        {
                            tempList.Add(
                                Enum.GetName(typeof(Keys), hotKey.JSONData.Keys[i]));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < hotKey.JSONData.Keys.Count; i++)
                        {
                            tempList.Add(
                                Enum.GetName(typeof(Keys), KeyBoardTool.LRKeyToKey(hotKey.JSONData.Keys[i])));
                        }
                    }

                    return tempList;
                }
            }

            /// <summary>
            /// 包装<see cref="HotKey.HotKeyJSON.Description"/>
            /// </summary>
            public string Description
            {
                get
                {
                    return hotKey.JSONData.Description;
                }
                set
                {
                    hotKey.JSONData.Description = value;
                    OnPropertyChanged();
                    hotKey.SaveJSONData();
                }
            }

            /// <summary>
            /// 当有按键操作时，通知View，<see cref="HotKey.HotKeyJSON.Keys"/>已更改
            /// </summary>
            Action<int, IntPtr, KeyBoardTool.KeyStruct> NotifyKeysChanged;

            public HotKeyViewModel(HotKey hk = null)
            {

                hotKey = hk;

                if (hk == null)
                {
                    hotKey = new HotKey(AppFileManager.FileViewPath + "\\" + "hello.chkey");    
                }

                HotKey.CancelRecord += (s, e) =>
                {
                    OnPropertyChanged("RecordHotKeyState");
                };

                NotifyKeysChanged = new Action<int, IntPtr, KeyBoardTool.KeyStruct>((n, w, i) =>
                {
                    OnPropertyChanged("Keys");
                });
                KeyBoardTool.HotKeyFunctions += NotifyKeysChanged;

                DeleteCommandItem = new RelayCommand(() => 
                {
                    if (SelectedCommandItem == null) return;

                    if (hotKey.JSONData.Commands.Count == 1)
                    {
                        DialogMessage.SendMessage(this, "Message", "命令项最少要有一个！", "error");
                        return;
                    }

                    hotKey.JSONData.Commands.Remove(SelectedCommandItem);
                });
                AddCommandItem = new RelayCommand(() => 
                {
                    SelectCommandTypeDialog sctd = new SelectCommandTypeDialog();
                    sctd.ShowDialog();

                    if (sctd.CommandType == null) return;

                    hotKey.JSONData.Commands.Add(
                        new HotKeyCommandItem() {
                            Open = false,
                            CommandType = HotKeyCommandItem.commandTypes[sctd.commandTypeNames.SelectedIndex].Name,
                            Command = (HotKeyCommand)Activator.CreateInstance(sctd.CommandType, new List<string>())

                        }
                    );
                });
            }

            ~HotKeyViewModel() 
            { 
                KeyBoardTool.HotKeyFunctions -= NotifyKeysChanged; 
                hotKey.SaveJSONData(); 
            }
        }
    }
}
