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
        public static List<TinhHuyenModel> TinhThanhs;
        public static List<TuiThuModel> TuiThus;

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

        public static OptionModel GetOptionAll()
        {
            Task<OptionModel> optionTemp = client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/Option").OnceSingleAsync<OptionModel>();
            optionTemp.Wait();
            optionModel = optionTemp.Result;
            return optionTemp.Result;
        }

        //public static List<TinhHuyenModel> LayDSTinh()
        //{
        //    if (!File.Exists(_fileTinhThanh))
        //    {
        //        return null;
        //    }
        //    string textFile = File.ReadAllText(_fileTinhThanh);

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
                optionModel = option;
                return option;
            }
        }

        public static List<string> LoadBuuCucsOffline()
        {
            if (!File.Exists(_fileBuuCucs))
            {
                SaveBuuCucsOffline(new List<string>());
            }

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(_fileBuuCucs))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                List<string> listBuuCucs = serializer.Deserialize<List<string>>(jReader);
                return listBuuCucs;
            }
        }

        public static List<FindItemModel> LoadFindItemOffline()
        {
            if (!File.Exists(_fileFindItem))
            {
                SaveFindItemOffline(new List<FindItemModel>());
            }

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(_fileFindItem))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                List<FindItemModel> listFindItems = serializer.Deserialize<List<FindItemModel>>(jReader);
                return listFindItems;
            }
        }

        public static void SaveFindItemOffline(List<FindItemModel> findItemModels)
        {
            if (!File.Exists(_fileFindItem))
            {
                using (FileStream fs = File.Create(_fileFindItem))
                {
                }
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_fileFindItem))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, findItemModels);
            }
        }

        public static string MQTTKEY;

        public static string LoadKeyMqtt()
        {
            if (!File.Exists(_fileKeyMqtt))
            {
                SaveKeyMqtt("");
            }

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(_fileKeyMqtt))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                string listFindItems = serializer.Deserialize<string>(jReader);
                MQTTKEY = listFindItems;
                return listFindItems;
            }
        }

        public static void SaveKeyMqtt(string findItemModels)
        {
            if (!File.Exists(_fileKeyMqtt))
            {
                using (FileStream fs = File.Create(_fileKeyMqtt))
                {
                }
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_fileKeyMqtt))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, findItemModels);
            }
        }

        public static void SaveBuuCucsOffline(List<string> list)
        {
            if (!File.Exists(_fileBuuCucs))
            {
                using (FileStream fs = File.Create(_fileBuuCucs))
                {
                }
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_fileBuuCucs))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, list);
            }
        }

        public static List<BD10InfoModel> LoadBD10Offline()
        {
            if (!File.Exists(_fileBD10))
            {
                SaveBD10Offline(new BD10InfoModel());
            }

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(_fileBD10))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                List<BD10InfoModel> listBD10 = serializer.Deserialize<List<BD10InfoModel>>(jReader);
                list = listBD10;
                return listBD10;
            }
        }

        public static List<LocBDInfoModel> LoadLocBD10sOffline()
        {
            if (!File.Exists(_fileLocBD10))
            {
                SaveLocBD10Offline(new List<LocBDInfoModel>());
            }

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(_fileLocBD10))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                List<LocBDInfoModel> locBD10s = serializer.Deserialize<List<LocBDInfoModel>>(jReader);
                return locBD10s;
            }
        }

        public static List<LocBDInfoModel> LoadLocKTBCPOffline()
        {
            if (!File.Exists(_fileLocKTBCP))
            {
                var list = new List<LocBDInfoModel>();
                list.Add(new LocBDInfoModel("59000"));
                list.Add(new LocBDInfoModel());
                SaveLocKTBCPOffline(list);
            }

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(_fileLocKTBCP))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                List<LocBDInfoModel> locBD10s = serializer.Deserialize<List<LocBDInfoModel>>(jReader);
                return locBD10s;
            }
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

        public static void SaveTinhThanhOffline(List<TinhHuyenModel> list)
        {
            if (!File.Exists(_fileTinhThanh))
            {
                using (FileStream fs = File.Create(_fileTinhThanh))
                {
                }
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_fileTinhThanh))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, list);
            }
        }

        public static List<TinhHuyenModel> LoadTinhThanhFromFile(string path)
        {
            IEnumerable<string> tinhThanhs = File.ReadLines(path);
            var list = new List<TinhHuyenModel>();
            foreach (string tinh in tinhThanhs)
            {
                if (string.IsNullOrEmpty(tinh))
                    continue;

                string[] splitText = tinh.Split('\t');
                list.Add(new TinhHuyenModel(splitText[1].Trim(), splitText[2].Trim()));
            }
            return list;
        }

        public static List<string> LoadBuuCucsFromFile(string path)
        {
            listBuuCuc = new List<string>();
            string text = File.ReadAllText(path);
            var texts = text.Split('\n');
            for (int i = 0; i < texts.Length; i++)
            {
                if (!string.IsNullOrEmpty(texts[i]))
                {
                    listBuuCuc.Add(texts[i].Trim());
                }
            }
            return listBuuCuc;
        }

        public static List<TinhHuyenModel> LoadTinhThanhOffline()
        {
            if (!File.Exists(_fileTinhThanh))
            {
                SaveTinhThanhOffline(new List<TinhHuyenModel>());
            }

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(_fileTinhThanh))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                List<TinhHuyenModel> listTinhThanh = serializer.Deserialize<List<TinhHuyenModel>>(jReader);
                return listTinhThanh;
            }
        }

        public static List<TinhHuyenModel> LoadTinhThanhOnFirebase()
        {
            Task<List<TinhHuyenModel>> cts = client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/TinhThanh").OrderByKey().OnceSingleAsync<List<TinhHuyenModel>>();
            cts.Wait();
            List<TinhHuyenModel> result = cts.Result;
            SaveTinhThanhOffline(result);
            return result;
        }

        public static List<string> LoadBuuCucsOnFirebase()
        {
            Task<List<string>> cts = client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/BuuCucs").OrderByKey().OnceSingleAsync<List<string>>();
            cts.Wait();
            List<string> result = cts.Result;
            SaveBuuCucsOffline(result);
            return result;
        }

        public static List<FindItemModel> LoadFindItemOnFirebase()
        {
            Task<List<FindItemModel>> cts = client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/FindItem").OrderByKey().OnceSingleAsync<List<FindItemModel>>();
            cts.Wait();
            List<FindItemModel> result = cts.Result;
            SaveFindItemOffline(result);
            return result;
        }

        public static List<BuuCucModel> LoadBuuCucOnFirebase()
        {
            Task<List<BuuCucModel>> cts = client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/LayBuuCuc").OrderByKey().OnceSingleAsync<List<BuuCucModel>>();
            cts.Wait();
            List<BuuCucModel> result = cts.Result;
            SaveBuuCucOffline(result);
            return result;
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
                listChuyenThu = listCT;
                return listCT;
            }
        }

        public static string[] ReadPrinterFromFile()
        {
            string[] result = new string[2];
            if (File.Exists("Data\\printerSave.txt"))
            {
                result = File.ReadAllLines("Data\\printerSave.txt");
                APIManager.namePrinterBD8 = result[0];
                APIManager.namePrinterBD10 = result[1];
            }
            return result;
        }

        public static List<ChuyenThuModel> LoadCTOnFirebase()
        {
            Task<List<ChuyenThuModel>> cts = client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/OptionCT").OrderByKey().OnceSingleAsync<List<ChuyenThuModel>>();
            cts.Wait();
            List<ChuyenThuModel> result = cts.Result;
            SaveCTOffline(result);
            return result;
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

        public static List<LayBD10Info> LoadLayBDOnFirebase()
        {
            Task<List<LayBD10Info>> cts = client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/LayBD10").OrderByKey().OnceSingleAsync<List<LayBD10Info>>();
            cts.Wait();
            List<LayBD10Info> result = cts.Result;
            SaveLayBDOffline(result);
            return result;
        }

        public static List<LocBDInfoModel> LoadLocBDOnFirebase()
        {
            Task<List<LocBDInfoModel>> cts = client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/OptionLocBD").OrderByKey().OnceSingleAsync<List<LocBDInfoModel>>();
            cts.Wait();
            List<LocBDInfoModel> result = cts.Result;
            SaveLocBD10Offline(result);
            return result;
        }

        public static void SaveLocBD10Firebase(List<LocBDInfoModel> locBDInfoModels)
        {
            if (locBDInfoModels.Count == 0)
                return;
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_fileLocBD10))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, locBDInfoModels);
            }
            //client.SetTaskAsync("QuanLy/593230",chuyenThus);
            client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/OptionLocBD").PutAsync(locBDInfoModels);
        }

        public static void OnSetupFileManager()
        {
            DirectoryInfo DataFolder = new DirectoryInfo(Environment.CurrentDirectory + "\\Data");
            if (!DataFolder.Exists)
            {
                DataFolder.Create();
            }
            GetOptionOffline();
            ReadPrinterFromFile();
            TinhThanhs = LoadTinhThanhOffline();
            TuiThus = LoadTuiThuOffline();
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

        public static void SaveBD10Offline(BD10InfoModel bd10)
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
                if (bd10 != null)
                    list.Add(bd10);
                serializer.Serialize(jWriter, list);
            }
        }

        public static void SaveBuuCuc(List<BuuCucModel> buuCucModels)
        {
            client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/LayBuuCuc").PutAsync(buuCucModels).Wait();
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

        public static void SaveLocBD10Offline(List<LocBDInfoModel> locBD10s)
        {
            if (!File.Exists(_fileLocBD10))
            {
                using (FileStream fs = File.Create(_fileLocBD10))
                {
                }
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_fileLocBD10))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, locBD10s);
            }
        }

        public static void SaveLocKTBCPOffline(List<LocBDInfoModel> locBD10s)
        {
            if (!File.Exists(_fileLocKTBCP))
            {
                using (FileStream fs = File.Create(_fileLocKTBCP))
                {
                }
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_fileLocKTBCP))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, locBD10s);
            }
        }

        public static void SaveCTOffline(List<ChuyenThuModel> chuyenThus)
        {
            if (!Directory.Exists("Data"))
            {
                _ = Directory.CreateDirectory("Data");
            }
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
        public static void SaveErrorOnFirebase(string errorText)
        {
            string error = DateTime.Now.ToString() + ":" + errorText;
            //client.SetTaskAsync("QuanLy/593230",chuyenThus);
            client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/ErrorList").PostAsync(error);
        }

        public static List<string> LoadErrorOnFirebase()
        {
            Task<List<string>> cts = client.Child(@"QuanLy//" + optionModel.MaKhaiThac + "/ErrorList").OrderByKey().OnceSingleAsync<List<string>>();
            cts.Wait();
            List<string> result = cts.Result;
            return result;
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
            client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/OptionCT").PutAsync(chuyenThus);
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

        public static void SaveLayBDFirebase(List<LayBD10Info> layBDs)
        {
            client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/LayBD10").PutAsync(layBDs).Wait();
        }

        public static void SaveLayTinhThanhFirebase(List<TinhHuyenModel> tinhThanhs)
        {
            client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/TinhThanh").PutAsync(tinhThanhs).Wait();
        }

        public static void SaveBuuCucsFirebase(List<string> buucucs)
        {
            client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/BuuCucs").PutAsync(buucucs).Wait();
        }

        public static void SaveFindItemFirebase(List<FindItemModel> buucucs)
        {
            client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/FindItem").PutAsync(buucucs).Wait();
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

        public static void SaveOptionAll(OptionModel option)
        {
            client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/Option").PutAsync(option).Wait();
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
                optionModel = option;
                serializer.Serialize(jWriter, option);
            }
        }

        private static async void test()
        {
            IReadOnlyCollection<FirebaseObject<ChuyenThuModel>> ss = await client.Child(@"QuanLy").OrderByKey().LimitToFirst(2).OnceAsync<ChuyenThuModel>();
            foreach (var item in ss)
            {
                var sss = item.Key;
            }
        }

        public static List<TuiThuModel> LoadTuiThuOnFirebase()
        {
            Task<List<TuiThuModel>> cts = client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/TuiThu").OrderByKey().OnceSingleAsync<List<TuiThuModel>>();
            cts.Wait();
            List<TuiThuModel> result = cts.Result;
            SaveTuiThuOffline(result);
            return result;
        }

        public static List<TuiThuModel> LoadTuiThuFromFile(string filename)
        {
            IEnumerable<string> tuiThus = File.ReadLines(filename);
            var list = new List<TuiThuModel>();
            foreach (string tinh in tuiThus)
            {
                if (string.IsNullOrEmpty(tinh))
                    continue;

                string[] splitText = tinh.Split('\t');
                list.Add(new TuiThuModel(splitText[1].Trim(), splitText[2].Trim()));
            }
            TuiThus = list;
            return list;
        }

        public static void SaveTuiThuOffline(List<TuiThuModel> tuiThuModels)
        {
            if (!File.Exists(_fileTuiThu))
            {
                using (FileStream fs = File.Create(_fileTuiThu))
                {
                }
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_fileTuiThu))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, tuiThuModels);
            }
        }

        public static List<TuiThuModel> LoadTuiThuOffline()
        {
            if (!File.Exists(_fileTuiThu))
            {
                SaveTuiThuOffline(new List<TuiThuModel>());
            }

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(_fileTuiThu))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                List<TuiThuModel> listTuiThu = serializer.Deserialize<List<TuiThuModel>>(jReader);
                TuiThus = listTuiThu;
                return listTuiThu;
            }
        }

        public static void SaveTuiThuFirebase(List<TuiThuModel> tuiThuModels)
        {
            client.Child(@"QuanLy/DanhSach/" + optionModel.MaKhaiThac + "/TuiThu").PutAsync(tuiThuModels).Wait();
        }

        public static List<string> LoadBuuCucTuDongsOffline()
        {
            if (!File.Exists(_fileBuuCucTuDongs))
            {
                SaveBuuCucTuDongsOffline(new List<string>());
            }

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(_fileBuuCucTuDongs))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                List<string> listBuuCucTuDongs = serializer.Deserialize<List<string>>(jReader);
                return listBuuCucTuDongs;
            }
        }

        public static void SaveBuuCucTuDongsOffline(List<string> list)
        {
            if (!File.Exists(_fileBuuCucTuDongs))
            {
                using (FileStream fs = File.Create(_fileBuuCucTuDongs))
                {
                }
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sWriter = new StreamWriter(_fileBuuCucTuDongs))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                serializer.Serialize(jWriter, list);
            }
        }

        public static FirebaseClient client;
        public static List<BD10InfoModel> list = new List<BD10InfoModel>();
        public static List<string> listBuuCuc = new List<string>();
        public static List<ChuyenThuModel> listChuyenThu = new List<ChuyenThuModel>();
        public static OptionModel optionModel = new OptionModel();
        private static string _fileBCCP = Environment.CurrentDirectory + "\\Data\\bccp.txt";
        private static string _fileBD10 = Environment.CurrentDirectory + "\\Data\\bd10.json";
        private static string _fileBD8 = Environment.CurrentDirectory + "\\Data\\matuithu.txt";
        private static string _fileBuuCuc = Environment.CurrentDirectory + "\\Data\\dataBuuCuc.json";
        private static string _fileCT = Environment.CurrentDirectory + "\\Data\\dataCT.json";
        private static string _fileLayBD = Environment.CurrentDirectory + "\\Data\\dataLayBD.json";
        private static string _fileOption = Environment.CurrentDirectory + "\\Data\\option.json";
        private static string _fileFindItem = Environment.CurrentDirectory + "\\Data\\finditem.json";
        private static string _fileTinhThanh = Environment.CurrentDirectory + "\\Data\\TinhThanh.json";
        private static string _fileTuiThu = Environment.CurrentDirectory + "\\Data\\TuiThu.json";
        private static string _fileBuuCucs = Environment.CurrentDirectory + "\\Data\\BuuCucs.json";
        private static string _fileKeyMqtt = Environment.CurrentDirectory + "\\Data\\KeyMqtt.json";
        private static string _fileBuuCucTuDongs = Environment.CurrentDirectory + "\\Data\\BuuCucTuDongs.json";
        private static string _fileLocBD10 = Environment.CurrentDirectory + "\\Data\\LocBD10.txt";
        private static string _fileLocKTBCP = Environment.CurrentDirectory + "\\Data\\LocKTBCP.txt";
        private static string auth = "Hw5ESVqVaYfqde21DIHqs4EGhYcqGIiEF4GROViU";
        private static string maBuuCuc = "";
    }
}