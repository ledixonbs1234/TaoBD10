using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.ViewModels
{
    public class TuyChonViewModel:ObservableObject
    {
        public TuyChonViewModel()
        {
            _Printers = new ObservableCollection<string>();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                _Printers.Add(printer);
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
