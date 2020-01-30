using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Xml;

namespace ReniBot.AimlEngine.Utils 
{
    /// <summary>
    /// The template for all classes that handle the AIML tags found within template nodes of a
    /// category.
    /// </summary>
    abstract public class AIMLTagHandler : TextTransformer
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
        public AIMLTagHandler   (   ILogger logger,
                                    XmlNode templateNode) :base(templateNode.OuterXml)
        {
            Logger = logger;
            TemplateNode = templateNode;
            TemplateNode.Attributes.RemoveNamedItem("xmlns");
        }

        /// <summary>
        /// Default ctor to use when late binding
        /// </summary>
        public AIMLTagHandler()
        {
        }

        public ILogger Logger;
        /// <summary>
        /// A flag to denote if inner tags are to be processed recursively before processing this tag
        /// </summary>
        public bool isRecursive = true;

         /// <summary>
        /// The template node to be processed by the class
        /// </summary>
        public XmlNode TemplateNode;


        /// <summary>
        /// Helper method that turns the passed string into an XML node
        /// </summary>
        /// <param name="outerXML">the string to XMLize</param>
        /// <returns>The XML node</returns>
        public static XmlNode getNode(string outerXML)
        {
            XmlDocument temp = new XmlDocument();
            temp.LoadXml(outerXML);
            return temp.FirstChild;
        }
    }
}
