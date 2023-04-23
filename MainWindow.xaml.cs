using CustomHotKey.Models;
using CustomHotKey.Properties;
using CustomHotKey.ViewModels;
using CustomHotKey.ViewModels.HotKeyCommands;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using static CustomHotKey.ViewModels.MainWindowViewModel;
using static System.Net.Mime.MediaTypeNames;
using Color = System.Windows.Media.Color;

namespace CustomHotKey
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel vm;
        NotifyIcon notifyIcon = new NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();
            UpdateEditCommandViewContent();

            this.Closing += (s, e) =>
            {
                InitialTray();
                e.Cancel = true;

                int id = 
                DialogMessage.SendMessage(this, "Message",
                    (vm.Lang.text_hidden_to_tray +
                    vm.Lang.text_or +
                    vm.Lang.text_close_window + "? "), 
                    "question", 
                    vm.Lang.text_hidden_to_tray, vm.Lang.text_close_window);

                DialogMessage.Return += CloseingWindowFunc;

                void CloseingWindowFunc(object sender, ReceiveEventArgs ea)
                {
                    foreach (HotKey item in HotKey.AllHotKey)
                    {
                        item.SaveJSONData();
                    }

                    if (ea.MessageID == id && ((bool)ea.Args[0]))
                    {
                        notifyIcon.Visible = true;

                        notifyIcon.ShowBalloonTip(500);

                        this.Visibility = Visibility.Hidden;
                    }
                    else if (!(bool)ea.Args[0])
                    {
                        Process.GetCurrentProcess().Kill();
                    }
                    DialogMessage.Return -= CloseingWindowFunc;
                }
            };

            vm = this.FindName("mwvm") as MainWindowViewModel;
        }

        private void InitialTray()
        {

            notifyIcon.BalloonTipTitle = vm.Lang.title_window;
            notifyIcon.BalloonTipText = vm.Lang.text_hidden_to_tray;
            notifyIcon.Text = vm.Lang.title_window;
            notifyIcon.Visible = false;
            notifyIcon.Icon = System.Drawing.Icon
                .ExtractAssociatedIcon(this.GetType().Assembly.Location);

            notifyIcon.BalloonTipClicked += (s, e) =>
            {
                this.Visibility = Visibility.Visible;
                notifyIcon.Visible = false;
            };
            notifyIcon.MouseDoubleClick += (s, e) =>
            {
                this.Visibility = Visibility.Visible;
                notifyIcon.Visible = false;
            };
        }

        private void FileViewTree_SelectedItemChanged
            (object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                vm.SelectedFileItem = (AppFileManager.FileItem)e.NewValue;
            }

            if (this.FileViewTree.SelectedItem != null && 
                (this.FileViewTree.SelectedItem as AppFileManager.FileItem).HotKey != null)
            {
                this.CommandDataContext.DataContext = vm.SelectedHotKey.Command;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (vm != null)
            {
                if (vm.SelectedHotKey != null)
                {
                    this.CommandDataContext.DataContext = vm.SelectedHotKey.Command;
                }
                if (this.EditCommandViewCB.SelectedIndex != -1)
                {

                    this.EditCommandViewCB.SelectedItem
                        = this.EditCommandViewCB.Items[this.EditCommandViewCB.SelectedIndex];
                }
            }
            UpdateEditCommandViewContent();
        }

        private void UpdateEditCommandViewContent()
        {
            if (this.EditCommandViewCB.SelectedItem != null)
            {

                this.EditCommandView.Navigate
                (new Uri(
                    ("/Views/HotKeyCommandView/" + (HotKeyCommand.CommandTypes[this.EditCommandViewCB.SelectedIndex].Name) + "View.xaml")
                    , UriKind.Relative));
            }
        }

    }
}
