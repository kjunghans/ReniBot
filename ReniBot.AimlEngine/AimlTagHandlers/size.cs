using Microsoft.Extensions.Logging;
using System;
using System.Xml;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// The size element tells the AIML interpreter that it should substitute the number of 
    /// categories currently loaded.
    /// 
    /// The size element does not have any content. 
    /// </summary>
    public class size : Utils.AIMLTagHandler
    {
        private readonly int _botSize;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public size(ILogger logger,
                    int botSize,
                        XmlNode templateNode)
            : base(logger, templateNode)
        {
            _botSize = botSize;
        }

        protected override string ProcessChange()
        {
            if (TemplateNode.Name.ToLower() == "size")
            {
                return Convert.ToString(_botSize);
            }
            return string.Empty;
        }
    }
}
