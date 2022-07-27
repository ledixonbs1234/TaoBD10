using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class XacNhanInfoModel
    {
        public string MaHieu { get; set; }
        public string LoaiCT { get; set; }
        public string SoCT { get; set; }
        public string[] Date { get; set; } = new string[0];
        public string MaBCDong { get; set; }
        public bool Is280 { get; set; } = false;
        public XacNhanInfoModel()
        {

        }
    }
}
