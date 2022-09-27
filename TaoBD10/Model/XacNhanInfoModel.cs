namespace TaoBD10.Model
{
    public class XacNhanInfoModel
    {
        public string MaHieu { get; set; }
        public string LoaiCT { get; set; }
        public string SoCT { get; set; }
        public string[] Date { get; set; } = new string[3];
        public string MaBCDong { get; set; }
        public bool Is280 { get; set; } = false;

        public XacNhanInfoModel()
        {
        }
    }
}