using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TaoBD10.Model;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Firebase.Database;
using System.Threading.Tasks;
using Firebase.Database.Query;

namespace TaoBD10.Manager
{
    public static class FileManager
    {
        public static List<BD10InfoModel> list = new List<BD10InfoModel>();
        public static List<string> listBuuCuc = new List<string>();
        static string auth = "Hw5ESVqVaYfqde21DIHqs4EGhYcqGIiEF4GROViU";

        public static FirebaseClient client;
        public static void onSetupFileManager()
        {
            //IFirebaseConfig config = new FirebaseConfig
            //{
            //    AuthSecret = "Hw5ESVqVaYfqde21DIHqs4EGhYcqGIiEF4GROViU",
            //    BasePath = "https://taoappbd10-default-rtdb.asia-southeast1.firebasedatabase.app/"
            //};
            //client = new FireSharp.FirebaseClient(config);
            //if (client!= null)
            //{
            //    APIManager.ShowSnackbar("Connected Firebase");
            //

            //}

            client = new FirebaseClient("https://taoappbd10-default-rtdb.asia-southeast1.firebasedatabase.app/", new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(auth) });
            client.

        }


        public static void GetCode()
        {
            string dir = Directory.GetCurrentDirectory();
            string text = File.ReadAllText(dir + @"/" + "data.txt");
            var texts = text.Split('\n');
            for (int i = 0; i < texts.Length; i++)
            {
                if (!string.IsNullOrEmpty(texts[i]))
                {
                    listBuuCuc.Add(texts[i]);
                }
            }
        }

        private static string _file = Environment.CurrentDirectory + "\\data.json";
        private static string _fileBD8 = Environment.CurrentDirectory + "\\matuithu.txt";
        private static string _fileCT = Environment.CurrentDirectory + "\\dataCT.json";

        public static void SaveData(BD10InfoModel bD10Info)
        {
            if (bD10Info != null)
                list.Add(bD10Info);
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_file))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, list);
            }
            //client.SetTaskAsync("QuanLyXe/DanhSachBD/"+bD10Info.DateCreateBD10.Year+"|"+bD10Info.DateCreateBD10.DayOfYear+"|"+new Random().Next(), bD10Info);
        }

        internal static async void getContent()
        {
            ////FirebaseResponse data =  client.Get("QuanLyXe/DanhSachBD");
            //List<BD10InfoModel> datas = data.ResultAs<List<BD10InfoModel>>();



        }

        public static List<ChuyenThuModel> LoadCT()
        {
            if (!File.Exists(_fileCT))
            {
                SaveCT(new List<ChuyenThuModel>());
                return null;
            }

            test();
            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(_fileCT))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                List<ChuyenThuModel> listCT = serializer.Deserialize<List<ChuyenThuModel>>(jReader);
                return listCT;
            }

        }
        static async void test()
        {
            var ss = await client.Child(@"QuanLyXe").OnceAsync<ChuyenThuModel>();
            string a = "dfd";
        }
        public static void SaveCT(List<ChuyenThuModel> chuyenThus)
        {
            if (chuyenThus.Count == 0)
                return;
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_fileCT))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, chuyenThus);
            }
            //client.SetTaskAsync("QuanLy/593230",chuyenThus);
            client.Child("QuanLyXe").PostAsync(chuyenThus[0]);

        }

        public static List<MaBD8Model> GetMaBD8s()
        {
            if (File.Exists(_fileBD8))
            {
                string text = File.ReadAllText(_fileBD8);
                List<MaBD8Model> list = new List<MaBD8Model>();
                foreach (var item in text.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        string[] item1 = item.Split('\t');
                        MaBD8Model maBd = new MaBD8Model(item1[1], item1[2]);
                        list.Add(maBd);
                    }
                }
                return list;
            }
            return null;
        }

        public static List<BD10InfoModel> LoadData()
        {
            if (!File.Exists(_file))
            {
                SaveData(new BD10InfoModel());
                return null;
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(_file))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                list = serializer.Deserialize<List<BD10InfoModel>>(jReader);
                return list;
            }
            //dgvDanhSachBD10.DataSource = null;
            //dgvDanhSachBD10.DataSource = listShowBD10Info;
            //dgvDanhSachBD10.Refresh();
        }
    }
}