using System;

namespace TaoBD10.Model
{
    [Serializable]
    public class ChuyenThuModel
    {
        public string Ten { get; set; }
        public string TenButton { get; set; }
        public string NumberTinh { get; set; }
        public string TextLoai { get; set; }
        public string TextTui { get; set; }
        public string CheckTinh { get; set; }
        public string CheckLoai { get; set; }
        public string CheckThuyBo { get; set; }
        public string NameMusic { get; set; }
        public string Barcode { get; set; }

        public ChuyenThuModel()
        {
        }
    }
}