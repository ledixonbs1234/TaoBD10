namespace TaoBD10.Model
{
    public class TuiXacNhanModel
    {
        public string TuiSo { get; set; }
        public string KhoiLuong { get; set; }
        public string DichVu { get; set; }
        public string DVChiTiet { get; set; }
        public string SCT { get; set; }

        public TuiXacNhanModel(string tuiso, string khoiluong, string dichvu, string dvChiTiet, string sct)
        {
            this.TuiSo = tuiso;
            this.KhoiLuong = khoiluong;
            this.DichVu = dichvu;
            this.DVChiTiet = dvChiTiet;
            this.SCT = sct;
        }
    }
}