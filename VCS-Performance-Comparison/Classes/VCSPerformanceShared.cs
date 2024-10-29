using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Git_Repositories_Performance_Comparison.Classes
{
    internal class VCSPerformanceShared
    {
        public void DisplayMessage(string Message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(Message);
        }
    }
}
