using System;

namespace ReniBot.Entities
{
    public class BotUserRequest
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string rawInput { get; set; }
        public DateTimeOffset startedOn { get; set; }
        //public int? resultId { get; set; }
    }
}
