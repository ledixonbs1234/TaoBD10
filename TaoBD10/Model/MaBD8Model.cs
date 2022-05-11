using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class MaBD8Model
    {
        public string Ma { get; set; }
        public string ChiTiet { get; set; }
        public MaBD8Model(string ma,string chitiet)
        {
            this.Ma = ma;
            this.ChiTiet = chitiet;
        }
    }
}
