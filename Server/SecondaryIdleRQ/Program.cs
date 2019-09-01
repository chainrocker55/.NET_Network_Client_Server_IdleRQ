using Nancy.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Timers;

namespace SecondaryIdleRQ
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string data;
            string text = string.Empty;
            PackageData package;
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.Title = "IdleRQ Secondary";
            socket.Bind(ipep);
            socket.Listen(10);
            Console.WriteLine("Waiting for client");
            Socket client = socket.Accept();
            IPEndPoint newClient = (IPEndPoint)client.RemoteEndPoint;
            Console.WriteLine("Connected with {0} at port {1}",newClient.Address,newClient.Port);

            NetworkStream ns = new NetworkStream(client);
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.WriteLine("Connected success");
            sw.Flush();

            while (true)
            {
                try
                {
                    data = sr.ReadLine();
                    if (data != null)
                    {
                        data = Base64Decode(data);
                        Console.WriteLine(data);
                        data = FormatType(data);
                        JPackage frame = JPackage.Deserialize(data);
                        package = frame.Value.ToObject<PackageData>();
                        text += package.DATA;
                    }
                   
                }
                catch (IOException)
                {
                    break;
                }

                sw.WriteLine("ACK");
                sw.Flush();
            }
            ns.Close();
            sr.Close();
            sw.Close();

            Console.WriteLine();
            Console.WriteLine("*********************** Text file ************************");
            Console.WriteLine();
            Console.WriteLine(text);
            Console.WriteLine("Disconnected");

        }
        public static string FormatType(string dataJson)
        {
            string dataFormat = dataJson.Replace("Idle_RQ", "SecondaryIdleRQ");
            dataFormat = dataFormat.Replace("Idle RQ", "SecondaryIdleRQ");
            return dataFormat;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
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
        static bool VerifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = ComputeMD5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (comparer.Compare(hashOfInput, hash) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
