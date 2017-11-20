using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagnosticInformation
{
    class DiagnosticJson
    {
        public string CpuUsage { get; set; }
        public string RamUsage { get; set; }
        public string ProcessesCount { get; set; }
        public string Time { get; set; }
        public List<String> UsagePerCore { get; set; }
    }
}
