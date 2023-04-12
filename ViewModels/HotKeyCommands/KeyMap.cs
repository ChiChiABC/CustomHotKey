using CommunityToolkit.Mvvm.Input;
using CustomHotKey.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomHotKey.ViewModels.HotKeyCommands
{
    /// <summary>
    /// 映射快捷键
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class KeyMap : HotKeyCommand
    {
        bool overrideOldKeys = true;

        private bool recordKey;

        [JsonIgnore]
        public bool RecordKey
        {
            get { return recordKey; }
            set { recordKey = value; OnPropertyChanged(); }
        }

        [JsonIgnore]
        public RelayCommand ChangeRecordKey { get; set; }

        [JsonIgnore]
        public ObservableCollection<string> Keys { get; set; } = new ObservableCollection<string>();

        void RecordKeyFunction(int nCode, IntPtr wp, KeyBoardTool.KeyboardHookStruct ip)
        {
            if ((int)wp != KeyBoardTool.WM_KEYUP && RecordKey)
            {
                if (overrideOldKeys == true)
                {
                    Keys.Clear();
                }
                overrideOldKeys = false;

                if (Keys.Contains(Enum.GetName(typeof(Keys), ip.vkCode)))
                {
                    return;
                }

                Keys.Add(Enum.GetName(typeof(Keys), ip.vkCode));
            }
            else
            {
                overrideOldKeys = true;
            }
        }

        public KeyMap(List<string> args) : base(args)
        {
            this.Args = args;
            if (Args != null)
            {
                foreach (var arg in Args)
                {
                    Keys.Add(arg);
                }
            }
            else Args = new List<string>();

            Keys.CollectionChanged += (s, e) =>
            {
                Args.Clear();
                foreach (var key in Keys)
                {
                    Args.Add(key);
                }
            };

            ChangeRecordKey = new RelayCommand(() =>
            {
                RecordKey = !RecordKey;
            });


            KeyBoardTool.HotKeyFunctions += RecordKeyFunction;
        }

        public async override void Invoke()
        {
            base.Invoke();

            foreach (var item in Args)
            {
                await Task.Delay(10);
                KeyBoardTool.keybd_event(Convert.ToByte((int)Enum.Parse(typeof(Keys), item)),
                    0, 0, 0);
            }
            foreach (var item in Args)
            {
                await Task.Delay(10);
                KeyBoardTool.keybd_event(Convert.ToByte((int)Enum.Parse(typeof(Keys), item)),
                    0, 2, 0);
            }
        }

        ~KeyMap()
        {
            KeyBoardTool.HotKeyFunctions -= RecordKeyFunction;
        }
    }
}
