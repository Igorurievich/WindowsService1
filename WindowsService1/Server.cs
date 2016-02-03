//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using FirebirdSql.Data.FirebirdClient;
//using System.Net.Sockets;
//using System.Net;
//using System.Data;
//using System.Net.NetworkInformation;
//using System.Xml;
//using System.Threading;

//namespace WindowsService
//{
//    class Server
//    {
//        String userID = String.Empty;
//        String password = String.Empty;
//        String database = String.Empty;
//        String charset = String.Empty;
//        FbConnection fb;
//        TcpListener Listener;
//        public Server(int port, IPAddress ipAddress)
//        {
//            IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, 5050);
//            Listener = new TcpListener(ipLocalEndPoint);
//            try
//            {
//                Listener.Start();
//            }
//            catch (Exception ex)
//            {
//                Log.Add(ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite);
//            }
//            getDatabaseSetting();
//            DataBaseConnect();
//            Log.Add("Server starting...");
//            while (true)
//            {
//                // Принимаем нового клиента
//                TcpClient Client = Listener.AcceptTcpClient();
//                // Создаем поток
//                Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
//                // И запускаем этот поток, передавая ему принятого клиента
//                Thread.Start(Client);
//            }
//        }
//        void ClientThread(Object StateInfo)
//        {
//            TryInternet();
//            new Client((TcpClient)StateInfo, fb);
//            Log.Add("_____________________________________________");
//        }
//        ~Server()
//        {
//            if (Listener != null)
//            {
//                Listener.Stop();
//            }
//        }
//        private void DataBaseConnect()
//        {

//            //Console.WriteLine(Msg.IMEI +" Connected to data");
//            var fb_con = new FbConnectionStringBuilder()
//            {

//                Charset = charset,
//                //кодировка
//                UserID = userID,
//                //логин
//                Password = password,
//                //пароль
//                //Database = @"D:\IB Expert 2.0/TESTBD.fdb",
//                Database = database,
//                //путь к файлу бд
//                ServerType = 0
//            };
//            //указываем тип сервера (0 - "полноценный Firebird" (classic или super server), 1 - встроенный (embedded))
//            fb = new FbConnection(fb_con.ToString()); //передаем нашу строку подключения объекту класса FbConnection
//            //FbDatabaseInfo fb_inf = null; //информация о БД
//            try
//            {
//                if (fb.State != ConnectionState.Open)
//                {
//                    fb.Open(); //открываем БД
//                }
//            }
//            catch (Exception ex)
//            {
//                //Console.WriteLine(ex.Message);
//                Log.Add(ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite);
//            }

//        }
//        private bool TryInternet()
//        {
//            IPStatus status = new IPStatus();
//            try
//            {
//                Ping p = new Ping();
//                PingReply pr = p.Send(@"8.8.8.8");
//                status = pr.Status;
//            }
//            catch (Exception ex)
//            {
//                //Console.WriteLine(ex.Message);
//                Log.Add(ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite);
//            }
//            if (status == IPStatus.Success)
//            {
//                //Console.WriteLine("Сервер работает");
//                Log.Add("Internet: true");
//                return true;
//            }
//            else
//            {
//                //Console.WriteLine("Сервер временно недоступен!");
//                Log.Add("Internet: false");
//                return false;
//            }
//        }
//        private void getDatabaseSetting()
//        {
//            XmlDocument xDoc = new XmlDocument();
//            xDoc.Load(@"D:\Service\Debug\data.xml");
//            // получим корневой элемент
//            XmlElement xRoot = xDoc.DocumentElement;
//            foreach (XmlNode xnode in xRoot)
//            {
//                // получаем атрибут name
//                if (xnode.Attributes.Count > 0)
//                {
//                    XmlNode attr = xnode.Attributes.GetNamedItem("name");
//                    //if (attr != null)
//                    //    Console.WriteLine(attr.Value);
//                }
//                // обходим все дочерние узлы элемента user
//                foreach (XmlNode childnode in xnode.ChildNodes)
//                {
//                    // если узел - company
//                    if (childnode.Name == "Charset")
//                    {
//                        charset = Convert.ToString(childnode.InnerText);
//                        //Console.WriteLine("Charset: {0}", childnode.InnerText);
//                    }
//                    if (childnode.Name == "UserID")
//                    {
//                        userID = Convert.ToString(childnode.InnerText);
//                        //Console.WriteLine("UserID: {0}", childnode.InnerText);
//                    }
//                    if (childnode.Name == "Password")
//                    {
//                        password = Convert.ToString(childnode.InnerText);
//                        //Console.WriteLine("Password: {0}", childnode.InnerText);
//                    }
//                    if (childnode.Name == "Database")
//                    {
//                        database = Convert.ToString(childnode.InnerText);
//                        //Console.WriteLine("Database: {0}", childnode.InnerText);
//                    }
//                }
//            }
//        }
//    }
//}
