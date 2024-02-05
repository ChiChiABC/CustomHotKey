using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomHotKey.Models.KeyTask
{
    public class OpenFile : IKeyTask
    {
        public ObservableCollection<KeyTaskArg> Args { get; set; } = new();

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
    }
}
