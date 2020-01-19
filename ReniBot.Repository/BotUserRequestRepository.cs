using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReniBot.Entities;

namespace ReniBot.Repository
{
    public class BotUserRequestRepository: GenericRepository<BotUserRequest>
    {
        public BotUserRequestRepository(BotContext context) : base(context) { }

    }
}
