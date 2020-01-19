using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReniBot.Entities;
using ReniBot.Repository;

namespace ReniBot.Service
{
    public class UserRequestService
    {
        public static BotUserRequest GetRequestById(int id)
        {
            UnitOfWork uow = new UnitOfWork();
            return uow.BotUserRequestRepository.GetItem(r => r.id == id).SingleOrDefault();
        }

        public static int Add(string rawInput, int userId)
        {
            BotUserRequest request = new BotUserRequest()
            {
                rawInput = rawInput,
                userId = userId,
                startedOn = DateTimeOffset.UtcNow
            };
            UnitOfWork uow = new UnitOfWork();
            uow.BotUserRequestRepository.Insert(request);
            uow.Save();
            return request.id;
        }


    }
}
