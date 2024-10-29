using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Git_Repositories_Performance_Comparison.Classes
{
    internal class VCSPerformanceReport
    {
        private VCSPerformanceShared shared;
        private string ReportPath;
        public VCSPerformanceReport(string reportPath)
        {
            ReportPath = reportPath;
            shared = new VCSPerformanceShared();
            ExportReportHeaders();
        }
        public void ExportReportHeaders()
        {
            shared.DisplayMessage("Preparing the report CSV file in the specified location...", ConsoleColor.White);
            if (File.Exists(ReportPath))
            {
                shared.DisplayMessage("The report file already exists in the specified location. Deleting it to create a new one...", ConsoleColor.Yellow);
                File.Delete(ReportPath);
            }
            using (StreamWriter writer = new StreamWriter(ReportPath, true))
            {
                writer.WriteLine("Attempt, Memory Usage (KB),Final Memory Usage (KB),Cpu Usage (%),Cpu Time (ms),Final Cpu Time (ms),Repository Size (KB)");
            }
            shared.DisplayMessage("The report file has been successfully created in the specified location...", ConsoleColor.Green);
        }
        public void ExportReportRecord(VCSPerformanceMetrics metrics, int attempt)
        {
            using (StreamWriter writer = new StreamWriter(ReportPath, true))
            {
                writer.WriteLine(attempt + "," + metrics.MemoryUsage + "," + metrics.FinalMemoryUsage + "," + metrics.CpuUsage + "," + metrics.CpuTimeInMilliseconds + "," + metrics.FinalCpuTimeInMilliseconds + "," + metrics.RepositorySize);
            }
            shared.DisplayMessage("The report file has been successfully created in the specified location...", ConsoleColor.Green);
        }
    }
}
