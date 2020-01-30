using ReniBot.Entities;

namespace ReniBot.Repository
{
    public class SettingRepository : GenericRepository<Setting>
    {
        public SettingRepository(BotContext context) : base(context) { }

    }
}
