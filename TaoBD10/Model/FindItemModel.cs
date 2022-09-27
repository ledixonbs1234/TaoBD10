using System;

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