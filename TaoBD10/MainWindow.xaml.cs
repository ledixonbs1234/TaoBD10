﻿using CommunityToolkit.Mvvm.Messaging;
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

       
          }
}