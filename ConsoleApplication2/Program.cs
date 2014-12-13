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
        //private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly log4net.ILog log =    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        { 
			// create a MemcachedClient
			// in your application you can cache the client in a static variable or just recreate it every time
            //MemcachedClient mc = new MemcachedClient();

			// you can create another client using a different section from your app/web.config
			// this client instance can have different pool settings, key transformer, etc.
			// MemcachedClient mc2 = new MemcachedClient("memcached");

			// or just initialize the client from code
            /*
			MemcachedClientConfiguration config = new MemcachedClientConfiguration();
            config.Servers.Add(new IPEndPoint(IPAddress.Parse("10.11.11.22"), 11211));
			config.Protocol = MemcachedProtocol.Text;

			var mc = new MemcachedClient(config);
            */ 
			//for (var i = 0; i < 100; i++)

            //bool res = mc.Store(StoreMode.Set, "testlog", "Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,Hello World,");
            //Console.WriteLine(res);


			// retrieve the item from the cache
			//Console.WriteLine(mc.Get("MyKey"));

			//Console.WriteLine(mc.Increment("num1", 1, 10));
			//Console.WriteLine(mc.Increment("num1", 1, 10));
			//Console.WriteLine(mc.Decrement("num1", 1, 14));
            log.Fatal("log4net Fatal");
            log.Error("log4net Error");
            log.Warn("log4net Warn");
            log.Info("log4net Info");
            log.Debug("log4net Debug");

			Console.ReadLine();
		}

		// objects must be serializable to be able to store them in the cache
		[Serializable]
		class Product
		{
			public double Price = 1.24;
			public string Name = "Mineral Water";

			public override string ToString()
			{
				return String.Format("Product {{{0}: {1}}}", this.Name, this.Price);
			}
        }
    }
}
