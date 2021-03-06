using Microsoft.Extensions.Logging;
using System.Xml;

namespace ReniBot.AimlEngine.Utils
{
    /// <summary>
    /// The template for all classes that handle the AIML tags found within template nodes of a
    /// category.
    /// </summary>
    abstract public class AIMLTagHandler 
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request itself</param>
        /// <param name="result">The result to be passed back to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public AIMLTagHandler(ILogger logger,
                                BotContext context,
                                string tagName) 
        {
            Logger = logger;
            //TemplateNode.Attributes.RemoveNamedItem("xmlns");
            Context = context;
            TagName = tagName;
        }

         public ILogger Logger { get; }
        /// <summary>
        /// A flag to denote if inner tags are to be processed recursively before processing this tag
        /// </summary>
        public bool isRecursive { get; set; } = true;

        public BotContext Context { get; }

        public string TagName { get; }

        /// <summary>
        /// Helper method that turns the passed string into an XML node
        /// </summary>
        /// <param name="outerXML">the string to XMLize</param>
        /// <returns>The XML node</returns>
        public static XmlNode GetNode(string outerXML)
        {
            XmlDocument temp = new XmlDocument();
            temp.LoadXml(outerXML);
            return temp.FirstChild;
        }

  
        /// <summary>
        /// The method that does the actual processing of the text.
        /// </summary>
        /// <returns>The resulting processed text</returns>
        public abstract string ProcessChange(XmlNode templateNode);
    }
}
