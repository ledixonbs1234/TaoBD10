using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaoBD10.Model;

namespace TaoBD10.Manager
{
    public static class ToolManager
    {
        public static List<TinhThanh> listTinh;
        public static void SetUpTinh()
        {
            listTinh = new List<TinhThanh>();
            string[] texts = File.ReadAllLines("MaTinh.txt");
            foreach (string item in texts)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    string[] split = item.Split('\t');
                    listTinh.Add(new TinhThanh(split[1], split[2]));
                }
            }
        }
        public static int AutoSetTinh(string address)
        {
            if (listTinh.Count == 0)
                return 0;
            List<string> fillAddress = address.Split('-').Select(s => s.Trim()).ToList();
            if (fillAddress == null)
                return 0;
            if (fillAddress.Count < 3)
                return 0;
            string addressBoDau = APIManager.BoDauAndToLower(fillAddress[fillAddress.Count - 1].Trim());
            foreach (var item in ToolManager.listTinh)
            {
                if (item.TinhKhongDau == addressBoDau)
                {
                    return int.Parse(item.MaTinh);
                }
            }
            return 0;

        }
    }
}
