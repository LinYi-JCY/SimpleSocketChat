using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Socket聊天客户端
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            //创建Socket对象
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //连接服务器
                socket.Connect(IPAddress.Parse(txtIp.Text), int.Parse(txtPort.Text));
            }
            catch (Exception)
            {
                //Thread.Sleep(500);
                //btnConnect_Click(this, e);

                MessageBox.Show("出现异常，请重新连接！");
                return;
            }

            //接收消息
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

        private void btnSend_Click(object sender, EventArgs e)
        {

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
