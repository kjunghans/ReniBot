using Microsoft.Extensions.Logging;
using System.Xml;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// The atomic version of the gender element is a shortcut for:
    /// 
    /// <gender><star/></gender> 
    ///
    /// The atomic gender does not have any content. 
    /// 
    /// The non-atomic gender element instructs the AIML interpreter to: 
    /// 
    /// 1. replace male-gendered words in the result of processing the contents of the gender element 
    /// with the grammatically-corresponding female-gendered words; and 
    /// 
    /// 2. replace female-gendered words in the result of processing the contents of the gender element 
    /// with the grammatically-corresponding male-gendered words. 
    /// 
    /// The definition of "grammatically-corresponding" is left up to the implementation.
    /// 
    /// Historically, implementations of gender have exclusively dealt with pronouns, likely due to the 
    /// fact that most AIML has been written in English. However, the decision about whether to 
    /// transform gender of other words is left up to the implementation.
    /// </summary>
    public class Gender : Utils.AIMLTagHandler
    {
        private readonly Utils.SubQuery _query;
        private readonly Request _request;
        private readonly BotConfiguration _config;
        private readonly ILogger _logger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public Gender(ILogger logger,
                        BotConfiguration config,
                        Utils.SubQuery query,
                        Request request,
                        XmlNode templateNode)
            : base(logger, templateNode)
        {
            _query = query;
            _config = config;
            _logger = logger;
            _request = request;
        }

        protected override string ProcessChange()
        {
            if (TemplateNode.Name.ToLower() == "gender")
            {
                if (TemplateNode.InnerText.Length > 0)
                {
                    // non atomic version of the node
                    return ReniBot.AimlEngine.Normalize.ApplySubstitutions.Substitute(_config.GenderSubstitutions, TemplateNode.InnerText);
                }
                else
                {
                    // atomic version of the node
                    XmlNode starNode = Utils.AIMLTagHandler.GetNode("<star/>");
                    Star recursiveStar = new Star(_logger, _query, _request, starNode);
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
