using ReniBot.Entities;

namespace ReniBot.Repository
{
    public class ApplicationRepository : GenericRepository<Application>
    {
        public ApplicationRepository(BotContext context) : base(context) { }

    }
}
