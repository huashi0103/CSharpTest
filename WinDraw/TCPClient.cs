using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Microsoft.SqlServer.Server;
using System.IO;

namespace WinDraw
{
    public delegate void GetNewDataHandle(TotalStationData data);
   public delegate void ConnectMsgHandle(bool Connected);

   public class TcpClientClass
   {
       /// <summary>服务端节点
       /// </summary>
       public IPEndPoint ServerEndPoint;
       /// <summary>是否连接
       /// </summary>
       public bool IsConnected
       {
           get { return _client != null && _client.Connected; }
       }
       /// <summary> 解析类
       /// </summary>
       private TotalStationBase _station;
       private TcpClient _client;
       private string sServer;
       private string sPort;
       /// <summary>收到新数据
       /// </summary>
       public event GetNewDataHandle GetNewData;
       /// <summary> 连接信息
       /// </summary>
       public event ConnectMsgHandle ConnectMsg;

       private NetworkStream _stream;
       private string logFilePath = "";
       public TcpClientClass()
       {
           LoadLogFile();

       }
       public void Init(TotalStationBase station, string ip = "127.0.0.1", int port = 8899)
       {

           _station = station;
           ServerEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

       }
       private void LoadLogFile()
       {
           logFilePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + string.Format("{0:yyyyMMdd}", DateTime.Now) + ".date";
           FileStream file = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
           file.Close();

       }
       private void LogData(string msg)
       {
           using (StreamWriter sw = new StreamWriter(logFilePath))
           {
               sw.WriteLine(msg);
           }
       }


       public void AsynConnect()
       {
           AsynConnect(ServerEndPoint.Address.ToString(), ServerEndPoint.Port);
       }
       /// <summary> 连接服务端
       /// </summary>
       /// <param name="ip">服务端IP</param>
       /// <param name="port">服务端端口</param>
       public void AsynConnect(string ip, int port)
       {

           ServerEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
           _client = new TcpClient();
           _client.BeginConnect(ServerEndPoint.Address, ServerEndPoint.Port, Connect, _client);
       }

       void Connect(IAsyncResult asynresult)
       {//异步连接回调
           try
           {
               _client.EndConnect(asynresult);

               if (_client.Connected)
               {
                   _stream = _client.GetStream();
                   var buf = new byte[_client.ReceiveBufferSize];

                   AsyncCallback readcallback = null;
                   readcallback = asynreadresult =>
                   {
                       int num = 0;
                       if (!_client.Connected) return;
                       num = _stream.EndRead(asynreadresult);
                       byte[] receivebytes = new byte[num];
                       Buffer.BlockCopy(buf, 0, receivebytes, 0, num);
                       var adata = _station.Analyze(receivebytes);
                       if (adata.StatusOK) LogData(adata.OldData);
                       if (GetNewData != null) GetNewData(adata);//获取新数据

                       _stream.BeginRead(buf, 0, buf.Length, readcallback, null);
                   };

                   _stream.BeginRead(buf, 0, buf.Length, readcallback, null);
               }
           }
           catch
           {

           }
           finally
           {
               if (ConnectMsg != null) ConnectMsg(_client.Connected);//连接事件
           }

       }

       /// <summary>发送消息
       /// </summary>
       /// <param name="msg"></param>
       public void Send(string msg)
       {
           if (_client.Connected)
           {
               byte[] buf = Encoding.Default.GetBytes(msg);
               _stream.BeginWrite(buf, 0, buf.Length, ar => { _stream.EndWrite(ar); }, null);
           }
       }
       /// <summary>发送测量xyz的指令
       /// </summary>
       public void SendMeasureXYZCommand()
       {
           this.Send("");
       }
       public void Closed()
       {
           if (_client == null) return;
           _client.Close();
           if (ConnectMsg != null) ConnectMsg(_client.Connected);
       }
   }

    public class ComClass
    {
        /// <summary>接收到新数据
        /// </summary>
        public event GetNewDataHandle GetNewData;
        /// <summary>是否连接
        /// </summary>
        public bool IsConnect {
            get { return m_port.IsOpen; }
        }

        private SerialPort m_port;
        private TotalStationBase _station;

        public ComClass(TotalStationBase station, string com = "com1", int prot = 9600)
        {
            m_port = new SerialPort(com, prot);
            _station = station;
        }
        /// <summary> 打开串口
        /// </summary>
        public void Connect()
        {
            try
            {
                if (m_port.IsOpen) m_port.Close();
                m_port.Open();
                m_port.DataReceived += Datareceive;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void Connect(string com, int port)
        {
            if (m_port.IsOpen) m_port.Close();
            m_port=new SerialPort(com,port);
            Connect();
        }
        /// <summary> 设置串口
        /// </summary>
        /// <param name="serialPort"></param>
        public void SetPort(SerialPort serialPort)
        {
            m_port = serialPort;
        }

        /// <summary> 关闭串口
        /// </summary>
        public void Close()
        {
            m_port.Close();
        }

        void Datareceive(object sender, SerialDataReceivedEventArgs e)
        {
            int length = m_port.ReadBufferSize;
            byte[] buf = new byte[length];
            m_port.Read(buf, 0, length);

            if (GetNewData != null) GetNewData(_station.Analyze(buf));

        }

    }


    /// <summary> 定义全站仪的数据处理基类
    /// </summary>
    public abstract class TotalStationBase
    {
        public virtual TotalStationData Analyze(byte[] data)
        {
            TotalStationData tsd = new TotalStationData();
            tsd.OriginalData=Encoding.Default.GetString(data);
            return tsd;
        }

    }

    //测试
    public class Test : TotalStationBase
    {
        public override TotalStationData Analyze(byte[] data)
        {
            return base.Analyze(data);
        }
    }


    /// <summary> 解析的数据
    /// </summary>
    public class TotalStationData
    {
        public bool StatusOK { get; set; }
        public string OldData { get; set; }
        public DataType StatinDataType { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public string OriginalData { get; set; }
        public double Angle { get; set; }
        public double SideLength { get; set; }
        public int PointCount { get; set; }

    }
    public enum DataType
    {
        /// <summary> 坐标
        /// </summary>
        XYZ = 0,
        /// <summary> 测角
        /// </summary>
        Angle = 1,
        /// <summary> 侧边
        /// </summary>
        Side = 2
    }
}
