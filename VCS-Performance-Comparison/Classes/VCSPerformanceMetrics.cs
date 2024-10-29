using System;
using System.Diagnostics;

namespace Git_Repositories_Performance_Comparison.Classes
{
    internal class VCSPerformanceMetrics
    {
        public VCSPerformanceMetrics() {}
        public float MemoryUsage {set; get;}
        public float FinalMemoryUsage { set; get; }
        public float CpuUsage { set; get; }
        public TimeSpan CpuTime { set; get; }
        public double CpuTimeInMilliseconds { set; get; }
        public TimeSpan FinalCpuTime { set; get; }
        public double FinalCpuTimeInMilliseconds { set; get; }
        public double RepositorySize { set; get; }
        // Internal Use only
        public PerformanceCounter CpuCounter { set; get; }
    }
}
