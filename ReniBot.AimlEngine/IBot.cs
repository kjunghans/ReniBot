using System.Xml;

namespace ReniBot.AimlEngine
{
    public interface IBot
    {
        void Learn(XmlDocument doc, string filename);
        Result Chat(Request request);
    }
}
