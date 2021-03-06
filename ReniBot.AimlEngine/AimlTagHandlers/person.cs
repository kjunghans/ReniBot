using Microsoft.Extensions.Logging;
using System.Xml;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// The atomic version of the person element is a shortcut for: 
    /// 
    /// <person><star/></person> 
    ///
    /// The atomic person does not have any content. 
    /// 
    /// The non-atomic person element instructs the AIML interpreter to: 
    /// 
    /// 1. replace words with first-person aspect in the result of processing the contents of the 
    /// person element with words with the grammatically-corresponding third-person aspect; and 
    /// 
    /// 2. replace words with third-person aspect in the result of processing the contents of the 
    /// person element with words with the grammatically-corresponding first-person aspect.
    /// 
    /// The definition of "grammatically-corresponding" is left up to the implementation. 
    /// 
    /// Historically, implementations of person have dealt with pronouns, likely due to the fact that 
    /// most AIML has been written in English. However, the decision about whether to transform the 
    /// person aspect of other words is left up to the implementation.
    /// </summary>
    public class Person : Utils.AIMLTagHandler
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
        public Person(ILogger logger,
                    BotContext context)
            : base(logger, context, "person")
        {
        }

        public override string ProcessChange(XmlNode TemplateNode)
        {
            if (TemplateNode.Name.ToLower() == "person")
            {
                if (TemplateNode.InnerText.Length > 0)
                {
                    // non atomic version of the node
                    return new Normalize.ApplySubstitutions(Context.Configuration.Substitutions).Transform(TemplateNode.InnerText);
                }
                else
                {
                    // atomic version of the node
                    Star recursiveStar = new Star(Logger, Context);
                    TemplateNode.InnerText = recursiveStar.ProcessChange(Utils.AIMLTagHandler.GetNode("<star/>"));
                    if (TemplateNode.InnerText.Length > 0)
                    {
                        return ProcessChange(Utils.AIMLTagHandler.GetNode("<star/>"));
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
