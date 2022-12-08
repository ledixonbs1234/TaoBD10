namespace TaoBD10.Model
{
    public class PNSNameModel
    {
        public string MaHieu { get; set; }
        public string NameReceive { get; set; }
        public string Address { get; set; }
        public string PhoneReceive { get; set; }
        public string TrangThai { get; set; }

        public PNSNameModel(string maHieu, string nameReceive, string address, string phoneReceive, string trangThai = null)
        {
            NameReceive = nameReceive;
            MaHieu = maHieu;
            Address = address;
            PhoneReceive = phoneReceive;
            TrangThai = trangThai;
        }
    }
}