using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        private static IPAddress ipAddr = ipHost.AddressList[0];
        private static IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 9050);
        private readonly BackgroundWorker worker = new BackgroundWorker();
        //AddressFamily.InterNetwork us IPV4
        private static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //private static readonly List<Socket> clientSockets = new List<Socket>();
        private const int BUFFER_SIZE = 2048;
        private static string data = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
        }


        private async void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            try
            {
                while (true)
                {
                    // Suspend while waiting for 
                    // incoming connection Using  
                    // Accept() method the server  
                    // will accept connection of client 
                    Socket clientSocket = serverSocket.Accept();
                    IPEndPoint newClient = (IPEndPoint)clientSocket.RemoteEndPoint;
                    Console.WriteLine("Connected with {0} at port {1}", newClient.AddressFamily, newClient.Port);
                    // Data buffer 
                    byte[] bytes = new Byte[BUFFER_SIZE];
                    int numByte = clientSocket.Receive(bytes, SocketFlags.None);
                    data = Encoding.ASCII.GetString(bytes, 0, numByte);

                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        txtReceive.AppendText("Connected with "+ newClient.AddressFamily+" at port "+ newClient.Port+"\n");
                        txtReceive.AppendText(data + "\n\n");
                        txtReceive.ScrollToEnd();
                    }));
                   
                    byte[] message = Encoding.ASCII.GetBytes(data.ToLower());
                    clientSocket.Send(message);
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void worker_RunWorkerCompleted(object sender,RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
            //txtReceive.AppendText(data + "\n");
        }

        private async void SetupServer()
        {
            try
            {
                Console.WriteLine("Setting up server...");
                txtReceive.Text = "Setting up server..." + Environment.NewLine;
                serverSocket.Bind(localEndPoint);
                serverSocket.Listen(10);
                await Task.Delay(750);
                Console.WriteLine("Waiting connection ... ");
                txtReceive.AppendText("Waiting connection ... " + Environment.NewLine);
                worker.DoWork += worker_DoWork;
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                worker.RunWorkerAsync();
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void StartSrv_Click(object sender, RoutedEventArgs e)
        {
            SetupServer();
        }
    }
}
