using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Socket聊天服务端
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private List<Socket> proxSocketList = new List<Socket>();

        /// <summary>
        /// 开启服务
        /// </summary>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            //创建Socket对象
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //绑定IP和端口
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse(txtIp.Text), int.Parse(txtPort.Text));
            socket.Bind(endPoint);

            //开启监听
            socket.Listen(10);//接收100个连接请求，只能处理一个连接，队列有10个等待连接，其他返回错误信息

            AppendTextToTxtLog("开启服务成功！");

            //开始接收客户端连接
            ThreadPool.QueueUserWorkItem(new WaitCallback(AcceptClientConnect), socket);

            btnConnect.Enabled = false;
        }

        /// <summary>
        /// 接收客户端连接
        /// </summary>
        /// <param name="obj">服务端socket</param>
        private void AcceptClientConnect(object obj)
        {
            Socket serverSocket = (Socket)obj;
            while (true)
            {
                Socket proxSocket = serverSocket.Accept();
                proxSocketList.Add(proxSocket);

                byte[] data = Encoding.Default.GetBytes("Welcome! Current Time: " + DateTime.Now.ToString(CultureInfo.CurrentCulture) + "\r\n");
                proxSocket.Send(data, 0, data.Length, SocketFlags.None);

                AppendTextToTxtLog("[" + proxSocket.RemoteEndPoint + "]连接到服务器！");

                //接收客户端的消息
                ThreadPool.QueueUserWorkItem(new WaitCallback(ReceiveClientMsg), proxSocket);
            }
        }

        /// <summary>
        /// 接收客户端消息
        /// </summary>
        /// <param name="obj">客户端socket</param>
        private void ReceiveClientMsg(object obj)
        {
            Socket proxSocket = (Socket)obj;
            byte[] data = new byte[1024 * 1024];
            while (true)
            {
                int realLength = 0;
                try
                {
                    realLength = proxSocket.Receive(data, 0, data.Length, SocketFlags.None);
                }
                catch (Exception)
                {
                    AppendTextToTxtLog("[" + proxSocket.RemoteEndPoint + "]异常断开连接！");
                    proxSocket.Close();
                    proxSocketList.Remove(proxSocket);
                    break;
                }

                //当客户端正常断开连接时
                if (realLength <= 0)
                {
                    AppendTextToTxtLog("[" + proxSocket.RemoteEndPoint + "]正常断开连接！");
                    proxSocket.Shutdown(SocketShutdown.Both);
                    proxSocket.Close();
                    proxSocketList.Remove(proxSocket);
                    return;
                }

                string msg = Encoding.Default.GetString(data).TrimEnd('\0');
                AppendTextToTxtLog("[" + proxSocket.RemoteEndPoint + "]：" + msg);
            }
        }

        /// <summary>
        /// 输出消息到文本框
        /// </summary>
        /// <param name="txt">消息内容</param>
        private void AppendTextToTxtLog(string txt)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action<string>(msg =>
                {
                    txtLog.Text = string.Format("{0}\r\n{1}", msg, txtLog.Text);
                }), txt);
            }
            else
            {
                txtLog.Text = string.Format("{0}\r\n{1}", txt, txtLog.Text);
            }
        }

        /// <summary>
        /// 服务端发送消息
        /// </summary>
        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtMsg.Text;
            foreach (Socket proxSocket in proxSocketList)
            {
                //如果客户端为连接状态
                if (proxSocket.Connected)
                {
                    byte[] data = Encoding.Default.GetBytes("[服务器]：" + msg + "\r\n");
                    proxSocket.Send(data, 0, data.Length, SocketFlags.None);
                }
            }
            AppendTextToTxtLog("[服务器]：" + msg);
            txtMsg.Text = "";
        }

        private void txtMsg_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //按下回车发送信息
            if (e.KeyValue == 13)
            {
                btnSend_Click(this, e);
            }
        }
    }
}
