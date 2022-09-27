using System.Collections.Generic;

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