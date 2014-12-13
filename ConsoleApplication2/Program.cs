using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
			// create a MemcachedClient
			// in your application you can cache the client in a static variable or just recreate it every time
			MemcachedClient mc = new MemcachedClient("logQueue");

			// you can create another client using a different section from your app/web.config
			// this client instance can have different pool settings, key transformer, etc.
			// MemcachedClient mc2 = new MemcachedClient("memcached");

			// or just initialize the client from code
/*
			MemcachedClientConfiguration config = new MemcachedClientConfiguration();
			config.Servers.Add(new IPEndPoint(IPAddress.Loopback, 11211));
			config.Protocol = MemcachedProtocol.Text;

			var mc = new MemcachedClient(config);
*/

			for (var i = 0; i < 100; i++)
				mc.Store(StoreMode.Set, "testlog", "Hello World");



			// retrieve the item from the cache
			//Console.WriteLine(mc.Get("MyKey"));

			//Console.WriteLine(mc.Increment("num1", 1, 10));
			//Console.WriteLine(mc.Increment("num1", 1, 10));
			//Console.WriteLine(mc.Decrement("num1", 1, 14));


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
