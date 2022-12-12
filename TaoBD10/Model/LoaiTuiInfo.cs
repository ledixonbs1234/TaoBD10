using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class LoaiTuiInfo
    {
        public string Ma { get; set; }

        public LoaiTuiInfo(string ma, string chiTiet)
        {
            Ma = ma;
            ChiTiet = chiTiet;
        }

        public string ChiTiet { get; set; }
    }
}