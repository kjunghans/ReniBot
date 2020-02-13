using Microsoft.Extensions.Logging;
using System.Xml;

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
    public class Srai : Utils.AIMLTagHandler
    {

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public Srai(ILogger logger,
                    BotContext context)
            : base(logger, context, "srai")
        {
        }

        public override string ProcessChange(XmlNode TemplateNode)
        {
            if (TemplateNode.Name.ToLower() == "srai")
            {
                if (TemplateNode.InnerText.Length > 0)
                {
                    Request subRequest = new Request(TemplateNode.InnerText, Context.User.UserId)
                    {
                        StartedOn = Context.Request.StartedOn // make sure we don't keep adding time to the request
                    };
                    Result subQuery = Context.Bot.Chat(subRequest);
                    Context.Request.HasTimedOut = subRequest.HasTimedOut;
                    return subQuery.Output;
                }
            }
            return string.Empty;
        }
    }
}
