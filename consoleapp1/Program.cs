using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Configuration;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static IDatabase cache;
        static void Main(string[] args)
        {
            string menuValue = "";
            InitRedis();
            while (true)
            {
                Console.WriteLine("Redis reader:  what next?");
                Console.WriteLine("1:  List Keys");
                Console.WriteLine("2:  Read Key");
                Console.WriteLine("3:  Write Key");
                Console.WriteLine("4:  Write Key to text file");
                Console.WriteLine("Q:  Quit");
                menuValue = Console.ReadLine();

                switch (menuValue)
                {
                    case "1":
                        GetKeys();
                        break;
                    case "2":
                        ReadKey();
                        break;
                    case "3":
                        WriteKey();
                        break;
                    case "4":
                        FileOutKey();
                        break;
                    case "Q":
                    case "q":
                        QuitApp();
                        break;                    
                    default:
                        break;
                }
            }
        }
        static string GetValue(string key)
        {
            return cache.StringGet(key);
        }

        static void FileOutKey()
        {
            //string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            string userName = Environment.UserName.ToString();
            string outPath = "c:\\Users\\" + userName + "\\Downloads\\";
            string readLnResult = "";
            string cacheCommand = "";
            string redisResult = "";

            Console.WriteLine("\nHello! Type the name of the key");
            readLnResult = Console.ReadLine();
            // Demostrate "SET Message" executed as expected...
            cacheCommand = "GET Message";
            Console.WriteLine("\nCache command  : " + cacheCommand + " or StringGet()");
            redisResult = cache.StringGet(readLnResult).ToString();
            if(redisResult != "")
            {
                int c = 1;
                bool addToFileCount = true;
                string newFilePath = "";
                string completeFilePath = outPath + readLnResult + ".txt";
                File.WriteAllText(completeFilePath, redisResult);
                //if (!File.Exists(completeFilePath))
                //{
                //    File.WriteAllText(completeFilePath, redisResult);
                //}else {
                //    newFilePath = completeFilePath.Remove(completeFilePath.Length - 4);
                //    while (addToFileCount)
                //    {
                //        if (!File.Exists(newFilePath + c.ToString() + ".txt"))
                //        {
                //            newFilePath = newFilePath + c.ToString() + ".txt";
                //            addToFileCount = false;
                //            File.WriteAllText(newFilePath, redisResult);
                //        }
                //        c++;
                //    }
                //};
                Console.WriteLine("Cache response written to file: " + (addToFileCount ? completeFilePath.ToString() : newFilePath.ToString()));
            } else{
                Console.WriteLine("Cache response :  Nothing was found ... check key name");
            };
        }

        static void GetKeys()
        {
            //var endpoints = Connection.GetEndPoints()[0];
            //var KeyResults = Connection.GetServer(endpoints).Keys(database: 0, pattern: "*");
            //foreach (var q in KeyResults)
            //{
            //    Console.WriteLine("Key: " + q.ToString() + " with : " + GetValue(q) + "/n:End");
            //}

            var myserver = Connection.GetServer("cibica.redis.cache.windows.net", 6380);
            foreach (var key in myserver.Keys(pattern: "*"))
            {
                Console.WriteLine("\n Key : " + key.ToString());
                //Console.WriteLine("Key: " + key.ToString() + " with : " + GetValue(key) + "/n:End");
            };
            Console.WriteLine("Cache response : \n" + cache.Execute("KEYS", "*"));
        }

        static void ReadKey()
        {
            string readLnResult = "";
            string cacheCommand = "";

            Console.WriteLine("\nHello! Type the name of the key");
            readLnResult = Console.ReadLine();
            // Demostrate "SET Message" executed as expected...
            cacheCommand = "GET Message";
            Console.WriteLine("\nCache command  : " + cacheCommand + " or StringGet()");
            Console.WriteLine("Cache response : " + cache.StringGet(readLnResult).ToString());
        }

        static void WriteKey()
        {
            string cacheCommand = "";
            string readLnResult = "";

            Console.WriteLine("\nHello! Type the name of the key");
            Console.WriteLine("\nThis Doesn't work yet Type Q to exit!");
            readLnResult = Console.ReadLine();

            if (readLnResult != "Q" || readLnResult != "q")
            {
                cacheCommand = "SET Message \"Hello! The cache is working from a .NET console app!\"";
                Console.WriteLine("\nCache command  : " + cacheCommand + " or StringSet()");
                //Console.WriteLine("Cache response : " + cache.StringSet("Message", "Hello! The cache is working from a .NET console app!").ToString());

                // Demostrate "SET Message" executed as expected...
                cacheCommand = "GET Message";
                Console.WriteLine("\nCache command  : " + cacheCommand + " or StringGet()");
                Console.WriteLine("Cache response : " + cache.StringGet("Message").ToString());
            }
            
        }

        static void InitRedis()
        {
            cache = lazyConnection.Value.GetDatabase();

            // Perform cache operations using the cache object...

            // Simple PING command
            string cacheCommand = "PING";
            Console.WriteLine("\nCache command  : " + cacheCommand);
            Console.WriteLine("Cache response : " + cache.Execute(cacheCommand).ToString());
        }

        static void QuitApp()
        {
            lazyConnection.Value.Dispose();
            Environment.Exit(0); //Application.Exit
        }
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = ConfigurationManager.AppSettings["CacheConnection"].ToString();
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }                                                                            
    }
}
