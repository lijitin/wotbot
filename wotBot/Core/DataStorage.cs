using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wotbBot.Core.UserAccounts;
using Newtonsoft.Json;
using System.IO;

namespace wotbBot.Core
{
    public static class DataStorage
    {
        // Save all userAccounts
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        // Get all userAccounts
        //public static 
        public static IEnumerable<UserAccount> LoadUserAccounts(string filePath)
        {
            // check for file existence
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }

        public static bool SaveExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
