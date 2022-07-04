namespace TaoBD10.Model
{
    public class ChiTietTuiModel
    {
        public string MaHieu { get; set; }
        public string Address { get; set; }

        public ChiTietTuiModel(string maHieu, string address)
        {
            Address = address;
            MaHieu = maHieu;
        }
    }
}