using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

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
            diff,
            branch,
            merge
        }
        private VCS VersionControlType;
        private string TargetFolder;
        private VCSPerformanceShared shared;

        public VCSPerformance() { }
        public VCSPerformance(VCS versionControlType, string targetFolder)
        {
            try
            {
                shared = new VCSPerformanceShared();
                TargetFolder = targetFolder;
                if (!Directory.Exists(TargetFolder))
                {
                    shared.DisplayMessage("Target Directory for the experiment was not found. Creating it...", ConsoleColor.Green);
                    System.IO.Directory.CreateDirectory(TargetFolder);
                }
                else
                {
                    shared.DisplayMessage("Target Directory already exists. Clearing the directory...", ConsoleColor.Yellow);
                    ClearExperimentDirectory();
                }
                shared.DisplayMessage("Setting the version control to \"" + versionControlType + "\"...", ConsoleColor.Green);
                VersionControlType = versionControlType;
            }
            catch(Exception ex)
            {
                shared.DisplayMessage("Error creating the experimentation directory and/or setting the version control type!", ConsoleColor.Red);
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
            return VersionControlType == VCS.git ? GetDirectorySize(new DirectoryInfo(TargetFolder + @"\.git")) * 0.0009765625 : GetDirectorySize(new DirectoryInfo(TargetFolder + @"\.hg")) * 0.0009765625;
        }
        public void InitializeRepository()
        {
            try
            {
                if (VersionControlType == VCS.git)
                {
                    shared.DisplayMessage("Verifying if \"" + VersionControlType + "\" repository exists...", ConsoleColor.White);
                    if (System.IO.File.Exists(TargetFolder + @"\.git"))
                    {
                        shared.DisplayMessage("An existing \"" + VersionControlType + "\" repository was detected. Deleting it to initialize a new one...", ConsoleColor.Yellow);
                        System.IO.File.Delete(TargetFolder + @"\.git");
                    }
                    else
                    {
                        shared.DisplayMessage("An existing \"" + VersionControlType + "\" repository was detected. Deleting it to initialize a new one...", ConsoleColor.Yellow);
                        System.IO.File.Delete(TargetFolder + @"\.hg");
                    }
                }
                shared.DisplayMessage("Initializing a new \"" + VersionControlType + "\" repository in the target directory...", ConsoleColor.White);
                Process process = new Process();
                process.StartInfo.FileName = @"cmd.exe";
                process.StartInfo.Arguments = VersionControlType == VCS.git ? "/c cd \"" + TargetFolder + "\" && git init" : "/c cd \"" + TargetFolder + "\" && hg init";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
                process.WaitForExit();
                shared.DisplayMessage("\"" + VersionControlType + "\" repository has been successfully created in the target directory...", ConsoleColor.Green);
            }
            catch(Exception ex)
            {
                shared.DisplayMessage("An error has occured while initializing the repository!", ConsoleColor.Red);
                shared.DisplayMessage(ex.Message.ToString(), ConsoleColor.Red);
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
        public void CreateDummyFileInDirectory(double sizeInMegaBytes, int fileNumber)
        {
            shared.DisplayMessage("Creating dummy JSON file of size " + sizeInMegaBytes + " Megabyte...", ConsoleColor.White);
            CreateDummyJsonFile(TargetFolder + @"\dummyfile_" + fileNumber + ".json", sizeInMegaBytes * 1024 * 1024);
            shared.DisplayMessage("Dummy JSON file of size " + sizeInMegaBytes + " Megabyte has been successfully created...", ConsoleColor.Green);
        }
        public VCSPerformanceMetrics PerformOperation(Operation operation)
        {
            switch (operation)
            {
                case Operation.pre_stage_status:
                    return PerformPreStageStatusOperation();
                case Operation.stage:
                    return PerformStageOperation();
                case Operation.commit:
                    return PerformCommitOperation();
                case Operation.post_change_status:
                    return PerformPostChangeStatusOperation();
                case Operation.diff:
                    return PerformDiffOperation();
                case Operation.branch:
                    return PerformBranchOperation();
                case Operation.merge:
                    return PerformMergeOperation();
                default:
                    return null;
            }
        }
        private void CreateDummyJsonFile(string filePath, double size)
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
        private VCSPerformanceMetrics PerformPreStageStatusOperation()
        {
            shared.DisplayMessage("Performing status opertion on the \""+VersionControlType+"\" repository...", ConsoleColor.White);
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
                metrics.MemoryUsage = process.WorkingSet64 / 1024; // Memory in kb
                metrics.CpuTime = process.TotalProcessorTime;
            }
            // Final resource usage after the process exits
            metrics.FinalMemoryUsage = process.WorkingSet64 / 1024; // Memory in kb
            metrics.FinalCpuTime = process.TotalProcessorTime;
            metrics.FinalCpuTimeInMilliseconds = process.TotalProcessorTime.TotalMilliseconds;
            process.WaitForExit();
            metrics.RepositorySize = GetRepositorySize();
            shared.DisplayMessage("The status opertion on the \"" + VersionControlType + "\" repository has bee successfully executed, repoting it now...", ConsoleColor.Green);
            return metrics;
        }
        private VCSPerformanceMetrics PerformStageOperation()
        {
            shared.DisplayMessage("Performing staging opertion on the \"" + VersionControlType + "\" repository...", ConsoleColor.White);
            VCSPerformanceMetrics metrics = new VCSPerformanceMetrics();

            Process process = new Process();
            process.StartInfo.FileName = @"cmd.exe";
            process.StartInfo.Arguments = VersionControlType == VCS.git ? "/c cd \"" + TargetFolder + "\" && git add ." : "/c cd \"" + TargetFolder + "\" && hg add .";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();
            while (!process.HasExited)
            {
                // Get memory and CPU usage
                metrics.MemoryUsage = process.WorkingSet64 / 1024; // Memory in kb
                metrics.CpuTime = process.TotalProcessorTime;
            }
            // Final resource usage after the process exits
            metrics.FinalMemoryUsage = process.WorkingSet64 / 1024; // Memory in kb
            metrics.FinalCpuTime = process.TotalProcessorTime;
            metrics.FinalCpuTimeInMilliseconds = process.TotalProcessorTime.TotalMilliseconds;
            process.WaitForExit();
            metrics.RepositorySize = GetRepositorySize();
            shared.DisplayMessage("The staging opertion on the \"" + VersionControlType + "\" repository has bee successfully executed, repoting it now...", ConsoleColor.Green);
            return metrics;
        }
        private VCSPerformanceMetrics PerformCommitOperation()
        {
            shared.DisplayMessage("Performing commit opertion on the \"" + VersionControlType + "\" repository...", ConsoleColor.White);
            
            // Stage the all files first
            Process process = new Process();
            process.StartInfo.FileName = @"cmd.exe";
            process.StartInfo.Arguments = VersionControlType == VCS.git ? "/c cd \"" + TargetFolder + "\" && git add ." : "/c cd \"" + TargetFolder + "\" && hg add .";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            while (!process.HasExited){}
            process.WaitForExit();

            // Perform the commit now once staged
            VCSPerformanceMetrics metrics = new VCSPerformanceMetrics();
            process = new Process();
            process.StartInfo.FileName = @"cmd.exe";
            process.StartInfo.Arguments = VersionControlType == VCS.git ? "/c cd \"" + TargetFolder + "\" && git commit -m \"Experimental Commit\"" : "/c cd \"" + TargetFolder + "\" && hg commit -m \"Experimental Commit\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            while (!process.HasExited) 
            {
                // Get memory and CPU usage
                metrics.MemoryUsage = process.WorkingSet64 / 1024; // Memory in kb
                metrics.CpuTime = process.TotalProcessorTime;
            }
            // Final resource usage after the process exits
            metrics.FinalMemoryUsage = process.WorkingSet64 / 1024; // Memory in kb
            metrics.FinalCpuTime = process.TotalProcessorTime;
            metrics.FinalCpuTimeInMilliseconds = process.TotalProcessorTime.TotalMilliseconds;
            process.WaitForExit();
            metrics.RepositorySize = GetRepositorySize();
            shared.DisplayMessage("The commit opertion on the \"" + VersionControlType + "\" repository has bee successfully executed, reporting it now...", ConsoleColor.Green);
            return metrics;
        }
        private VCSPerformanceMetrics PerformPostChangeStatusOperation()
        {
            ChangeTheContentOfTheFiles();
            shared.DisplayMessage("Performing post change status opertion on the \"" + VersionControlType + "\" repository...", ConsoleColor.White);
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
                metrics.MemoryUsage = process.WorkingSet64 / 1024; // Memory in kb
                metrics.CpuTime = process.TotalProcessorTime;
            }
            // Final resource usage after the process exits
            metrics.FinalMemoryUsage = process.WorkingSet64 / 1024; // Memory in kb
            metrics.FinalCpuTime = process.TotalProcessorTime;
            metrics.FinalCpuTimeInMilliseconds = process.TotalProcessorTime.TotalMilliseconds;
            process.WaitForExit();
            metrics.RepositorySize = GetRepositorySize();
            shared.DisplayMessage("The post change status opertion on the \"" + VersionControlType + "\" repository has bee successfully executed, repoting it now...", ConsoleColor.Green);
            return metrics;
        }
        private VCSPerformanceMetrics PerformDiffOperation()
        {
            ChangeTheContentOfTheFiles();
            shared.DisplayMessage("Performing diff opertion on the \"" + VersionControlType + "\" repository...", ConsoleColor.White);
            VCSPerformanceMetrics metrics = new VCSPerformanceMetrics();

            Process process = new Process();
            process.StartInfo.FileName = @"cmd.exe";
            process.StartInfo.Arguments = VersionControlType == VCS.git ? "/c cd \"" + TargetFolder + "\" && git diff" : "/c cd \"" + TargetFolder + "\" && hg diff";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();
            while (!process.HasExited)
            {
                // Get memory and CPU usage
                metrics.MemoryUsage = process.WorkingSet64 / 1024; // Memory in kb
                metrics.CpuTime = process.TotalProcessorTime;
            }
            // Final resource usage after the process exits
            metrics.FinalMemoryUsage = process.WorkingSet64 / 1024; // Memory in kb
            metrics.FinalCpuTime = process.TotalProcessorTime;
            metrics.FinalCpuTimeInMilliseconds = process.TotalProcessorTime.TotalMilliseconds;
            process.WaitForExit();
            metrics.RepositorySize = GetRepositorySize();
            shared.DisplayMessage("The diff opertion on the \"" + VersionControlType + "\" repository has bee successfully executed, repoting it now...", ConsoleColor.Green);
            return metrics;
        }
        private VCSPerformanceMetrics PerformBranchOperation()
        {
            shared.DisplayMessage("Performing branch opertion on the \"" + VersionControlType + "\" repository...", ConsoleColor.White);

            // Stage & Commit the all files first
            Process process = new Process();
            process.StartInfo.FileName = @"cmd.exe";
            process.StartInfo.Arguments = VersionControlType == VCS.git ? "/c cd \"" + TargetFolder + "\" && git add . && git commit -m \"Experimental Commit\"" : "/c cd \"" + TargetFolder + "\" && hg add . && hg commit -m \"Experimental Commit\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            while (!process.HasExited) { }
            process.WaitForExit();

            // Perform the branch now once committed
            VCSPerformanceMetrics metrics = new VCSPerformanceMetrics();
            process = new Process();
            process.StartInfo.FileName = @"cmd.exe";
            process.StartInfo.Arguments = VersionControlType == VCS.git ? "/c cd \"" + TargetFolder + "\" && git branch experimental_branch && git switch experimental_branch" : "/c cd \"" + TargetFolder + "\" && hg branch experimental_branch";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            while (!process.HasExited)
            {
                // Get memory and CPU usage
                metrics.MemoryUsage = process.WorkingSet64 / 1024; // Memory in kb
                metrics.CpuTime = process.TotalProcessorTime;
            }
            // Final resource usage after the process exits
            metrics.FinalMemoryUsage = process.WorkingSet64 / 1024; // Memory in kb
            metrics.FinalCpuTime = process.TotalProcessorTime;
            metrics.FinalCpuTimeInMilliseconds = process.TotalProcessorTime.TotalMilliseconds;
            process.WaitForExit();
            metrics.RepositorySize = GetRepositorySize();
            // For mercurial only, you have to commit to the new branch in order to see it
            if (VersionControlType == VCS.mercurial)
            {
                // Apply a slight change in order to stage & commit
                ChangeTheContentOfTheFiles();
                VCSPerformanceMetrics mercurialMetrics = new VCSPerformanceMetrics();
                process = new Process();
                process.StartInfo.FileName = @"cmd.exe";
                process.StartInfo.Arguments = "/c cd \"" + TargetFolder + "\" && hg add . && hg commit -m \"Experimental Commit to Branch\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
                while (!process.HasExited)
                {
                    // Get memory and CPU usage
                    mercurialMetrics.MemoryUsage = process.WorkingSet64 / 1024; // Memory in kb
                    mercurialMetrics.CpuTime = process.TotalProcessorTime;
                }
                // Final resource usage after the process exits
                mercurialMetrics.FinalMemoryUsage = process.WorkingSet64 / 1024; // Memory in kb
                mercurialMetrics.FinalCpuTime = process.TotalProcessorTime;
                mercurialMetrics.FinalCpuTimeInMilliseconds = process.TotalProcessorTime.TotalMilliseconds;
                process.WaitForExit();
                metrics.MemoryUsage += mercurialMetrics.MemoryUsage;
                metrics.CpuTime += mercurialMetrics.CpuTime;
                metrics.FinalMemoryUsage += mercurialMetrics.FinalMemoryUsage;
                metrics.FinalCpuTime += mercurialMetrics.FinalCpuTime;
                metrics.FinalCpuTimeInMilliseconds += mercurialMetrics.FinalCpuTimeInMilliseconds;
                metrics.RepositorySize = GetRepositorySize();
            }
            shared.DisplayMessage("The branch opertion on the \"" + VersionControlType + "\" repository has bee successfully executed, reporting it now...", ConsoleColor.Green);
            return metrics;
        }
        private VCSPerformanceMetrics PerformMergeOperation()
        {
            shared.DisplayMessage("Performing merge opertion on the \"" + VersionControlType + "\" repository...", ConsoleColor.White);

            // Stage & Commit all files first
            Process process = new Process();
            process.StartInfo.FileName = @"cmd.exe";
            process.StartInfo.Arguments = VersionControlType == VCS.git ? "/c cd \"" + TargetFolder + "\" && git add . && git commit -m \"Experimental Commit\"" : "/c cd \"" + TargetFolder + "\" && hg add . && hg commit -m \"Experimental Commit\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            while (!process.HasExited) { }
            process.WaitForExit();

            // Perform the branch now once committed
            process = new Process();
            process.StartInfo.FileName = @"cmd.exe";
            process.StartInfo.Arguments = VersionControlType == VCS.git ? "/c cd \"" + TargetFolder + "\" && git branch experimental_branch && git switch experimental_branch" : "/c cd \"" + TargetFolder + "\" && hg branch experimental_branch";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            while (!process.HasExited){ }
            process.WaitForExit();

            // Apply a slight change to the file(s)
            ChangeTheContentOfTheFiles();
            
            // Commit the new change to the new branch
            process = new Process();
            process.StartInfo.FileName = @"cmd.exe";
            process.StartInfo.Arguments = VersionControlType == VCS.git ? "/c cd \"" + TargetFolder + "\" && git add . && git commit -m \"Experimental Commit to Branch\" && git switch main" : " /c cd \"" + TargetFolder + "\" && hg add . && hg commit -m \"Experimental Commit to Branch\" && hg update default";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            while (!process.HasExited){ }
            process.WaitForExit();

            shared.DisplayMessage("The merge opertion on the \"" + VersionControlType + "\" repository has bee successfully executed, reporting it now...", ConsoleColor.Green);
            return null;
        }
        private void ChangeTheContentOfTheFiles()
        {
            DirectoryInfo d = new DirectoryInfo(TargetFolder);
            FileInfo[] Files = d.GetFiles("*.json");
            List<VCSPerformanceJSONItem> items = null;
            int fileNumber = 1;
            string fileContent = File.ReadAllText(TargetFolder + @"\dummyfile_" + fileNumber + ".json");
            var list = JsonConvert.DeserializeObject<List<VCSPerformanceJSONItem>>(fileContent);
            var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
            foreach (FileInfo file in Files)
            {
                shared.DisplayMessage("Changing the content of file #" + fileNumber, ConsoleColor.White);
                // Initialize the file contnet list
                items = new List<VCSPerformanceJSONItem>();
                // Read the file content
                fileContent = File.ReadAllText(TargetFolder + @"\dummyfile_" + fileNumber + ".json");
                list = JsonConvert.DeserializeObject<List<VCSPerformanceJSONItem>>(fileContent);
                // Replace the last item in the JSON file
                list.RemoveAt(list.Count - 1);
                list.Add(new VCSPerformanceJSONItem(999999, "Experimental Insertion", 999999));
                convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
                // Write the JSON content back to the file
                File.WriteAllText(TargetFolder + @"\dummyfile_" + fileNumber + ".json", Newtonsoft.Json.JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));
                fileNumber++;
            }
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
    }
}