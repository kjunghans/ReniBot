using System;
using System.Linq;
using ReniBot.Common;
using ReniBot.Entities;
using ReniBot.Repository;

namespace ReniBot.Service
{
    public class UserPredicateService : IUserPredicateService
    {
        private readonly int _userId;

        public UserPredicateService(int userId)
        {
            _userId = userId;
        }

        public string grabSetting(string key)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.BotUserPredicateRepository.GetItem(p => p.userId == _userId && p.key == key)
                .Select(p => p.value).SingleOrDefault();
        }

        public void addSetting(string key, string value)
        {
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    uow.BotUserPredicateRepository.Insert(new BotUserPredicate()
                    {
                        userId = _userId,
                        key = key,
                        value = value
                    });
                    uow.Save();
                }
            }
            catch (Exception) //Failed because of duplciate key. TODO: get exact exception for dup key.
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    BotUserPredicate predicate = uow.BotUserPredicateRepository.GetItem(p => p.userId == _userId && p.key == key).SingleOrDefault();
                    predicate.value = value;
                    uow.BotUserPredicateRepository.Update(predicate);
                    uow.Save();
                }
            }
        }

        public void removeSetting(string key)
        {
            UnitOfWork uow = new UnitOfWork();
            uow.BotUserPredicateRepository.Delete(new { _userId, key });
            uow.Save();
        }
    }
}
