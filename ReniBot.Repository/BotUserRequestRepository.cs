using ReniBot.Entities;

namespace ReniBot.Repository
{
    public class BotUserRequestRepository : GenericRepository<BotUserRequest>
    {
        public BotUserRequestRepository(BotContext context) : base(context) { }

    }
}
