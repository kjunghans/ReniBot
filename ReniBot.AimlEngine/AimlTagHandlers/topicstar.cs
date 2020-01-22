using System;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace ReniBot.AimlEngine.AIMLTagHandlers
{
    /// <summary>
    /// The topicstar element tells the AIML interpreter that it should substitute the contents of 
    /// a wildcard from the current topic (if the topic contains any wildcards).
    /// 
    /// The topicstar element has an optional integer index attribute that indicates which wildcard 
    /// to use; the minimum acceptable value for the index is "1" (the first wildcard). Not 
    /// specifying the index is the same as specifying an index of "1". 
    /// 
    /// The topicstar element does not have any content. 
    /// </summary>
    public class topicstar : Utils.AIMLTagHandler
    {
        Utils.SubQuery _query;
        Request _request;
        ILogger _logger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public topicstar(ILogger logger,
            ReniBot.AimlEngine.Bot bot,
                        ReniBot.AimlEngine.User user,
                        ReniBot.AimlEngine.Utils.SubQuery query,
                        ReniBot.AimlEngine.Request request,
                        ReniBot.AimlEngine.Result result,
                        XmlNode templateNode)
            : base(logger, templateNode)
        {
            _logger = logger;
            _request = request;
            _query = query;
        }

        protected override string ProcessChange()
        {
            if (this.templateNode.Name.ToLower() == "topicstar")
            {
                if (this.templateNode.Attributes.Count == 0)
                {
                    if (_query.TopicStar.Count > 0)
                    {
                        return (string)_query.TopicStar[0];
                    }
                    else
                    {
                        _logger.LogError("An out of bounds index to topicstar was encountered when processing the input: " + _request.rawInput);
                    }
                }
                else if (this.templateNode.Attributes.Count == 1)
                {
                    if (this.templateNode.Attributes[0].Name.ToLower() == "index")
                    {
                        if (this.templateNode.Attributes[0].Value.Length > 0)
                        {
                            try
                            {
                                int result = Convert.ToInt32(this.templateNode.Attributes[0].Value.Trim());
                                if (_query.TopicStar.Count > 0)
                                {
                                    if (result > 0)
                                    {
                                        return (string)_query.TopicStar[result - 1];
                                    }
                                    else
                                    {
                                        _logger.LogError("An input tag with a bady formed index (" + this.templateNode.Attributes[0].Value + ") was encountered processing the input: " + _request.rawInput);
                                    }
                                }
                                else
                                {
                                    _logger.LogError("An out of bounds index to topicstar was encountered when processing the input: " + _request.rawInput);
                                }
                            }
                            catch
                            {
                                _logger.LogError("A thatstar tag with a bady formed index (" + templateNode.Attributes[0].Value + ") was encountered processing the input: " + _request.rawInput);
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}
