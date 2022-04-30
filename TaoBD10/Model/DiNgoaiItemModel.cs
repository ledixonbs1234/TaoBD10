using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class DiNgoaiItemModel
    {
        public int Index { get; set; }
        public string Code { get; set; }
        public string BuuCucGui { get; set; }
        public string AddressSend { get; set; }

        public string Address { get; set; }
        public string MaTinh { get; set; }
        public string MaBuuCuc { get; set; }
        public string TenBuuCuc { get; set; }

        public string TenTinh { get; set; }

        public DiNgoaiItemModel(int index, string code)
        {
            this.Index = index;
            Code = code;
        }
    }
}
