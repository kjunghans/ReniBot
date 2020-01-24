using ReniBot.Entities;
using System.Collections.Generic;
using System.Xml;

namespace ReniBot.Common
{
    public interface IApplicationService
    {
        void DeleteApplication(string key);
        string GenAppKey();
        string GetAimlAsString(int appId, string name);
        XmlDocument GetAimlAsXml(int appId, string name);
        List<AimlDoc> GetAimlDocs(int appId);
        List<XmlDocument> GetAimlXmlDocs(int appId);
        Application GetApplication(string key);
        List<Application> GetApplicationForUser(string userId);
        int GetApplicationIdFromKey(string key);
        int GetApplicationIdFromName(string name);
        byte[] GetBrain(int appId);
        void ImportAimlFromFile(int appId, string name, string fileName);
        void ImportAimlFromPath(int appId, string path, string mask = "*.*");
        string InsertApp(string name, string description, string userId);
        void UpdateAimlDocument(int appId, string name, string document);
        void UpdateApplication(Application app);
        void UpdateBrain(int appId, byte[] brain);
    }
}