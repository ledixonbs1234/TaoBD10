using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class MessageToPhoneModel
    {
        public MessageToPhoneModel(string lenh, object doiTuong = null)
        {
            Lenh = lenh;
            TimeStamp = GetTimestamp(DateTime.Now);
            DoiTuong = doiTuong;
        }

        private String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public string Lenh { get; set; }
        public string TimeStamp { get; set; }
        public Object DoiTuong { get; set; }
    }
}