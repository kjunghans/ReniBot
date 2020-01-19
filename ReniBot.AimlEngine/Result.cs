using ReniBot.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReniBot.AimlEngine
{
    /// <summary>
    /// Encapsulates information about the result of a request to the bot
    /// </summary>
    public class Result
    {
        /// <summary>
        /// The request from the user
        /// </summary>
        public int requestId { get; set; }

        /// <summary>
        /// The raw input from the user
        /// </summary>
        public string RawInput { get; set; }

        

        /// <summary>
        /// The normalized sentence(s) (paths) fed into the graphmaster
        /// </summary>
        public List<string> NormalizedPaths = new List<string>();

        /// <summary>
        /// The amount of time the request took to process
        /// </summary>
        public TimeSpan Duration {get; set;}

        public bool HasTimedOut { get; set; }

        /// <summary>
        /// The result from the bot with logging and checking
        /// </summary>
        public string Output
        {
            get
            {
                if (OutputSentences.Count > 0)
                {
                    return this.RawOutput;
                }
                else
                {
                    if (request.hasTimedOut)
                    {
                        return "Timed out waiting for a response."; //TODO: Make this message configurable?
                    }
                    else
                    {
                        StringBuilder paths = new StringBuilder();
                        foreach (string pattern in this.NormalizedPaths)
                        {
                            paths.Append(pattern + Environment.NewLine);
                        }
                        //TODO: Need a more versital logging method that is not coupled with the bot.
                        //this.bot.writeToLog("The bot could not find any response for the input: " + this.RawInput + " with the path(s): " + Environment.NewLine + paths.ToString() + " from the user with an id: " + this.user.UserKey);
                        return string.Empty;
                    }
                }
            }
        }

        public Request request { get; set; }

        /// <summary>
        /// Returns the raw sentences without any logging 
        /// </summary>
        public string RawOutput
        {
            get
            {
                StringBuilder result = new StringBuilder();
                foreach (string sentence in OutputSentences)
                {
                    string sentenceForOutput = sentence.Trim();
                    if (!this.checkEndsAsSentence(sentenceForOutput))
                    {
                        sentenceForOutput += ".";
                    }
                    result.Append(sentenceForOutput + " ");
                }
                return result.ToString().Trim();
            }
        }

        /// <summary>
        /// The subQueries processed by the bot's graphmaster that contain the templates that 
        /// are to be converted into the collection of Sentences
        /// </summary>
        public List<Utils.SubQuery> SubQueries = new List<Utils.SubQuery>();

        /// <summary>
        /// The individual sentences produced by the bot that form the complete response
        /// </summary>
        public List<string> OutputSentences = new List<string>();

        /// <summary>
        /// The individual sentences that constitute the raw input from the user
        /// </summary>
        public List<string> InputSentences = new List<string>();

        private int _userId;
        private string[] Splitters = { ".", "!", "?", ";", ":" };

        public int UserId { get { return _userId; } }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="user">The user for whom this is a result</param>
        /// <param name="bot">The bot providing the result</param>
        /// <param name="request">The request that originated this result</param>
        public Result(Request request)
        {
            _userId = request.userId;
            this.requestId = request.id;
            this.RawInput = request.rawInput;
            this.request = request;
        }

        private void SetOutputSentences(string rawOutput)
        {
            OutputSentences = new List<string>();
            string[] sentences = rawOutput.Split(Splitters, StringSplitOptions.RemoveEmptyEntries);
            OutputSentences.AddRange(sentences);
        }

        public Result(Request request, string rawOutput, TimeSpan duration, bool hasTimedOut)
        {
            _userId = request.userId;
            this.requestId = request.id;
            this.RawInput = request.rawInput;
            this.request = request;
            SetOutputSentences(rawOutput);
            this.Duration = duration;
            this.HasTimedOut = hasTimedOut;
        }

        /// <summary>
        /// Returns the raw output from the bot
        /// </summary>
        /// <returns>The raw output from the bot</returns>
        public override string ToString()
        {
            return this.Output;
        }

        /// <summary>
        /// Checks that the provided sentence ends with a sentence splitter
        /// </summary>
        /// <param name="sentence">the sentence to check</param>
        /// <returns>True if ends with an appropriate sentence splitter</returns>
        private bool checkEndsAsSentence(string sentence)
        {
            foreach (string splitter in Splitters)
            {
                if (sentence.Trim().EndsWith(splitter))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
