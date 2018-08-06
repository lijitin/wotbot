using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wotbBot.Core.UserAccounts
{
    public class UserAccount //additional data to be stored
    {
        public ulong UserId { get; set; }

        public uint Points { get; set; } // unsigned int

        public uint EP { get; set; }
    }
}
