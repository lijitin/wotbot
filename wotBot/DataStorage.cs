using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace wotbBot
{
    class DataStorage
    {
        // Load, save and change some data from command input
        public static Dictionary<string, string> pairs = new Dictionary<string, string>();

        internal const string path = "DataStorage.json";

        public static void AddPairToStorage(string key, string value)
        {
            pairs.Add(key, value);
            SaveData();
        }

        public static int GetPairCount()
        {
            return pairs.Count();
        }

        static DataStorage() //constructor for DataStorage class
        {
            // Load data
            if(!ValidateStorageFile(path)) return;
            string json = File.ReadAllText(path);
            pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static void SaveData()
        {
            // Save data
            string json = JsonConvert.SerializeObject(pairs, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        private static bool ValidateStorageFile(string file)
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, "");
                SaveData();
                return false;
            }
            return true;
        }
    }
}
