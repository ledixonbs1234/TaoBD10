using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class LocBDInfoModel
    {
        public string TenBD { get; set; }
        public bool IsTinh { get; set; } = true;
        public List<TinhHuyenModel> DanhSachTinh { get; set; }
        public List<TinhHuyenModel> DanhSachHuyen { get; set; }
        public List<HangHoaDetailModel> HangHoas { get; set; }

        public LocBDInfoModel()
        {
            DanhSachTinh = new List<TinhHuyenModel>();
            DanhSachHuyen=new List<TinhHuyenModel>(); 
        }
    }
}
