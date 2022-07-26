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

        public static OptionModel GetOptionOffline()
        {
            if (!File.Exists(_fileOption))
            {
                SaveOptionOffline(new OptionModel());
            }

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(_fileOption))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                OptionModel option = serializer.Deserialize<OptionModel>(jReader);
                return option;
            }

            Task<OptionModel> optionTemp = client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/Option").OnceSingleAsync<OptionModel>();
            optionTemp.Wait();
            optionModel = optionTemp.Result;
            return optionTemp.Result;
        }

        public static void SaveOptionOffline(OptionModel option)
        {
            if (!File.Exists(_fileOption))
            {
                using (FileStream fs = File.Create(_fileOption))
                {

                }
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_fileOption))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, option);
            }
        }



        public static OptionModel GetOptionAll()
        {
            Task<OptionModel> optionTemp = client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/Option").OnceSingleAsync<OptionModel>();
            optionTemp.Wait();
            optionModel = optionTemp.Result;
            return optionTemp.Result;
        }
        public static List<BuuCucModel> LoadBuuCucOffline()
        {
            if (!File.Exists(_fileBuuCuc))
            {
                SaveBuuCucOffline(new List<BuuCucModel>());
            }

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(_fileBuuCuc))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                List<BuuCucModel> listBuuCuc = serializer.Deserialize<List<BuuCucModel>>(jReader);
                return listBuuCuc;
            }

        }

        public static void SaveBuuCucOffline(List<BuuCucModel> buucucs)
        {
            if (!File.Exists(_fileBuuCuc))
            {
                using (FileStream fs = File.Create(_fileBuuCuc))
                {

                }
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_fileBuuCuc))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, buucucs);
            }
        }


        public static List<BuuCucModel> LoadBuuCucOnFirebase()
        {
            onSetupFileManager();
            Task<List<BuuCucModel>> cts = client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/LayBuuCuc").OrderByKey().OnceSingleAsync<List<BuuCucModel>>();
            cts.Wait();
            List<BuuCucModel> result = cts.Result;
            SaveBuuCucOffline(result);
            return result;

        }

        public static List<ChuyenThuModel> LoadCTOnFirebase()
        {
            onSetupFileManager();
            Task<List<ChuyenThuModel>> cts = client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/OptionCT").OrderByKey().OnceSingleAsync<List<ChuyenThuModel>>();
            cts.Wait();
            List<ChuyenThuModel> result = cts.Result;
            SaveCTOffline(result);
            return result;
        }

        public static List<BD10InfoModel> LoadBD10Offline()
        {
            if (!File.Exists(_fileBD10))
            {
                SaveBD10Offline(new List<BD10InfoModel>());
            }

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(_fileBD10))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                List<BD10InfoModel> listBD10 = serializer.Deserialize<List<BD10InfoModel>>(jReader);
                return listBD10;
            }
        }
        public static void SaveBD10Offline(List<BD10InfoModel> bd10s)
        {
            if (!File.Exists(_fileBD10))
            {
                using (FileStream fs = File.Create(_fileBD10))
                {

                }
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_fileBD10))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, bd10s);
            }
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

        public static List<LayBD10Info> LoadLayBDOffline()
        {
            if (!File.Exists(_fileLayBD))
            {
                SaveLayBDOffline(new List<LayBD10Info>());
            }

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(_fileLayBD))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                List<LayBD10Info> listLayBD = serializer.Deserialize<List<LayBD10Info>>(jReader);
                return listLayBD;
            }
        }

        public static void SaveLayBDOffline(List<LayBD10Info> laybds)
        {
            if (!File.Exists(_fileLayBD))
            {
                using (FileStream fs = File.Create(_fileLayBD))
                {

                }
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_fileLayBD))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, laybds);
            }
        }



        public static List<LayBD10Info> LoadLayBDOnFirebase()
        {
            onSetupFileManager();
            Task<List<LayBD10Info>> cts = client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/LayBD10").OrderByKey().OnceSingleAsync<List<LayBD10Info>>();
            cts.Wait();
            List<LayBD10Info> result = cts.Result;
            SaveLayBDOffline(result);
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

        public static void onSetupFileManager()
        {
            if (client == null)
            {
                if (!File.Exists(_fileBCCP))
                {
                    using (FileStream fs = File.Create(_fileBCCP)) { }
                }
                IEnumerable<string> lines = File.ReadLines(_fileBCCP);
                foreach (var item in lines)
                {
                    maBuuCuc = item.Trim();
                    break;
                }
                client = new FirebaseClient("https://taoappbd10-default-rtdb.asia-southeast1.firebasedatabase.app/", new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(auth) });
            }

        }

        public static void SaveBuuCuc(List<BuuCucModel> buuCucModels)
        {
            client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/LayBuuCuc").PutAsync(buuCucModels).Wait();
        }

        public static void SaveCTOffline(List<ChuyenThuModel> chuyenThus)
        {
            if (!File.Exists(_fileCT))
            {
                using (FileStream fs = File.Create(_fileCT)) { }
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_fileCT))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, chuyenThus);
            }
        }

        public static void SaveCTOnFirebase(List<ChuyenThuModel> chuyenThus)
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

        public static void SaveLayBD(List<LayBD10Info> layBDs)
        {
            client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/LayBD10").PutAsync(layBDs).Wait();
        }

        public static void SaveOptionAll(OptionModel option)
        {
            client.Child(@"QuanLy/DanhSach/" + maBuuCuc + "/Option").PutAsync(option).Wait();
        }

        public static List<ChuyenThuModel> LoadCTOffline()
        {
            if (!File.Exists(_fileCT))
            {
                SaveCTOffline(new List<ChuyenThuModel>());
                return null;
            }

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
            IReadOnlyCollection<FirebaseObject<ChuyenThuModel>> ss = await client.Child(@"QuanLy").OrderByKey().LimitToFirst(2).OnceAsync<ChuyenThuModel>();
            foreach (var item in ss)
            {
                var sss = item.Key;
            }
        }

        public static FirebaseClient client;
        public static List<BD10InfoModel> list = new List<BD10InfoModel>();
        public static List<string> listBuuCuc = new List<string>();
        public static List<ChuyenThuModel> listChuyenThu = new List<ChuyenThuModel>();
        public static OptionModel optionModel = new OptionModel();
        private static string _fileBD10 = Environment.CurrentDirectory + "\\Data\\bd10.json";
        private static string _fileOption = Environment.CurrentDirectory + "\\Data\\option.json";
        private static string _fileBD8 = Environment.CurrentDirectory + "\\Data\\matuithu.txt";
        private static string _fileCT = Environment.CurrentDirectory + "\\Data\\dataCT.json";
        private static string _fileLayBD = Environment.CurrentDirectory + "\\Data\\dataLayBD.json";
        private static string _fileBuuCuc = Environment.CurrentDirectory + "\\Data\\dataBuuCuc.json";
        private static string _fileBCCP = Environment.CurrentDirectory + "\\Data\\bccp.txt";
        static string auth = "Hw5ESVqVaYfqde21DIHqs4EGhYcqGIiEF4GROViU";
        static string maBuuCuc = "";
    }
}