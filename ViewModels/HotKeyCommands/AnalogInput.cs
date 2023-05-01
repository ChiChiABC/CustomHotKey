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
using KeyStruct = CustomHotKey.Models.KeyBoardTool.KeyStruct;
using MouseEventFlag = CustomHotKey.Models.KeyBoardTool.MouseEventFlag;

namespace CustomHotKey.ViewModels.HotKeyCommands
{
    /// <summary>
    /// 模拟输入命令 模拟键盘以及鼠标的输入
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptOut)]
    public class AnalogInput : HotKeyCommand
    {
        [JsonIgnore]
        public RelayCommand ChangeRecord { get; set; }

        [JsonIgnore]
        public RelayCommand ClearInputs { get; set; }

        [JsonIgnore]
        public bool Record { get; set; }

        [JsonIgnore]
        public long RecordStartTime { get; set; }

        private ObservableCollection<KeyStruct> inputs = new ObservableCollection<KeyStruct>();

        [JsonIgnore]
        public ObservableCollection<KeyStruct> Inputs
        {
            get { return inputs; }
            set { inputs = value; }
        }

        private void AddInputToInputs(int n, IntPtr wp, KeyStruct ip)
        {

            if (ip.keyCode == (int)Keys.Escape && (int)wp != KeyBoardTool.WM_KEYUP)
            {
                Record = false;
                OnPropertyChanged(nameof(Record));
                return;
            }

            if (!Record) return;

            ip.time -= RecordStartTime;

            Inputs.Add(ip);
        }

        public AnalogInput(List<string> args) : base(args)
        {
            this.Args = args;
            if (Args != null)
            {
                foreach (string item in Args)
                {
                    inputs.Add(StringToKeyStruct(item));
                }
            }
            else Args = new List<string>();

            Inputs.CollectionChanged += (s, e) =>
            {
                Args.Clear();
                foreach (KeyStruct input in Inputs)
                {
                    Args.Add(KeyStructToString(input));
                }
            };
            KeyBoardTool.HotKeyFunctions += AddInputToInputs;

            ChangeRecord = new RelayCommand(() => {
                Record = !Record;
                OnPropertyChanged(nameof(Record));

                RecordStartTime = (long)(DateTime.UtcNow -
                    new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

                if (Record) Inputs.Clear();

            });
            ClearInputs = new RelayCommand(() =>{
                Inputs.Clear();
            });
        }

        ~AnalogInput()
        {
            KeyBoardTool.HotKeyFunctions -= AddInputToInputs;
        }

        public override void Invoke()
        {
            base.Invoke();

            foreach (var item in Inputs)
            {
                AnalogKey(item);
            }
        }

        public async void AnalogKey(KeyStruct key) {
            await Task.Delay((int)key.time);
            if (key.senderType == KeyBoardTool.SenderType.KeyBoard)
            {
                byte keyCode = (byte)KeyBoardTool.LRKeyToKey(key.keyCode);

                int flag = (key.flag == KeyBoardTool.WM_KEYUP ? 2 : 0);

                KeyBoardTool.keybd_event(keyCode, (byte)key.data, flag, 0);

            } 
            if (key.senderType == KeyBoardTool.SenderType.Mouse) {

                int flag = 0;

                switch (key.flag)
                {
                    case KeyBoardTool.LBUTTON:
                        flag = (int)MouseEventFlag.LeftDown;
                        break;
                    case KeyBoardTool.RBUTTON:
                        flag = (int)MouseEventFlag.RightDown;
                        break;
                    case KeyBoardTool.MBUTTON:
                        flag = (int)MouseEventFlag.MiddleDown;
                        break;
                    case KeyBoardTool.LBUTTON + 1:
                        flag = (int)MouseEventFlag.LeftUp;
                        break;
                    case KeyBoardTool.RBUTTON + 1:
                        flag = (int)MouseEventFlag.RightUp;
                        break;
                    case KeyBoardTool.MBUTTON + 1:
                        flag = (int)MouseEventFlag.MiddleUp;
                        break;
                    case KeyBoardTool.MBUTTON_WHEEL:
                        flag = (int)MouseEventFlag.Wheel;
                        break;
                    default:
                        break;
                }
                KeyBoardTool.SetCursorPos((int)key.pt.x, (int)key.pt.y);
                KeyBoardTool.mouse_event((int)MouseEventFlag.Absolute | flag, (int)key.pt.x, (int)key.pt.y, key.data, 0);
            }
        }

        public static KeyStruct StringToKeyStruct(string value)
        {
            KeyStruct keyStruct = new KeyStruct();

            // senderType#keyCode#data#time#flag#123,321
            string[] props = value.Split('#');

            keyStruct.senderType = (props[0] == "0" ? KeyBoardTool.SenderType.KeyBoard : KeyBoardTool.SenderType.Mouse);
            keyStruct.keyCode = int.Parse(props[1]);
            keyStruct.data = int.Parse(props[2]);
            keyStruct.time = long.Parse(props[3]);
            keyStruct.flag = int.Parse(props[4]);

            if (props.Length == 6)
                keyStruct.pt = new KeyBoardTool.POINT()
                {
                    x = uint.Parse(props[5].Split(',').First()),
                    y = uint.Parse(props[5].Split(',').Last())
                };
            else keyStruct.pt = null;

            return keyStruct;
        }
        public static string KeyStructToString(KeyStruct value)
        {

            string keyStructStr = $"{(byte)value.senderType}#{value.keyCode}#{value.data}#{value.time}#{value.flag}";

            if (value.pt != null) keyStructStr += $"#{value.pt.x},{value.pt.y}";

            return keyStructStr;
        }
    }
}
