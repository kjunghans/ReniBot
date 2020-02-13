using Microsoft.Extensions.Logging;
using System.Xml;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// NOT IMPLEMENTED FOR SECURITY REASONS
    /// </summary>
    public class javascript : ReniBot.AimlEngine.Utils.AIMLTagHandler
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
        public javascript(ILogger logger,
                    BotContext context)
            : base(logger, context, "javascript")
        {
        }

        public override string ProcessChange(XmlNode TemplateNode)
        {
            Logger.LogWarning("The javascript tag is not implemented in this bot");
            return string.Empty;
        }
    }
}
