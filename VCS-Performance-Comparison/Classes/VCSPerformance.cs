using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Data;

namespace Git_Repositories_Performance_Comparison.Classes
{
    internal class VCSPerformance
    {
        public enum VCS
        {
            git,
            mercurial
        }
        private VCS VersionControlType;
        private string TargetFolder;
        public VCSPerformance() { }
        public VCSPerformance(VCS versionControlType, string targetFolder)
        {
            try
            {
                TargetFolder = targetFolder;
                if (!Directory.Exists(TargetFolder))
                {
                    DisplayMessage("Target Directory for the experiment was not found. Creating it...", ConsoleColor.Green);
                    System.IO.Directory.CreateDirectory(TargetFolder);
                }
                else
                {
                    DisplayMessage("Target Directory already exists. Clearing the directory...", ConsoleColor.Yellow);
                    ClearExperimentDirectory(targetFolder);
                }
                DisplayMessage("Setting the version control to \"" + versionControlType + "\"...", ConsoleColor.Green);
                VersionControlType = versionControlType;
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
        public void InitializeRepository()
        {
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
        private void ClearExperimentDirectory(string targetFolder)
        {
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(targetFolder);
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
        private void DisplayMessage(string Message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(Message);
        }
    }
}
