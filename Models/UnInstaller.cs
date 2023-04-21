using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomHotKey.Models
{
    /// <summary>
    /// 提供程序的卸载操作
    /// </summary>
    public static class UnInstaller
    {
        /// <summary>
        /// 卸载函数
        /// </summary>
        public static void StartUnInstaller() {
            Console.WriteLine(Environment.Is64BitOperatingSystem);
            CancelBindingFile();
            DeleteDir();
            DeleteExe();
            Environment.Exit(0);
        }

        private static void CancelBindingFile() {

            // 注册表操作
            RegistryKey registryKey = Registry.LocalMachine
                .OpenSubKey(@"Software\Classes", true);

            registryKey.DeleteSubKeyTree("CC.CustomHotKey.1");

            registryKey.Close();
        }

        private static void DeleteDir() 
        {
            // 文件操作
            Directory.Delete(Initialization.GetProgramFilePath() + "\\CustomHotKey\\" , true);

            Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\Language\\", true);
            Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\Key\\", true);
        }

        private static void DeleteExe() {
            Process.Start("cmd.exe", "/c TIMEOUT /T 1 /NOBREAK & Del " + Language.Lang.GetType().Assembly.Location);
        }
    }
}
