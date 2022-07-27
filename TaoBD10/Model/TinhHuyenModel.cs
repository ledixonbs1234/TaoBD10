using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class TinhHuyenModel
    {
        public bool IsChecked { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public TinhHuyenModel(string ten, string ma)
        {
            Ten = ten;
            Ma = ma;
        }
    }
}
