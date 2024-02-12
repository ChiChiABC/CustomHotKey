using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomHotKey.Models.KeyTask.TaskView;
using Newtonsoft.Json;

namespace CustomHotKey.Models.KeyTask
{
    public class OpenFile : IKeyTask
    {
        [JsonIgnore]
        public ITaskView? View { get; set; }
        public ObservableCollection<KeyTaskArg> Args { get; set; } = new();

        public void InitView()
        {
            View = new OpenFileTaskView(this);
        }

        public void Execute()
        {
            foreach (var arg in Args)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(arg) {UseShellExecute = true});
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
