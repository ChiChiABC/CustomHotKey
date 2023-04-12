using CustomHotKey.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomHotKey.Models
{
    /// <summary>
    /// 为程序提供多语言文本，以JSON键值对形式存储
    /// </summary>
    public static class Language
    {

        /// <summary>
        /// 此实例提供实时的多语言文本
        /// </summary>
        public static LanguageJSON Lang { get; set; }

        /// <summary>
        /// 读取JSON语言文件的文件夹路径
        /// </summary>
        private static string languageFolderPath =
            Directory.GetCurrentDirectory() + "\\Language\\";

        
        private static List<string> languageNames = new List<string>();

        /// <summary>
        /// JSON语言文件的集合
        /// </summary>
        public static List<string> LanguageNames
        {
            get { return languageNames; }
        }


        /// <summary>
        /// 选中的语言名称
        /// </summary>
        /// <remarks>
        /// 注意不是JSON文件的名称，而是<see cref="LanguageJSON.language_name"/>属性的值
        /// </remarks>
        public static string SelectedLanguageName
        {
            get { return Settings.Default.language; }
            set { 
                Settings.Default.language = value;
                Settings.Default.Save();
                foreach (var item in Directory.GetFiles(languageFolderPath))
                {
                    string data = File.ReadAllText(item);
                    LanguageJSON langJSON = JsonConvert.DeserializeObject<LanguageJSON>(data);
                    if (langJSON.language_name == Settings.Default.language)
                    {
                        Lang = langJSON;
                        break;
                    }
                }
            }
        }

        static Language()
        {
            Directory.CreateDirectory(languageFolderPath);

            string[] languageFiles = Directory.GetFiles(languageFolderPath);
            for (int i = 0; i < languageFiles.Length; i++)
            {
                if (languageFiles[i].Split('\\').Last().Contains(".json"))
                {
                    string data = File.ReadAllText(languageFiles[i]);
                    languageNames.Add(
                        (JsonConvert.DeserializeObject<LanguageJSON>(data)).language_name
                    );
                }
            }

            SelectedLanguageName = Settings.Default.language;
        }

        /// <summary>
        /// 提供对JSON语言文件的读操作
        /// </summary>
        public class LanguageJSON
        {
            public string language_name { get; set; }
            public string title_window { get; set; }
            public string title_file_view { get; set; }
            public string title_edit_view { get; set; }
            public string text_language { get; set; }
            public string text_theme { get; set; }
            public string text_ok { get; set; }
            public string text_cancel { get; set; }
            public string text_or { get; set; }
            public string text_input_file_name { get; set; }
            public string text_input_folder_name { get; set; }
            public string text_input_name { get; set; }
            public string text_will_permanently_delete { get; set; }
            public string text_click_record_hot_key { get; set; }
            public string text_click_cancel_record_hot_key { get; set; }
            public string text_click_record_or_cancel_hot_key { get; set; }
            public string text_retain_command_line_window { get; set; }
            public string text_hidden_to_tray { get; set; }
            public string text_close_window { get; set; }
            public string text_name_input_error { get; set; }
            public string menu_file { get; set; }
            public string menu_folder { get; set; }
            public string menu_select_directory { get; set; }
            public string menu_add { get; set; }
            public string menu_delete { get; set; }
            public string menu_rename { get; set; }
            public string edit_view_open { get; set; }
            public string edit_view_is_open { get; set; }
            public string edit_view_is_not_open { get; set; }
            public string edit_view_distinguish_left_and_right { get; set; }
            public string edit_view_task { get; set; }
            public string task_double_click_add_file { get; set; }
            public string command_openfile { get; set; }
            public string command_runcommand { get; set; }
            public string command_keymap { get; set; }
        }
    }
}