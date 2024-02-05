using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomHotKey.Models.KeyListener
{
    /// <summary>
    /// 适用于windows的按键监听类，使用Hook技术实现监听
    /// </summary>
    public class WindowsKeyListener : IKeyListener
    {
        private int keyBoardHook = 0;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYUP = 0x0101;
        private const int WM_KEYDOWN = 0x0100;

        public event EventHandler<Keys>? KeyDown;
        public event EventHandler<Keys>? KeyUp;

        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr iParam);
        private HookProc kbdProc;

        public void Listen()
        {
            if (Design.IsDesignMode) return;

            kbdProc = (int nCode, IntPtr w, IntPtr i) =>
            {
                KeyStruct? data = Marshal.PtrToStructure<KeyStruct>(i);

                if (w == WM_KEYUP)
                {
                    KeyUp?.Invoke(this, (Keys)Enum.ToObject(typeof(Keys), data.vkCode));
                }
                else
                {
                    KeyDown?.Invoke(this, (Keys)Enum.ToObject(typeof(Keys), data.vkCode));
                }

                return CallNextHookEx(keyBoardHook, nCode, w, i);
            };

            

            // 安装键盘Hook
            keyBoardHook =
            SetWindowsHookEx(13, kbdProc,
                Marshal.GetHINSTANCE(System.Reflection.
                    Assembly.GetExecutingAssembly().GetModules()[0]),
                0);
        }

        public void Stop()
        {
            if (keyBoardHook != 0)
            {
                UnhookWindowsHookEx(keyBoardHook);
            }
        }

        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx
            (int idHook, HookProc proc, IntPtr hInstance, int threadId);

        [DllImport("user32.dll")]
        public extern static bool UnhookWindowsHookEx(int handle);

        [DllImport("user32.dll")]
        public extern static int CallNextHookEx(int handle, int code, IntPtr wparam, IntPtr iparam);

        [StructLayout(LayoutKind.Sequential)]
        private class KeyStruct
        {
            public int vkCode;
            public int scanCode;
            public uint flags;
            public long time;
            IntPtr dwExtraInfo;
        }
    }
}
