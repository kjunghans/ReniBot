using System;
using System.Xml;
using System.Text;
using Microsoft.Extensions.Logging;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// The id element tells the AIML interpreter that it should substitute the user ID. 
    /// The determination of the user ID is not specified, since it will vary by application. 
    /// A suggested default return value is "localhost". 
    /// 
    /// The id element does not have any content.
    /// </summary>
    public class id : ReniBot.AimlEngine.Utils.AIMLTagHandler
    {
        User _user;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public id(ILogger logger,
                        ReniBot.AimlEngine.User user,
                         XmlNode templateNode)
            : base(logger, templateNode)
        {
            _user = user;
        }

        protected override string ProcessChange()
        {
            if (TemplateNode.Name.ToLower() == "id")
            {
                return _user.UserKey;
            }
            return string.Empty;
        }
    }
}
