using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.IO.Ports;


namespace tcpserverTest
{
   
    public partial class Form1 : Form
    {


        TcpListener listener = null;
        private bool m_listen = true;
        List<TcpClientState> clientArray = new List<TcpClientState>();
        string[] datas = { "*81..00+0000000496732928 82..00+0000003439545282 83..00+0000000000000000\r\n" ,
                                     "*81..00+0000000496742928 82..00+0000003439545282 83..00+0000000000040655\r\n",
                                    "*81..00+0000000496732928 82..00+0000003439555282 83..00+0000000000040655\r\n",
                                    "*81..00+0000000496742928 82..00+0000003439555282 83..00+0000000000040655\r\n"};
        
        SerialPort m_port = null;

        public Form1()
        {
            InitializeComponent();

        }
        private delegate void ShowInfoDelegate(BarCodeHook.BarCodes barCode);
        private void ShowInfo(BarCodeHook.BarCodes barCode)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowInfoDelegate(ShowInfo), new object[] { barCode });
            }
            else
            {
                string s = string.Format("键名:{0},虚拟码:{1},扫描码:{2},AscII:{3},字符:{4}",
                    barCode.KeyName, barCode.VirtKey, barCode.ScanCode, barCode.AscII, barCode.Chr);
                textBox1.AppendText(s + Environment.NewLine);
                //在这里进行键入值
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // ReSharper disable once ArgumentsStyleOther
            comport.Items.AddRange( items: SerialPort.GetPortNames());
            this.comport.SelectedIndex = 0;
            this.comb.SelectedIndex = 0;
            this.textBox1.AppendText(Convert.ToChar(27).ToString());
            this.txData.Text = datas[0];
        }


        #region//tcp
        private void ClientConnect(IAsyncResult ar)
        {
            try
            {
                TcpListener listener = (TcpListener)ar.AsyncState;
                //接受客户的连接,得到连接的Socket
                TcpClient client = listener.EndAcceptTcpClient(ar);
                if (this != null)
                {
                    this.Invoke(new EventHandler(delegate
                    {
                        this.textBox1.AppendText(client.Client.RemoteEndPoint.ToString() + "连接成功" + Environment.NewLine);
                    }));
                }

                byte[] buf = new byte[client.ReceiveBufferSize];
                TcpClientState innerclient = new TcpClientState(client, buf);
                this.clientArray.Add(innerclient);
                NetworkStream netstream = client.GetStream();
                netstream.BeginRead(innerclient.Buffer, 0, innerclient.Buffer.Length, DataReceived, innerclient);

                listener.BeginAcceptTcpClient(ClientConnect, ar.AsyncState);
            }

            catch(Exception ex) {
                this.Invoke(new EventHandler(delegate
                {
                    this.textBox1.AppendText(ex.Message+Environment.NewLine);
                }));
                
            }

        }
        int index = 0;
        private void DataReceived(IAsyncResult ar)
        {
            TcpClientState innerclient = (TcpClientState)ar.AsyncState;
            if ((innerclient.InnerClient == null) || (!innerclient.InnerClient.Connected))
                return;
            NetworkStream stream = innerclient.InnerClient.GetStream();
            int num = 0;
            //异常断开
            try { num = stream.EndRead(ar); }
            catch { num = 0; }

            if (num < 1)//断开连接
            {
                this.Invoke(new EventHandler(delegate
                {
                    this.textBox1.AppendText(innerclient.InnerClient.Client.RemoteEndPoint.ToString()+"断开连接" + Environment.NewLine);
                }));
                
                clientArray.Remove(innerclient);
                return;
            }

            byte[] receivebytes = new byte[num];
            Buffer.BlockCopy(innerclient.Buffer, 0, receivebytes, 0, num);
            string s = Encoding.Default.GetString(receivebytes);
            if (this != null)
            {
                this.Invoke(new EventHandler(delegate
                {
                    this.textBox1.AppendText(s);
                    if (!string.IsNullOrEmpty(s))
                    {
                        //string data="*81..00+0000000496732928 82..00+0000003439545282 83..00+0000000000040655\r\n";
                        this.textBox1.AppendText("发送:" + this.txData.Text + Environment.NewLine);
                        this.Send(innerclient.InnerClient, Encoding.Default.GetBytes(this.txData.Text));
                        index++;
                        if (index > 3) index = 0;
                        this.txData.Text = datas[index];

                    }
                }));
            }
       

            stream.BeginRead(
             innerclient.Buffer, 0, innerclient.Buffer.Length, DataReceived, innerclient);

        }
        private void Send(TcpClient client, byte[] data)
        {
            client.GetStream().BeginWrite(data, 0, data.Length, ar => { ((TcpClient)ar.AsyncState).GetStream().EndWrite(ar); }, client);
        }
        private void btnlisten_Click(object sender, EventArgs e)
        {
            if (listener == null)
            {
                IPAddress local = IPAddress.Any;
                IPEndPoint iep = new IPEndPoint(local, (int)this.NUMPort.Value);
                listener = new TcpListener(iep);
                m_listen = true;
                listener.Start();
                byte[] inValue = new byte[] { 1, 0, 0, 0, 0x20, 0x4e, 0, 0, 0xd0, 0x07, 0, 0 };
                listener.Server.IOControl(IOControlCode.KeepAliveValues, inValue, null);
                this.textBox1.AppendText("开启监听");

                listener.BeginAcceptTcpClient(new AsyncCallback(ClientConnect), listener);
                this.btnlisten.Enabled = false;

            }
            else
            {
                m_listen = false;
                this.textBox1.AppendText("开启监听");
            }
        } 
        #endregion


        #region com
        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                m_port = new SerialPort(comport.Text, int.Parse(comb.Text));
                m_port.DataReceived += new SerialDataReceivedEventHandler(m_port_DataReceived);
                m_port.Open();
                if (m_port.IsOpen)
                {
                    this.btnOpen.Enabled = false;
                    this.txComm.AppendText("打开串口成功"+Environment.NewLine);
                }

            }
            catch (Exception ex)
            {
                txComm.AppendText(ex.Message + Environment.NewLine);
            }

        }

        private StringBuilder _data = new StringBuilder();
        void m_port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int length = m_port.ReadBufferSize;
            byte[] buf = new byte[length];
            m_port.Read(buf, 0, length);
            string s = Encoding.Default.GetString(buf);
            _data.Append(s);
            double x, y, z;
            parse(out x, out y, out z);
            this.Invoke(new EventHandler(delegate {
                this.txComm.AppendText(s);
                this.txComm.AppendText(string.Format("x={0},y={1},z={2}", x, y, z)+Environment.NewLine);

            }));
            
        }
        void parse( out double x, out double y, out double  z)
        {// "*81..00+0000000496732928 82..00+0000003439545282 83..00+0000000000040655\r\n"
            x = 0; y = 0; z = 0;
            try
            {
                string data = _data.ToString();
                int startIndex = 0;
                while (true)
                {
                    startIndex = data.IndexOf('*', startIndex);
                    if (startIndex >= 0)
                    {
                        int endIndex = data.IndexOf("\r\n", startIndex);
                        if (endIndex > 0)
                        {
                            string[] xyz = data.Substring(startIndex + 1, endIndex - startIndex - 1).Split(' ');
                            y = double.Parse(xyz[0].Substring(6)) / 1000;
                            x = double.Parse(xyz[1].Substring(6)) / 1000;
                            z = double.Parse(xyz[2].Substring(6)) / 1000;
                            _data.Remove(startIndex, endIndex);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                    startIndex++;
                }
            }
            catch
            {

            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (m_port != null)
            {
                m_port.Close();
                this.btnOpen.Enabled = true;
            
            }
        }

        private void btnComSend_Click(object sender, EventArgs e)
        {
            if (m_port != null)
            {
                if (m_port.IsOpen)
                {
      
                    byte[] buf=Encoding.ASCII.GetBytes("GET/M/WI81/WI82/WI83\r\n");
                    m_port.Write(buf, 0, buf.Length);
                    this.txComm.AppendText("发送GET/M/WI81/WI82/WI83"+Environment.NewLine);
                }
            }
        }
        #endregion

        private void btnCls_Click(object sender, EventArgs e)
        {
            if (listener != null)
            {
                listener.Stop();
                this.btnlisten.Enabled = true;
            }
        }
    }

    public class TcpClientState
    {
        public TcpClient InnerClient { get; set;}
        public byte[] Buffer{get; set;}
        
        public TcpClientState(TcpClient client, byte[] buf)
        {
            this.InnerClient = client;
            Buffer = buf;
        }
    }
}
