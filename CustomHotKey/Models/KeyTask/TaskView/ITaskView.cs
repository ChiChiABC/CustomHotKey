namespace CustomHotKey.Models.KeyTask.TaskView;

public interface ITaskView
{
    public IKeyTask? Task { get; set; }
    
    public KeyTaskArg? SelectedArg { get; set; }
}