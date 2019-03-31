using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.IO;

namespace BittrexCore.Models
{
    public static class Logger
    {
        private static List<string> Logs = new List<string>();
        private static Timer Timer = new Timer(1000);
        

        public static void Log(string log, int logLvl = 1)
        {
            Logs.Add(DateTime.Now.ToShortDateString() + " : " + log);
            if (!Timer.Enabled)
            {
                Timer.Start();
                Timer.Elapsed += Timer_Elapsed;
            }
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Logs.Count > 0)
            {
                var logCount = Math.Min(Logs.Count - 1, 100);
                var writingLogs = Logs.GetRange(0, logCount);
                Logs.RemoveRange(0, logCount);

                File.AppendAllLines(Consts.LogFilePath, writingLogs);
            }
        }
    }


}
