using ReniBot.AimlEngine.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReniBot.AimlEngine
{
    public class BotContext
    {
        public IBot Bot { get; set; }
        public User User { get; set; }
        public BotConfiguration Configuration { get; set; }
        public Request Request { get; set; }
        public SubQuery query { get; set; }
    }
}
