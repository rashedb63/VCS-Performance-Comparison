using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Data;
using System.Text;

namespace Git_Repositories_Performance_Comparison.Classes
{
    internal class VCSPerformance
    {
        public enum VCS
        {
            git,
            mercurial
        }
        public enum Operation
        {
            pre_stage_status,
            stage,
            commit,
            post_change_status,
            log,
            branch,
            merge,
            diff
        }
        private VCS VersionControlType;
        private string TargetFolder;
        private string ReportPath;
        public VCSPerformance() { }
        public VCSPerformance(VCS versionControlType, string targetFolder, string reportPath)
        {
            try
            {
                TargetFolder = targetFolder;
                ReportPath = reportPath;
                if (!Directory.Exists(TargetFolder))
                {
                    DisplayMessage("Target Directory for the experiment was not found. Creating it...", ConsoleColor.Green);
                    System.IO.Directory.CreateDirectory(TargetFolder);
                }
                else
                {
                    DisplayMessage("Target Directory already exists. Clearing the directory...", ConsoleColor.Yellow);
                    ClearExperimentDirectory();
                }
                DisplayMessage("Setting the version control to \"" + versionControlType + "\"...", ConsoleColor.Green);
                VersionControlType = versionControlType;
                ExportReportHeaders(ReportPath);
            }
            catch(Exception ex)
            {
                DisplayMessage("Error creating the experimentation directory and/or setting the version control type!", ConsoleColor.Red);
                Console.WriteLine(ex.Message.ToString());
            }
        }
        public void SetExperimentDirectory(string targetFolder)
        {
            TargetFolder = targetFolder;
            if (!Directory.Exists(TargetFolder))
            {
                System.IO.Directory.CreateDirectory(TargetFolder);
            }
        }
        public string getExperimentDirectory()
        {
            return TargetFolder;
        }
        public void SetVersionControlType(VCS versionControlType)
        {
            VersionControlType = versionControlType;
        }
        public VCS GetVersionControlType()
        {
            return VersionControlType;
        }
        private double GetRepositorySize()
        {
            return VersionControlType == VCS.git ? GetDirectorySize(new DirectoryInfo(TargetFolder + @"\.git")) * 0.001 : GetDirectorySize(new DirectoryInfo(TargetFolder + @"\.hg")) * 0.001;
        }
        public void InitializeRepository()
        {
            try
            {
                if (VersionControlType == VCS.git)
                {
                    DisplayMessage("Verifying if \"" + VersionControlType + "\" repository exists...", ConsoleColor.White);
                    if (System.IO.File.Exists(TargetFolder + @"\.git"))
                    {
                        DisplayMessage("An existing \"" + VersionControlType + "\" repository was detected. Deleting it to initialize a new one...", ConsoleColor.Yellow);
                        System.IO.File.Delete(TargetFolder + @"\.git");
                    }
                    else
                    {
                        DisplayMessage("An existing \"" + VersionControlType + "\" repository was detected. Deleting it to initialize a new one...", ConsoleColor.Yellow);
                        System.IO.File.Delete(TargetFolder + @"\.hg");
                    }
                }
                DisplayMessage("Initializing a new \"" + VersionControlType + "\" repository in the target directory...", ConsoleColor.White);
                Process process = new Process();
                process.StartInfo.FileName = @"cmd.exe";
                process.StartInfo.Arguments = VersionControlType == VCS.git ? "/c cd \"" + TargetFolder + "\" && git init" : "/c cd \"" + TargetFolder + "\" && hg init";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
                process.WaitForExit();
                DisplayMessage("\"" + VersionControlType + "\" repository has been successfully created in the target directory...", ConsoleColor.Green);
            }
            catch(Exception ex)
            {
                DisplayMessage("An error has occured while initializing the repository!", ConsoleColor.Red);
                DisplayMessage(ex.Message.ToString(), ConsoleColor.Red);
            }
        }
        public void ClearExperimentDirectory()
        {
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(TargetFolder);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to clear the experimentation folder caused by: ");
                Console.WriteLine(ex.Message.ToString());
            }
        }
        public void CreateDummyFileInDirectory(int sizeInMegaBytes)
        {
            DisplayMessage("Creating dummy JSON file of size " + sizeInMegaBytes + " Megabyte...", ConsoleColor.White);
            CreateDummyJsonFile(TargetFolder + @"\dummyfile.json", sizeInMegaBytes * 1024 * 1024);
            DisplayMessage("Dummy JSON file of size " + sizeInMegaBytes + " Megabyte has been successfully created...", ConsoleColor.Green);
        }
        public void PerformOperation(Operation operation)
        {
            if(operation == Operation.pre_stage_status)
            {
                PerformPreStageStatusOperation();
            }
        }
        private void CreateDummyJsonFile(string filePath, long size)
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fs, Encoding.UTF8))
            {
                long writtenBytes = 0;
                const string jsonTemplate = "{{\"id\":{0},\"name\":\"Name{0}\",\"value\":{1}}},";
                int id = 0;
                // Write JSON array start
                writer.WriteLine("[");
                writtenBytes += 2; // for the opening brackets

                while (writtenBytes < size)
                {
                    // Create a JSON object
                    string jsonLine = string.Format(jsonTemplate, id, new Random().Next(1, 1000));
                    writer.Write(jsonLine);
                    writtenBytes += jsonLine.Length;
                    id++;
                    // Add a new line for readability, if you want
                    if (writtenBytes < size) // Avoid adding an extra comma at the end
                    {
                        writer.WriteLine();
                    }
                }
                // Write JSON array end
                writer.WriteLine("]");
                writtenBytes += 2; // for the closing brackets
            }
        }
        private void PerformPreStageStatusOperation()
        {
            DisplayMessage("Performing status opertion on the \""+VersionControlType+"\" repository...", ConsoleColor.White);
            VCSPerformanceMetrics metrics = new VCSPerformanceMetrics();

            Process process = new Process();
            process.StartInfo.FileName = @"cmd.exe";
            process.StartInfo.Arguments = VersionControlType == VCS.git ? "/c cd \"" + TargetFolder + "\" && git status" : "/c cd \"" + TargetFolder + "\" && hg status";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            
            process.Start();
            while (!process.HasExited)
            {
                // Get memory and CPU usage
                metrics.MemoryUsage = process.WorkingSet64; // Memory in bytes
                metrics.CpuTime = process.TotalProcessorTime;
                // Get CPU usage after process execution
                metrics.CpuUsage = new PerformanceCounter("Process", "% Processor Time", Process.GetProcessById(process.Id).ProcessName).NextValue();
            }
            // Final resource usage after the process exits
            metrics.FinalMemoryUsage = process.WorkingSet64 / 1024;
            metrics.FinalCpuTime = process.TotalProcessorTime;
            process.WaitForExit();
            metrics.RepositorySize = GetRepositorySize();
            ExportReportRecord(metrics);
            DisplayMessage("The status opertion on the \"" + VersionControlType + "\" repository has bee successfully executed and reported...", ConsoleColor.Green);
        }
        private double GetDirectorySize(DirectoryInfo d)
        {
            double size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += GetDirectorySize(di);
            }
            return size;
        }
        private void ExportReportRecord(VCSPerformanceMetrics metrics)
        {
            using (StreamWriter writer = new StreamWriter(ReportPath, true))
            {
                writer.WriteLine(metrics.MemoryUsage + "," + metrics.FinalMemoryUsage + "," + metrics.CpuUsage + "," + metrics.CpuTime + "," + metrics.FinalCpuTime + "," + metrics.RepositorySize);
            }
            DisplayMessage("The report file has been successfully created in the specified location...", ConsoleColor.Green);
        }
        private void ExportReportHeaders(string reportPath)
        {
            DisplayMessage("Preparing the report CSV file in the specified location...", ConsoleColor.White);
            if(File.Exists(ReportPath))
            {
                DisplayMessage("The report file already exists in the specified location. Deleting it to create a new one...", ConsoleColor.Yellow);
                File.Delete(ReportPath);
            }
            using (StreamWriter writer = new StreamWriter(reportPath, true))
            {
                writer.WriteLine("Memory Usage,Final Memory Usage,Cpu Usage,Cpu Time,Final Cpu Time,Repository Size");
            }
            DisplayMessage("The report file has been successfully created in the specified location...", ConsoleColor.Green);
        }
        private void DisplayMessage(string Message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(Message);
        }
    }
}