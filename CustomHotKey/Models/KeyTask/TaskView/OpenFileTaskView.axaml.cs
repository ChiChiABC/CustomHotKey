using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CustomHotKey.Models.KeyTask.TaskView;

public partial class OpenFileTaskView : UserControl, ITaskView
{
    public OpenFileTaskView(IKeyTask task)
    {
        InitializeComponent();
        Task = task;
        this.DataContext = Task;
    }

    public IKeyTask? Task { get; set; }
    
    private void ChangeArg()
    {
        var file = TopLevel.GetTopLevel(this).StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
                {
                    AllowMultiple = false
                });
                if (file.Result.Count >= 1)
                {
                    try
                    {
                        (ArgList.SelectedItem as KeyTaskArg).ArgValue = file.Result[0].Path.LocalPath;
                        
                    }
                    catch (Exception) {}
                }
    }
    private void Button_OnClick(object? sender, RoutedEventArgs e) => ChangeArg();

    private void MenuItem_OnClick(object? sender, RoutedEventArgs e) => ChangeArg();
}