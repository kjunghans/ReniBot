using ReniBot.Entities;

namespace ReniBot.Repository
{
    public class BotUserRepository : GenericRepository<BotUser>
    {
        public BotUserRepository(BotContext context) : base(context) { }

    }
}
