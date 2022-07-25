namespace TaoBD10.Model
{
    public class ChiTietTuiModel
    {
        public string MaHieu { get; set; }
        public string Address { get; set; }
        public string BCChapNhan { get; set; }

        public ChiTietTuiModel(string maHieu, string address, string bCChapNhan)
        {
            Address = address;
            MaHieu = maHieu;
            BCChapNhan = bCChapNhan;
        }
    }
}