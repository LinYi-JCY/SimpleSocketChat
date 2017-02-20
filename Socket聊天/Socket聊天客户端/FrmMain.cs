using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
        /// 客户端发送消息
        /// </summary>
        private void btnSend_Click(object sender, EventArgs e)
        {

        }

        private void txtMsg_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                btnSend_Click(this, e);
            }
        }
    }
}
