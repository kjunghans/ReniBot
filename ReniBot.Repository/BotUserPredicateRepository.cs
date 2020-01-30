using ReniBot.Entities;

namespace ReniBot.Repository
{
    public class BotUserPredicateRepository : GenericRepository<BotUserPredicate>
    {
        public BotUserPredicateRepository(BotContext context) : base(context) { }

    }
}
