using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class MaHieuDiNgoaiInfo
    {
        public string Code { get; set; }
        public string BuuCucGui { get; set; }
        public string TinhGocGui { get; set; }
        public string BuuCucNhanTemp { get; set; }
        public MaHieuDiNgoaiInfo(string code, string buuCucGui, string tinhGocGui, string buuCucNhanTemp)
        {
            Code = code;
            BuuCucGui = buuCucGui;
            TinhGocGui = tinhGocGui;
            BuuCucNhanTemp = buuCucNhanTemp;
        }
        public MaHieuDiNgoaiInfo(string code)
        {
            Code = code;
        }
    }
}
