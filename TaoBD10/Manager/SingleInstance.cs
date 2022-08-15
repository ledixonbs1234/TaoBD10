using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TaoBD10.Manager
{
    public sealed class SingleInstance
    {
        public static bool AlreadyRunning()
        {
            bool running = false;
            try
            {
                // Getting collection of process  
                Process currentProcess = Process.GetCurrentProcess();

                // Check with other process already running   
                foreach (var p in Process.GetProcesses())
                {
                    if (p.Id != currentProcess.Id) // Check running process   
                    {
                        if (p.ProcessName.Equals(currentProcess.ProcessName) == true)
                        {
                            running = true;
                            IntPtr hFound = p.MainWindowHandle;
                            if (APIManager.IsIconic(hFound)) // If application is in ICONIC mode then  
                                APIManager.ShowWindow(hFound, APIManager.SW_RESTORE);
                            APIManager.SetForegroundWindow(hFound); // Activate the window, if process is already running  
                            MessageBox.Show("Có chương trình đang chạy");
                            break;
                        }
                    }
                }
            }
            catch { }
            return running;
        }
    }
}
