using System;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Threading;
using System.Xml;
using FirebirdSql.Data.FirebirdClient;

namespace WindowsService
{
    public partial class TCPSERVER_SERVICE : ServiceBase
    {
        String userID = string.Empty;
        String password = string.Empty;
        String database = string.Empty;
        String charset = string.Empty;
        int port;
        IPAddress ipAddress;
        FbConnection fb;
        TcpClient Client;
        TcpListener Listener;
        public TCPSERVER_SERVICE()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            GetDatabaseSettings();
            while (true)
            {
                if (DataBaseOpenConnection())
                {
                    StartServer();
                    break;
                }
                Thread.Sleep(2000);
                GetDatabaseSettings();
            }
        }
        private void StartWork(Object StateInfo)
        {
            while (true)
            {
                TcpClient Client = Listener.AcceptTcpClient();
                Thread Thread = new Thread(ClientThread);
                Thread.Start(Client);
            }
        }
        void ClientThread(object StateInfo)
        {
            if(CheckNetwork())
            {
                new Client((TcpClient)StateInfo, fb);
                Log.Add("_____________________________________________");
            }
        }
        private void StartServer()
        {
            IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, 5050);
            Listener = new TcpListener(ipLocalEndPoint);
            try
            {
                Listener.Start();
            }
            catch (Exception ex)
            {
                Log.Add(ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite);
            }
            Log.Add("Server starting...");
            Listener.Start();
            Thread worker = new Thread(StartWork);
            worker.Start();
        }
        private bool DataBaseOpenConnection()
        {
            var fb_con = new FbConnectionStringBuilder
            {
                Charset = charset,
                UserID = userID,
                Password = password,
                Database = database,
                ServerType = 0
            };
            fb = new FbConnection(fb_con.ToString());
            try
            {
                if (fb.State != ConnectionState.Open)
                {
                    fb.Open();
                }
            }
            catch (Exception ex)
            {
                Log.Add(ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite);
                return false;
            }
            return true;
        }
        private static bool CheckNetwork() // some implementation of network availability
        {
            IPStatus status = new IPStatus();
            try
            {
                Ping p = new Ping();
                PingReply pr = p.Send(@"8.8.8.8");
                status = pr.Status;
            }
            catch (Exception ex)
            {
                Log.Add(ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite);
            }
            if (status == IPStatus.Success)
            {
                Log.Add("Internet: true");
                return true;
            }
            Log.Add("Internet: false");
            return false;
        }
        private void GetDatabaseSettings()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("config.xml");
            XmlElement xRoot = xDoc.DocumentElement;
            if (xRoot == null) return;
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Attributes != null && xnode.Attributes.Count > 0)
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("name");
                }
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    switch (childnode.Name)
                    {
                        case "Charset":
                        {
                            charset = Convert.ToString(childnode.InnerText);
                            Log.Add(charset);
                            break;
                        }
                        case "UserID":
                        {
                            userID = Convert.ToString(childnode.InnerText);
                            Log.Add(userID);
                            break;
                        }
                        case "Password":
                        {
                            password = Convert.ToString(childnode.InnerText);
                            Log.Add(password);
                            break;
                        }
                        case "Database":
                        {
                            database = Convert.ToString(childnode.InnerText);
                            Log.Add(database);
                            break;
                        }
                        case "ip":
                        {
                            ipAddress = Dns.Resolve(childnode.InnerText).AddressList[0];
                            Log.Add(ipAddress.ToString());
                            break;
                        }
                        case "port":
                        {
                            port = Convert.ToInt32(childnode.InnerText);
                            Log.Add(port.ToString());
                            break;
                        }
                    }
                }
            }
        }
        protected override void OnStop()
        {
            if (Listener != null)
            {
                Listener.Stop();
            }
        }
        protected override void OnContinue()
        {
            GetDatabaseSettings();
        }
    }
}
