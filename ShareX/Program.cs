using BarRaider.SdTools;
using CommandLine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace ShareX
{

    internal static class Extensions
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In, Out] ref uint lpdwSize);

        public static string GetMainModuleFileName(this Process process, int buffer = 1024)
        {
            var fileNameBuilder = new StringBuilder(buffer);
            uint bufferLength = (uint)fileNameBuilder.Capacity + 1;
            return QueryFullProcessImageName(process.Handle, 0, fileNameBuilder, ref bufferLength) ?
                fileNameBuilder.ToString() :
                null;
        }
    }

    static class Globals
    {

        // global int
        public static string xpath;

        public static bool missingx { get; internal set; }

        public static string FindShareX()
        {
            foreach (var p in Process.GetProcesses())
            {
                if (p.ProcessName == "ShareX")
                {
                    return(p.GetMainModuleFileName());
                }
            }
            return "";
        }

    }

    class Program
    {
        static void Main(string[] args)
        {

            // Uncomment this line of code to allow for debugging
            //while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }

            // Searches common install locations for ShareX
            if (File.Exists("C:\\Program Files\\ShareX\\sharex.exe"))
            {
                Globals.xpath = "C:\\Program Files\\ShareX\\sharex.exe";
                Globals.missingx = false;
            }
            else if (File.Exists("C:\\Program Files (x86)\\ShareX\\sharex.exe"))
            {
                Globals.xpath = "C:\\Program Files (x86)\\ShareX\\sharex.exe";
                Globals.missingx = false;
            }
            else if (File.Exists("C:\\Program Files (x86)\\Steam\\steamapps\\common\\ShareX\\sharex.exe"))
            {
                Globals.xpath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\ShareX\\sharex.exe";
                Globals.missingx = false;
            }
            // Checks PATH to see if ShareX exists
            else if (File.Exists("sharex"))
            {
                Globals.xpath = "sharex";
                Globals.missingx = false;
            }
            // Checks currently running process for ShareX
            else if (Globals.FindShareX() != "")
            {
                Globals.xpath = Globals.FindShareX();
                Globals.missingx = false;
            }
            // Alert user that ShareX was unable to be found.
            else
            {
                Globals.missingx = true;
            }
            SDWrapper.Run(args);
        }
    }
}
