using ReniBot.Entities;

namespace ReniBot.Repository
{
    public class TemplateRepository : GenericRepository<Template>
    {
        public TemplateRepository(BotContext context) : base(context) { }

    }
}
