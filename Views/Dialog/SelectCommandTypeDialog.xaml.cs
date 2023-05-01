using CustomHotKey.Models;
using CustomHotKey.ViewModels.HotKeyCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CustomHotKey.Views.Dialog
{
    /// <summary>
    /// SelectCommandTypeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SelectCommandTypeDialog : Window
    {
        public Type CommandType { get; set; }
        public string[] CommandTypeNames {
            get {
                string[] temp = new string[HotKeyCommandItem.commandTypes.Count];
                for (int i = 0; i < HotKeyCommandItem.commandTypes.Count; i++)
                {
                    temp[i] = typeof(Language.LanguageJSON)
                        .GetProperty("command_" + HotKeyCommandItem.commandTypes[i].Name.ToLower())
                            .GetValue(Models.Language.Lang).ToString();
                }
                return temp;
            }
        }
        public SelectCommandTypeDialog()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommandType = HotKeyCommandItem.commandTypes[this.commandTypeNames.SelectedIndex];
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
