using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomHotKey.Models.KeyListener
{
    public interface IKeyListener
    {
        public event EventHandler<Keys>? KeyDown;
        public event EventHandler<Keys>? KeyUp;

        public void Listen();
        public void Stop();

        public static IKeyListener? GetListener()
        {
            IKeyListener? listener = null;
            
            if (OperatingSystem.IsWindows()) listener = new WindowsKeyListener();

            return listener;
        }
    }
}
