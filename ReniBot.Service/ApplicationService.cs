using ReniBot.Entities;
using ReniBot.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using ReniBot.Common;

namespace ReniBot.Service
{
    public class ApplicationService : IApplicationService
    {
        public string GenAppKey()
        {
            return ShortGuid.NewGuid().ToString();
        }

        public string InsertApp(string name, string description, string userId)
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

        public List<Application> GetApplicationForUser(string userId)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.ApplicationRepository.GetItem(a => a.userId == userId).ToList();
        }

        public Application GetApplication(string key)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.ApplicationRepository.GetItem(a => a.key == key).SingleOrDefault();
        }

        public int GetApplicationIdFromKey(string key)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.ApplicationRepository.GetItem(a => a.key == key).Select(a => a.id).SingleOrDefault();
        }

        public int GetApplicationIdFromName(string name)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.ApplicationRepository.GetItem(a => a.Name == name).Select(a => a.id).SingleOrDefault();
        }

        public void UpdateApplication(Application app)
        {
            UnitOfWork uow = new UnitOfWork();
            uow.ApplicationRepository.Update(app);
        }

        public void DeleteApplication(string key)
        {
            int id = GetApplicationIdFromKey(key);
            if (id <= 0)
                throw new Exception("Could not find application with that key");
            UnitOfWork uow = new UnitOfWork();
            uow.ApplicationRepository.Delete(id);

        }

        public void UpdateBrain(int appId, byte[] brain)
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

        public byte[] GetBrain(int appId)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.BrainRepository.GetItem(b => b.appId == appId).Select(b => b.knowledge).SingleOrDefault();
        }

        public void UpdateAimlDocument(int appId, string name, string document)
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

        public void ImportAimlFromFile(int appId, string name, string fileName)
        {
            string aimlDoc = File.ReadAllText(fileName);
            UpdateAimlDocument(appId, name, aimlDoc);
        }

        public void ImportAimlFromPath(int appId, string path, string mask = "*.*")
        {
            if (Directory.Exists(path))
            {
                string[] fileEntries = Directory.GetFiles(path, mask);
                if (fileEntries.Length > 0)
                {
                    foreach (string filename in fileEntries)
                    {
                        char[] delims = { '/', '\\', '.' };
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

        public string GetAimlAsString(int appId, string name)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.AimlDocRepository.GetItem(a => a.appId == appId && a.name == name).Select(a => a.document).SingleOrDefault();
        }

        public XmlDocument GetAimlAsXml(int appId, string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(GetAimlAsString(appId, name));
            return doc;
        }

        public List<AimlDoc> GetAimlDocs(int appId)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.AimlDocRepository.GetItem(a => a.appId == appId).ToList();
        }

        public List<XmlDocument> GetAimlXmlDocs(int appId)
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
