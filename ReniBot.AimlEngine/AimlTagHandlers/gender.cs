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
                    BotContext context)
            : base(logger, context, "gender")
        {
        }

        public override string ProcessChange(XmlNode TemplateNode)
        {
            if (TemplateNode.Name.ToLower() == "gender")
            {
                if (TemplateNode.InnerText.Length > 0)
                {
                    // non atomic version of the node
                    return  new Normalize.ApplySubstitutions(Context.Configuration.GenderSubstitutions).Transform( TemplateNode.InnerText);
                }
                else
                {
                    // atomic version of the node
                    Star recursiveStar = new Star(Logger, Context);
                    TemplateNode.InnerText = recursiveStar.ProcessChange(Utils.AIMLTagHandler.GetNode("<star/>"));
                    if (TemplateNode.InnerText.Length > 0)
                    {
                        return ProcessChange(TemplateNode);
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
