using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomHotKey.Models.KeyTask.TaskView;

namespace CustomHotKey.Models.KeyTask
{
    public class OpenFile : IKeyTask
    {
        public ITaskView? View { get; set; }
        public ObservableCollection<KeyTaskArg> Args { get; set; } = new();

        public void InitView()
        {
            
        }

        public void Execute()
        {
            foreach (var arg in Args)
            {
                try
                {
                    Process.Start(arg);
                }
                catch (Exception) { }
            }
        }

        public override string ToString()
        {
            return typeof(OpenFile).Name;
        }
    }
}
