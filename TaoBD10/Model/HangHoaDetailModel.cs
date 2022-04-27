using Microsoft.Toolkit.Mvvm.ComponentModel;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.Model
{
    public class HangHoaDetailModel:ObservableObject
    {
        public TuiHangHoa TuiHangHoa { get; set; }
        //public TrangThaiBD TrangThaiBD { get; set; } = TrangThaiBD.ChuaChon;
        private TrangThaiBD _TrangThaiBD = Manager.EnumAll.TrangThaiBD.ChuaChon;

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