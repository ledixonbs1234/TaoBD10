using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.IO;
using System.Threading;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class TuyChonViewModel : ObservableObject
    {
        private ObservableCollection<TestAPIModel> _Controls;

        public ObservableCollection<TestAPIModel> Controls
        {
            get { return _Controls; }
            set { SetProperty(ref _Controls, value); }
        }

        public TuyChonViewModel()
        {
            _Printers = new ObservableCollection<string>();
            _Controls = new ObservableCollection<TestAPIModel>();
            ApplyCommand = new RelayCommand(Apply);
            ListControlCommand = new RelayCommand(ListControl);
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                _Printers.Add(printer);
            }
            ReadPrinter();
        }

        private void ReadPrinter()
        {
            if (File.Exists("printerSave.txt"))
            {
                string[] lines = File.ReadAllLines("printerSave.txt");
                PrintBD8 = lines[0];
                APIManager.namePrinterBD8 = PrintBD8;
                PrintBD10 = lines[1];
                APIManager.namePrinterBD10 = PrintBD10;
            }
        }

        public ICommand ApplyCommand { get; }
        public ICommand ListControlCommand { get; }

        private void ListControl()
        {
            Thread.Sleep(3000);
            var currenWindow = APIManager.GetActiveWindowTitle();
            if (currenWindow == null)
                return;
            var controls = APIManager.GetListControlText(currenWindow.hwnd);
            Controls.Clear();
            foreach (TestAPIModel control in controls)
            {
                Controls.Add(control);
            }
        }

        private void Apply()
        {
            if (!string.IsNullOrEmpty(PrintBD8) && !string.IsNullOrEmpty(PrintBD10))
            {
                string[] lines = { PrintBD8, PrintBD10 };
                APIManager.namePrinterBD8 = PrintBD8;
                APIManager.namePrinterBD10 = PrintBD10;
                File.WriteAllLines("printerSave.txt", lines);
            }
        }

        private ObservableCollection<string> _Printers;

        public ObservableCollection<string> Printers
        {
            get { return _Printers; }
            set { SetProperty(ref _Printers, value); }
        }

        private string _PrintBD8;

        public string PrintBD8
        {
            get { return _PrintBD8; }
            set { SetProperty(ref _PrintBD8, value); }
        }

        private string _PrintBD10;

        public string PrintBD10
        {
            get { return _PrintBD10; }
            set { SetProperty(ref _PrintBD10, value); }
        }
    }
}