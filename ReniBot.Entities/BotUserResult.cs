using System;

namespace ReniBot.Entities
{
    public class BotUserResult
    {
        public int id { get; set; }
        public int userId { get; set; }
        public int Duration { get; set; }
        public string rawOutput { get; set; }
        public bool hasTimedOut { get; set; }
        public int requestId { get; set; }
        public DateTimeOffset timeStamp { get; set; }


    }
}
