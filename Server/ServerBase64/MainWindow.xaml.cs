using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace ServerBase64
{
    public partial class MainWindow : Window
    {
        private static IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        private static IPAddress ipAddr = ipHost.AddressList[0];
        private static IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 8189);
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private static Socket serverSocket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        //private static readonly List<Socket> clientSockets = new List<Socket>();
        private static string base64 = string.Empty;
        private static string path = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            SetupServer();


        }
        private async void SetupServer()
        {
            try
            {
                Console.WriteLine("Setting up server...");

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    txtReceive.Text = "Setting up server..." + Environment.NewLine;
                }));
                serverSocket.Bind(localEndPoint);
                serverSocket.Listen(10);
                await Task.Delay(750);
                Console.WriteLine("Waiting connection ... ");
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    txtReceive.AppendText("Waiting connection ... " + Environment.NewLine);
                }));

                while (true)
                {

                    Socket clientSocket = serverSocket.Accept();
                    NetworkStream ns = new NetworkStream(clientSocket, FileAccess.ReadWrite);
                    StreamReader sr = new StreamReader(ns);
                    StreamWriter sw = new StreamWriter(ns);

                    IPEndPoint newClient = (IPEndPoint)clientSocket.RemoteEndPoint;
                    Console.WriteLine("Connected with {0} at port {1}", newClient.AddressFamily, newClient.Port);

                    string[] data = Regex.Split(sr.ReadToEnd(), @",");          
                    path = data[0];
                    base64 = data[1];
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        txtReceive.Text = "Connected with " + newClient.AddressFamily + " at port " + newClient.Port + "\n";
                        txtReceive.AppendText(base64);
                    }));
                    File.WriteAllBytes(path, Convert.FromBase64String(base64));
                    //sw.WriteLine("Success");
                    //sw.Flush();
                    Console.WriteLine("Success");

                    ns.Close();
                    sr.Close();
                    sw.Close();
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void StartSrv_Click(object sender, RoutedEventArgs e)
        {
            if (startSrv.Content.Equals("Start"))
            {
                worker.DoWork += worker_DoWork;
                worker.WorkerSupportsCancellation = true;
                worker.RunWorkerAsync();
                startSrv.Content = "Stop";
            }
            else
            {
                worker.CancelAsync();
                startSrv.Content = "Start";
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();


            }
        }

    }
}
