using System;
using System.Xml;
using System.Text;
using Microsoft.Extensions.Logging;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// NOT IMPLEMENTED FOR SECURITY REASONS
    /// </summary>
    public class system : ReniBot.AimlEngine.Utils.AIMLTagHandler
    {
        ILogger _logger;
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public system(ILogger logger,
                       XmlNode templateNode)
            : base(logger, templateNode)
        {
            _logger = logger;
        }

        protected override string ProcessChange()
        {
            _logger.LogWarning("The system tag is not implemented in this bot");
            return string.Empty;
        }
    }
}
