using Microsoft.Extensions.Logging;
using System.Xml;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// The gossip element instructs the AIML interpreter to capture the result of processing the 
    /// contents of the gossip elements and to store these contents in a manner left up to the 
    /// implementation. Most common uses of gossip have been to store captured contents in a separate 
    /// file. 
    /// 
    /// The gossip element does not have any attributes. It may contain any AIML template elements.
    /// </summary>
    public class gossip : Utils.AIMLTagHandler
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
        public gossip(ILogger logger,
                    BotContext context)
            : base(logger, context, "gossip")
        {
        }

        public override string ProcessChange(XmlNode TemplateNode)
        {
            if (TemplateNode.Name.ToLower() == "gossip")
            {
                // gossip is merely logged by the bot and written to log files
                if (TemplateNode.InnerText.Length > 0)
                {
                    Logger.LogInformation("GOSSIP from user: " + Context.User.UserKey + ", '" + TemplateNode.InnerText + "'");
                }
            }
            return string.Empty;
        }
    }
}
