using System;
using System.Xml;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{

    /// <summary>
    /// The date element tells the AIML interpreter that it should substitute the system local 
    /// date and time. No formatting constraints on the output are specified.
    /// 
    /// The date element does not have any content. 
    /// </summary>
    public class date : ReniBot.AimlEngine.Utils.AIMLTagHandler
    {
        private CultureInfo _local;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public date(ILogger logger,
                    CultureInfo local,
                     XmlNode templateNode)
            : base(logger, templateNode)
        {
            _local = local;
        }

        protected override string ProcessChange()
        {
            if (TemplateNode.Name.ToLower() == "date")
            {
                return DateTime.Now.ToString(_local);
            }
            return string.Empty;
        }
    }
}
