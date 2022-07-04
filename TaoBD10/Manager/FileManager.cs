using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TaoBD10.Model;

namespace TaoBD10.Manager
{
    public static class FileManager
    {
        public static List<BD10InfoModel> list = new List<BD10InfoModel>();
        public static List<string> listBuuCuc = new List<string>();

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