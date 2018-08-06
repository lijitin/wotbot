using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using wotbBot.Core;
using Discord.Rest;
using wotbBot.Core.UserAccounts;

namespace wotbBot
{
    class Program
    {
        DiscordSocketClient _client;
        CommandHandler _handler;

        static void Main(string[] args)
        => new Program().StartAsync().GetAwaiter().GetResult();
        //start a async program instead of normal program
        public async Task StartAsync()
        {
            if (Config.bot.token == "" || Config.bot.token == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            _client.Log += Log;
            _client.Ready += RepeatingTimer.StartTimer;
            _client.ReactionAdded += OnReactionAdded;
            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.StartAsync();
            Global.Client = _client;
            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
            await Task.Delay(-1); //await until all operation ends
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            //  capture all the naughty eggplants
            if(reaction.Emote.Name == "🍆")
            {
                var account = UserAccounts.GetAccount((SocketUser)reaction.User);
                account.EP += 1;
                UserAccounts.SaveAccounts();
            }
            if (Global.MessageIdToTrack[0].Contains(reaction.MessageId))
            {
                string[] checklist = { "👋" };
                if (checklist.Contains(reaction.Emote.Name))
                {
                    RestUserMessage msg = await channel.SendMessageAsync(":eggplant::sweat_drops:");
                    Global.MessageIdToTrack[1].Add(msg.Id);
                }
            }
            if (Global.MessageIdToTrack[1].Contains(reaction.MessageId))
            {
                string[] checklist = { "👋" };
                if (checklist.Contains(reaction.Emote.Name))
                {
                    RestUserMessage msg = await channel.SendMessageAsync(":eggplant::sweat_drops::sweat_drops:");
                    Global.MessageIdToTrack[2].Add(msg.Id);
                }
            }
            if (Global.MessageIdToTrack[2].Contains(reaction.MessageId))
            {
                string[] checklist = { "👋" };
                if (checklist.Contains(reaction.Emote.Name))
                {
                    await channel.SendFileAsync("Resources/emotes/" + "judge.png");
                    await channel.SendMessageAsync("GOD IS WATCHING");
                }
            }
        }

        private async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
        }
    }
}
