using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    [Serializable]
    public class BuuCucModel
    {
        public string MaBuuCuc { get; set; }
        public string TenBuuCuc { get; set; }
        public bool IsBaoDam { get; set; }
        public string MaBCCP { get; set; }
        public string GoFastBCCP { get; set; }
        public BuuCucModel()
        {

        }
    }
}
