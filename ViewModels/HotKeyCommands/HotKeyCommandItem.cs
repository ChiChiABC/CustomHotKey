using CommunityToolkit.Mvvm.ComponentModel;
using CustomHotKey.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomHotKey.ViewModels.HotKeyCommands
{
    [JsonObject(MemberSerialization = MemberSerialization.OptOut)]
    public class HotKeyCommandItem : ObservableObject
    {
        public static List<Type> commandTypes = new List<Type> {
            typeof(OpenFile),
            typeof(RunCommand),
            typeof(KeyMap),
            typeof(AnalogInput),
        };

        private void notifyCommandNameChanged(object s, EventArgs e) {
            OnPropertyChanged("CommandName");
        }

        [JsonIgnore]
        public string CommandViewPath {
            get 
            {
                return @"\Views\HotKeyCommandView\" + CommandType + "View.xaml";
            }
        }
        [JsonIgnore]
        public string CommandName
        {
            get
            {
                return typeof(Language.LanguageJSON)
                    .GetProperty("command_" + CommandType.ToLower()).GetValue(Language.Lang).ToString();
            }
        }

        /// <summary>
        /// 命令的开关状态
        /// </summary>
        public bool Open { get; set; }

        public string CommandType { get; set; }


        private HotKeyCommand command;

        public HotKeyCommand Command
        {
            get { return command; }
            set {

                HotKeyCommand temp;

                if (value != null)
                {
                    temp = (HotKeyCommand)Activator
                        .CreateInstance(commandTypes.Find(t => t.Name == CommandType), value.Args);

                }
                else
                {
                    temp = (HotKeyCommand)Activator
                        .CreateInstance(commandTypes.Find(t => t.Name == CommandType), new List<string>());
                }

                command = (HotKeyCommand)temp;
                OnPropertyChanged("Command");
            }
        }

        public HotKeyCommandItem() {
            Language.LangChanged += notifyCommandNameChanged;
        }
        ~HotKeyCommandItem() {
            Language.LangChanged -= notifyCommandNameChanged;
        }
    }
}
