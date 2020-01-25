using System.Xml;

namespace ReniBot.AimlEngine.Utils
{
    public interface IAimlLoader
    {
        Node LoadAIML(string path);

        Node loadAIMLFile(string filename);

        Node loadAIMLFromXML(XmlDocument doc, string filename);
        
        string GeneratePath(string pattern, string that, string topicName, bool isUserInput);

        void SetGraphMaster(Node graphMaster);
    }
}
