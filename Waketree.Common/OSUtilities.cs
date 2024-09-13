using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;

namespace Waketree.Common
{
    public static class OSUtilities
    {
        public static int GetCPUCores()
        {
            return Environment.ProcessorCount;
        }

        public static float GetCPUUtilizationPercent()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using (var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
                {
                    cpuCounter.NextValue();
                    Thread.Sleep(1000);
                    return cpuCounter.NextValue();
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string result = OSUtilities.ExecuteUnixShellCommand("top -bn 1 | awk '/^%Cpu/ {print $2}'");
                if (float.TryParse(result, NumberStyles.Float, CultureInfo.InvariantCulture, out float cpuUsage))
                {
                    return cpuUsage;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                string result = OSUtilities.ExecuteUnixShellCommand("top -l 1 | awk '/^CPU usage:/ {print $3}'");
                result = result.Substring(0, result.Length - 1); // remove the trailing %
                if (float.TryParse(result, NumberStyles.Float, CultureInfo.InvariantCulture, out float cpuUsage))
                {
                    return cpuUsage;
                }
            }
            return -1;
        }

        public static string GetOSPlatform()
        {
            return (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) ? "Windows" :
                (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) ? "Linux" :
                (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) ? "macOS" : string.Empty;
        }

        public static string GetOSVersion()
        {
            return Environment.OSVersion.Version.ToString();
        }

        public static long GetTotalMemoryMB()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
                {
                    foreach (ManagementObject wmiObject in searcher.Get())
                    {
                        object value = wmiObject["TotalPhysicalMemory"];
                        if (value != null)
                        {
                            return Convert.ToInt64(value) / (1024 * 1024);
                        }
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var memInfoContent = File.ReadAllText("/proc/meminfo");
                var lines = memInfoContent.Split('\n');
                foreach (var line in lines)
                {
                    if (line.StartsWith("MemTotal:", StringComparison.OrdinalIgnoreCase))
                    {
                        // Extract the numeric part of the line
                        var numericPart = line.Split(':')[1];
                        var bytesString = numericPart.Substring(0, numericPart.Length - 2).Trim();

                        if (long.TryParse(bytesString, out long totalKilobytes))
                        {
                            return totalKilobytes / 1024; // Convert from KB to MB
                        }
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = "/usr/sbin/sysctl";
                    process.StartInfo.Arguments = "hw.memsize";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();

                    var result = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();

                    if (long.TryParse(result.Substring(result.IndexOf(':') + 1), out long totalBytes))
                    {
                        return totalBytes / (1024 * 1024);
                    }
                }
            }
            return -1;
        }

        public static long GetMemoryFreeMB()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using (PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available Bytes"))
                {
                    return Convert.ToInt64(ramCounter.NextValue()) / (1024 * 1024);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var memInfoContent = File.ReadAllText("/proc/meminfo");
                var lines = memInfoContent.Split('\n');
                foreach (var line in lines)
                {
                    if (line.StartsWith("MemFree:", StringComparison.OrdinalIgnoreCase))
                    {
                        // Extract the numeric part of the line
                        var numericPart = line.Split(':')[1];
                        var bytesString = numericPart.Substring(0, numericPart.Length - 2).Trim();

                        if (long.TryParse(bytesString, out long totalKilobytes))
                        {
                            return totalKilobytes / 1024; // Convert from KB to MB
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // get total MB used
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "/bin/ps";
                    process.StartInfo.Arguments = "-caxm -orss,comm";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();

                    string result = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();

                    var usedTotalKB = 0L;
                    var lines = result.Split("\n");
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var trimmedLine = lines[i].Trim();
                        var trimmedSplitLine = trimmedLine.Split(" ");
                        var amountKB = 0L;
                        if (long.TryParse(trimmedSplitLine[0].Trim(), out amountKB))
                        {
                            usedTotalKB += amountKB;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    var usedTotalMB = usedTotalKB / 1024;

                    // get total free memory MB
                    return OSUtilities.GetTotalMemoryMB() - usedTotalMB;
                }
            }
            return -1;
        }
        public static string GetComputerName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Environment.MachineName;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return File.ReadAllText("/etc/hostname").Trim();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "/bin/hostname";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();
                    string result = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();
                    return result;
                }
            }
            return string.Empty;
        }

        private static string ExecuteUnixShellCommand(string command)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "/bin/bash";
                process.StartInfo.Arguments = $"-c \"{command}\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                string result = process.StandardOutput.ReadToEnd().Trim();

                process.WaitForExit();

                return result;
            }
        }
    }
}
