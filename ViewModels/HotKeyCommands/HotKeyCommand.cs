using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CustomHotKey.ViewModels.HotKeyCommands
{
    /// <summary>
    /// 为热键的Command提供基类
    /// </summary>
    public class HotKeyCommand : ObservableObject
    {

        /// <summary>
        /// 热键的Command实例要进行读取、写入时，必须对<see cref="HotKeyCommand.Args"进行操作/>
        /// </summary>
        public List<string> Args { get; set; }

        /// <summary>
        /// Command执行时调用的方法
        /// </summary>
        public virtual void Invoke() { }

        public HotKeyCommand(List<string> args)
        {
            if (args == null) { args = new List<string>(); }
            Args = args;
        }
    }
}
