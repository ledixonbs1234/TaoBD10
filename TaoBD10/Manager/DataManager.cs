using System.Collections.Generic;
using System.Linq;
using TaoBD10.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace TaoBD10.Manager
{
    public static class DataManager
    {
        public static WebContentModel WebContent = new WebContentModel();
        public static bool IsWaitingCompleteLayComplete = false;
        private static List<TinhHuyenModel> tinhs;

        public static void SetUp()
        {
            tinhs = FileManager.LoadTinhThanhOffline();
        }

        public static string AutoSetTinh(string address)
        {
            if (tinhs.Count == 0)
                return "";
            List<string> fillAddress = address.Split('-').Select(s => s.Trim()).ToList();
            if (fillAddress == null)
                return "";
            if (fillAddress.Count < 3)
                return "";
            string addressBoDau = APIManager.BoDauAndToLower(fillAddress[fillAddress.Count - 1].Trim());
            foreach (var item in tinhs)
            {
                if (APIManager.BoDauAndToLower(item.Ten) == addressBoDau)
                {
                    return item.Ma;
                }
            }
            return "";
        }
    }
}