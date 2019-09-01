using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WPFClientBase64
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int PORT = 8189;
        private static string filePath = string.Empty;
        private static string fileName = string.Empty;
        private static string fileExtention = string.Empty;
        private static string base64 = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }
        private static void ShowErrorDialog(string message)
        {
            MessageBox.Show(message);
        }
        public static string Base64Encode(string str)
        {

            return Convert.ToBase64String(Encoding.Default.GetBytes(str));
        }

        public static string Base64Decode(string str)
        {
            return Encoding.Default.GetString(Convert.FromBase64String(str));
        }
        private void ExecuteClient()
        {
            status.Content = "Waiting";
            // Establish the remote endpoint  
            // for the socket. This example  
            // uses port 11111 on the local  
            // computer. 
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, PORT);

            // Creation TCP/IP Socket using  
            // Socket Class Costructor 
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // Connect Socket to the remote  
                // endpoint using method Connect() 
                sender.Connect(localEndPoint);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
                return;
            }
            if (sender.Connected)
            {
                try
                {
                    NetworkStream ns = new NetworkStream(sender, FileAccess.ReadWrite);
                    StreamReader sr = new StreamReader(ns);
                    StreamWriter sw = new StreamWriter(ns);
                   
                    sw.WriteLine(getFullPath() + "," + base64);
                    sw.Flush();
                    
                    //Console.WriteLine(sr.ReadLine());
                    ns.Close();
                    sr.Close();
                    sw.Close();

                    Console.WriteLine("Disconnected to server");
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                    status.Content = "Success";
                    status.Foreground = Brushes.Green;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }
        private string getFullPath()
        {
            return filePath + "\\" + fileName + "_Origin" + fileExtention;
        }
        private static void writeFile(byte[] bytes)
        {
            var fs = File.OpenWrite(filePath + "\\" + fileName + "_Origin" + fileExtention);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();

            //using (var fs = File.OpenWrite(filePath + "\\test" + fileExtention))
            //{
            //    var buffer = File.ReadAllBytes(openFileDialog.FileName);
            //    fs.Write(buffer, 0, buffer.Length);
            //    //buffer = File.ReadAllBytes(Path.Combine(filePath, "2.mp3"));
            //    //fs.Write(buffer, 0, buffer.Length);
            //    fs.Flush();
            //}
        }
        /// <summary>
        /// Close socket and exit program.
        /// </summary>

        private void BtnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = @"D:\Network\Client",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "mp3",
                Filter = "All Files|*.mp3;*.jpg; *.jpeg;",
                Multiselect = false,
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
                fileExtention = Path.GetExtension(openFileDialog.FileName);
                Console.WriteLine("Type of file {0}", fileExtention);
                var fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    int len = (int)reader.BaseStream.Length;
                    byte[] bytes = new byte[len];
                    BufferedStream buff = new BufferedStream(fileStream);
                    try
                    {
                        buff.Read(bytes, 0, len);
                        base64 = Convert.ToBase64String(bytes);
                        txtBase64.Text = base64;

                        buff.Close();
                        reader.Close();
                    }
                    catch (Exception)
                    {

                    }
                }
                ExecuteClient();

            }

        }

    }
}
