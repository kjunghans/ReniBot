using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReniBot.Repository;
using ReniBot.Entities;

namespace ReniBot.Service
{
    public class BotUserService
    {
        public static int GetUserId(int appId, string userKey)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.BotUserRepository.GetItem(u => u.appId == appId && u.userKey == userKey).Select(u => u.id).SingleOrDefault();
        }

        public static int AddUser(int appId, string userKey, string topic = "*")
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

        public static string GetTopic(int userId)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.BotUserRepository.GetItem(u => u.id == userId).Select(u => u.topic).SingleOrDefault();
        }

        public static void UpdateTopic(int userId, string topic)
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
