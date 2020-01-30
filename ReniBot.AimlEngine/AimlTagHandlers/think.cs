using Microsoft.Extensions.Logging;
using System.Xml;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// The think element instructs the AIML interpreter to perform all usual processing of its 
    /// contents, but to not return any value, regardless of whether the contents produce output.
    /// 
    /// The think element has no attributes. It may contain any AIML template elements.
    /// </summary>
    public class Think : ReniBot.AimlEngine.Utils.AIMLTagHandler
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
        public Think(ILogger logger,
                      XmlNode templateNode)
            : base(logger, templateNode)
        {
        }

        protected override string ProcessChange()
        {
            return string.Empty;
        }
    }
}
