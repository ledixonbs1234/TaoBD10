using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class MaQuanHuyenInfo
    {
        public MaQuanHuyenInfo(int maQuanHuyen, string tenQuanHuyen, string tenTinh)
        {
            MaQuanHuyen = maQuanHuyen;
            TenQuanHuyen = tenQuanHuyen;
            TenTinh = tenTinh;
        }

        public int MaQuanHuyen { get; set; }
        public string TenQuanHuyen { get; set; }
        public string TenTinh { get; set; }
    }
}