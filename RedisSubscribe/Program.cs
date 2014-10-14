using System;
using System.Configuration;
using StackExchange.Redis;
using PubSubModels;
using Newtonsoft.Json;

namespace RedisSubscribe
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: RedisSubscribe <channel1> <channel2>");
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

            string channel1 = args[0];
            string channel2 = args[1];

            ISubscriber sub = connection.GetSubscriber();

            Console.WriteLine("Monitoring channels '{0}' and '{1}'.  Press 'Q' to quit.", channel1, channel2);

            sub.Subscribe(channel1, (channelName, message) =>
            {
                ChatMessage msg = JsonConvert.DeserializeObject<ChatMessage>(message);

                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Blue;

                Console.WriteLine("{2}: User '{0}' said: {1}", msg.User, msg.Message, msg.Created.ToLocalTime());

                Console.ResetColor();
            });

            sub.Subscribe(channel2, (channelName, message) =>
            {
                ChatMessage msg = JsonConvert.DeserializeObject<ChatMessage>(message);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.DarkMagenta;

                Console.WriteLine("{2}: User '{0}' said: {1}", msg.User, msg.Message, msg.Created.ToLocalTime());

                Console.ResetColor();
            });


            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    if ((key.KeyChar == 'Q') || (key.KeyChar == 'q'))
                    {
                        break;
                    }
                }
            }
        }
    }
}
