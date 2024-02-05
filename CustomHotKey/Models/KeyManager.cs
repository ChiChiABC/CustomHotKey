using Avalonia.Controls;
using CustomHotKey.Models.KeyListener;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace CustomHotKey.Models
{
    public static class KeyManager
    {
        public static IKeyListener? Listener { get; private set; }

        public static DirectoryInfo WorkDirectory { get; set; }

        public static ObservableCollection<HotKeyGroup> HotKeyGroups { get; set; }

        public static ObservableCollection<Keys> NowPressKey { get; set; }

        public static event EventHandler Loaded;

        static KeyManager()
        {
            HotKeyGroups = new ObservableCollection<HotKeyGroup>();
            NowPressKey = new ObservableCollection<Keys>();

            Listener = IKeyListener.GetListener();
            Listener?.Listen();

            Listener.KeyDown += (s, e) =>
            {
                if(!NowPressKey.Contains(e)) NowPressKey.Add(e);
            };

            Listener.KeyUp += (s, e) =>
            {
                NowPressKey.Remove(e);
            };

            WorkDirectory = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Groups"));
            LoadAll(WorkDirectory);
        }

        /// <summary>
        /// 加载.chkey文件到<see cref="KeyManager.HotKeyGroups"/>
        /// </summary>
        public static void LoadAll(DirectoryInfo dir)
        {
            try
            {
                HotKeyGroups.Clear();
                var files = dir.GetFiles("*.chkey", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    HotKeyGroups.Add(HotKeyGroup.LoadForm(file));
                }
                Loaded?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 存储<see cref="KeyManager.HotKeyGroups"/>
        /// </summary>
        public static void SaveAll()
        {
            foreach (var group in HotKeyGroups)
            {
                group.Save();
            }
        }

        public static void ChangeWorkDirectory(string dirPath)
        {
            WorkDirectory = new DirectoryInfo(dirPath);
            LoadAll(WorkDirectory);
        }

        public static void AddHotKeyGroup(string name)
        {
            if (string.IsNullOrEmpty(name)) name = DateTime.Now.ToString("yyyyMMdd_HH_mm_ss");

            string path = Path.Combine(WorkDirectory.FullName, $"{name}.chkey");
            try
            {
                File.Create(path).Close();
                var group = HotKeyGroup.LoadForm(new FileInfo(path));
                group.Name = name;
                group.Save();
                KeyManager.LoadAll(WorkDirectory);
            }
            catch (Exception) { }
        }

        public static void RemoveHotKeyGroup(HotKeyGroup group)
        {
            try
            {
                HotKeyGroups.Remove(group);
                File.Delete(group.ChkFile.FullName);
                LoadAll(WorkDirectory);
            }
            catch (Exception) { }
        }
    }
}
