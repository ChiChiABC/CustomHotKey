using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomHotKey.Models.KeyTask
{
    public static class KeyTask
    {
        public static ObservableCollection<Type> TaskTypes = new()
        {
            typeof(RunCommand),
            typeof(OpenFile),
        };


        public static ObservableCollection<IKeyTask>? JArrayToTasks(JArray array)
        {
            ObservableCollection<IKeyTask> tasks = new();

            foreach (JObject jObject in array)
            {
                try
                {
                    string taskType = jObject["TaskType"].ToString();
                    var args = jObject["Args"].ToObject<ObservableCollection<KeyTaskArg>>();

                    for (int i = 0; i < args.Count; i++)
                    {
                        if (args[i] == null) args[i] = "";
                    }

                    IKeyTask KeyTask = (IKeyTask)Activator.CreateInstance(TaskTypes.First(x => x.Name == taskType));
                    KeyTask.Args.Clear();
                    KeyTask.Args = args;
                    tasks.Add(KeyTask);
                }
                catch (Exception) { }
            }

            return tasks;
        }

        public static JArray? TasksToJArray(ObservableCollection<IKeyTask> tasks)
        {
            JArray? jArray = new();

            foreach (IKeyTask keyTask in tasks)
            {
                jArray.Add(KeyTask.TaskToJObject(keyTask));
            }

            return jArray;
        }

        public static JObject TaskToJObject(IKeyTask task)
        {
            JObject obj = new();

            obj["TaskType"] = task.GetType().Name;

            var args = new List<string>();

            foreach (var arg in task.Args)
            {
                args.Add(arg.ArgValue);
            }

            obj["Args"] = JArray.FromObject(args);

            return obj;
        }
    }
}
