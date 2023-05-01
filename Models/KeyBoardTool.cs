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
        /// 键盘按键的结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private class KeyBoardStruct
        {
            public int vkCode;
            public int scanCode;
            public uint flags;
            public long time;
            IntPtr dwExtraInfo;
        }

        /// <summary>
        /// 键盘按键的结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private class MouseStruct
        {
            public POINT pt;
            public int mouseData;
            public uint flags;
            public long time;
            IntPtr dwExtraInfo;
        }

        /// <summary>
        /// 描述当前按键的结构, 将键盘按键和鼠标按键结构用到的信息整合起来
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KeyStruct

        {
            public SenderType senderType { get; set; }

            public int flag { get; set; }

            // 键码
            public int keyCode { get; set; }

            // 数据
            public int data { get; set; }

            // 位置
            public POINT pt { get; set; }

            // 时间
            public long time { get; set; }
        }

        public enum SenderType : byte
        {
            KeyBoard = 0,
            Mouse = 1,
        }

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public uint x;
            public uint y;
        }

        public const int WM_KEYUP = 0x0101;
        public const int WM_KEYDOWN = 0x0100;
        public const int LBUTTON = 513;
        public const int MBUTTON = 519;
        public const int MBUTTON_WHEEL = 522;
        public const int RBUTTON = 516;
        public const int XBUTTON = 523;


        public enum MouseEventFlag
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000

        }


        /// <summary>
        /// 全局键盘钩子
        /// </summary>
        public const int WH_KEYBOARD_LL = 13;
        /// <summary>
        /// 全局键盘钩子
        /// </summary>
        public const int WH_MOUSE_LL = 14;

        private static int KeyBoardHook;
        private static int MouseHook;

        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr iparam);

        /// <summary>
        /// 键盘钩子的处理函数，包含了<see cref="NowPressKey"/>的处理，<see cref="HotKeyFunctions"/>的调用
        /// </summary>
        private static HookProc keyBoardHookProc = (int nCode, IntPtr wParam, IntPtr iparam) =>
        {
            KeyBoardTool.KeyBoardStruct iData =
                        (KeyBoardTool.KeyBoardStruct)
                            Marshal.PtrToStructure(iparam, typeof(KeyBoardTool.KeyBoardStruct));
            KeyStruct keyData = new KeyStruct()
            {
                flag = (int)wParam,
                senderType = SenderType.KeyBoard,
                keyCode = iData.vkCode,
                data = iData.scanCode,
                pt = null,
                time = iData.time
            };
            keyData.time = (long)(DateTime.UtcNow -
                    new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

            if ((int)wParam != WM_KEYUP)
            {
                NowPressKey.Add(keyData.keyCode);
            }

            if ((int)wParam == WM_KEYUP)
            {
                NowPressKey.Clear();
            }

            HotKeyFunctions?.Invoke(nCode, wParam, keyData);

            return CallNextHookEx(KeyBoardHook, nCode, wParam, iparam);
        };

        private static HookProc MouseHookProc = (int nCode, IntPtr wParam, IntPtr iparam) =>
        {
            KeyBoardTool.MouseStruct iData =
                        (KeyBoardTool.MouseStruct)
                            Marshal.PtrToStructure(iparam, typeof(KeyBoardTool.MouseStruct));

            KeyStruct keyData = new KeyStruct()
            {
                flag = (int)wParam,
                senderType = SenderType.Mouse,
                data = iData.mouseData,
                pt = iData.pt,
                time = iData.time
            };

            keyData.time = (long)(DateTime.UtcNow -
                    new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

            switch ((int)wParam)
            {

                case LBUTTON:
                    keyData.keyCode = (int)Keys.LButton;
                    break;
                case MBUTTON:
                    keyData.keyCode = (int)Keys.MButton;
                    break;
                case RBUTTON:
                    keyData.keyCode = (int)Keys.RButton;
                    break;
                case XBUTTON:
                    if (keyData.data == 131072)
                    {
                        keyData.keyCode = (int)Keys.XButton1;
                    }
                    else
                    {
                        keyData.keyCode = (int)Keys.XButton2;
                    }
                    break;
                case MBUTTON_WHEEL:
                    if (keyData.data < 0)
                    {
                        keyData.keyCode = (int)Keys.Down;
                    }
                    else
                    {
                        keyData.keyCode = (int)Keys.Up;
                    }
                    break;
                default:
                    break;
            }
            switch ((int)wParam)
            {
                case 512:
                    return CallNextHookEx(MouseHook, nCode, wParam, iparam);
                case LBUTTON:
                case MBUTTON:
                case RBUTTON:
                case XBUTTON:
                case MBUTTON_WHEEL:
                    wParam = (IntPtr)WM_KEYDOWN;
                    break;
                default:
                    wParam = (IntPtr)WM_KEYUP;
                    break;
            }

            if ((int)wParam != WM_KEYUP && !NowPressKey.Contains(keyData.keyCode))
            {
                NowPressKey.Add(keyData.keyCode);
            }
            else
            {
                NowPressKey.Clear();
            }

            HotKeyFunctions?.Invoke(nCode, wParam, keyData);

            return CallNextHookEx(MouseHook, nCode, wParam, iparam);
        };

        /// <summary>
        /// 当按键变化时会调用此委托
        /// </summary>
        public static Action<int, IntPtr, KeyStruct> HotKeyFunctions;

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

        // 模拟鼠标事件
        [DllImport("user32.dll", EntryPoint = "mouse_event", SetLastError = true)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtrainfo);// 模拟鼠标事件
        
        // 设置鼠标位置
        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int x, int y);

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
            // 安装键盘钩子
            KeyBoardHook =
            SetWindowsHookEx(WH_KEYBOARD_LL,
                keyBoardHookProc,
                Marshal.GetHINSTANCE(System.Reflection.
                    Assembly.GetExecutingAssembly().GetModules()[0]),
                0);

            // 安装鼠标钩子
            MouseHook =
            SetWindowsHookEx(WH_MOUSE_LL,
                MouseHookProc,
                Marshal.GetHINSTANCE(System.Reflection.
                    Assembly.GetExecutingAssembly().GetModules()[0]),
                0);

        }

        /// <summary>
        /// 卸载钩子
        /// </summary>
        public static void Stop()
        {
            UnhookWindowsHookEx(KeyBoardHook);
            UnhookWindowsHookEx(MouseHook);
        }
    }
}
