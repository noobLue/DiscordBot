using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotAndroid
{
    public class Config
    {
        public string Token { get; set; }
        public string Appid { get; set; }
        public string AppSecret { get; set; }
        public ulong Server { get; set; }
        public ulong Channel { get; set; }
        public string CommandPrefix { get; set; }
    }
}
