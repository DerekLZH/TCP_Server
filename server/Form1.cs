using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            TextBox.CheckForIllegalCrossThreadCalls = false;

        }

        Thread threadWatch = null;
        Socket socketWatch = null;

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnServerConn_Click(object sender, EventArgs e)
        {
            try
            {
                socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipaddress = IPAddress.Parse(this.txtIP.Text.Trim());
                IPEndPoint endpoint = new IPEndPoint(ipaddress, int.Parse(this.txtPort.Text.Trim()));

                socketWatch.Bind(endpoint);
                socketWatch.Listen(20);
                threadWatch = new Thread(WatchConnecting);
                threadWatch.IsBackground = true;
                threadWatch.Start();
                txtMsg.AppendText("start listening from Client..." + "\r\n");
                this.btnServerConn.Enabled = false;
            }
            catch (Exception ex)
            {
                txtMsg.AppendText("start Server failure!" + "\r\n");
                this.btnServerConn.Enabled = true;
            }
        }

        Socket socConnection = null;

        
       private void WatchConnecting()
        {
            while (true)
            {
                socConnection = socketWatch.Accept();
                txtMsg.AppendText("connect to Client successfully!" + "\r\n");
               /* ParameterizedThreadStart pts = new ParameterizedThreadStart(ServerRecMsg);
                Thread thr = new Thread(pts);*/
                Thread thr = new Thread(ServerRecMsg);
                thr.IsBackground = true;
                thr.Start(socConnection);
            }
        }

        private void ServerSendMsg(String sendMsg)
        {
            try
            {
                byte[] arrSendMsg = Encoding.UTF8.GetBytes(sendMsg);
                socConnection.Send(arrSendMsg);
                txtMsg.AppendText("Server"+"\t" + GetCurrentTime() + "\r\n" + sendMsg + "\r\n");
            }
            catch (Exception ex)
            {
                txtMsg.AppendText("Client is disconnected, cannot send message!" + "\r\n");
            }
        }

        private void ServerRecMsg(object socketClientPara)
        {
            Socket socketServer = socketClientPara as Socket;
            while (true)
            {
                byte[] arrServerRecMsg = new byte[1024 * 1024];
                try
                {
                    int length = socketServer.Receive(arrServerRecMsg);
                    string strSRecMsg = Encoding.UTF8.GetString(arrServerRecMsg, 0, length);
                    txtMsg.AppendText("Client"+"\t" + GetCurrentTime() + "\r\n" + strSRecMsg + "\r\n");
                }
                catch (Exception ex)
                {
                    txtMsg.AppendText("Client is disconnected!" + "\r\n");
                    break;
                }
            }
        }

        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            ServerSendMsg(this.txtSendMsg.Text.Trim());
            this.txtSendMsg.Clear();
        }

        private void txtSendMsg_TextChanged(object sender, EventArgs e)
        {

        }
        private DateTime GetCurrentTime()
        {
            DateTime currentTime = new DateTime();
            currentTime = DateTime.Now;
            return currentTime;
        }

        public IPAddress GetLocalIPv4Address()
        {
            IPAddress localIpv4 = null;
            IPAddress[] IpList = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress IP in IpList)
            {
                if (IP.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIpv4 = IP;

                }
                else
                {
                    continue;
                }
            }
            return localIpv4;
        }

        private void btnGetLocalIP_Click(object sender, EventArgs e)
        {
            IPAddress localIP = GetLocalIPv4Address();
            this.txtIP.Text = localIP.ToString();

        }

       

        private void txtSendMsg_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ServerSendMsg(txtSendMsg.Text.Trim());
                this.txtSendMsg.Clear();
            }
        }
    }
}
