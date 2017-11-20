using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiagnosticInformation
{
    class Program
    {
        static void Main(string[] args)
        {
            Diagnostics a = new Diagnostics();
           
            while(true)
            {
                Thread.Sleep(3000);
                Console.WriteLine(a.String);
            }
        }
    }
}
