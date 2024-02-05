using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomHotKey.Models.KeyTask
{

    public interface IKeyTask
    {
        public ObservableCollection<KeyTaskArg> Args { get; set; }

        public void Execute();
    }
}
