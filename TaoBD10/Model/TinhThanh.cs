using TaoBD10.Manager;

namespace TaoBD10.Model
{
    public class TinhThanh
    {
        public string MaTinh { get; set; }
        public string TinhKhongDau { get; set; }

        public TinhThanh(string maTinh, string tinh)
        {
            MaTinh = maTinh;
            Tinh = tinh;
            TinhKhongDau = APIManager.BoDauAndToLower(tinh);
        }

        public string Tinh { get; set; }
    }
}