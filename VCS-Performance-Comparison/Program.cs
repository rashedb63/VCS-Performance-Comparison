using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Git_Repositories_Performance_Comparison.Classes;

namespace VCS_Performance_Comparison
{
    internal class Program
    {
        // Specify the target folder for the experimentation
        const string TargetLocation = @"C:\Users\rashe\OneDrive\Desktop\Experiment";
        static void Main(string[] args)
        {
            // Initialize the class to use or create the folder if doesn't exist
            VCSPerformance performance = new VCSPerformance(VCSPerformance.VCS.mercurial, TargetLocation);
            performance.InitializeRepository();
            //long memoryUsage;
            //float cpuUsage;
            //TimeSpan cpuTime;
            //PerformanceCounter cpuCounter;

            //Process process = new Process();
            //process.StartInfo.FileName = @"cmd.exe";
            //process.StartInfo.Arguments = "/c cd Desktop"; // Note the /c command (*)
            //process.StartInfo.UseShellExecute = false;
            //process.StartInfo.RedirectStandardOutput = true;
            //process.StartInfo.RedirectStandardError = true;
            //process.Start();
            //////* Read the output (or the error)
            ////string output = process.StandardOutput.ReadToEnd();
            ////Console.WriteLine(output);
            ////string err = process.StandardError.ReadToEnd();
            ////Console.WriteLine(err);
            ////// Monitor resource usage
            //while (!process.HasExited)
            //{
            //    // Get memory and CPU usage
            //    memoryUsage = process.WorkingSet64; // Memory in bytes
            //    cpuTime = process.TotalProcessorTime;
            //    cpuCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetProcessById(process.Id).ProcessName);

            //    // Get CPU usage after process execution
            //    cpuUsage = cpuCounter.NextValue();

            //    // Output the results
            //    Console.WriteLine($"CPU Usage: {cpuUsage}%");

            //    // Wait for a short interval before checking again
            //    //Thread.Sleep(500);
            //}

            //// Final resource usage after the process exits
            //long finalMemoryUsage = process.WorkingSet64;
            //TimeSpan finalCpuTime = process.TotalProcessorTime;

            //Console.WriteLine($"Final Memory Usage: {finalMemoryUsage / 1024} KB");
            //Console.WriteLine($"Final CPU Time: {finalCpuTime.TotalMilliseconds} ms");

            //process.WaitForExit(); // Optional: Wait for the process to exit completely

            Console.Read();
        }
    }
}
