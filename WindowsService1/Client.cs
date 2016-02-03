using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using FirebirdSql.Data.FirebirdClient;

namespace WindowsService
{
    internal class Client
    {
        private readonly Message Msg;
        public byte[] Buff = new byte[1024];
        private FbTransaction fbt;

        public Client(TcpClient Client, FbConnection fb)
        {
            byte[] bf = {1};
            Client.GetStream().Write(bf, 0, bf.Length);
            do
            {
                int BytesCount;
                try
                {
                    BytesCount = Client.GetStream().Read(Buff, 0, Buff.Length);
                }
                catch (Exception ex)
                {
                    Log.Add(ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite);
                    break;
                }
                if (Msg.IMEI == 0 && Buff.Length > 0)
                {
                    try
                    {
                        Msg.IMEI = Msg.GetIMEI(BytesCount, Buff);
                    }
                    catch (Exception ex)
                    {
                        Log.Add(ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite);
                        break;
                    }
                    Log.Add("Client: " + Msg.IMEI + " connected!");
                }
                if (Msg.IMEI > 0)
                {
                    Msg.InitComponents(Buff);
                    var offset = Msg.GetOffset();
                    try
                    {
                        Log.Add("[" + Msg.IMEI + "] " + " [" + DateTime.Now + "]");
                        Log.Add("Parsing client data");
                        var InvalidMsg = new List<Message>();
                        for (var i = 0; i < Msg.MessageCount; i++)
                        {
                            Msg.DTime = Msg.ByteToDateTime(offset + 2, Buff);
                            Msg.Longitude = Msg.ByteToInt(offset + 11, Buff);
                            Msg.Latitude = Msg.ByteToInt(offset + 15, Buff);
                            Msg.Speed = Msg.ByteToShort(offset + 24, Buff);
                            offset -= 33;
                            if (Msg.Speed == 0)
                            {
                            }
                            else if (Msg.Speed > 0)
                            {
                                DataBaseSendData(Msg.IMEI, Msg.DTime, Msg.Latitude, Msg.Longitude, Msg.Speed, fb);
                                Log.Add("DB operation complete");
                            }
                        }
                        Msg.IMEI = 0;
                    }
                    catch (Exception ex)
                    {
                        Log.Add(ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite);
                        break;
                    }
                }
            } while (Msg.IMEI > 0);
            try
            {
                Client.Close();
                Log.Add("Client was closed");
            }
            catch (Exception ex)
            {
                Log.Add("Client not closed");
                Log.Add(string.Format(ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite));
            }
        }

        private void DataBaseSendData(long IMEI, DateTime dt, int latitude, int longitude, int speed, FbConnection fb)
        {
            if ((latitude != 0) && (longitude != 0))
            {
                fbt = fb.BeginTransaction();
                using (
                    var insertSQL = new FbCommand("insert into sms" +"(sms.IMEI, sms.DATE_, sms.TIME_, sms.LATITUDE, sms.LONGITUDE, sms.SPEED) values('"+ IMEI 
                    + "','" + dt.ToShortDateString() 
                    + "','" + dt.ToLongTimeString() 
                    + "','" + latitude 
                    + "','" + longitude 
                    + "','" + speed 
                    + "');", fb)) // using Firebird DB, FBCommand haven't method "AddWithValue".
                {
                    if (fb.State == ConnectionState.Closed)
                    {
                        fb.Open();
                    }
                    insertSQL.Transaction = fbt;
                    try
                    {
                        insertSQL.ExecuteNonQueryAsync();
                        fbt.Commit();
                    }
                    catch (Exception ex)
                    {
                        fbt.Rollback();
                        Log.Add(ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite);
                    }
                }
            }
        }
    }
}