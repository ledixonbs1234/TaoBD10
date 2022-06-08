using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace TaoBD10.Model
{
    public class DiNgoaiItemModel : ObservableObject
    {
        public DiNgoaiItemModel(int index, string code)
        {
            this.Index = index;
            Code = code;
        }

        public DiNgoaiItemModel(int index, string code, string buuCucGui, string buuCucNhan, string tinhGocGui)
        {
            this.Index = index;
            Code = code;
            this.BuuCucGui = buuCucGui;
            BuuCucNhanTemp = buuCucNhan;
            TinhGocGui = tinhGocGui;
        }

        public string Address
        {
            get
            {
                return
                  _Address;
            }
            set { SetProperty(ref _Address, value); }
        }

        public string AddressSend { get; set; }
        public string BuuCucGui { get; set; }
        public string BuuCucNhanTemp
        {
            get { return _BuuCucNhanTemp; }
            set { SetProperty(ref _BuuCucNhanTemp, value); }
        }

        public string Code { get; set; }
        public int Index { get; set; }
        public string MaBuuCuc
        {
            get { return _MaBuuCuc; }
            set { SetProperty(ref _MaBuuCuc, value); }
        }

        public string MaTinh
        {
            get { return _MaTinh; }
            set { SetProperty(ref _MaTinh, value); }
        }

        public string TenBuuCuc
        {
            get { return _TenBuuCuc; }
            set { SetProperty(ref _TenBuuCuc, value); }
        }

        public string TenTinh
        {
            get { return _TenTinh; }
            set { SetProperty(ref _TenTinh, value); }
        }

        public string TinhGocGui { get; set; }
    
        private string _Address;
        private string _BuuCucNhanTemp;
        private string _MaBuuCuc;
        private string _MaTinh;
        private string _TenBuuCuc;
        private string _TenTinh;
    }
}