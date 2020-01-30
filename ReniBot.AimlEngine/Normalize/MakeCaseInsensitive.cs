namespace ReniBot.AimlEngine.Normalize
{
    /// <summary>
    /// Normalizes the input text into upper case
    /// </summary>
    public class MakeCaseInsensitive : ReniBot.AimlEngine.Utils.TextTransformer
    {
        public MakeCaseInsensitive(string inputString) : base(inputString)
        { }


        protected override string ProcessChange()
        {
            return InputString.ToUpper();
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
