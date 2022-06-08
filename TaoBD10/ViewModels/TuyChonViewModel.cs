﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TaoBD10.Manager;

namespace TaoBD10.ViewModels
{
    public class TuyChonViewModel : ObservableObject
    {
        public TuyChonViewModel()
        {
            _Printers = new ObservableCollection<string>();
            ApplyCommand = new RelayCommand(Apply);
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                _Printers.Add(printer);
            }
            ReadPrinter();

        }
        void ReadPrinter()
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


        void Apply()
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