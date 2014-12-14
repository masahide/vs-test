using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using log4net.Layout;
using log4net.Core;
using log4net.Util;

namespace ConsoleApplication2
{
    class Program
    {
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        { 
            log.Fatal("log4net Fatal");
            log.Error("log4net Error");
            log.Warn("log4net Warn");
            log.Info("log4net Info");
            log.Debug("log4net Debug");
            

			Console.ReadLine();
		}
    }
}
