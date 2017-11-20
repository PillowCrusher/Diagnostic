using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagnosticInformation
{
    class Diagnostics
    {
        Task taskDiagnostics;
       public string String { get; set; }
        public string Json { get; set; }
        public Diagnostics()
        {
            taskDiagnostics = new Task(() => new DiagnosticInformation(this));
            taskDiagnostics.Start();
        }
    }
}
