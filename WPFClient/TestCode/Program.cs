using System.Collections.Generic;

namespace TestCode
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Timers;
    
    class Program
    {
        private enum StdHandle { Stdin = -10, Stdout = -11, Stderr = -12 };
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(StdHandle std);
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hdl);

        private static int count = 0;
        private static System.Timers.Timer aTimer = new System.Timers.Timer();
        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        const int VK_RETURN = 0x0D;
        const int WM_KEYDOWN = 0x100;

        static void Main(string[] args)
        {
            Console.Write("Switch focus to another window now.\n");

            ThreadPool.QueueUserWorkItem((o) =>
            {
                Thread.Sleep(4000);

                var hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                PostMessage(hWnd, WM_KEYDOWN, VK_RETURN, 0);
            });

            Console.ReadLine();

            Console.Write("ReadLine() successfully aborted by background thread.\n");
            Console.Write("[any key to exit]");
            Console.ReadKey();
        


        // P/Invoke:


        //string str = "11112222333344445";
        //int chunkSize = 4;
        //string[] s = Split(str, chunkSize);          
        //Array.ForEach(s, Console.WriteLine);
        //Console.WriteLine();

        /*
        aTimer.Interval = 1000;

        // Hook up the Elapsed event for the timer. 
        aTimer.Elapsed += OnTimedEvent;

        // Have the timer fire repeated events (true is the default)
        aTimer.AutoReset = true;

        // Start the timer
        aTimer.Start();

        Console.WriteLine("Press the Enter key to exit the program at any time... ");
        Console.ReadLine();
        */
    }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
            count++;
            if (count == 10)
            {
                aTimer.Stop();
            }
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
    }
}
