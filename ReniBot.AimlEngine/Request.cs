using System;
using System.Collections.Generic;
using System.Text;

namespace ReniBot.AimlEngine
{
    /// <summary>
    /// Encapsulates all sorts of information about a request to the bot for processing
    /// </summary>
    public class Request
    {
        #region Attributes

        public int id { get; set; }
        /// <summary>
        /// The raw input from the user
        /// </summary>
        public string rawInput { get; set; }

        /// <summary>
        /// The time at which this request was created within the system
        /// </summary>
        public DateTimeOffset StartedOn { get; set; }

        /// <summary>
        /// The user who made this request
        /// </summary>
        public int userId {get; set;}


        /// <summary>
        /// The final result produced by this request
        /// </summary>
        public int? resultId { get; set; }

        /// <summary>
        /// Flag to show that the request has timed out
        /// </summary>
        public bool hasTimedOut { get; set; }

        #endregion


        public Request()
        {
            hasTimedOut = false;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="rawInput">The raw input from the user</param>
        /// <param name="user">The user who made the request</param>
        /// <param name="bot">The bot to which this is a request</param>
        public Request(string rawInput, int userId)
        {
            hasTimedOut = false;
            this.rawInput = rawInput;
            this.userId = userId;
            this.StartedOn = DateTimeOffset.UtcNow;
        }
    }
}
