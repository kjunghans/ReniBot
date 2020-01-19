using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReniBot.Entities;

namespace ReniBot.Repository
{
    public class BotUserResultRepostiory: GenericRepository<BotUserResult>
    {
        public BotUserResultRepostiory(BotContext context) : base(context) { }

    }
}
