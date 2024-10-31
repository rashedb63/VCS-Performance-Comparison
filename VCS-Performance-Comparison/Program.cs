using System;
using Git_Repositories_Performance_Comparison.Classes;

namespace VCS_Performance_Comparison
{
    internal class Program
    {
        // Specify the VCS Type
        const VCSPerformance.VCS versionControlType = VCSPerformance.VCS.mercurial;
        // Specify the Operation Type
        const VCSPerformance.Operation operationType = VCSPerformance.Operation.branch;
        // Specify the target folder for the experimentation
        const string TargetLocation = @"C:\Users\rashe\OneDrive\Desktop\Experiment";
        // Specify the report full path for the experimentation
        const string ReportPath = @"C:\Users\rashe\OneDrive\Desktop\Results";
        // Specify the file size in MB
        const double FileSize = 1;
        // Specify whether you want to split the file to multiple (Use 1 for a single file)
        static double NumberOfFiles = 1;
        // Specify the number of commits to be made for the experiment
        const int Commits = 10;
        // Specify the number of attempts
        const int OperationAttempts = 1;
        static void Main(string[] args)
        {
            // Initialize the Version Control System class
            VCSPerformance performance = new VCSPerformance(versionControlType, TargetLocation, Commits);
            VCSPerformanceReport report = new VCSPerformanceReport(ReportPath + @"\" + versionControlType + "_" + operationType + "_" + FileSize + "MB.csv");
            for (int i = 0; i < OperationAttempts; i++)
            {
                performance = new VCSPerformance(versionControlType, TargetLocation, Commits);
                performance.InitializeRepository();
                if (NumberOfFiles > 1) // File needs to be splitted
                {
                    for (int j = 0; j < NumberOfFiles; j++)
                    {
                        performance.CreateDummyFileInDirectory(FileSize / NumberOfFiles, j + 1);
                    }
                }
                else
                {
                    performance.CreateDummyFileInDirectory(FileSize, 1);
                }
                report.ExportReportRecord(performance.PerformOperation(operationType), i + 1);
            }
            Console.Read();
        }
    }
}