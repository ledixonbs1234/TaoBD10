using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace TaoBD10.Model
{
    public class KiemTraModel : ObservableObject
    {
        private int _Index;
        private string _Date;
        private string _MaHieu;

        private string _Key;

        public string Key
        {
            get { return _Key; }
            set { SetProperty(ref _Key, value); }
        }

        public string MaHieu
        {
            get { return _MaHieu; }
            set { SetProperty(ref _MaHieu, value); }
        }

        public string Date
        {
            get { return _Date; }
            set { SetProperty(ref _Date, value); }
        }

        private string _BuuCucDong;

        public string BuuCucDong
        {
            get { return _BuuCucDong; }
            set { SetProperty(ref _BuuCucDong, value); }
        }

        public List<ThongTinTrangThaiModel> ThongTins { get; set; }

        private string _BuuCucNhan;

        public string BuuCucNhan
        {
            get { return _BuuCucNhan; }
            set { SetProperty(ref _BuuCucNhan, value); }
        }

        private string _TTCT;

        public string TTCT
        {
            get { return _TTCT; }
            set { SetProperty(ref _TTCT, value); }
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

        private bool _IsDaPhan = false;

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