using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class ThongTinTrangThaiModel
    {
        public string Date { get; set; }

        public ThongTinTrangThaiModel(string date, string hour, string trangThai, string buuCuc)
        {
            Date = date;
            Hour = hour;
            TrangThai = trangThai;
            BuuCuc = buuCuc;
        }

        public string Hour { get; set; }
        public string TrangThai { get; set; }
        public string BuuCuc { get; set; }
        public ThongTinTrangThaiModel()
        {

        }
    }
}
