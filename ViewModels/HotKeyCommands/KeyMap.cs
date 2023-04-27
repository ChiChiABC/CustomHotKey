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

        private int cycle = 1;

        public int Cycle
        {
            get { return cycle; }
            set
            {
                cycle = value;
                OnPropertyChanged();
                UpdateArgs();
            }
        }

        private int interval = 10;

        public int Interval
        {
            get { return interval; }
            set
            {
                interval = value;
                OnPropertyChanged();
                UpdateArgs();
            }
        }


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

        void RecordKeyFunction(int nCode, IntPtr wp, KeyBoardTool.KeyStruct ip)
        {

            if (Enum.GetName(typeof(Keys), ip.keyCode).Contains("Button"))
            {
                return;
            }
            if ((int)wp != KeyBoardTool.WM_KEYUP && RecordKey)
            {
                if (overrideOldKeys == true)
                {
                    Keys.Clear();
                }
                overrideOldKeys = false;

                if (Keys.Contains(Enum.GetName(typeof(Keys), ip.keyCode)))
                {
                    return;
                }

                Keys.Add(Enum.GetName(typeof(Keys), ip.keyCode));
            }
            else
            {
                overrideOldKeys = true;
            }
        }

        public KeyMap(List<string> args) : base(args)
        {
            this.Args = args;
            if (Args != null && Args.Count > 1)
            {
                cycle = int.Parse(Args[0]);
                interval = int.Parse(Args[1]);
                for (int i = 2; i < Args.Count; i++)
                {
                    Keys.Add(Args[i]);
                }
            }
            else Args = new List<string>();

            Keys.CollectionChanged += (s, e) =>
            {
                UpdateArgs();
            };

            ChangeRecordKey = new RelayCommand(() =>
            {
                RecordKey = !RecordKey;
            });

            KeyBoardTool.HotKeyFunctions += RecordKeyFunction;
        }

        private void UpdateArgs()
        {
            Args.Clear();
            Args.Add(Cycle.ToString()); ;
            Args.Add(Interval.ToString());

            foreach (string key in Keys)
            {
                Args.Add(key);
            }
        }

        public async override void Invoke()
        {
            base.Invoke();

            try
            {
                for (int i = 0; i < cycle; i++)
                {
                    for (int j = 2; j < Args.Count; j++)
                    {

                        await Task.Delay(interval);
                        KeyBoardTool.keybd_event(Convert.ToByte((int)Enum.Parse(typeof(Keys), Args[j])),
                            0, 0, 0);
                    }
                    for (int k = 2; k < Args.Count; k++)
                    {
                        await Task.Delay(interval);
                        KeyBoardTool.keybd_event(Convert.ToByte((int)Enum.Parse(typeof(Keys), Args[k])),
                            0, 2, 0);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        ~KeyMap()
        {
            KeyBoardTool.HotKeyFunctions -= RecordKeyFunction;
        }
    }
}
