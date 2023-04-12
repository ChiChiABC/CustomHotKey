using CustomHotKey.Properties;
using CustomHotKey.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace CustomHotKey.Models
{
    /// <summary>
    /// 此程序的文件操作全部封装在此类。
    /// </summary>
    public static class AppFileManager
    {
        /// <summary>
        /// 用于记录热键文件的后缀名
        /// </summary>
        public const string FileSuffix = ".chkey";

        /// <summary>
        /// 新建热键文件时的默认内容
        /// </summary>
        public static string DefaultFileContent =
            @"{""Description"":""Hello!"",""Open"":false,""DistinguishLR"": true, ""Keys"":[67,72,75],""CommandType"":""OpenFile"",""Command"":{""Args"":[]}}";

        static AppFileManager()
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Key\");


            if (Settings.Default.FileViewPath == "NULL")
            {
                FileViewPath = Directory.GetCurrentDirectory() + @"\Key\";
            }
            else
            {
                FileViewPath = Settings.Default.FileViewPath;
            }
        }


        private static string fileViewPath;

        /// <summary>
        /// 提供文件视图的根目录
        /// </summary>
        public static string FileViewPath
        {
            get { return fileViewPath; }
            set { 
                fileViewPath = new Regex(@"[\\]+").Replace(value, @"\") + "\\"; 
                GetAllFileAndDirectories(fileViewPath, ref files);
                Settings.Default.FileViewPath = FileViewPath;
                Settings.Default.Save();
            }
        }

        private static ObservableCollection<FileItem> files;

        /// <summary>
        /// 文件视图显示的内容
        /// </summary>
        public static ObservableCollection<FileItem> Files
        {
            get { return files; }
            set { files = value; }
        }

        /// <summary>
        /// 新建热键文件
        /// </summary>
        /// <param name="path">热键文件路径</param>
        /// <param name="FileName">热键文件名称</param>
        public static void AddFile(string path, string fileName)
        {
            fileName += FileSuffix;

            File.Create(path + fileName).Close();

            // 将默认的热键文件内容写入新建的热键文件
            File.WriteAllText(path + fileName, DefaultFileContent);

            // 更新文件视图
            GetAllFileAndDirectories(FileViewPath, ref files);
        }

        /// <summary>
        /// 新建文件夹
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="folderName">文件夹名称</param>
        public static void AddFolder(string path, string folderName)
        {
            Directory.CreateDirectory(path + folderName);

            // 更新文件视图
            GetAllFileAndDirectories(FileViewPath, ref files);
        }

        /// <summary>
        /// 删除热键文件(文件夹)
        /// </summary>
        /// <param name="fileItem">删除此<see cref="FileItem"/>内<see cref="FileItem.Path"/>路径指定的热键文件(或文件夹)</param>
        public static void DeleteFileItem(FileItem fileItem)
        {
            if (fileItem.Type == FileItem.FileType.File)
            {
                File.Delete(fileItem.Path);
            }
            else
            {
                Directory.Delete(fileItem.Path, true);
            }

            // 更新文件视图
            GetAllFileAndDirectories(FileViewPath, ref files);

        }

        /// <summary>
        /// 重命名热键文件(文件夹)
        /// </summary>
        /// <param name="fileItem">重命名此<see cref="FileItem"/>内
        ///     <see cref="FileItem.Path"/>指定的热键文件(或文件夹)</param>
        /// <param name="name">重命名之后的名称</param>
        public static void RenameFileItem(FileItem fileItem, string name)
        {
            try
            {
                if (fileItem.Type == FileItem.FileType.File)
                {
                    string directoryPath = new FileInfo(fileItem.Path).DirectoryName + "\\";

                    name += FileSuffix;

                    File.Move(fileItem.Path, directoryPath + "\\" + name);
                }
                else
                {
                    string directoryPath = new DirectoryInfo(fileItem.Path + "\\").Parent.FullName;
                    Directory.Move(fileItem.Path + "\\", directoryPath + "\\" + name);
                }
            }
            catch (Exception)
            {
                return;
            }

            // 更新文件视图
            GetAllFileAndDirectories(FileViewPath, ref files);
        }

        /// <summary>
        /// 获取<c>path</c>下的热键文件、子目录、以及子目录的热键文件
        /// </summary>
        /// <param name="path">指定路径</param>
        /// <param name="collection">将获取到的内容添加进该参数指定的集合中</param>
        public static void GetAllFileAndDirectories(
            string path, ref ObservableCollection<FileItem> collection)
        {
            if (collection == null) collection = new ObservableCollection<FileItem>();

            collection.Clear();
            string[] _files = Directory.GetFiles(path, "*" + FileSuffix);
            string[] _directories = Directory.GetDirectories(path);

            foreach (var item in _directories)
            {
                collection.Add(new FileItem(item, FileItem.FileType.Folder));
            }
            foreach (var item in _files)
            {
                collection.Add(new FileItem(item, FileItem.FileType.File));
            }
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Key\");

        }


        /// <summary>
        /// 用于指定文件项，类型可以是文件或是文件夹
        /// </summary>
        public class FileItem
        {
            private Brush iconBrush;

            /// <summary>
            /// 文件图标，当<see cref="Type"/>为<see cref="FileItem.FileType.Folder"/>时，该属性为空
            /// </summary>
            public Brush IconBrush
            {
                get { return iconBrush; }
            }

            private FileType type; 

            /// <summary>
            /// 当前对象指定的文件项的种类
            /// </summary>
            public FileType Type
            {
                get { return type; }
            }

            private string path; 

            /// <summary>
            /// 指定当前对象在磁盘上对应文件项的路径
            /// </summary>
            public string Path
            {
                get { return path; }
            }

            private string name; 

            /// <summary>
            /// 当前对象指定的文件项的名称
            /// </summary>
            public string Name
            {
                get { return name; }
            }

            private HotKey hotKey; 

            /// <summary>
            /// 当前对象指定文件项的热键实例
            /// </summary>
            public HotKey HotKey
            {
                get { return hotKey; }
                set { hotKey = value; }
            }


            private ObservableCollection<FileItem> dircetories;

            /// <summary>
            /// <para>
            /// 如果当前对象的<see cref="Type"/>为<see cref="FileItem.FileType.Folder"/>，
            /// 则该属性为对应文件项包含的文件、目录的集合; 
            /// </para>
            /// <para>
            /// 如果当前对象的<see cref="Type"/>为<see cref="FileItem.FileType.File"/>，
            /// 则该属性为空
            /// </para>
            /// </summary>
            public ObservableCollection<FileItem> Directories
            {
                get { return dircetories; }
            }

            public FileItem(string path, FileType type)
            {
                this.path = path;
                this.type = type;
                this.name = this.path.Split('\\').Last();

                if (this.type == FileType.Folder)
                {
                    GetAllFileAndDirectories(path, ref dircetories);
                }
                else
                {
                    // 获取文件的图标
                    var icon = System.Drawing.Icon.ExtractAssociatedIcon(path);

                    this.iconBrush = new ImageBrush(Imaging.CreateBitmapSourceFromHIcon
                        (icon.Handle,
                        new Int32Rect(0, 0, icon.Height, icon.Width),
                        BitmapSizeOptions.FromEmptyOptions()))
                    { 
                        Stretch = Stretch.Fill,
                    };

                    // 创建一个Path属性与自身Path相等的HotKey对象赋值给HotKey
                    this.HotKey = new HotKey(path);
                }
            }

            /// <summary>
            /// <see cref="FileItem"/>内部枚举，用于指定<see cref="FileItem.Type"/>的值
            /// </summary>
            public enum FileType
            {
                Folder = 0,
                File = 1
            }

        }
    }
}
