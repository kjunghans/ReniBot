using System.Xml;

namespace ReniBot.Entities
{
    public class AimlDoc
    {
        public int appId { get; set; }
        public string name { get; set; }
        public string document { get; set; }
        public XmlDocument XmlDoc
        {
            get { XmlDocument xmlDoc = new XmlDocument(); xmlDoc.LoadXml(document); return xmlDoc; }
        }
    }
}
