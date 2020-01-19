using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReniBot.Entities;

namespace ReniBot.Repository
{
    public class SettingRepository: GenericRepository<Setting>
    {
        public SettingRepository(BotContext context) : base(context) { }

    }
}
