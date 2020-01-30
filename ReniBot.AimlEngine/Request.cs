using System;

namespace ReniBot.AimlEngine
{
    /// <summary>
    /// Encapsulates all sorts of information about a request to the bot for processing
    /// </summary>
    public class Request
    {

        public int Id { get; set; }
        /// <summary>
        /// The raw input from the user
        /// </summary>
        public string RawInput { get; set; }

        /// <summary>
        /// The time at which this request was created within the system
        /// </summary>
        public DateTimeOffset StartedOn { get; set; }

        /// <summary>
        /// The user who made this request
        /// </summary>
        public int UserId { get; set; }


        /// <summary>
        /// The final result produced by this request
        /// </summary>
        public int? ResultId { get; set; }

        /// <summary>
        /// Flag to show that the request has timed out
        /// </summary>
        public bool HasTimedOut { get; set; }


        public Request()
        {
            HasTimedOut = false;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="rawInput">The raw input from the user</param>
        /// <param name="user">The user who made the request</param>
        /// <param name="bot">The bot to which this is a request</param>
        public Request(string rawInput, int userId)
        {
            HasTimedOut = false;
            RawInput = rawInput;
            UserId = userId;
            StartedOn = DateTimeOffset.UtcNow;
        }
    }
}
