using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;

namespace CustomHotKey.Models.KeyTask.TaskView;

public partial class RunCommandTaskView : UserControl, ITaskView
{
    public IKeyTask? Task { get; set; }
    public RunCommandTaskView(IKeyTask task)
    {
        InitializeComponent();
        Task = task;
        this.DataContext = Task;
    }
}