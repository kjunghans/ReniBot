using ReniBot.Entities;
using ReniBot.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Web;

namespace ReniBot.Service
{
    public class ApplicationService
    {
        public static string GenAppKey()
        {
            return ShortGuid.NewGuid().ToString();
        }

        public static string InsertApp(string name, string description, string userId)
        {
            UnitOfWork uow = new UnitOfWork();
            string newKey = GenAppKey();
            uow.ApplicationRepository.Insert(new Application()
            {
                Description = description,
                Name = name,
                userId = userId,
                key = newKey
            });
            uow.Save();
            return newKey;
        }

        public static List<Application> GetApplicationForUser(string userId)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.ApplicationRepository.GetItem(a => a.userId == userId).ToList();
        }

        public static Application GetApplication(string key)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.ApplicationRepository.GetItem(a => a.key == key).SingleOrDefault();
        }

        public static int GetApplicationIdFromKey(string key)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.ApplicationRepository.GetItem(a => a.key == key).Select(a => a.id).SingleOrDefault();
        }

        public static int GetApplicationIdFromName(string name)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.ApplicationRepository.GetItem(a => a.Name == name).Select(a => a.id).SingleOrDefault();
        }

        public static void UpdateApplication(Application app)
        {
            UnitOfWork uow = new UnitOfWork();
            uow.ApplicationRepository.Update(app);
        }

        public static void DeleteApplication(string key)
        {
            int id = GetApplicationIdFromKey(key);
            if (id <= 0)
                throw new Exception("Could not find application with that key");
            UnitOfWork uow = new UnitOfWork();
            uow.ApplicationRepository.Delete(id);

        }

        public static void UpdateBrain(int appId, byte[] brain)
        {
            UnitOfWork uow = new UnitOfWork();
            Brain currentBrain = uow.BrainRepository.GetItem(b => b.appId == appId).SingleOrDefault();
            if (currentBrain == null)
            {
                uow.BrainRepository.Insert(new Brain() { appId = appId, knowledge = brain });
                uow.Save();
            }
            else
            {
                currentBrain.knowledge = brain;
                uow.BrainRepository.Update(currentBrain);
                uow.Save();
            }
        }

        public static byte[] GetBrain(int appId)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.BrainRepository.GetItem(b => b.appId == appId).Select(b => b.knowledge).SingleOrDefault();
        }

        public static void UpdateAimlDocument(int appId, string name, string document)
        {
            UnitOfWork uow = new UnitOfWork();
            AimlDoc doc = uow.AimlDocRepository.GetItem(a => a.appId == appId && a.name == name).SingleOrDefault();
            if (doc == null)
            {
                uow.AimlDocRepository.Insert(new AimlDoc() { appId = appId, name = name, document = document });
                uow.Save();
            }
            else
            {
                doc.document = document;
                uow.AimlDocRepository.Update(doc);
                uow.Save();
            }
        }

        public static void ImportAimlFromFile(int appId, string name, string fileName)
        {
            string aimlDoc = File.ReadAllText(fileName);
            UpdateAimlDocument(appId, name, aimlDoc);
        }

        public static void ImportAimlFromPath(int appId, string path, string mask = "*.*")
        {
            if (Directory.Exists(path))
            {
                string[] fileEntries = Directory.GetFiles(path, mask);
                if (fileEntries.Length > 0)
                {
                    foreach (string filename in fileEntries)
                    {
                        char[] delims = {'/','\\','.'};
                        string[] tokens = filename.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                        int len = tokens.Count();
                        string name = "";
                        if (len > 1)
                            name = tokens[len - 2];
                        ImportAimlFromFile(appId, name, filename);
                    }
                }
                else
                {
                    throw new FileNotFoundException("Could not find any .aiml files in the specified directory (" + path + ").");
                }
            }
            else
            {
                throw new FileNotFoundException("The directory specified as the path to the AIML files (" + path + ") cannot be found.");
            }

        }

        public static string GetAimlAsString(int appId, string name)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.AimlDocRepository.GetItem(a => a.appId == appId && a.name == name).Select(a => a.document).SingleOrDefault();
        }

        public static XmlDocument GetAimlAsXml(int appId, string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(GetAimlAsString(appId, name));
            return doc;
        }

        public static List<AimlDoc> GetAimlDocs(int appId)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.AimlDocRepository.GetItem(a => a.appId == appId).ToList();
        }

        public static List<XmlDocument> GetAimlXmlDocs(int appId)
        {
            UnitOfWork uow = new UnitOfWork();
            List<XmlDocument> docList = new List<XmlDocument>();
            var docs = uow.AimlDocRepository.GetItem(a => a.appId == appId);
            foreach (var doc in docs)
                docList.Add(doc.XmlDoc);
            return docList;

        }
    }
}
