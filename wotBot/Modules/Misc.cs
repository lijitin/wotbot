using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NReco.ImageGenerator;
using wotbBot.Core.UserAccounts;
using System.Net;
using Newtonsoft.Json;
using System.Timers;
using Discord.Rest;

namespace wotbBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("eggplant")]
        public async Task Eggplant()
        {
            RestUserMessage msg = await Context.Channel.SendMessageAsync(":eggplant:");
            Global.MessageIdToTrack.First().Add(msg.Id); //  first elem
        }
        private static Timer cdtimer;
        private int total;
        [Command("countdown")]
        public async Task Countdown()
        {
            cdtimer = new Timer()
            {
                Interval = 1000,
                AutoReset = true,
                Enabled = true
            };
            total = 3;
            cdtimer.Elapsed += Cdtimer_Elapsed;   
        }
        
        private async void Cdtimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(total < 0)
            {
                cdtimer.Close();
                return;
            }
            else
            {
                await Context.Channel.SendMessageAsync(Context.User.Mention + $" `{total}!`");
                total -= 1;
            }
            
        }

        [Command("rand_person")]
        public async Task GetRandomPerson()
        {
            string json = "";
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString("https://randomuser.me/api/?gender=female");
            }
            var dataObject = JsonConvert.DeserializeObject<dynamic>(json);

            string gender = dataObject.results[0].gender.ToString();
            string firstName = dataObject.results[0].name.first.ToString();
            string lastName = dataObject.results[0].name.last.ToString();
            string avatarURL = dataObject.results[0].picture.large.ToString();

            var embed = new EmbedBuilder();
            embed.WithThumbnailUrl(avatarURL);
            embed.WithTitle("Generated Person");
            embed.AddInlineField("First Name", firstName.First().ToString().ToUpper() + firstName.Substring(1));
            embed.AddInlineField("Last Name", lastName.First().ToString().ToUpper() + lastName.Substring(1));
            
            await Context.Channel.SendMessageAsync("", embed: embed);
        }

        [Command("echo")]
        public async Task Echo([Remainder]string message)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle(Utilities.GetFormattedAlert("ECHO_&NAME", Context.User.Username));
            embed.WithDescription(message);
            embed.WithColor(new Color(178, 102, 255));

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("pick")]
        public async Task PickOne([Remainder]string message)
        {
            string[] options = message.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            Random r = new Random();
            string selection = options[r.Next(0, options.Length)];
            if (options.Length <= 1)
            {
                await Context.Channel.SendMessageAsync("Give me more options, you dummy!");
                return;
            }

            var embed = new EmbedBuilder();
            embed.WithTitle("Choice for " + Context.User.Username);
            embed.WithDescription(selection);
            embed.WithColor(new Color(178, 102, 255));
            embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("secret")]
        public async Task secret()
        {
            if (!UserIsSecretOwner((SocketGuildUser)Context.User))
            {
                await Context.Channel.SendMessageAsync(":x:You need more Faps for that. :eggplant::sweat_drops:" + Context.User.Mention);
                return;
            }
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync(Utilities.GetFormattedAlert("SECRET"));
        }

        private static bool UserIsSecretOwner(SocketGuildUser user)
        {
            string targetRoleName = "Fapfapfap";
            var result = from r in user.Guild.Roles     //linq, .net query call similar to sql
                            where r.Name == targetRoleName
                            select r.Id;
            ulong roleID = result.FirstOrDefault();
            if (roleID == 0) return false;
            var targetRole = user.Guild.GetRole(roleID);
            return user.Roles.Contains(targetRole);
        }

        [Command("data")]
        public async Task GetData()
        {
            await Context.Channel.SendMessageAsync("Data Has " + DataStorage.GetPairCount() + " pairs.");
            DataStorage.AddPairToStorage("Count" + DataStorage.GetPairCount(), "TheCount" + DataStorage.GetPairCount());
        }

        [Command(".stats")]
        public async Task _Stats([Remainder]string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = mentionedUser ?? Context.User;
            
            var account = UserAccounts.GetAccount(target);
            var guildUser = (SocketGuildUser)target;
            await Context.Channel.SendMessageAsync($"{guildUser.Nickname} has {account.EP} naughty points. (˵ ͡≖ ͜ʖ ͡≖˵)♡");
        }
        [Command("stats")]
        public async Task Stats([Remainder]string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = mentionedUser ?? Context.User;

            var account = UserAccounts.GetAccount(target);
            var guildUser = (SocketGuildUser)target;
            await Context.Channel.SendMessageAsync($"{guildUser.Nickname} has {account.Points} goodboy points.");
        }

        [Command("addPts")]
        public async Task addPts(uint pts, [Remainder]string args)
        {
            if (UserIsSecretOwner((SocketGuildUser)Context.User))
            {
                SocketUser target = null;
                var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
                target = mentionedUser ?? Context.User;
                var account = UserAccounts.GetAccount(target);

                uint amount = args.LastOrDefault();
                account.Points += pts;
                UserAccounts.SaveAccounts();
                await Context.Channel.SendMessageAsync($"{target.Username} gained {pts} goodboy points. ({account.Points})");
                //send msg to memes
                // var channel = (ISocketMessageChannel)Context.Guild.GetChannel(399544374624780288);
                // await channel.SendMessageAsync($"{target.Username} gained {pts} goodboy points. ({account.Points})");
            }
            else
            {
                await Smug();
            }
        }

        [Command("hello")]
        public async Task Hello(string color = "red")
        {
            string css = "<style>\n    h1{\n        color: " + color + ";\n    }\n</style>";
            string html = "\n<h1>woot</h1>";
            var converter = new HtmlToImageConverter
            {
                Width = 80,
                Height = 50
            };
            var jpgBytes = converter.GenerateImage(css + html, NReco.ImageGenerator.ImageFormat.Jpeg);
            await Context.Channel.SendFileAsync(new System.IO.MemoryStream(jpgBytes), "woot.jpg");
        }

        
        // -------------------------------------------------------------------------------
        const string dir = "Resources/emotes/";
        [Command("judge")]
        public async Task Judge()
        {
            await Context.Channel.SendFileAsync(dir + "judge.png");
            await Context.Channel.SendMessageAsync("GOD IS WATCHING");
        }
        [Command("coolmeme")]
        public async Task Coolmeme()
        {
            await Context.Channel.SendFileAsync(dir + "coolmeme.png");
        }
        [Command("succ")]
        public async Task Succ([Remainder]string args = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            if (mentionedUser == null)
            {
                await Context.Channel.SendFileAsync(dir + "succ.png");
                return;
            }
            target = mentionedUser ?? Context.User;
            string avatarUrl = target.GetAvatarUrl();
            var guildUser = (SocketGuildUser)target;

            string css = "<style>\n    #avatar{\n        position: absolute;\n        top: 377px;\n        left: 175px;\n        z-index: 1;\n        width: 65px;\n        height: 65px;\n    }\n    body {\n        margin: 0;\n    }\n</style>";
            string imgpath = "https://cdn.discordapp.com/attachments/464773560393662465/475988185915195402/succ_edit.png";
            string body = "\n<img id=\"back\" src=\""+ imgpath + "\">\n";
            string avatarhtml = "<img id=\"avatar\" src = \"" + avatarUrl + "\">";
            string fullhtml = css + body + avatarhtml;
            var converter = new HtmlToImageConverter
            {
                Width = 375,
                Height = 535
            };
            
            var jpgBytes = converter.GenerateImage(fullhtml, NReco.ImageGenerator.ImageFormat.Jpeg);
            await Context.Channel.SendFileAsync(new System.IO.MemoryStream(jpgBytes), "succd.jpg");

        }
        [Command("memequeen")]
        public async Task Memequeen()
        {
            string link = "https://nhentai.net/g/191390/";
            var embed = new EmbedBuilder();
            embed.WithColor(new Color(255, 223, 0));
            embed.WithTitle("Legend of The Meme Queen");
            embed.WithUrl(link);
            embed.WithImageUrl("https://cdn.discordapp.com/attachments/464773560393662465/466532499569508362/eggplant.PNG");
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        // -----------------------------------------------------------------------
        private async Task Smug()
        {
            var embed = new EmbedBuilder();
            embed.WithDescription("__***Nice try, my guy.***__");
            embed.WithThumbnailUrl("https://media.discordapp.net/attachments/464773560393662465/468497390266220544/wee.PNG");
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        // -----------------------------------------------------------------------
        [Command("debug")]
        public async Task Debug()
        {
            //await Smug();
            var converter = new HtmlToImageConverter
            {
                Width = 375,
                Height = 535
            };
            var jpgBytes = converter.GenerateImageFromFile("./Resources/html/succ_template.html", NReco.ImageGenerator.ImageFormat.Jpeg);
            await Context.Channel.SendFileAsync(new System.IO.MemoryStream(jpgBytes), "succd.jpg");
        }
    }
}
