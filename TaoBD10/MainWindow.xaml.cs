using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Diagnostics;
using System.Windows;
using TaoBD10.Model;
using static System.Net.Mime.MediaTypeNames;

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
            //bool a = ValidHD();
            //if (!a)
            //    Close();
        }

        //private string key = "ledixonbs";

        //public string Decrypt(string toDecrypt)
        //{
        //    bool useHashing = true;
        //    byte[] keyArray;
        //    byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

        //    if (useHashing)
        //    {
        //        MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
        //        keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
        //    }
        //    else
        //        keyArray = Encoding.UTF8.GetBytes(key);

        //    TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
        //    tdes.Key = keyArray;
        //    tdes.Mode = CipherMode.ECB;
        //    tdes.Padding = PaddingMode.PKCS7;

        //    ICryptoTransform cTransform = tdes.CreateDecryptor();
        //    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        //    return Encoding.UTF8.GetString(resultArray);
        //}

        //private bool ValidHD()
        //{
        //    string hdSN = string.Empty;
        //    ManagementObjectSearcher moSearcher = new ManagementObjectSearcher("select * from Win32_DiskDrive");
        //    foreach (ManagementObject wmi_HDD in moSearcher.Get())
        //    {
        //        hdSN = wmi_HDD["SerialNumber"].ToString();
        //        break;
        //    }
        //    string text = File.ReadAllText("key.txt");
        //    string decryText = Decrypt(text);
        //    return decryText == hdSN;
        //}

        private void MainView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void TabTui_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        private void UpdateApp(string pathAppCurrent, string tempFilePath, string pathNewLocation, string pathOpenApp)
        {
            ProcessStartInfo app = new ProcessStartInfo();
            app.WindowStyle = ProcessWindowStyle.Hidden;
            string argument = "/C Choice /C Y /N /D Y /T 4 & Del /F /Q \"{0}\" & Choice /C Y /N /D Y /T 2 & Move /Y \"{1}\" \"{2}\" & Start \"\" /D \"{3}\"  \"{4}\"";
            app.Arguments = string.Format(argument, @"J:\text2.txt", @"J:\Nhac\filemoi.txt", @"J:\text2.txt", @"J:\", @"text2.txt");

            app.CreateNoWindow = true;
            app.FileName = "cmd.exe";
            Process.Start(app);
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            App.Current.Shutdown();
        }
    }
}