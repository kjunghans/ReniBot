using System;

namespace ReniBot.AimlEngine.Utils
{
    /// <summary>
    /// Encapsulates all the required methods and attributes for any text transformation.
    /// 
    /// An input string is provided and various methods and attributes can be used to grab
    /// a transformed string.
    /// 
    /// The protected ProcessChange() method is abstract and should be overridden to contain 
    /// the code for transforming the input text into the output text.
    /// </summary>
    abstract public class TextTransformer
    {
        /// <summary>
        /// Instance of the input string
        /// </summary>
        //protected string inputString;


        /// <summary>
        /// The input string to be transformed in some way
        /// </summary>
        public string InputString { get; set; }
 
        /// <summary>
        /// The transformed string
        /// </summary>
        public string OutputString
        {
            get{return Transform();}
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bot">The bot this transformer is a part of</param>
        /// <param name="inputString">The input string to be transformed</param>
        public TextTransformer(string inputString)
        {
            InputString = inputString;
        }

 
        /// <summary>
        /// Default ctor for used as part of late binding mechanism
        /// </summary>
        public TextTransformer()
        {
            InputString = string.Empty;
        }

        /// <summary>
        /// Do a transformation on the supplied input string
        /// </summary>
        /// <param name="input">The string to be transformed</param>
        /// <returns>The resulting output</returns>
        public string Transform(string input)
        {
            InputString = input;
            return Transform();
        }

        /// <summary>
        /// Do a transformation on the string found in the InputString attribute
        /// </summary>
        /// <returns>The resulting transformed string</returns>
        public string Transform()
        {
           
            if (InputString.Length > 0)
            {
                return ProcessChange();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// The method that does the actual processing of the text.
        /// </summary>
        /// <returns>The resulting processed text</returns>
        protected abstract string ProcessChange();
    }
}
