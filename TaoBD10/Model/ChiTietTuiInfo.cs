using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class ChiTietTuiInfo
    {
        public string Key { get; set; } = "";
        public List<ChiTietTuiModel> ChiTietTuis { get; set; }
        public ChiTietTuiInfo()
        {
            ChiTietTuis = new List<ChiTietTuiModel>();

        }

    }
}
