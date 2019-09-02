using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
namespace Primary
{
    class Program
    {
        private static Timer aTimer = new Timer();
        private static bool timeout = false;
        private static bool status = false;
        static void Main(string[] args)
        {
            string data;
            string ack = string.Empty;
            int start = 0;
            int chunkSize = 8;

            string temp = File.ReadAllText(@"D:\Network\Client\Lowercase.txt");
            Console.WriteLine();
            Console.WriteLine("*********************** Text file ************************");
            Console.WriteLine();
            Console.WriteLine(temp);
            Console.WriteLine();
            Console.Title = "IdleRQ Primary";

            PackageData package;
            PackageData[] ArrayPackage = GetArrayPackage(temp, chunkSize);
            Console.WriteLine("All size of package is {0}", ArrayPackage.Length);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.Connect(ipep);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Unable to connect to server");
                Console.WriteLine(e.ToString());
                return;
            }

            NetworkStream ns = new NetworkStream(server);
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            ack = sr.ReadLine();
            Console.WriteLine(ack);
            Console.WriteLine("Press the Enter key to start");
            Console.ReadLine();
            StartTime();

            while (start < ArrayPackage.Length)
            {
                
                while (timeout) { }
                //Console.WriteLine("Time out repeat frame {0} again", start);
               // while (status) { }

                if (ack.Equals("ACK") || ack.Equals("Connected success"))
                {
                    package = ArrayPackage[start];
                    string obj = JPackage.Serialize(JPackage.FromValue(package));
                    Console.WriteLine(obj);
                    obj = Base64Encode(obj);
                    sw.WriteLine(obj);
                    sw.Flush();
                    //status = true;
                    start++;
                    ack = string.Empty;
                    timeout =  true;
                }
                else
                {
                    package = ArrayPackage[start];
                    string obj = JPackage.Serialize(JPackage.FromValue(package));
                    Console.WriteLine(obj);
                    obj = Base64Encode(obj);
                    sw.WriteLine(obj);
                    sw.Flush();
                    //status = true;
                    ack = string.Empty;
                    timeout = false;
                    aTimer.Start();
                }
                ack = sr.ReadLine();
                Console.WriteLine("Server response : {0}", ack);
                Console.WriteLine();

            }
            ns.Close();
            sr.Close();
            sw.Close();
            server.Shutdown(SocketShutdown.Both);
            server.Close();
            aTimer.Stop();
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            timeout = !timeout;
            status = false;
        }
        public static void StartTime()
        {
            aTimer.Interval = 1000;

            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            aTimer.AutoReset = true;

            // Start the timer
            aTimer.Enabled = true;

        }
        public static PackageData[] GetArrayPackage(string text, int chunkSize)
        {
            string[] listStr = Split(text, chunkSize);
            PackageData[] NewArrayPacket = new PackageData[listStr.Length];
            for (int i = 0; i < listStr.Length; i++)
            {
                string hashMD5 = ComputeMD5Hash(listStr[i]);
                NewArrayPacket[i] = new PackageData(i, listStr[i], hashMD5);
            }
            return NewArrayPacket;

        }
        public static string[] Split(string str, int chunkSize)
        {
            List<string> list = new List<string>();
            int stringLength = str.Length;
            for (int i = 0; i < stringLength; i += chunkSize)
            {
                if (i + chunkSize > stringLength)
                    chunkSize = stringLength - i;

                list.Add(str.Substring(i, chunkSize));

            }
            return list.ToArray();
        }
       
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string ComputeMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
