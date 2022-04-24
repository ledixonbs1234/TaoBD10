using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.Model
{
    public class HangHoaDetailModel
    {
        public TuiHangHoa TuiHangHoa { get; set; }
        public PhanLoaiTinh PhanLoai { get; set; } = PhanLoaiTinh.None;

        public HangHoaDetailModel(TuiHangHoa tuiHangHoa, PhanLoaiTinh phanLoaiTinh)
        {
            TuiHangHoa = tuiHangHoa;
            PhanLoai = phanLoaiTinh;
        }
    }
}