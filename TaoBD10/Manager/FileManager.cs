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

        private static string _file = Environment.CurrentDirectory + "\\data.json";

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