using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace DiscordBotAndroid
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private Config _config;
        private Dictionary<String, String> _messageDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("messages.json"));
        

        public async Task MainAsync()
        {
            _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));

            _client = new DiscordSocketClient();

            _client.Log += Log;
            _client.MessageReceived += MessageReceived;

            await _client.LoginAsync(TokenType.Bot, _config.Token);
            await _client.StartAsync();


            // Block this task until the program is closed.
            await Task.Delay(-1);

            await _client.LogoutAsync();
        }

        

        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Author.IsBot) return;

            if (message.Content.StartsWith(_config.CommandPrefix))
            {
                string key = message.Content.Substring(1);
                if (_messageDict.ContainsKey(key))
                {
                    GetDefaultChannel()?.SendMessageAsync(_messageDict[key]);
                }

                if (message.Content.Equals("!porn", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (WebClient wc = new WebClient())
                    {
                        Uri url = new Uri(@"https://i.imgur.com/yE16HVd.png");
                        byte[] originalData = wc.DownloadData(url);
                        MemoryStream stream = new MemoryStream(originalData);

                        await GetDefaultChannel()?.SendFileAsync(stream, Path.GetFileName(url.LocalPath), ";)");
                    }
                }

                if (message.Content.StartsWith("!add"))
                {
                    string[] arr = message.Content.Split(" \"");
                    if (arr.Length == 3)
                    {
                        _messageDict[arr[1].Trim('!').Trim('\"')] = arr[2].Trim('\"');
                    }
                    //Serialize to json?
                }

                if (message.Content.StartsWith("!repeat"))
                {
                    await GetDefaultChannel().SendMessageAsync(message.Author.Username + " says: " + message.Content.Substring(message.Content.IndexOf(' ')+1));
                }

                if (message.Content.StartsWith("!remindme "))
                {
                    int delay = Int32.Parse(message.Content.Split(' ')[1]);
                    Timer(delay, "Alarm ringing after "+delay+ " seconds");
                }

                if (message.Content.StartsWith("!msg", StringComparison.InvariantCultureIgnoreCase))
                {
                    String s = "```\n";
                    foreach (string k in _messageDict.Keys)
                    {
                        s += "Command: !" + k + ",      message: "+_messageDict[k]+"\n";
                    }
                    s += "```";
                    await GetDefaultChannel().SendMessageAsync(s);
                }

            }
        }

        private async void Timer(int delay, string msg)
        {
            await Task.Delay(delay * 1000);
            await GetDefaultChannel().SendMessageAsync(msg);
        }

        private SocketTextChannel GetDefaultChannel()
        {
            return (_client.GetChannel(_config.Channel) as SocketTextChannel);
        }


        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
