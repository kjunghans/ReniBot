using System;
using System.Xml;
using System.Text;
using Microsoft.Extensions.Logging;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// The srai element instructs the AIML interpreter to pass the result of processing the contents 
    /// of the srai element to the AIML matching loop, as if the input had been produced by the user 
    /// (this includes stepping through the entire input normalization process). The srai element does 
    /// not have any attributes. It may contain any AIML template elements. 
    /// 
    /// As with all AIML elements, nested forms should be parsed from inside out, so embedded srais are 
    /// perfectly acceptable. 
    /// </summary>
    public class srai : ReniBot.AimlEngine.Utils.AIMLTagHandler
    {
        Bot _bot;
        User _user;
        Request _request;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public srai(ILogger logger,
                        Bot bot,
                        User user,
                        Request request,
                        XmlNode templateNode)
            : base(logger, templateNode)
        {
            _bot = bot;
            _user = user;
            _request = request;
        }

        protected override string ProcessChange()
        {
            if (this.templateNode.Name.ToLower() == "srai")
            {
                if (this.templateNode.InnerText.Length > 0)
                {
                    Request subRequest = new Request(this.templateNode.InnerText, _user.UserId);
                    subRequest.StartedOn = _request.StartedOn; // make sure we don't keep adding time to the request
                    Result subQuery = _bot.Chat(subRequest);
                    _request.hasTimedOut = subRequest.hasTimedOut;
                    return subQuery.Output;
                }
            }
            return string.Empty;
        }
    }
}