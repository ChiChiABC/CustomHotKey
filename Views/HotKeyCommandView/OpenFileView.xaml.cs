using CustomHotKey.ViewModels;
using CustomHotKey.ViewModels.HotKeyCommands;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using File = CustomHotKey.ViewModels.HotKeyCommands.File;

namespace CustomHotKey.Views.HotKeyCommandView
{
    /// <summary>
    /// OpenFileView.xaml 的交互逻辑
    /// </summary>
    public partial class OpenFileView : Page
    {
        public OpenFileView()
        {
            InitializeComponent();
            this.tb.DataContext =
                (App.Current.MainWindow.DataContext as MainWindowViewModel);
        }

        private void AddFileToArgsLBSource(string path)
        {
            FileInfo fi = new FileInfo(path);
            if ((fi.Attributes & FileAttributes.Directory) == 0)
            {
                ((ObservableCollection<File>)
                this.ArgsLB.ItemsSource).Add(new File(path));
            }
        }

        private void ArgsLB_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.ShowDialog();

            foreach (var item in ofd.FileNames)
            {
                AddFileToArgsLBSource(item);
            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(this.ArgsLB.SelectedItem != null) 
                ((ObservableCollection<File>)this.ArgsLB.ItemsSource)
                    .Remove((File)this.ArgsLB.SelectedItem);
        }
    }
}
