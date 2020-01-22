using System;
using System.Globalization;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// The uppercase element tells the AIML interpreter to render the contents of the element
    /// in uppercase, as defined (if defined) by the locale indicated by the specified language
    /// if specified).
    /// 
    /// If no character in this string has a different uppercase version, based on the Unicode 
    /// standard, then the original string is returned. 
    /// </summary>
    public class uppercase : Utils.AIMLTagHandler
    {
        private readonly CultureInfo _locale;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public uppercase(ILogger logger,
                            CultureInfo locale,
                         XmlNode templateNode)
            : base(logger, templateNode)
        {
            _locale = locale;
        }

        protected override string ProcessChange()
        {
            if (this.templateNode.Name.ToLower() == "uppercase")
            {
                return this.templateNode.InnerText.ToUpper(_locale);
            }
            return string.Empty;
        }
    }
}
