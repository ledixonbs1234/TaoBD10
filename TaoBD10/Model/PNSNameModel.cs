namespace TaoBD10.Model
{
    public class PNSNameModel
    {
        public string MaHieu { get; set; }
        public string NameReceive { get; set; }
        public string Address { get; set; }
        public string PhoneReceive { get; set; }

        public PNSNameModel(string maHieu, string nameReceive, string address, string phoneReceive)
        {
            NameReceive = nameReceive;
            MaHieu = maHieu;
            Address = address;
            PhoneReceive = phoneReceive;
        }

        public PNSNameModel()
        {
        }
    }
}