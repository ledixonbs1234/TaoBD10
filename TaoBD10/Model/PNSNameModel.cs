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

        public PNSNameModel( string maHieu,string nameReceive)
        {
            NameReceive = nameReceive;
            MaHieu = maHieu;
        }

        public PNSNameModel()
        {

        }
    }
}
