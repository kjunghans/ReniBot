using Microsoft.Extensions.Logging;
using System.Xml;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// The atomic version of the person2 element is a shortcut for: 
    /// 
    /// <person2><star/></person2> 
    /// 
    /// The atomic person2 does not have any content.
    /// 
    /// The non-atomic person2 element instructs the AIML interpreter to: 
    /// 
    /// 1. replace words with first-person aspect in the result of processing the contents of the 
    /// person2 element with words with the grammatically-corresponding second-person aspect; and,
    /// 
    /// 2. replace words with second-person aspect in the result of processing the contents of the 
    /// person2 element with words with the grammatically-corresponding first-person aspect. 
    /// 
    /// The definition of "grammatically-corresponding" is left up to the implementation.
    /// 
    /// Historically, implementations of person2 have dealt with pronouns, likely due to the fact 
    /// that most AIML has been written in English. However, the decision about whether to transform 
    /// the person aspect of other words is left up to the implementation.
    /// </summary>
    public class Person2 : Utils.AIMLTagHandler
    {
        private readonly BotConfiguration _config;
        readonly Utils.SubQuery _query;
        readonly Request _request;
        readonly ILogger _logger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public Person2(ILogger logger,
            BotConfiguration config,
                        ReniBot.AimlEngine.Utils.SubQuery query,
                        ReniBot.AimlEngine.Request request,
                        XmlNode templateNode)
            : base(logger, templateNode)
        {
            _config = config;
            _query = query;
            _request = request;
            _logger = logger;
        }

        protected override string ProcessChange()
        {
            if (TemplateNode.Name.ToLower() == "person2")
            {
                if (TemplateNode.InnerText.Length > 0)
                {
                    // non atomic version of the node
                    return new Normalize.ApplySubstitutions(_config.Person2Substitutions).Transform(TemplateNode.InnerText);
                }
                else
                {
                    // atomic version of the node
                    XmlNode starNode = Utils.AIMLTagHandler.GetNode("<star/>");
                    Star recursiveStar = new Star(_logger,  _query, _request, starNode);
                    TemplateNode.InnerText = recursiveStar.Transform();
                    if (TemplateNode.InnerText.Length > 0)
                    {
                        return ProcessChange();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            return string.Empty;
        }
    }
}
