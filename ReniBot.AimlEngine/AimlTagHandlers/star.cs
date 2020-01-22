using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Xml;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// The star element indicates that an AIML interpreter should substitute the value "captured" 
    /// by a particular wildcard from the pattern-specified portion of the match path when returning 
    /// the template. 
    /// 
    /// The star element has an optional integer index attribute that indicates which wildcard to use. 
    /// The minimum acceptable value for the index is "1" (the first wildcard), and the maximum 
    /// acceptable value is equal to the number of wildcards in the pattern. 
    /// 
    /// An AIML interpreter should raise an error if the index attribute of a star specifies a wildcard 
    /// that does not exist in the category element's pattern. Not specifying the index is the same as 
    /// specifying an index of "1". 
    /// 
    /// The star element does not have any content. 
    /// </summary>
    public class star : ReniBot.AimlEngine.Utils.AIMLTagHandler
    {
        private readonly ILogger _logger;
        private readonly Utils.SubQuery _query;
        private readonly Request _request;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public star(ILogger logger,
                        ReniBot.AimlEngine.User user,
                        ReniBot.AimlEngine.Utils.SubQuery query,
                        ReniBot.AimlEngine.Request request,
                        ReniBot.AimlEngine.Result result,
                        XmlNode templateNode)
            : base(logger, templateNode)
        {
            _logger = logger;
            _query = query;
            _request = request;
        }

        protected override string ProcessChange()
        {
            if (this.templateNode.Name.ToLower() == "star")
            {
                if (_query.InputStar.Count > 0)
                {
                    if (this.templateNode.Attributes.Count == 0)
                    {
                        // return the first (latest) star in the List<>
                        return (string)_query.InputStar[0];
                    }
                    else if (this.templateNode.Attributes.Count == 1)
                    {
                        if (this.templateNode.Attributes[0].Name.ToLower() == "index")
                        {
                            try
                            {
                                int index = Convert.ToInt32(templateNode.Attributes[0].Value);
                                index--;
                                if ((index >= 0) & (index < _query.InputStar.Count))
                                {
                                    return (string)_query.InputStar[index];
                                }
                                else
                                {
                                    _logger.LogWarning("InputStar out of bounds reference caused by input: " + _request.rawInput);
                                }
                            }
                            catch
                            {
                                _logger.LogWarning("Index set to non-integer value whilst processing star tag in response to the input: " + _request.rawInput);
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("A star tag tried to reference an empty InputStar collection when processing the input: "+ _request.rawInput);
                }
            }
            return string.Empty;
        }
    }
}
