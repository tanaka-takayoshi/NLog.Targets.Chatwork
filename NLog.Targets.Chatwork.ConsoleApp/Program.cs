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
            LogManager.GetCurrentClassLogger().Info(new string('1', 1000000));
        }
    }
}
