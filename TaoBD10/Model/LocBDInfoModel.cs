using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    [Serializable]
    public class LocBDInfoModel:ObservableObject
    {
        public string TenBD { get; set; }
        public bool IsTinh { get; set; } = true;
        private ObservableCollection<TinhHuyenModel> _DanhSachTinh;

        public ObservableCollection<TinhHuyenModel> DanhSachTinh
        {
            get { return _DanhSachTinh; }
            set { SetProperty(ref _DanhSachTinh, value); }
        }





        public string DanhSachHuyen { get; set; }
        //public List<HangHoaDetailModel> HangHoas { get; set; }

        public LocBDInfoModel()
        {
            DanhSachTinh = new ObservableCollection<TinhHuyenModel>();
        }
    }
}
