using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;
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
            TestCommand = new RelayCommand(Test);
            ListControlCommand = new RelayCommand(ListControl);
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                _Printers.Add(printer);
            }
            ReadPrinter();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool InvalidateRect(IntPtr hWnd, IntPtr rect, bool bErase);

        public ICommand TestCommand { get; }

        private void Test()
        {
            List<string> list = FileManager.LoadErrorOnFirebase();
            String text = "";
            foreach (var item in list)
            {
                text += item + "\n";
            }
            APIManager.OpenNotePad(text, "Error Text");
            //var window = APIManager.WaitingFindedWindow("xem chuyen thu");
            //System.Collections.Generic.List<TestAPIModel> controls = APIManager.GetListControlText(window.hwnd);
            ////var control = controls.FirstOrDefault(m => m.Text.ToLower().IndexOf("danh sách bưu gửi") != -1);
            ////APIManager.setTextControl(control.Handle, "ddddddddddddddddddddddddd");
            ////InvalidateRect(control.Handle, IntPtr.Zero, true);

            //var control = controls[12];
            //AutomationElement element = AutomationElement.FromHandle(control.Handle);
            //AutomationElementCollection headers = element.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Custom));

            ////var headerCol1 = headers[1].Current.Name; ;
            ////var headerCol2 = headers[2].Current.Name;
            //for (var i = 0; i < headers.Count; i++)
            //{
            //    var cacheRequest = new CacheRequest
            //    {
            //        AutomationElementMode = AutomationElementMode.None,
            //        TreeFilter = Automation.RawViewCondition
            //    };

            //    cacheRequest.Add(AutomationElement.NameProperty);
            //    cacheRequest.Add(AutomationElement.AutomationIdProperty);

            //    cacheRequest.Push();

            //    var targetText = headers[i].FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "TextBlock"));

            //    cacheRequest.Pop();

            //    var myString = targetText.Cached.Name;
            //}
        }

        //AutomationElement datagrid = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "dataGridView1"));

        private void ReadPrinter()
        {
            PrintBD8 = APIManager.namePrinterBD8;
            PrintBD10 = APIManager.namePrinterBD10;
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
                File.WriteAllLines("Data\\printerSave.txt", lines);
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