using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    [Serializable]
    public class FindItemModel
    {
        public string SHTui { get; set; }

        public FindItemModel(string sHTui, string time)
        {
            SHTui = sHTui;
            Time = time;
        }

        public string Time { get; set; }
        

    }
}
