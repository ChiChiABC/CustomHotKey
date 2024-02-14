using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomHotKey.Models;
using CustomHotKey.Models.KeyTask;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using CustomHotKey.Models.KeyTask.TaskView;
using CustomHotKey.Models.UnInstaller;

namespace CustomHotKey.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<string> Languages { get; set; } = Language.GetLanguages();
        public Language? Lang { get; set; }
        
        [ObservableProperty]
        private HotKeyGroup? selectedHotKeyGroup;

        public string SearchValue
        {
            set => Search(value);
        }


        public ObservableCollection<HotKeyGroup> SearchResult { get; set; } = new();

        public ObservableCollection<Type> TaskTypes { get; set; } = KeyTask.TaskTypes;

        public void Search(string value)
        {
            value = value.ToLower();

            var result = KeyManager.HotKeyGroups.ToList()
                .FindAll(x => (x.Name.ToLower().Contains(value) || x.Description.ToLower().Contains(value)));

            SearchResult.Clear();
            foreach (var item in result)
            {
                SearchResult.Add(item);
            }
        }

        [RelayCommand]
        public void UnInstall()
        {
            UnInstaller.UnInstall();
        }

        [RelayCommand]
        public void ChangeLanguage(string? languageId)
        {
            Lang = Language.LoadFromString(languageId);
            OnPropertyChanged(nameof(Lang));
        }

        [RelayCommand]
        public void AddHotKeyGroup(string name)
        {
            KeyManager.AddHotKeyGroup(name);
        }

        [RelayCommand]
        public void RemoveHotKeyGroup(HotKeyGroup group)
        {
            KeyManager.RemoveHotKeyGroup(group);
        }

        [RelayCommand]
        public void AddKeyTask(string taskTypeName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(taskTypeName))
                {
                    var task = new RunCommand();
                    task.InitView();
                    SelectedHotKeyGroup.KeyTasks.Add(task);
                } else
                {
                    var task = (IKeyTask)Activator
                        .CreateInstance(KeyTask.TaskTypes.ToList().Find(x => x.Name == taskTypeName));
                    task.InitView();
                    SelectedHotKeyGroup.KeyTasks
                        .Add(task);
                }
            }
            catch (Exception) { }
        }
        [RelayCommand]
        public void RemoveKeyTask(IKeyTask task)
        {
            SelectedHotKeyGroup.KeyTasks.Remove(task);
        }
        [RelayCommand]
        public void AddKeyTaskArg(IKeyTask keyTask)
        {
            keyTask.Args.Add("");
        }

        [RelayCommand]
        public void RemoveKeyTaskArg(Control taskView)
        {
            try
            {
                if (taskView == null || SelectedHotKeyGroup == null) return;
                var arg = (taskView as ITaskView).SelectedArg;
                SelectedHotKeyGroup.KeyTasks.ToList().Find(x => x.Args.Contains(arg))
                .Args.Remove(arg);
            }
            catch (Exception) { }
        }

        [RelayCommand]
        public void ChangeWorkDirectory(Control control)
        {
            var folder = TopLevel.GetTopLevel(control).StorageProvider
                .OpenFolderPickerAsync(new FolderPickerOpenOptions()
                {
                    AllowMultiple = false
                });

            if (folder.Result.Count >= 1)
            {
                KeyManager.ChangeWorkDirectory(folder.Result[0].Path.LocalPath);
            }
        }

        [RelayCommand]
        public void SaveAll()
        {
            KeyManager.SaveAll();
        }

        public MainWindowViewModel()
        {
            Lang = Language.LoadFromString("zh_cn");
            
            Search("");
            KeyManager.Loaded += (s, e) => Search("");
            KeyManager.Listener.KeyDown += (s, e) =>
            {
                if (KeyManager.NowPressKey.Count == 2 && 
                    (KeyManager.NowPressKey.Contains(Keys.LControlKey) || KeyManager.NowPressKey.Contains(Keys.RControlKey))
                    && KeyManager.NowPressKey.Contains(Keys.S))
                {
                    SaveAll();
                }
            };
        }
    }
}
