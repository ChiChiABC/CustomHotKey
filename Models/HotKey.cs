using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CustomHotKey.ViewModels.HotKeyCommands;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace CustomHotKey.Models
{
    /// <summary>
    /// 代表一个热键
    /// </summary>
    public class HotKey
    {

        public static event EventHandler CancelRecord;
        /// <summary>
        /// 所有<see cref="HotKey"/>实例的集合
        /// </summary>
        public static List<HotKey> AllHotKey = new List<HotKey>();

        /// <summary>
        /// 用于检测热键状态，执行热键命令
        /// </summary>
        private Action<int, IntPtr, KeyBoardTool.KeyStruct> HotKeyFunction;

        /// <summary>
        /// 用于记录热键
        /// </summary>
        private Action<int, IntPtr, KeyBoardTool.KeyStruct> RecordHotKeyFunction;

        /// <summary>
        /// 指定记录热键时是否覆盖之前的热键
        /// </summary>
        private bool overrideOldKey = true;

        /// <summary>
        /// 指定记录热键的开启状态
        /// </summary>
        public bool recordHotKeyState = false;

        private string path;

        /// <summary>
        /// 指定热键文件的路径
        /// </summary>
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        private string name;

        /// <summary>
        /// 热键文件的文件名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        /// <summary>
        /// 热键的JSON对象，用于读写JSON数据
        /// </summary>
        private HotKeyJSON jsonData;
        public HotKeyJSON JSONData
        {
            get { return jsonData; }
            set { jsonData = value; }
        }


        public HotKey(string path)
        {
            HotKey.AllHotKey.Add(this);
            this.path = path;
            this.name = path.Split('\\').Last();

            // 加载JSON数据
            LoadJSONData();

            HotKeyFunction = new Action<int, IntPtr, KeyBoardTool.KeyStruct>((i, wp, ip) =>
            {
                bool keysIsInNowPressKey = true;

                if (jsonData == null) return;

                foreach (var x in jsonData.Keys)
                {
                    if (!keysIsInNowPressKey)
                    {
                        return;
                    }
                    foreach (var y in KeyBoardTool.NowPressKey)
                    {
                        if (!this.jsonData.DistinguishLR)
                        {
                            if (KeyBoardTool.LRKeyToKey(x) == KeyBoardTool.LRKeyToKey(y))
                            {
                                keysIsInNowPressKey = true;
                                break;
                            }
                        }
                        else
                        {
                            if (x == y)
                            {
                                keysIsInNowPressKey = true;
                                break;
                            }
                        }
                        keysIsInNowPressKey = false;
                    }
                }

                if (keysIsInNowPressKey == true)
                {
                    keysIsInNowPressKey = jsonData.Keys.Count == KeyBoardTool.NowPressKey.Count;
                }

                if (
                keysIsInNowPressKey
                && (int)wp != KeyBoardTool.WM_KEYUP
                && !this.recordHotKeyState
                && this.jsonData.Open == true)
                {
                    // 执行逻辑
                    foreach (HotKeyCommandItem item in JSONData.Commands)
                    {
                        if (item.Open)
                        {
                            item.Command.Invoke();
                        }
                    }
                }
            });
            RecordHotKeyFunction = new Action<int, IntPtr, KeyBoardTool.KeyStruct>((i, wp, ip) =>
            {
                if (ip.keyCode == (int)Keys.Space && KeyBoardTool.NowPressKey.Count == 1)
                {
                    recordHotKeyState = false;
                    CancelRecord?.Invoke(null, new EventArgs());
                    return;
                }
                if ((int)wp != KeyBoardTool.WM_KEYUP && this.recordHotKeyState)
                {
                    if (overrideOldKey)
                    {
                        this.jsonData.Keys.Clear();
                    }
                    if (!this.jsonData.Keys.Contains(ip.keyCode))
                    {
                        this.jsonData.Keys.Add(ip.keyCode);
                    }
                    overrideOldKey = false;
                }
                else
                {
                    overrideOldKey = true;
                }
            });

            KeyBoardTool.HotKeyFunctions += HotKeyFunction;
            KeyBoardTool.HotKeyFunctions += RecordHotKeyFunction;
        }

        ~HotKey() { KeyBoardTool.HotKeyFunctions -= HotKeyFunction; KeyBoardTool.HotKeyFunctions -= RecordHotKeyFunction; }

        /// <summary>
        /// 加载JSON数据，用于初始化
        /// </summary>
        public void LoadJSONData()
        {
            string data = System.IO.File.ReadAllText(this.path);

            this.jsonData = JsonConvert.DeserializeObject<HotKeyJSON>(data);

            SaveJSONData();
        }

        /// <summary>
        /// 保存JSON数据
        /// </summary>
        public void SaveJSONData()
        {

            if (System.IO.File.Exists(this.Path))
            {
                System.IO.File.WriteAllText(this.path, JsonConvert.SerializeObject(this.jsonData, Formatting.Indented));
            }
        }

        /// <summary>
        /// <see cref="HotKey"/>实例的JSON对象
        /// </summary>
        public class HotKeyJSON
        {
            /// <summary>
            /// 热键的描述
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// 热键是否开启
            /// </summary>
            public bool Open { get; set; }

            /// <summary>
            /// 构成热键的键的集合
            /// </summary>
            public ObservableCollection<int> Keys { get; set; }

            /// <summary>
            /// 热键是否区分左右
            /// </summary>
            /// 
            /// <remarks>
            /// 假设热键为 LControlKey + RMenuKey + C (左Ctrl + 右Alt + C)，
            /// 在该属性开启的状态下，正常显示
            /// 在该属性关闭的状态下，将显示为 ControlKey + MenuKey + C (Ctrl + Alt + C)，
            ///     <see cref="HotKey.HotKeyFunction"/>的检测逻辑也会受影响
            /// </remarks>
            public bool DistinguishLR { get; set; }


            public ObservableCollection<HotKeyCommandItem> Commands { get; set; }
        }
    }
}
