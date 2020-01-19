using System;
using System.Text.RegularExpressions;
using System.Text;

namespace ReniBot.AimlEngine.Normalize
{
    /// <summary>
    /// Strips any illegal characters found within the input string. Illegal characters are referenced from
    /// the bot's Strippers regex that is defined in the setup XML file.
    /// </summary>
    public class StripIllegalCharacters : ReniBot.AimlEngine.Utils.TextTransformer
    {
        public StripIllegalCharacters(ReniBot.AimlEngine.Bot bot, string inputString) : base(bot, inputString)
        { }

        public StripIllegalCharacters(ReniBot.AimlEngine.Bot bot)
            : base(bot) 
        { }

        protected override string ProcessChange()
        {
            return this.bot.Strippers.Replace(this.inputString, " ");
        }
    }
}
