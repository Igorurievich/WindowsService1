using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService
{
    static class Program
    {
        static void Main()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var servicesToRun = new ServiceBase[] 
            { 
                new TCPSERVER_SERVICE() 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
