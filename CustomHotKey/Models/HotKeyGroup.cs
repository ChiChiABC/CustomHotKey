using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CustomHotKey.Models.KeyTask;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CustomHotKey.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public partial class HotKeyGroup : ObservableObject
    {
        [JsonIgnore]
        private FileInfo chkFile;

        [JsonIgnore]
        public FileInfo ChkFile
        {
            get => chkFile;
        }


        [JsonIgnore]
        [ObservableProperty]
        private string name = "HotKeyGroup";

        [JsonIgnore]
        [ObservableProperty]
        private string description = "This is a HotKeyGroup";

        [JsonIgnore]
        [ObservableProperty]
        private ObservableCollection<Keys> hotKeys = new();

        [JsonIgnore]
        [ObservableProperty]
        private ObservableCollection<IKeyTask> keyTasks = new();

        [JsonIgnore]
        private bool isRecording;
        [JsonIgnore]
        public bool IsRecording
        {
            get { return isRecording; }
            set { isRecording = value; OnPropertyChanged(); }
        }


        [JsonIgnore]
        private bool isClearOld = true;

        public HotKeyGroup()
        {
            AddEventHandler();
        }

        public HotKeyGroup(string name, string description, ObservableCollection<Keys> hotKeys, ObservableCollection<IKeyTask> keyTasks)
        {
            Name = name;
            Description = description;
            HotKeys = hotKeys;
            KeyTasks = keyTasks;
            AddEventHandler();
        }

        public override string? ToString()
        {
            var jsonText = JsonConvert.SerializeObject(this);
            return jsonText;
        }

        private void AddEventHandler()
        {
            KeyManager.Listener.KeyDown += (s, e) =>
            {
                if (hotKeys == null) hotKeys = new ObservableCollection<Keys>();

                if (KeyManager.NowPressKey.Count == HotKeys.Count && KeyManager.NowPressKey.All(x => HotKeys.Any(y => y == x)))
                {
                    Execute();
                }

                if (IsRecording)
                {
                    if (isClearOld) HotKeys.Clear();
                    isClearOld = false;

                    if (!HotKeys.Contains(e)) HotKeys.Add(e);
                }
            };

            KeyManager.Listener.KeyUp += (s, e) =>
            {
                if (IsRecording)
                {
                    if (KeyManager.NowPressKey.Count == 0)
                    {
                        isClearOld = true;
                    }
                }
            };
        }

        private void Execute()
        {
            if (!IsRecording)
            {
                foreach (var keyTask in KeyTasks)
                {
                    keyTask.Execute();
                }
            }
        }

        public static HotKeyGroup LoadForm(FileInfo info)
        {
            string jsonText = File.ReadAllText(info.FullName);
            try
            {
                JObject obj = JObject.Parse(jsonText);
                var KeyTasks = KeyTask.KeyTask.JArrayToTasks(JArray.Parse(obj["KeyTasks"].ToString()));
                obj.Remove("KeyTasks");
                var group = obj.ToObject<HotKeyGroup>();

                group.chkFile = info;
                group.KeyTasks = KeyTasks;

                return group;
            }
            catch (Exception)
            {
                HotKeyGroup group = new HotKeyGroup();
                group.chkFile = info;
                return group;
            }
        }

        public void Save()
        {
            var obj = JObject.FromObject(this);

            obj["KeyTasks"] = KeyTask.KeyTask.TasksToJArray(KeyTasks);

            File.WriteAllText(ChkFile.FullName, obj.ToString());
        }
    }
}
