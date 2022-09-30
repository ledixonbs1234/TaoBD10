using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class ThongTinGiaoNhanBDModel
    {
        public string NgayBD { get; set; }

        public ThongTinGiaoNhanBDModel(string ngayBD, string ngayXacNhanDi, string buuCucGiao, string buuCucNhan, string lanLap, string maBD10)
        {
            NgayBD = ngayBD;
            NgayXacNhanDi = ngayXacNhanDi;
            BuuCucGiao = buuCucGiao;
            BuuCucNhan = buuCucNhan;
            LanLap = lanLap;
            MaBD10 = maBD10;
        }

        public string NgayXacNhanDi { get; set; }

        public string BuuCucGiao { get; set; }
        public string BuuCucNhan { get; set; }

        public string LanLap { get; set; }
        public string MaBD10 { get; set; }

    }
}
