using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class ThongTinCoBanModel
    {
        public string MaHieu { get; set; }
        public string BuuCucChapNhan { get; set; }
        public string NguoiGui { get; set; }
        public string DiaChiGui { get; set; }
        public string NguoiNhan { get; set; }
        public string DiaChiNhan { get; set; }
        public List<ThongTinTrangThaiModel> ThongTinTrangThais { get; set; } 
        public List<ThongTinGiaoNhanBDModel> ThongTinGiaoNhanBDs { get; set; }
        public List<ThongTinChuyenThuModel> ThongTinChuyenThus { get; set; }
        public ThongTinCoBanModel()
        {

        }


    }
}
