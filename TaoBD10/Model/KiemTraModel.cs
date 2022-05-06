using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class KiemTraModel :ObservableObject
    {
        private int _Index;
        private string _Date;

        public string Date
        {
            get { return _Date; }
            set { SetProperty(ref _Date, value); }
        }


        public int Index
        {
            get { return _Index; }
            set { SetProperty(ref _Index, value); }
        }
        private string _Address;

        public string Address
        {
            get { return _Address; }
            set { SetProperty(ref _Address, value); }
        }
        private string _MaBuuTa;

        public string MaBuuTa
        {
            get { return _MaBuuTa; }
            set { SetProperty(ref _MaBuuTa, value); }
        }
        private bool _IsDaPhan =false;

        public bool IsDaPhan
        {
            get { return _IsDaPhan; }
            set { SetProperty(ref _IsDaPhan, value); }
        }
        public KiemTraModel()
        {

        }

    }
}
