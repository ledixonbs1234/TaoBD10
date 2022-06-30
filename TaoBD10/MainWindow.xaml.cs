using Microsoft.Toolkit.Mvvm.Messaging;
using System;
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