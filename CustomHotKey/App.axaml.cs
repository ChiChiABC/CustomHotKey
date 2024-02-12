using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CustomHotKey.ViewModels;
using CustomHotKey.Views;

namespace CustomHotKey
{
    public partial class App : Application
    {
        private Window mainWindow;
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
                mainWindow = desktop.MainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void NativeMenuItem_OnClick(object? sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void NativeMenuItem_OnClick2(object? sender, EventArgs e)
        {
            mainWindow.Show();
        }
    }
}