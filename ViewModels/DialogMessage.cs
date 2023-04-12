using CustomHotKey.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomHotKey.ViewModels
{
    /// <summary>
    /// 用于ViewModel或View与对话框之间通信
    /// </summary>
    public static class DialogMessage
    {
        /// <summary>
        /// 提供事件处理的委托
        /// </summary>
        /// <param name="sender">事件发起者</param>
        /// <param name="e">事件参数</param>
        public delegate void ReceiveEventHandler(object sender, ReceiveEventArgs e);

        /// <summary>
        /// 接收事件，某个对象调用<see cref="SendMessage(object, object[])"/>方法后，会发出此事件
        /// </summary>
        public static event ReceiveEventHandler Receive;

        /// <summary>
        /// 返回事件，某个对象调用<see cref="ReturnMessage(object, int, object[])"/>方法后，会发出此事件
        /// </summary>
        public static event ReceiveEventHandler Return;

        /// <summary>
        /// 发出消息
        /// <para>
        /// 某个对象调用此方法后，会发出<see cref="Receive"/>事件，对话框类对事件进行处理
        /// </para>
        /// </summary>
        /// <param name="sender">发出者</param>
        /// <param name="args">传递给对话框的参数</param>
        /// <returns>调用者在<see cref="Return"/>事件内通过<see cref="ReceiveEventArgs.MessageID"/>判断对话框是否是响应自身发出的事件</returns>
        public static int SendMessage(object sender, params object[] args)
        {
            int id = -1;
            while (!ReceiveEventArgs.AllMessageID.Contains(id))
            {
                id = (new Random()).Next(0, int.MaxValue);
                if (!ReceiveEventArgs.AllMessageID.Contains(id))
                {
                    ReceiveEventArgs.AllMessageID.Add(id);
                }
            }

            Receive?.Invoke(sender, new ReceiveEventArgs(id, args));

            return id;
        }

        /// <summary>
        /// 返回消息
        /// <para>
        /// 对话框类接收到<see cref="Receive"/>事件后，根据传递的参数与用户进行交互，
        /// 交互完成后对话框类会调用<see cref="ReturnMessage(object, int, object[])"/>方法发出Return事件，附带了与用户交互的结果
        /// </para>
        /// </summary>
        /// <param name="sender">发出者</param>
        /// <param name="id">返回事件参数的<see cref="ReceiveEventArgs.MessageID"/></param>
        /// <param name="args">与用户交互后的参数</param>
        public static void ReturnMessage(object sender, 
            int id, params object[] args)
        {
            Return?.Invoke(sender, new ReceiveEventArgs(id, args));
            ReceiveEventArgs.AllMessageID.Remove(id);
        }
    }

    /// <summary>
    /// <see cref="DialogMessage.Receive"/>和<see cref="DialogMessage.Return"/>事件的参数
    /// </summary>
    public class ReceiveEventArgs : EventArgs
    {
        /// <summary>
        /// 所有未处理的消息
        /// </summary>
        public static List<int> AllMessageID { get; set; } = new List<int>();

        /// <summary>
        /// 消息ID
        /// </summary>
        public int MessageID { get; set; }

        /// <summary>
        /// 消息参数
        /// </summary>
        public object[] Args { get; set; }
        public ReceiveEventArgs(int messageID, params object[] args)
        {
            MessageID = messageID;
            Args = args;

            AllMessageID.Add(MessageID);
        }
    }
}
