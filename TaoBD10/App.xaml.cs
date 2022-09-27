using System;
using System.Windows;
using System.Windows.Threading;
using TaoBD10.Manager;

namespace TaoBD10
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            if (SingleInstance.AlreadyRunning())
                App.Current.Shutdown(); // Just shutdown the current application,if any instance found.

            base.OnStartup(e);
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //e.Handled = true;
            //var st = new StackTrace(e.Exception, true);
            //var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            //var line = frame.GetFileLineNumber();
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message + "Line: " + APIManager.GetLineNumber(e.Exception) + " Them:" + e.Exception.StackTrace, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //e.Handled = true;
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message + "Line: " + APIManager.GetLineNumber(e.Exception) + " Them:" + e.Exception.StackTrace, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // we cannot handle this, but not to worry, I have not encountered this exception yet.
            // However, you can show/log the exception message and show a message that if the application is terminating or not.
            var isTerminating = e.IsTerminating;
            //MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            //e.Handled = trkue;
        }
    }
}