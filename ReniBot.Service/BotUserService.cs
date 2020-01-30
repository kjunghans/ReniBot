using ReniBot.Common;
using ReniBot.Entities;
using ReniBot.Repository;
using System.Linq;

namespace ReniBot.Service
{
    public class BotUserService : IBotUserService
    {
        public int GetUserId(int appId, string userKey)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.BotUserRepository.GetItem(u => u.appId == appId && u.userKey == userKey).Select(u => u.id).SingleOrDefault();
        }

        public int AddUser(int appId, string userKey, string topic = "*")
        {
            BotUser user = new BotUser()
            {
                appId = appId,
                userKey = userKey,
                topic = topic
            };
            UnitOfWork uow = new UnitOfWork();
            uow.BotUserRepository.Insert(user);
            uow.Save();
            return user.id;
        }

        public string GetTopic(int userId)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.BotUserRepository.GetItem(u => u.id == userId).Select(u => u.topic).SingleOrDefault();
        }

        public void UpdateTopic(int userId, string topic)
        {
            UnitOfWork uow = new UnitOfWork();
            BotUser user = uow.BotUserRepository.GetItem(u => u.id == userId).SingleOrDefault();
            if (user != null)
            {
                user.topic = topic;
                uow.BotUserRepository.Update(user);
                uow.Save();
            }

        }
    }
}
