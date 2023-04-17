using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Forms;

namespace CustomHotKey.Models
{
    /// <summary>
    /// 程序关于按键的操作集中在此类
    /// </summary>
    public static class KeyBoardTool
    {
        /// <summary>
        /// 现在按下的键，当松开任意键时清空
        /// </summary>
        public static List<int> NowPressKey = new List<int>();

        /// <summary>
        /// 描述当前按键的结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct

        {
            public int vkCode;

            public int scanCode;

            public int flags;

            public int time;

            public int dwExtraInfo;

        }

        /// <summary>
        /// 按键向上(松开按键)
        /// </summary>
        public const int WM_KEYUP = 0x0101;

        /// <summary>
        /// 全局键盘钩子
        /// </summary>
        public const int WH_KEYBOARD_LL = 13;

        private static int hKeyBoardHook;

        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr iParam);

        /// <summary>
        /// 键盘钩子的处理函数，包含了<see cref="NowPressKey"/>的处理，<see cref="HotKeyFunctions"/>的调用
        /// </summary>
        private static HookProc hookProc = (int nCode, IntPtr wParam, IntPtr iParam) =>
        {
            KeyBoardTool.KeyboardHookStruct keyData =
                        (KeyBoardTool.KeyboardHookStruct)
                            Marshal.PtrToStructure(iParam, typeof(KeyBoardTool.KeyboardHookStruct));
            
            if ((int)wParam != WM_KEYUP)
            {
                NowPressKey.Add(keyData.vkCode);
            }

            if ((int)wParam == WM_KEYUP)
            {
                NowPressKey.Clear();
            }

            HotKeyFunctions?.Invoke(nCode, wParam, keyData);

            return KeyBoardTool.CallNextHookEx(nCode, wParam, iParam);
        };

        /// <summary>
        /// 当按键变化时会调用此委托
        /// </summary>
        public static Action<int, IntPtr, KeyboardHookStruct> HotKeyFunctions;

        // 安装钩子
        [DllImport("user32.dll")]
        private static extern int SetWindowsHookEx
            (int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        // 卸载钩子
        [DllImport("user32.dll")]
        private extern static bool UnhookWindowsHookEx(int handle);

        // 继续下一个钩子
        [DllImport("user32.dll")]
        private extern static int CallNextHookEx(int handle, int code, IntPtr wparam, IntPtr iparam);

        // 模拟键盘事件
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]

        public static extern void keybd_event(
             byte bVk,    // 虚拟键值
             byte bScan, 
             int dwFlags,  // 0 为按下，2为释放
             int dwExtraInfo  
        );

        // 包装 CallNextHookEx
        public static int CallNextHookEx(int code, IntPtr wparam, IntPtr iparam)
        {
            return CallNextHookEx(hKeyBoardHook, code, wparam, iparam);
        }

        /// <summary>
        /// 将带有LR前缀的键码转换为不带有LR前缀的键码
        /// </summary>
        /// <param name="key">要转换的键码</param>
        /// <returns>转换完成的键码，如果<paramref name="key"/>的键码不需要转换，将不做处理</returns>
        public static int LRKeyToKey(int key)
        {
            string keyName = null;

            if (Enum.GetName(typeof(Keys), key).Length > 1)
            {
                if (Enum.GetName(typeof(Keys), key).Contains("L"))
                {
                    keyName = Enum.GetName(typeof(Keys), key).Split('L').Last();
                }
                if (Enum.GetName(typeof(Keys), key).Contains("R"))
                {
                    keyName = Enum.GetName(typeof(Keys), key).Split('R').Last();
                }

                Console.WriteLine(keyName);

                if (keyName != null)
                {
                    Keys k = new Keys();

                    Enum.TryParse<Keys>(keyName, out k);

                    if ((int)k == 0)
                    {
                        Enum.TryParse<Keys>(key.ToString(), out k);
                    }

                    return (int)k;
                }
            }
            return key;
        }

        static KeyBoardTool()
        {
            hKeyBoardHook =
            SetWindowsHookEx(WH_KEYBOARD_LL,
                hookProc,
                Marshal.GetHINSTANCE(System.Reflection.
                    Assembly.GetExecutingAssembly().GetModules()[0]),
                0);
        }

        /// <summary>
        /// 卸载钩子
        /// </summary>
        public static void Stop()
        {
            UnhookWindowsHookEx(hKeyBoardHook);
        }
    }
}
