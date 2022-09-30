using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class ThongTinChuyenThuModel
    {
        public string DateCT { get; set; }
        public string BuuCucDong { get; set; }
        public string BuuCucNhan { get; set; }
        public string ThongTinCT { get; set; }
        public string HourCT { get; set; }
        public ThongTinChuyenThuModel( string dateCT, string hourct,string buuCucDong, string buuCucNhan, string thongTinCT)
        {
            HourCT = hourct;
            DateCT = dateCT;
            BuuCucDong = buuCucDong;
            BuuCucNhan = buuCucNhan;
            ThongTinCT = thongTinCT;
        }

    }
}
