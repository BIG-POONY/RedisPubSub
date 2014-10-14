using System;
using System.Configuration;
using StackExchange.Redis;
using PubSubModels;
using Newtonsoft.Json;

namespace RedisPublish
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: RedisPublish <channel> <user>");
                return;
            }

            ConnectionMultiplexer connection;

            try
            {
                connection = ConnectionMultiplexer.Connect(String.Format("{0}.redis.cache.windows.net,password={1}",
                                        ConfigurationManager.AppSettings["CacheName"],
                                        ConfigurationManager.AppSettings["CacheKey"]));
            }
            catch (Exception e)
            {
                Console.WriteLine("**** EXCEPTION OCCURRED WHEN TRYING TO CONNECT TO REDIS ***");
                Console.WriteLine(e.ToString());

                Console.Write("Press a key to quit...");
                Console.ReadKey(true);
                return;
            }

            string channel = args[0];
            string user = args[1];

            Console.WriteLine("Greetings, {0}. This is the Azure Redis Publishing demo", user);

            Console.WriteLine("Type QUIT to exit.");

            ISubscriber sub = connection.GetSubscriber();

            while (true)
            {
                Console.Write("Channel '{0}' - enter message to publish: ", channel);

                string input = Console.ReadLine();

                if (input.Trim().ToUpper() == "QUIT")
                {
                    break;
                }

                if (input.Trim().Length == 0)
                {
                    continue;
                }

                ChatMessage msg = new ChatMessage(user, input);

                Console.WriteLine(" -- ready to publish");
                sub.Publish(channel, JsonConvert.SerializeObject(msg));
                Console.WriteLine(" -- message published");
            }
                 
        }
    }
}
