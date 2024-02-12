using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Styling;
using CustomHotKey.ViewModels;

namespace CustomHotKey.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Closing += (s, e) =>
            {
                e.Cancel = true;
                this.Hide();
            };
        }
    }
}