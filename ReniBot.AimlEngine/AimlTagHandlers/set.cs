using Microsoft.Extensions.Logging;
using System.Xml;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// The set element instructs the AIML interpreter to set the value of a predicate to the result 
    /// of processing the contents of the set element. The set element has a required attribute name, 
    /// which must be a valid AIML predicate name. If the predicate has not yet been defined, the AIML 
    /// interpreter should define it in memory. 
    /// 
    /// The AIML interpreter should, generically, return the result of processing the contents of the 
    /// set element. The set element must not perform any text formatting or other "normalization" on 
    /// the predicate contents when returning them. 
    /// 
    /// The AIML interpreter implementation may optionally provide a mechanism that allows the AIML 
    /// author to designate certain predicates as "return-name-when-set", which means that a set 
    /// operation using such a predicate will return the name of the predicate, rather than its 
    /// captured value. (See [9.2].) 
    /// 
    /// A set element may contain any AIML template elements.
    /// </summary>
    public class set : Utils.AIMLTagHandler
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
        public set(ILogger logger,
                    BotContext context)
            : base(logger, context, "set")
        {
        }

        public override string ProcessChange(XmlNode TemplateNode)
        {
            if (TemplateNode.Name.ToLower() == "set")
            {
                if (Context.Configuration.GlobalSettings.Count > 0)
                {
                    if (TemplateNode.Attributes.Count == 1)
                    {
                        if (TemplateNode.Attributes[0].Name.ToLower() == "name")
                        {
                            if (TemplateNode.InnerText.Length > 0)
                            {
                                Context.User.Predicates.addSetting(TemplateNode.Attributes[0].Value, TemplateNode.InnerText);
                                return Context.User.Predicates.grabSetting(TemplateNode.Attributes[0].Value);
                            }
                            else
                            {
                                // remove the predicate
                                Context.User.Predicates.removeSetting(TemplateNode.Attributes[0].Value);
                                return string.Empty;
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}
