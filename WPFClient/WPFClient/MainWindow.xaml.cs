using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
 
namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int PORT = 9050;
        private const int BUFFER_SIZE = 2048;
        private static string fileContent = string.Empty;
        private static string filePath = string.Empty;
        private static string fileName = string.Empty;


        public MainWindow()
        {
            InitializeComponent();

        }
        private static void ShowErrorDialog(string message)
        {
            MessageBox.Show(message);
        }
        private void ExecuteClient()
        {

            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), PORT);

                Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {

                    sender.Connect(localEndPoint);
                    Console.WriteLine("Socket connected to -> {0} ", sender.RemoteEndPoint.ToString());

                    byte[] textSender = Encoding.ASCII.GetBytes(fileContent);
                    int byteSent = sender.Send(textSender, textSender.Length, SocketFlags.None);

                    byte[] textReceived = new byte[BUFFER_SIZE];
                    int byteRecv = sender.Receive(textReceived);
                    string dataFromServer = Encoding.ASCII.GetString(textReceived, 0, byteRecv);
                    Console.WriteLine("Message from Server -> {0}", dataFromServer);
                    txtServer.Text = dataFromServer;
                    writeFile(dataFromServer);

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }

                catch (ArgumentNullException ane)
                {

                    ShowErrorDialog(ane.Message);
                }

                catch (SocketException se)
                {

                    ShowErrorDialog(se.Message);
                }

                catch (Exception e)
                {
                    ShowErrorDialog(e.Message);
                }
            }

            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
        }
        private void writeFile(string text)
        {
            File.WriteAllText(filePath + "\\" + fileName + "_Small.txt", text);
        }

        private void BtnSelectFile_Click(object sender, RoutedEventArgs  e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = @"D:\Network\Client",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 1,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true,
                AddExtension = true

            };
            if (openFileDialog.ShowDialog() == true)
            {

                txtPath.Text = openFileDialog.FileName;
                filePath = openFileDialog.InitialDirectory;
                fileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);

                var fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                }
                txtClient.Text = fileContent;
                ExecuteClient();

            }
        }
    }
}
