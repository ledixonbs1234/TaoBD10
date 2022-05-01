using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class DiNgoaiItemModel : ObservableObject
    {
        public int Index { get; set; }
        public string Code { get; set; }
        public string BuuCucGui { get; set; }
        public string AddressSend { get; set; }

        private string _Address;

        public string Address
        {
            get { return _Address; }
            set { SetProperty(ref _Address, value); }
        }

        private string _MaTinh;

        public string MaTinh
        {
            get { return _MaTinh; }
            set { SetProperty(ref _MaTinh, value); }
        }

        private string _MaBuuCuc;

        public string MaBuuCuc
        {
            get { return _MaBuuCuc; }
            set { SetProperty(ref _MaBuuCuc, value); }
        }

        private string _TenBuuCuc;

        public string TenBuuCuc
        {
            get { return _TenBuuCuc; }
            set { SetProperty(ref _TenBuuCuc, value); }
        }


        private string _TenTinh;

        public string TenTinh
        {
            get { return _TenTinh; }
            set { SetProperty(ref _TenTinh, value); }
        }


        public DiNgoaiItemModel(int index, string code)
        {
            this.Index = index;
            Code = code;
        }
    }
}
