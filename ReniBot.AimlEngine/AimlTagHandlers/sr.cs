using Microsoft.Extensions.Logging;
using System.Xml;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// The sr element is a shortcut for: 
    /// 
    /// <srai><star/></srai> 
    /// 
    /// The atomic sr does not have any content. 
    /// </summary>
    public class Sr : Utils.AIMLTagHandler
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
        public Sr(ILogger logger,
                    BotContext context)
            : base(logger, context, "sr")
        {
 
        }

        public override string ProcessChange(XmlNode TemplateNode)
        {
            if (TemplateNode.Name.ToLower() == "sr")
            {
                XmlNode starNode = Utils.AIMLTagHandler.GetNode("<star/>");
                Star recursiveStar = new Star(Logger, Context);
                string starContent = recursiveStar.ProcessChange(starNode);

                XmlNode sraiNode = ReniBot.AimlEngine.Utils.AIMLTagHandler.GetNode("<srai>" + starContent + "</srai>");
                Srai sraiHandler = new Srai(Logger, Context);
                return sraiHandler.ProcessChange(sraiNode);
            }
            return string.Empty;
        }
    }
}
