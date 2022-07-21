using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TaoBD10.Model;

namespace TaoBD10.Manager
{
    public static class FileManager
    {
        public static List<BD10InfoModel> list = new List<BD10InfoModel>();
        public static List<string> listBuuCuc = new List<string>();
        static string auth = "Hw5ESVqVaYfqde21DIHqs4EGhYcqGIiEF4GROViU";
        static string maBuuCuc = "";
        public static List<ChuyenThuModel> listChuyenThu = new List<ChuyenThuModel>();


        public static OptionModel GetOptionAll()
        {
            Task<OptionModel> optionTemp = client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/Option").OnceSingleAsync<OptionModel>();
            optionTemp.Wait();
            optionModel = optionTemp.Result;
            return optionTemp.Result;
        }

        public static void SaveOptionAll(OptionModel option)
        {
            client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/Option").PutAsync(option).Wait();
        }

        public static void SaveBuuCuc(List<BuuCucModel> buuCucModels)
        {
            client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/LayBuuCuc").PutAsync(buuCucModels).Wait();
        }
        public static void SaveLayBD(List<LayBD10Info> layBDs)
        {
            client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/LayBD10").PutAsync(layBDs).Wait();
        }
        public static List<LayBD10Info> LoadLayBD()
        {
            Task<List<LayBD10Info>> cts = client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/LayBD10").OrderByKey().OnceSingleAsync<List<LayBD10Info>>();
            cts.Wait();
            List<LayBD10Info> result = cts.Result;
            return result;



            //if (!File.Exists(_fileCT))
            //{
            //    SaveCT(new List<ChuyenThuModel>());
            //    return null;
            //}

            //JsonSerializer serializer = new JsonSerializer();
            //using (StreamReader sReader = new StreamReader(_fileCT))
            //using (JsonReader jReader = new JsonTextReader(sReader))
            //{
            //    List<ChuyenThuModel> listCT = serializer.Deserialize<List<ChuyenThuModel>>(jReader);
            //    return listCT;
            //}

        }

        public static List<BuuCucModel> LoadBuuCuc()
        {
            Task<List<BuuCucModel>> cts = client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/LayBuuCuc").OrderByKey().OnceSingleAsync<List<BuuCucModel>>();
            cts.Wait();
            List<BuuCucModel> result = cts.Result;
            return result;



            //if (!File.Exists(_fileCT))
            //{
            //    SaveCT(new List<ChuyenThuModel>());
            //    return null;
            //}

            //JsonSerializer serializer = new JsonSerializer();
            //using (StreamReader sReader = new StreamReader(_fileCT))
            //using (JsonReader jReader = new JsonTextReader(sReader))
            //{
            //    List<ChuyenThuModel> listCT = serializer.Deserialize<List<ChuyenThuModel>>(jReader);
            //    return listCT;
            //}

        }

        public static OptionModel optionModel = new OptionModel();

        public static FirebaseClient client;
        public static void onSetupFileManager()
        {
            if (!File.Exists("bccp.txt"))
            {
                return;
            }
            IEnumerable<string> lines = File.ReadLines("bccp.txt");
            foreach (var item in lines)
            {
                maBuuCuc = item.Trim();
                break;
            }


            client = new FirebaseClient("https://taoappbd10-default-rtdb.asia-southeast1.firebasedatabase.app/", new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(auth) });
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
            //JsonSerializer serializer = new JsonSerializer();
            //using (StreamWriter sWriter = new StreamWriter(_file))
            //using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            //{
            //    serializer.Serialize(jWriter, list);
            //}
            client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/BD10").PutAsync(list);
            //client.SetTaskAsync("QuanLyXe/DanhSachBD/"+bD10Info.DateCreateBD10.Year+"|"+bD10Info.DateCreateBD10.DayOfYear+"|"+new Random().Next(), bD10Info);
        }

        internal static async void getContent()
        {
            ////FirebaseResponse data =  client.Get("QuanLyXe/DanhSachBD");
            //List<BD10InfoModel> datas = data.ResultAs<List<BD10InfoModel>>();



        }

        public static List<ChuyenThuModel> LoadCT()
        {
            Task<List<ChuyenThuModel>> cts = client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/OptionCT").OrderByKey().OnceSingleAsync<List<ChuyenThuModel>>();
            cts.Wait();
            List<ChuyenThuModel> result = cts.Result;
            listChuyenThu = result;
            return result;



            //if (!File.Exists(_fileCT))
            //{
            //    SaveCT(new List<ChuyenThuModel>());
            //    return null;
            //}

            //JsonSerializer serializer = new JsonSerializer();
            //using (StreamReader sReader = new StreamReader(_fileCT))
            //using (JsonReader jReader = new JsonTextReader(sReader))
            //{
            //    List<ChuyenThuModel> listCT = serializer.Deserialize<List<ChuyenThuModel>>(jReader);
            //    return listCT;
            //}

        }
        static async void test()
        {
            IReadOnlyCollection<FirebaseObject<ChuyenThuModel>> ss = await client.Child(@"QuanLy").OrderByKey().LimitToFirst(2).OnceAsync<ChuyenThuModel>();
            foreach (var item in ss)
            {
                var sss = item.Key;
            }
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
            client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/OptionCT").PutAsync(chuyenThus);
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

            if (list.Count == 0)
            {
                Task<List<BD10InfoModel>> a = client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/BD10").OnceSingleAsync<List<BD10InfoModel>>();
                a.Wait();
                list = a.Result;
            }
            return list;

            //if (!File.Exists(_file))
            //{
            //    SaveData(new BD10InfoModel());
            //    return null;
            //}
            //JsonSerializer serializer = new JsonSerializer();
            //using (StreamReader sReader = new StreamReader(_file))
            //using (JsonReader jReader = new JsonTextReader(sReader))
            //{
            //    list = serializer.Deserialize<List<BD10InfoModel>>(jReader);

            //    client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/BD10").PutAsync(list);
            //    return list;
            //}
            //dgvDanhSachBD10.DataSource = null;
            //dgvDanhSachBD10.DataSource = listShowBD10Info;
            //dgvDanhSachBD10.Refresh();
        }
    }
}