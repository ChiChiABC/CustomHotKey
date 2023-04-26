using CommunityToolkit.Mvvm.ComponentModel;
using CustomHotKey.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using CustomHotKey.ViewModels.HotKeyCommands;
using CommunityToolkit.Mvvm.Messaging;
using CustomHotKey.Properties;

namespace CustomHotKey.ViewModels
{
    public partial class MainWindowViewModel
    {

        /// <summary>
        /// 将<see cref="HotKey"/>的属性暴露给View
        /// </summary>
        public class HotKeyViewModel : ObservableObject
        {

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
            /// 包装<see cref="HotKey.HotKeyJSON.CommandType"/>
            /// </summary>
            public int CommandIndex
            {
                get
                {
                    int temp = 0;
                    for (int i = 0; i < HotKeyCommand.CommandTypes.Length; i++)
                    {
                        if (hotKey.JSONData.CommandType == HotKeyCommand.CommandTypes[i].Name)
                        {
                            temp = i;
                            break;
                        }
                    }
                    return temp;
                }
                set
                {
                    if (value != -1)
                    {
                        hotKey.JSONData.CommandType = HotKeyCommand.CommandTypes[value].Name;
                        OnPropertyChanged();
                        HotKey.UpdateJSONCommand(hotKey.JSONData);
                        hotKey.SaveJSONData();
                    }
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
            /// 包装<see cref="HotKey.UpdateJSONCommand(HotKey.HotKeyJSON, bool)"/>方法，传入自身的<see cref="hotKey"/> 
            /// (更新<see cref="hotKey"/>的Command)
            /// </summary>
            public void UpdateJSONCommand()
            {
                HotKey.UpdateJSONCommand(hotKey.JSONData);
            }

            /// <summary>
            /// 包装<see cref="HotKey.HotKeyJSON.com"/>
            /// </summary>
            public HotKeyCommand Command
            {
                get
                {
                    return hotKey.JSONData.Command;
                }
                set
                {
                    hotKey.JSONData.Command = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// 当有按键操作时，通知View，<see cref="HotKey.HotKeyJSON.Keys"/>已更改
            /// </summary>
            Action<int, IntPtr, KeyBoardTool.KeyStruct> NotifyKeysChanged;

            public HotKeyViewModel(HotKey hk = null)
            {

                if (hk == null)
                {
                    hotKey = new HotKey(AppFileManager.FileViewPath + "\\" + "hello.chkey");    
                }

                hotKey = hk;

                HotKey.CancelRecord += (s, e) =>
                {
                    OnPropertyChanged("RecordHotKeyState");
                };

                NotifyKeysChanged = new Action<int, IntPtr, KeyBoardTool.KeyStruct>((n, w, i) =>
                {
                    OnPropertyChanged("Keys");
                });
                KeyBoardTool.HotKeyFunctions += NotifyKeysChanged;
            }

            ~HotKeyViewModel() 
            { 
                KeyBoardTool.HotKeyFunctions -= NotifyKeysChanged; 
                hotKey.SaveJSONData(); 
            }
        }
    }
}
