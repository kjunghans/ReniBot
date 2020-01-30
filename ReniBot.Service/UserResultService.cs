using ReniBot.Common;
using ReniBot.Entities;
using ReniBot.Repository;
using System;
using System.Linq;

namespace ReniBot.Service
{
    public class UserResultService : IUserResultService
    {

        public int Count(int userId)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.BotUserResultRepository.GetItem(r => r.userId == userId).Count();
        }

        public BotUserResult GetLast(int userId)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.BotUserResultRepository.GetLast(r => r.userId == userId, r => r.timeStamp);
        }

        public string GetLastOutput(int userId)
        {
            UnitOfWork uow = new UnitOfWork();
            BotUserResult result = uow.BotUserResultRepository.GetLast(r => r.userId == userId, r => r.timeStamp);
            string output = "*";
            if (result != null)
                output = result.rawOutput;
            return output;
        }

        public BotUserResult GetNResult(int index, int userId)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.BotUserResultRepository.GetN(index, r => r.userId == userId, r => r.timeStamp);
        }

        public void Add(int duration, bool hasTimedOut, string rawOutput, int requestId, int userId)
        {
            BotUserResult result = new BotUserResult()
            {
                Duration = duration,
                hasTimedOut = hasTimedOut,
                rawOutput = rawOutput,
                requestId = requestId,
                userId = userId,
                timeStamp = DateTimeOffset.UtcNow
            };
            UnitOfWork uow = new UnitOfWork();
            uow.BotUserResultRepository.Insert(result);
            uow.Save();
        }
    }
}
