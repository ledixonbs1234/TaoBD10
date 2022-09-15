using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    [Serializable]
    public class BD10DenInfo
    {
        public string Name { get; set; }

        public BD10DenInfo(string name, string lanLap, string soLuong, string trongLuong, string trangThai)
        {
            Name = name;
            LanLap = lanLap;
            SoLuong = soLuong;
            TrongLuong = trongLuong;
            TrangThai = trangThai;
        }

        public string LanLap { get; set; }
        public string SoLuong { get; set; }
        public string TrongLuong { get; set; }
        public string TrangThai { get; set; }

    }
}
