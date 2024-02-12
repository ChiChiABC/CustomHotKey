using Avalonia.Controls;

namespace CustomHotKey.Models.KeyTask.TaskView;

public partial class RunCommandTaskView : UserControl, ITaskView
{
    public RunCommandTaskView(IKeyTask task)
    {
        InitializeComponent();
        Task = task;
        this.DataContext = Task;
    }
    
    public IKeyTask? Task { get; set; }
    public KeyTaskArg? SelectedArg { get; set; }
}