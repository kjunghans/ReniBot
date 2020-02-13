using Microsoft.Extensions.Logging;
using System.Xml;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// The version element tells the AIML interpreter that it should substitute the version number
    /// of the AIML interpreter.
    /// 
    /// The version element does not have any content. 
    /// </summary>
    public class version : Utils.AIMLTagHandler
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
        public version(ILogger logger,
                    BotContext context)
            : base(logger, context, "version")
        {
        }

        public override string ProcessChange(XmlNode TemplateNode)
        {
            if (TemplateNode.Name.ToLower() == "version")
            {
                return Context.Configuration.GlobalSettings.grabSetting("version");
            }
            return string.Empty;
        }
    }
}
