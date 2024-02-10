using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CustomHotKey.Models.KeyTask.TaskView;

namespace CustomHotKey.Models.KeyTask
{

    public interface IKeyTask
    {
        [JsonIgnore]
        public ITaskView? View { get; set; }
        public ObservableCollection<KeyTaskArg> Args { get; set; }

        public void InitView();
        public void Execute();
    }
}
