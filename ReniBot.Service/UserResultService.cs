using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReniBot.Entities;
using ReniBot.Repository;

namespace ReniBot.Service
{
    public class UserResultService
    {
        private readonly int _userId;

        public UserResultService(int userId)
        {
            _userId = userId;
        }

        public int Count()
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.BotUserResultRepository.GetItem(r => r.userId == _userId).Count();
        }

        public BotUserResult GetLast()
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.BotUserResultRepository.GetLast(r => r.userId == _userId, r => r.timeStamp);
        }

        public string GetLastOutput()
        {
            UnitOfWork uow = new UnitOfWork();
            BotUserResult result = uow.BotUserResultRepository.GetLast(r => r.userId == _userId, r => r.timeStamp);
            string output = "*";
            if (result != null)
                output = result.rawOutput;
            return output;
        }

        public BotUserResult GetNResult(int index)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.BotUserResultRepository.GetN(index,  r => r.userId == _userId, r => r.timeStamp);
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
