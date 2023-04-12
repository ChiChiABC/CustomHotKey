using CustomHotKey.ViewModels.HotKeyCommands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace CustomHotKey.Views.HotKeyCommandView
{
    /// <summary>
    /// RunCommandView.xaml 的交互逻辑
    /// </summary>
    public partial class RunCommandView : Page
    {

        public RunCommandView()
        {
            InitializeComponent();
            this.cb.DataContext = App.Current.MainWindow.DataContext;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            (this.ArgsLB.ItemsSource as ObservableCollection<CommandItem>)
                .Add(new CommandItem("dir"));
        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.ArgsLB.SelectedItem != null)
            {
                CommandItem SelectedItem = (this.ArgsLB.SelectedItem as CommandItem);
                (this.ArgsLB.ItemsSource as ObservableCollection<CommandItem>).Remove(SelectedItem);
            }
        }

        private void Button_Click_up(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.ArgsLB.SelectedItem == null)
            {
                return;
            }
            var sourceCollection = this.ArgsLB.ItemsSource as ObservableCollection<CommandItem>;
            var selectedItem = this.ArgsLB.SelectedItem as CommandItem;

            int selectedItemIndex = sourceCollection.IndexOf(selectedItem);

            if (selectedItemIndex > 0)
            {
                SwapElement<CommandItem>(sourceCollection, selectedItemIndex, selectedItemIndex - 1);
                this.ArgsLB.SelectedIndex = selectedItemIndex;
            }
        }

        private void Button_Click_down(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.ArgsLB.SelectedItem == null)
            {
                return;
            }
            var sourceCollection = this.ArgsLB.ItemsSource as ObservableCollection<CommandItem>;
            var selectedItem = this.ArgsLB.SelectedItem as CommandItem;

            int selectedItemIndex = sourceCollection.IndexOf(selectedItem);

            if (selectedItemIndex < sourceCollection.Count -1)
            {
                SwapElement<CommandItem>(sourceCollection, selectedItemIndex, selectedItemIndex + 1);
                this.ArgsLB.SelectedIndex = selectedItemIndex;
            }
        }

        private void SwapElement<E>(ObservableCollection<E> c, int index1, int index2)
        {
            E temp = c[index1];
            c[index1] = c[index2];
            c[index2] = temp;
        }

        private void ListBoxItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (e.OriginalSource as ListBoxItem).IsSelected = true;
        }

        private void ScrollViewer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                (sender as ScrollViewer).LineUp();
            } else
            {
                (sender as ScrollViewer).LineDown();
            }
        }
    }
}
