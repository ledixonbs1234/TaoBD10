using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace TaoBD10.Model
{
    public class HangTonModel : ObservableObject
    {
        private string _Index;
        private string _MaHieu;

        private string _TimeGui;
        private string _TimeCapNhat;
        private string _BuuCucPhatHanh;
        private string _NguoiGui;
        private string _BuuCucLuuGiu;
        private string _ChuyenHoan;

        public string ChuyenHoan
        {
            get { return _ChuyenHoan; }
            set { SetProperty(ref _ChuyenHoan, value); }
        }

        public string BuuCucLuuGiu
        {
            get { return _BuuCucLuuGiu; }
            set { SetProperty(ref _BuuCucLuuGiu, value); }
        }

        public string NguoiGui
        {
            get { return _NguoiGui; }
            set { SetProperty(ref _NguoiGui, value); }
        }

        public string BuuCucPhatHanh
        {
            get { return _BuuCucPhatHanh; }
            set { SetProperty(ref _BuuCucPhatHanh, value); }
        }

        public string TimeCapNhat
        {
            get { return _TimeCapNhat; }
            set { SetProperty(ref _TimeCapNhat, value); }
        }

        public string TimeGui
        {
            get { return _TimeGui; }
            set { SetProperty(ref _TimeGui, value); }
        }

        private string _KhoiLuong;

        public string KhoiLuong
        {
            get { return _KhoiLuong; }
            set { SetProperty(ref _KhoiLuong, value); }
        }

        private string _Address;
        private string _TienThuHo;

        public string TienThuHo
        {
            get { return _TienThuHo; }
            set { SetProperty(ref _TienThuHo, value); }
        }

        public string Address
        {
            get { return _Address; }
            set { SetProperty(ref _Address, value); }
        }

        public string MaHieu
        {
            get { return _MaHieu; }
            set { SetProperty(ref _MaHieu, value); }
        }

        public string Index
        {
            get { return _Index; }
            set { SetProperty(ref _Index, value); }
        }
        private string _BuuCucDong;

        public string BuuCucDong
        {
            get { return _BuuCucDong; }
            set { SetProperty(ref _BuuCucDong, value); }
        }
        private string _BuuCucNhan;

        public string BuuCucNhan
        {
            get { return _BuuCucNhan; }
            set { SetProperty(ref _BuuCucNhan, value); }
        }


        public HangTonModel()
        {
        }
    }
}