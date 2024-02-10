using Avalonia.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using CustomHotKey.Models.KeyTask.TaskView;
using Newtonsoft.Json;

namespace CustomHotKey.Models.KeyTask
{
    public class RunCommand : IKeyTask
    {

        public ObservableCollection<KeyTaskArg> Args { get; set; } = new() { "echo hello" };

        [JsonIgnore]
        public ITaskView? View { get; set; } = null;

        public void InitView()
        {
            View = new RunCommandTaskView(this);
        }

        public void Execute()
        {
            string command = "";
            foreach (var arg in Args)
            {
                command = arg.ToString() + "&";
            }

            command = command.Replace('/', '\\');

            if (OperatingSystem.IsWindows())
            {
                Process.Start("cmd.exe", "/K" + command);
            }
        }

        public override string? ToString()
        {
            return typeof(RunCommand).Name;
        }
    }
}
