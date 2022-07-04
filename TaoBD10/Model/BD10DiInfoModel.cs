using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaoBD10.Manager;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.Model
{
    public class BD10DiInfoModel
    {
        public string Name { get; set; }

        public BD10DiInfoModel(string name, string date, int lanLap, int soLuong,string sTrangThai)
        {
            Name = name;
            MaBuuCuc = name.Substring(0,6);
            Date = date;
            LanLap = lanLap;
            SoLuong = soLuong;
            string text = APIManager.BoDauAndToLower(sTrangThai);
            if(sTrangThai.IndexOf("da di")!= -1)
            {
                TrangThai = StateBD10Di.DaDi;
                
            }else if(sTrangThai.IndexOf("khoi tao")!= -1)
            {
                TrangThai = StateBD10Di.KhoiTao;
            }else
            {
                TrangThai = StateBD10Di.None;
            }
        }

        public string MaBuuCuc { get; set; }
        public string Date { get; set; }
        public int LanLap { get; set; }
        public int SoLuong { get; set; }
        public StateBD10Di TrangThai { get; set; }
        

    }
}
