using Microsoft.Toolkit.Mvvm.ComponentModel;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.Model
{
    public class HangHoaDetailModel:ObservableObject
    {
        public TuiHangHoa TuiHangHoa { get; set; }
        //public TrangThaiBD TrangThaiBD { get; set; } = TrangThaiBD.ChuaChon;
        private TrangThaiBD _TrangThaiBD = Manager.EnumAll.TrangThaiBD.ChuaChon;
        private string _Address;

        public string Address
        {
            get { return _Address; }
            set { SetProperty(ref _Address, value); }
        }

        private bool _IsTamQuan = false;

        public bool IsTamQuan
        {
            get { return _IsTamQuan; }
            set { SetProperty(ref _IsTamQuan, value); }
        }


        private string _Code;

        public string Code
        {
            get { return _Code; }
            set { SetProperty(ref _Code, value); }
        }


        public TrangThaiBD TrangThaiBD
        {
            get { return _TrangThaiBD; }
            set { SetProperty(ref _TrangThaiBD, value); }
        }

        public PhanLoaiTinh PhanLoai { get; set; } = PhanLoaiTinh.None;

        public HangHoaDetailModel(TuiHangHoa tuiHangHoa, PhanLoaiTinh phanLoaiTinh)
        {
            TuiHangHoa = tuiHangHoa;
            PhanLoai = phanLoaiTinh;
        }
    }
}