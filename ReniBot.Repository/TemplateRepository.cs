using ReniBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReniBot.Repository
{
    public class TemplateRepository: GenericRepository<Template>
    {
        public TemplateRepository(BotContext context) : base(context) { }

    }
}
