using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomHotKey.ViewModels.HotKeyCommands
{
    /// <summary>
    /// 运行命令
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class RunCommand : HotKeyCommand
    {
        private bool retainWindow = false;

        [JsonIgnore]
        public bool RetainWindow
        {
            get { return retainWindow; }
            set { retainWindow = value; }
        }

        [JsonIgnore]
        public ObservableCollection<CommandItem> CommandItems { get; set; } = new ObservableCollection<CommandItem>();
        public RunCommand(List<string> args) : base(args)
        {
            Args = args;
            if (Args != null && Args.Count != 0)
            {
                this.RetainWindow = (Args[0] == "True");
                for (int i = 1; i < Args.Count; i++)
                {
                    CommandItems.Add(new CommandItem(Args[i]));
                }
            }
            else Args = new List<string>() { "dir" };

            
            CommandItem.OnCommandChanged += (s, e) =>
            {
                Args.Clear();

                if (Args.Count != 0)
                {
                    Args[0] = this.RetainWindow.ToString();
                }
                else Args.Add(this.RetainWindow.ToString());

                foreach (var item in CommandItems)
                {
                    Args.Add(item.Command);
                }
            };
            CommandItems.CollectionChanged += (s, e) =>
            {
                Args.Clear();


                if (Args.Count != 0)
                {
                    Args[0] = this.RetainWindow.ToString();
                }
                else Args.Add(this.RetainWindow.ToString());

                foreach (var item in CommandItems)
                {
                    Args.Add(item.Command);
                }
            };
        }

        public override void Invoke()
        {
            base.Invoke();
            Console.WriteLine(RetainWindow);
            string tempCommand = "";

            tempCommand = (this.RetainWindow ? "/k " : "/c ");

            for (int i = 1; i < Args.Count; i++)
            {
                if (Args[i] != null)
                {
                    tempCommand += (Args[i] + "&");
                }
            };

            System.Diagnostics.Process.Start("cmd.exe", tempCommand);
        }
    }
    public class CommandItem : ObservableObject
    {
        public static event EventHandler OnCommandChanged; 

        private string command;

        public string Command
        {
            get { return command; }
            set { command = value; OnPropertyChanged(); OnCommandChanged?.Invoke(this, EventArgs.Empty); }
        }

        public CommandItem(string str)
        {
            Command = str;
        }
    }
}
