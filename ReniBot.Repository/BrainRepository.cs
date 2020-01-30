using ReniBot.Entities;

namespace ReniBot.Repository
{
    public class BrainRepository : GenericRepository<Brain>
    {
        public BrainRepository(BotContext context) : base(context) { }

    }
}
