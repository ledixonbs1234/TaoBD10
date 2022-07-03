using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class PNSNameModel
    {
        public string MaHieu { get; set; }
        public string NameReceive { get; set; }
        public string Address { get; set; }

        public PNSNameModel(string maHieu, string nameReceive, string address)
        {
            NameReceive = nameReceive;
            MaHieu = maHieu;
            Address = address;
        }

        public PNSNameModel()
        {

        }
    }
}
