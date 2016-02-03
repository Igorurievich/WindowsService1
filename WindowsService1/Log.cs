using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService
{
    class Log
    {
        public static void Add(string Message)
        {
            try
            {
                string file = "Log[" + DateTime.Now.ToShortDateString() + "].txt";
                string path = @"C:\GPStrackers\Logs" + "\\" + file;
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine("StartLog");
                    }
                }
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLineAsync(DateTime.Now + "  " + Message);
                }
            }
            catch (Exception e)
            {
                Log.Add(e.Message);
            }
        }
    }
}
