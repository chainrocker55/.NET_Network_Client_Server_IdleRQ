using System.Collections.Generic;

namespace TestCode
{
    using System;
    using System.Timers;
    
    class Program
    {
        private static int count = 0;
        private static Timer aTimer = new Timer();
        static void Main(string[] args)
        {
            //string str = "11112222333344445";
            //int chunkSize = 4;
            //string[] s = Split(str, chunkSize);          
            //Array.ForEach(s, Console.WriteLine);
            //Console.WriteLine();
            aTimer.Interval = 1000;

            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            aTimer.AutoReset = true;

            // Start the timer
            aTimer.Start();
           
            Console.WriteLine("Press the Enter key to exit the program at any time... ");
            Console.ReadLine();
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
