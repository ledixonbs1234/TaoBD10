using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    [Serializable]
    public class TinhHuyenModel
    {
        public bool IsChecked { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public TinhHuyenModel(string ma, string ten)
        {
            Ten = ten;
            Ma = ma;
        }
    }
}
