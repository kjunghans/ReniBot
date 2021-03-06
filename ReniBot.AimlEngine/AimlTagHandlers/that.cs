using Microsoft.Extensions.Logging;
using System;
using System.Xml;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// The template-side that element indicates that an AIML interpreter should substitute the 
    /// contents of a previous bot output. 
    /// 
    /// The template-side that has an optional index attribute that may contain either a single 
    /// integer or a comma-separated pair of integers. The minimum value for either of the integers 
    /// in the index is "1". The index tells the AIML interpreter which previous bot output should be 
    /// returned (first dimension), and optionally which "sentence" (see [8.3.2.]) of the previous bot
    /// output (second dimension). 
    /// 
    /// The AIML interpreter should raise an error if either of the specified index dimensions is 
    /// invalid at run-time. 
    /// 
    /// An unspecified index is the equivalent of "1,1". An unspecified second dimension of the index 
    /// is the equivalent of specifying a "1" for the second dimension. 
    /// 
    /// The template-side that element does not have any content. 
    /// </summary>
    public class that : Utils.AIMLTagHandler
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
        public that(ILogger logger,
                    BotContext context)
            : base(logger, context, "that")
        {
        }

        public override string ProcessChange(XmlNode TemplateNode)
        {
            if (TemplateNode.Name.ToLower() == "that")
            {
                if (TemplateNode.Attributes.Count == 0)
                {
                    return Context.User.GetThat();
                }
                else if (TemplateNode.Attributes.Count == 1)
                {
                    if (TemplateNode.Attributes[0].Name.ToLower() == "index")
                    {
                        if (TemplateNode.Attributes[0].Value.Length > 0)
                        {
                            try
                            {
                                // see if there is a split
                                string[] dimensions = TemplateNode.Attributes[0].Value.Split(",".ToCharArray());
                                if (dimensions.Length == 2)
                                {
                                    int result = Convert.ToInt32(dimensions[0].Trim());
                                    int sentence = Convert.ToInt32(dimensions[1].Trim());
                                    if ((result > 0) & (sentence > 0))
                                    {
                                        return Context.User.GetThat(result - 1, sentence - 1);
                                    }
                                    else
                                    {
                                        Logger.LogError("ERROR! An input tag with a bady formed index (" + TemplateNode.Attributes[0].Value + ") was encountered processing the input: " + Context.Request.RawInput);
                                    }
                                }
                                else
                                {
                                    int result = Convert.ToInt32(TemplateNode.Attributes[0].Value.Trim());
                                    if (result > 0)
                                    {
                                        return Context.User.GetThat(result - 1);
                                    }
                                    else
                                    {
                                        Logger.LogError("ERROR! An input tag with a bady formed index (" + TemplateNode.Attributes[0].Value + ") was encountered processing the input: " + Context.Request.RawInput);
                                    }
                                }
                            }
                            catch
                            {
                                Logger.LogError("ERROR! An input tag with a bady formed index (" + TemplateNode.Attributes[0].Value + ") was encountered processing the input: " + Context.Request.RawInput);
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}
