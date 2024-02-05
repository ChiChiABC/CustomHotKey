using Avalonia.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomHotKey.Models.KeyTask
{
    public class RunCommand : IKeyTask
    {

        public ObservableCollection<KeyTaskArg> Args { get; set; } = new() { "echo hello" };

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
