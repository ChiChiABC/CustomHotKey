using System;
using System.IO;

namespace CustomHotKey.Models.UnInstaller;

public static class UnInstaller
{
    public static void UnInstall()
    {
        if (OperatingSystem.IsWindows()) WindowsUnInstall();
    }

    public static void WindowsUnInstall()
    {
        try
        {
            Directory.Delete(Language.LanguageDirectory);
            Directory.Delete(KeyManager.WorkDirectory.FullName);
            File.Delete(typeof(UnInstaller).Assembly.Location);
        }
        catch (Exception e)
        {
            
        }
    }
}