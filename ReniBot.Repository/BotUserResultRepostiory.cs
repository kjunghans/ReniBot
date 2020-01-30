using ReniBot.Entities;

namespace ReniBot.Repository
{
    public class BotUserResultRepostiory : GenericRepository<BotUserResult>
    {
        public BotUserResultRepostiory(BotContext context) : base(context) { }

    }
}
