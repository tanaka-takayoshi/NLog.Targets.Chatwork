using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLog.Targets.Chatwork.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = LogManager.GetCurrentClassLogger();
            log.Fatal("Fatal");
        }
    }
}
