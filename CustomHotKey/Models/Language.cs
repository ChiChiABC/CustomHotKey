using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace CustomHotKey.Models;

public class Language
{
    private static string languageDirectory = "";
    public static string LanguageDirectory => languageDirectory;
    public static ObservableCollection<string> Languages => GetLanguages();

    public string? Menu_File { get; set; }
    public string? Menu_Language { get; set; }
    public string? Menu_File_ChangeWorkDirectory { get; set; }
    public string? Menu_File_Add_HotKeyGroup { get; set; }
    public string? Menu_File_SaveAll { get; set; }
    public string? Search { get; set; }
    public string? Editor_HotKeys { get; set; }
    public string? Editor_Name { get; set; }
    public string? Editor_Description { get; set; }
    public string? Editor_KeyTasks { get; set; }
    public string? Editor_Args { get; set; }
    public string? TaskView_RunCommand_ShowCommandWindow { get; set; }
    public string? TaskView_OpenFile_ChangeArg { get; set; }

    public static ObservableCollection<string> GetLanguages()
    {
        ObservableCollection<string> languages = new();
        var files = new DirectoryInfo(languageDirectory).GetFiles("*.json", SearchOption.AllDirectories);
        foreach (var fileInfo in files)
        {
            languages.Add(fileInfo.Name.Split(".json")[0]);
        }

        return languages;
    }
    
    public static Language? LoadFromString(string? languageId)
    {
        var info = new DirectoryInfo(languageDirectory);
        var langFile = info.GetFiles("*.json", SearchOption.AllDirectories).ToList().Find(x => x.Name == $"{languageId}.json");
        if (langFile == null) return null;

        try
        {
            var lang = JsonConvert.DeserializeObject<Language>(File.ReadAllText(langFile.FullName));
            return lang;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    static Language()
    {
        DirectoryInfo info = new(Path.Combine(Directory.GetCurrentDirectory(), "Language"));
        if (!info.Exists)
        {
            string zh_cn = 
                @"
{
    ""Menu_File"": ""文件 (_F)"",
    ""Menu_Language"": ""语言 (_L)"",
    ""Menu_File_ChangeWorkDirectory"": ""更改工作目录 (_C)"",
    ""Menu_File_Add_HotKeyGroup"": ""添加热键组 (_A)"",
    ""Menu_File_SaveAll"": ""全部保存 (_S)"",
    ""Search"": ""搜索"",
    ""Editor_HotKeys"": ""热键"",
    ""Editor_Name"": ""名称"",
    ""Editor_Description"": ""描述"",
    ""Editor_KeyTasks"": ""按键任务: "",
    ""Editor_Args"": ""任务参数: "",
    ""TaskView_RunCommand_ShowCommandWindow"": ""显示命令窗口: "",
    ""TaskView_OpenFile_ChangeArg"": ""选择文件""
}
            ";
            info.Create();
            var langFile = new FileInfo(Path.Combine(info.FullName, "zh_cn.json"));
            langFile.Create().Close();
            File.WriteAllText(langFile.FullName, @zh_cn);
        }
        languageDirectory = info.FullName;
    }
}