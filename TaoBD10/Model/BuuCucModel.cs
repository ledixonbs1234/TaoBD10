using System;

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