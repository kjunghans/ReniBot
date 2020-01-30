using System.Text.RegularExpressions;

namespace ReniBot.AimlEngine.Normalize
{
    /// <summary>
    /// Strips any illegal characters found within the input string. Illegal characters are referenced from
    /// the bot's Strippers regex that is defined in the setup XML file.
    /// </summary>
    public class StripIllegalCharacters : ReniBot.AimlEngine.Utils.TextTransformer
    {
        private readonly Regex _strippers;
        public StripIllegalCharacters(Regex strippers, string inputString) : base(inputString)
        {
            _strippers = strippers;
        }

        public StripIllegalCharacters(Regex strippers)
            : base()
        {
            _strippers = strippers;
        }

        protected override string ProcessChange()
        {
            return _strippers.Replace(InputString, " ");
        }
    }
}
