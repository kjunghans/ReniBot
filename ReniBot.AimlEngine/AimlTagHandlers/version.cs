using System;
using System.Xml;
using System.Text;
using Microsoft.Extensions.Logging;

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
        private readonly BotConfiguration _config;
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
                        BotConfiguration config,
                        XmlNode templateNode)
            : base(logger, templateNode)
        {
            _config = config;
        }

        protected override string ProcessChange()
        {
            if (TemplateNode.Name.ToLower() == "version")
            {
                return _config.GlobalSettings.grabSetting("version");
            }
            return string.Empty;
        }
    }
}
