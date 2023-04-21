using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace CustomHotKey.Models
{
    /// <summary>
    /// 用于应用程序的初始化 
    /// </summary>
    public static class Initialization
    {

        private static bool isFirstStartup = true;

        /// <summary>
        /// 是否第一次启动
        /// </summary>
        public static bool IsFirstStartup
        {
            get { return isFirstStartup; }
        }


        // 初始化
        public static void Initialize()
        {
            // 每次启动都先更新一下注册表打开操作的路径
            RegistryKey registryKey = Registry.LocalMachine
                .OpenSubKey("\\Software\\Classes\\CC.CustomHotKey.1\\Shell\\Open\\Command", true);
            registryKey.SetValue("", Language.Lang.GetType().Assembly.Location);

            // 如果 C:\Program Files 或 Program Files (x86)\CustomHotKey\ 路径存在，就不是第一次启动
            if (Directory.Exists(GetProgramFilePath() + "\\CustomHotKey\\"))
            {
                isFirstStartup = false;

                // 终止函数
                return;
            }

            // 下面是第一次启动的操作 ↓

            Directory.CreateDirectory(GetProgramFilePath() + "\\CustomHotKey\\");

            // 调用文件关联函数
            BindingFile();

        }

        /// <summary>
        /// 根据当前操作系统的位数获取合适的ProgramFiles文件夹
        /// </summary>
        /// <returns>ProgramFiles文件夹路径</returns>
        public static string GetProgramFilePath() {

            // 三元表达式，如果当前操作系统是64位，获取program files目录，相反则获取program files (x86)目录
        
            return Environment.GetFolderPath(
                                    Environment.Is64BitOperatingSystem ?
                                    Environment.SpecialFolder.ProgramFiles :
                                    Environment.SpecialFolder.ProgramFilesX86);
        }

        /// <summary>
        /// 文件关联函数
        /// </summary>
        private static void BindingFile()
        {

            // 将文件图标存进 C:\Program Files或x86\CustomHotKey\ 路径

            StreamResourceInfo sri = Application.GetResourceStream(new Uri("/Resources/Icon/fileIcon.ico", UriKind.Relative));

            Stream resFilestream = sri.Stream;

            if (resFilestream != null)
            {
                BinaryReader br = new BinaryReader(resFilestream);
                FileStream fs = new FileStream(GetProgramFilePath() + "\\CustomHotKey\\fileIcon.ico", FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                bw.Write(ba);
                br.Close();
                bw.Close();
                resFilestream.Close();
            }


            // 注册表操作
            RegistryKey registryKey = Registry.LocalMachine
                .OpenSubKey(@"Software\Classes", true);

            registryKey.CreateSubKey(AppFileManager.FileSuffix);
            registryKey.CreateSubKey("CC.CustomHotKey.1");

            registryKey = registryKey.OpenSubKey(AppFileManager.FileSuffix, true);

            registryKey.SetValue("", "CC.CustomHotKey.1");

            registryKey = Registry.LocalMachine.OpenSubKey(@"Software\Classes\CC.CustomHotKey.1", true);

            registryKey.SetValue("", "热键文件");

            registryKey.CreateSubKey("DefaultIcon");

            registryKey = registryKey.OpenSubKey("DefaultIcon", true);

            registryKey.SetValue("", GetProgramFilePath() + "\\CustomHotKey\\fileIcon.ico");

            registryKey = Registry.LocalMachine.OpenSubKey(@"Software\Classes\CC.CustomHotKey.1", true);

            registryKey.CreateSubKey("Shell");

            registryKey = registryKey.OpenSubKey("Shell", true);

            registryKey.CreateSubKey("Open");

            registryKey = registryKey.OpenSubKey("Open", true);

            registryKey.CreateSubKey("Command");

            registryKey = registryKey.OpenSubKey("Command", true);

            registryKey.SetValue("", Language.Lang.GetType().Assembly.Location);

            registryKey.Close();

        }
    }
}