using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class ChuyenThuInQuanLyModel
    {
        public ChuyenThuInQuanLyModel(string buuCucDong, string dichVu, string soCT, string dateCreate, string sLTui, string sLBuuGui, string kL)
        {
            BuuCucDong = buuCucDong;
            DichVu = dichVu;
            SoCT = soCT;
            SLTui = sLTui;
            DateCreate = dateCreate;
            SLBuuGui = sLBuuGui;
            KL = kL;
        }

        public string DateCreate { get; set; }

        public string BuuCucDong { get; set; }
        public string DichVu { get; set; }
        public string SoCT { get; set; }
        public string SLTui { get; set; }
        public string SLBuuGui { get; set; }
        public string KL { get; set; }
    }
}