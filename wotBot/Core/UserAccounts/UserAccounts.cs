﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace wotbBot.Core.UserAccounts
{
    public static class UserAccounts
    {
        private static List<UserAccount> accounts;

        private static string accountsFile = "Resources/account.json";
        
        static UserAccounts()
        {
            if(DataStorage.SaveExists(accountsFile))
            {
                accounts = DataStorage.LoadUserAccounts(accountsFile).ToList();
            }
            else
            {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts()
        {
            DataStorage.SaveUserAccounts(accounts, accountsFile);
        }

        public static UserAccount GetAccount(SocketUser user)
        {
            return GetOrCreateAccount(user.Id);
        }

        private static UserAccount GetOrCreateAccount(ulong id)
        {
            var result = from a in accounts
                where a.UserId == id
                select a;

            var account = result.FirstOrDefault();
            if (account == null) account = CreateUserAccount(id);
            return account;
        }

        private static UserAccount CreateUserAccount(ulong id)
        {
            var newAccount = new UserAccount()
            {
                UserId = id,
                Points = 10,
                EP = 0
            };

            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}
