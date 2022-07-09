using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Management;
using System.Windows;
using TaoBD10.Model;

namespace TaoBD10
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "Active")
                {
                    if (m.Content == "MainView")
                    {
                        MainView.Activate();
                    }
                }
            });
            bool a =ValidHD();
            if (!a)
                this.Close();

        }
        private static bool ValidHD()
        {
            string hdSN = String.Empty;
            ManagementObjectSearcher moSearcher = new ManagementObjectSearcher("select * from Win32_DiskDrive");
            foreach (ManagementObject wmi_HDD in moSearcher.Get())
            {
                hdSN = wmi_HDD["SerialNumber"].ToString();
            }


            if (hdSN == "Your_SN_Here")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void MainView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}