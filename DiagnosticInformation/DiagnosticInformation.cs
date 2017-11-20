using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiagnosticInformation
{
    class DiagnosticInformation
    {
        public string DisplayString { private set; get; }
        private Diagnostics d;
        private string cpuUsage;
        private string ramUsage;
        private string processesCount;
        PerformanceCounter memoryCounter;
        PerformanceCounter cpuCounter;
        PerformanceCounter pc;
        PerformanceCounterCategory cat;
        string[] instances;
        Dictionary<string, double> cs;
        private PerformanceCounter perfSystemCounter;
        private string time;

       private bool keepLoopAlive = true;
        public DiagnosticInformation(Diagnostics d)
        {
            perfSystemCounter = new PerformanceCounter("System", "System Up Time"); 
            this.d = d;
             pc = new PerformanceCounter("Processor Information", "% Processor Time");
             cat = new PerformanceCounterCategory("Processor Information");
             instances = cat.GetInstanceNames();
            cs = new Dictionary<string, double>();
            foreach (var s in instances)
            {
                string a = s;
                if(s!="_Total"&&s!="0,_Total")
                { 
                pc.InstanceName = s;
                pc.NextValue();
                cs.Add(s, pc.NextValue());
                }
            }
            memoryCounter = new PerformanceCounter("Memory", "Available MBytes");
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            while (keepLoopAlive)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(CpuUsage));
                ThreadPool.QueueUserWorkItem(new WaitCallback(MemoryUsage));
                ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessesCount));
                ThreadPool.QueueUserWorkItem(new WaitCallback(UsagePerCore));
                ThreadPool.QueueUserWorkItem(new WaitCallback(GetUpTime));
                Thread.Sleep(1000);
                SetString();
                Thread.Sleep(2000);
            }
        }

        public void CpuUsage(object state)
        {
            cpuUsage = "Total CPU usage: " +cpuCounter.NextValue()+"%";
        } 

        public void MemoryUsage(object state)
        {
            ramUsage = "Memory available: " + memoryCounter.NextValue() + "MB";
        }

        public void ProcessesCount(object state)
        {
            processesCount = "Processes running: " + Process.GetProcesses().Count();
        }
        public void UsagePerCore(object state)
        {
            List<string> temp = cs.Keys.ToList();
           foreach(var s in temp)
            {
                pc.InstanceName = s;
                pc.NextValue();
                Thread.Sleep(500);
                float usage = pc.NextValue();
                cs[s] = Math.Round(usage,2) ;
            }
        }

        public void GetUpTime(object state)
        {
            perfSystemCounter.NextValue();
            double uptime = (double)perfSystemCounter.NextValue();
            var hours = Math.Floor(uptime / 3600);
            var minutes = Math.Floor((uptime - (hours * 3600)) / 60);
            var seconds = uptime - (hours * 3600) - (minutes * 60);
            var secondsRounded = Math.Round(seconds, 0);
            time = hours + " uur, " + minutes + " minuten en " + secondsRounded + " seconden"; 
        }
        public void SetString()
        {
            DisplayString = cpuUsage + "\n" + ramUsage + "\n" + processesCount+"\n"+time;
            foreach(string logicCore in cs.Keys)
            {
                DisplayString += "\n Thread " + logicCore + " uses " + cs[logicCore];
            }
           DisplayString = DisplayString.Replace("\n", Environment.NewLine);
            d.String = DisplayString+"\n \n";
            SetJson();
        }
        public void Stop()
        {
            keepLoopAlive = false;
        }
        public void SetJson()
        {
            DiagnosticJson diagnosticJson = new DiagnosticJson();
            diagnosticJson.CpuUsage = cpuUsage;
            diagnosticJson.ProcessesCount = processesCount;
            diagnosticJson.Time = time;
            diagnosticJson.RamUsage = ramUsage;
            List<string> usagePerCore = new List<string>();
            foreach (string item in cs.Keys)
            {
                usagePerCore.Add("Thread " + item + " uses " + cs[item]);
            }
            diagnosticJson.UsagePerCore = usagePerCore;
          d.Json = JsonConvert.SerializeObject(diagnosticJson);
        }
    }
}
