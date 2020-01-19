using System;
using System.Collections.Generic;
using System.Text;

namespace ReniBot.AimlEngine.Normalize
{
    /// <summary>
    /// Normalizes the input text into upper case
    /// </summary>
    public class MakeCaseInsensitive : ReniBot.AimlEngine.Utils.TextTransformer
    {
        public MakeCaseInsensitive(ReniBot.AimlEngine.Bot bot, string inputString) : base(bot, inputString)
        { }

        public MakeCaseInsensitive(ReniBot.AimlEngine.Bot bot) : base(bot) 
        { }

        protected override string ProcessChange()
        {
            return this.inputString.ToUpper();
        }

        /// <summary>
        /// An ease-of-use static method that re-produces the instance transformation methods
        /// </summary>
        /// <param name="input">The string to transform</param>
        /// <returns>The resulting string</returns>
        public static string TransformInput(string input)
        {
            return input.ToUpper();
        }
    }
}
