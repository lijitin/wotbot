using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wotbBot
{
    internal static class Global
    {
        internal static DiscordSocketClient Client { get; set; }
        internal static List<List<ulong>> MessageIdToTrack { get; set; }
        static Global() => MessageIdToTrack = new List<List<ulong>>()
            {
                new List<ulong>(),
                new List<ulong>(),
                new List<ulong>()
            };

    }
}
