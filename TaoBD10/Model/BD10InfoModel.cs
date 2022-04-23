using System;
using System.Collections.Generic;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.Model
{
    [Serializable]
    public class BD10InfoModel
    {
        public string Name { get; set; }
        public string LanLap { get; set; }
        public bool isChecked { get; set; }

        public DateTime DateCreateBD10 { get; set; }
        public TimeSet TimeTrongNgay { get; set; }
        public string CountTui { get; set; }
        public List<TuiHangHoa> TuiHangHoas { get; set; }

        public BD10InfoModel()
        {
        }

        public BD10InfoModel(string name, List<TuiHangHoa> tuiHangHoas, DateTime timeCreate, TimeSet timeTrongNgay, string lanLap)
        {
            isChecked = false;
            this.Name = name;
            CountTui = tuiHangHoas.Count.ToString();
            this.TimeTrongNgay = timeTrongNgay;
            this.DateCreateBD10 = timeCreate;
            this.LanLap = lanLap;
            TuiHangHoas = new List<TuiHangHoa>();
            foreach (var item in tuiHangHoas)
            {
                TuiHangHoas.Add(item);
            }
        }
    }
}