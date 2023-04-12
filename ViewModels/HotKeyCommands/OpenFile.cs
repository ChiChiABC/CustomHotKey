using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CustomHotKey.ViewModels.HotKeyCommands
{
    /// <summary>
    /// 打开文件
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class OpenFile : HotKeyCommand
    {
        [JsonIgnore]
        public ObservableCollection<File> Files { get; set; } = new ObservableCollection<File>();

        public OpenFile(List<string> args) : base(args)
        {
            this.Args = args;
            if (Args != null)
            {
                foreach (var item in Args)
                {
                    Files.Add(new File(item));
                }
            }
            else Args = new List<string>();

            Files.CollectionChanged += (s, e) =>
            {
                Args.Clear();
                foreach (var item in Files)
                {
                    Args.Add(item.Path);
                }
            };
        }

        public override void Invoke()
        {
            base.Invoke();

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();

            string tempCommand = "";
            foreach (var item in Args)
            {
                tempCommand += $"\"{item}\"&";
            }
            p.StandardInput.WriteLine(tempCommand);

            p.StandardInput.WriteLine("exit");
            p.WaitForExit();
            p.Close();
        }
    }
    public class File
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public File(string path)
        {

            Path = path;
            Name = System.IO.Path.GetFileName(path);
            try
            {
                var i = System.Drawing.Icon.ExtractAssociatedIcon(path);
                Icon = Imaging.CreateBitmapSourceFromHIcon
                    (i.Handle,
                    new Int32Rect(0, 0, i.Height, i.Width),
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch
            {

            }
        }

    }
}
