using System;
using System.Collections.Generic;
using System.Text;
using ReniBot.Service;
using ReniBot.AimlEngine.Utils;

namespace ReniBot.AimlEngine
{
    /// <summary>
    /// Encapsulates information and history of a user who has interacted with the bot
    /// </summary>
    public class User
    {
        #region Attributes

        /// <summary>
        /// The local instance of the GUID that identifies this user to the bot
        /// </summary>
        private string _userKey;
        private int _appId;
        private int _userId;

        /// <summary>
        /// The GUID that identifies this user to the bot
        /// </summary>
        public string UserKey
        {
            get{return this._userKey;}
        }

        public int UserId
        {
            get { return _userId; }
        }

        /// <summary>
        /// A collection of all the result objects returned to the user in this session
        /// </summary>
        private UserResultService Results;

		/// <summary>
		/// the value of the "topic" predicate
		/// </summary>
        public string Topic
        {
            get
            {
                return Predicates.grabSetting("topic");
            }
        }

		/// <summary>
		/// the predicates associated with this particular user
		/// </summary>
        public UserPredicateService Predicates;

        /// <summary>
        /// The most recent result to be returned by the bot
        /// </summary>
        public Result LastResult
        {
            get
            {
                return BotConverter.Convert(Results.GetLast());
            }
        }

		#endregion
		
		#region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="userKey">The GUID of the user</param>
        /// <param name="bot">the bot the user is connected to</param>
		public User(int appId, string userKey)
		{
            if (!string.IsNullOrEmpty(userKey))
            {
                _userKey = userKey;
                _appId = appId;
                _userId = BotUserService.GetUserId(appId, userKey);
                if (_userId < 1)
                    _userId = BotUserService.AddUser(appId, userKey);
                Predicates = new UserPredicateService(_userId);
                Results = new UserResultService(_userId);
                //this.bot.DefaultPredicates.Clone(this.Predicates);
                this.Predicates.addSetting("topic", "*");
            }
            else
            {
                throw new Exception("The UserKey cannot be empty");
            }
		}

        public User(int userId)
        {
                _userKey = string.Empty;
                _appId = 0;
                _userId = userId;
                Predicates = new UserPredicateService(_userId);
                Results = new UserResultService(_userId);
                //this.bot.DefaultPredicates.Clone(this.Predicates);
                this.Predicates.addSetting("topic", "*");
        }


        /// <summary>
        /// Returns the string to use for the next that part of a subsequent path
        /// </summary>
        /// <returns>the string to use for that</returns>
        public string getLastBotOutput()
        {
            if (this.Results.Count() > 0)
            {
                return (BotConverter.Convert(Results.GetLast())).RawOutput;
            }
            else
            {
                return "*";
            }
        }

        /// <summary>
        /// Returns the first sentence of the last output from the bot
        /// </summary>
        /// <returns>the first sentence of the last output from the bot</returns>
        public string getThat()
        {
            return this.getThat(0,0);
        }

        /// <summary>
        /// Returns the first sentence of the output "n" steps ago from the bot
        /// </summary>
        /// <param name="n">the number of steps back to go</param>
        /// <returns>the first sentence of the output "n" steps ago from the bot</returns>
        public string getThat(int n)
        {
            return this.getThat(n, 0);
        }

        /// <summary>
        /// Returns the sentence numbered by "sentence" of the output "n" steps ago from the bot
        /// </summary>
        /// <param name="n">the number of steps back to go</param>
        /// <param name="sentence">the sentence number to get</param>
        /// <returns>the sentence numbered by "sentence" of the output "n" steps ago from the bot</returns>
        public string getThat(int n, int sentence)
        {
            if ((n >= 0) & (n < this.Results.Count()))
            {
                Result historicResult = BotConverter.Convert(Results.GetNResult(n));
                if ((sentence >= 0) & (sentence < historicResult.OutputSentences.Count))
                {
                    return (string)historicResult.OutputSentences[sentence];
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Returns the first sentence of the last output from the bot
        /// </summary>
        /// <returns>the first sentence of the last output from the bot</returns>
        public string getResultSentence()
        {
            return this.getResultSentence(0, 0);
        }

        /// <summary>
        /// Returns the first sentence from the output from the bot "n" steps ago
        /// </summary>
        /// <param name="n">the number of steps back to go</param>
        /// <returns>the first sentence from the output from the bot "n" steps ago</returns>
        public string getResultSentence(int n)
        {
            return this.getResultSentence(n, 0);
        }

        /// <summary>
        /// Returns the identified sentence number from the output from the bot "n" steps ago
        /// </summary>
        /// <param name="n">the number of steps back to go</param>
        /// <param name="sentence">the sentence number to return</param>
        /// <returns>the identified sentence number from the output from the bot "n" steps ago</returns>
        public string getResultSentence(int n, int sentence)
        {
            if ((n >= 0) & (n < this.Results.Count()))
            {
                Result historicResult = BotConverter.Convert(this.Results.GetNResult(n));
                if ((sentence >= 0) & (sentence < historicResult.InputSentences.Count))
                {
                    return (string)historicResult.InputSentences[sentence];
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Adds the latest result from the bot to the Results collection
        /// </summary>
        /// <param name="latestResult">the latest result from the bot</param>
        public void addResult(Result latestResult)
        {
            this.Results.Add(latestResult.Duration.Milliseconds, latestResult.HasTimedOut, latestResult.RawOutput,
                latestResult.requestId, latestResult.UserId);
        }
        #endregion
    }
}