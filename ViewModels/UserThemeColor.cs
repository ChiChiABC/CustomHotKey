using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CustomHotKey.ViewModels
{
    /// <summary>
    /// 用于获取当前系统的用户主题色
    /// </summary>
    public static class UserThemeColor
    {
        private static Color themeColor;

        /// <summary>
        /// 当前主题色的Color
        /// </summary>
        public static Color ThemeColor
        {
            get { return themeColor; }
            set { 
                themeColor = value;
                ThemeColorBrush.Color = themeColor;
            }
        }

        /// <summary>
        /// 当前主题色的Brush
        /// </summary>
        public static SolidColorBrush ThemeColorBrush { get; set; } = new SolidColorBrush();

        static UserThemeColor()
        {
            // 读取注册表，将ThemeColor初始化为当前系统的用户主题色
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM");
            if (rk != null)
            {
                int color = (int)rk.GetValue("ColorizationColor");
                string HEXColor = ((uint)color).ToString("X8");
                byte A = (byte)Convert.ToInt32(HEXColor.Substring(0, 2), 16);
                byte R = (byte)Convert.ToInt32(HEXColor.Substring(2, 2), 16);
                byte G = (byte)Convert.ToInt32(HEXColor.Substring(4, 2), 16);
                byte B = (byte)Convert.ToInt32(HEXColor.Substring(6, 2), 16);

                ThemeColor = new Color()
                {
                    A = A,
                    R = R,
                    G = G,
                    B = B
                };
            }
            rk.Close();
        }
    }
}
