using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Xml;

namespace ReniBot.AimlEngine.Utils
{
    public class AimlTagHandlerFactory
    {
        private readonly ILogger _logger;
        private readonly BotConfiguration _config;

        public AimlTagHandlerFactory(ILogger logger, BotConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public AIMLTagHandler CreateTagHandler(string tagName, Bot bot, User user, SubQuery query, Request request, Result result, XmlNode node)
        {
            AIMLTagHandler tagHandler = GetBespokeTags(user, query, request, result, node);
            if (object.Equals(null, tagHandler))
            {
                tagHandler = tagName switch
                {
                    "bot" => new AIMLTagHandlers.bot(_logger, _config.GlobalSettings, node),
                    "condition" => new AIMLTagHandlers.condition(_logger, user, node),
                    "date" => new AIMLTagHandlers.date(_logger, _config.Locale, node),
                    "formal" => new AIMLTagHandlers.formal(_logger, node),
                    "gender" => new AIMLTagHandlers.Gender(_logger, _config, query, request, node),
                    "get" => new AIMLTagHandlers.Get(_logger, _config, user, node),
                    "gossip" => new AIMLTagHandlers.gossip(_logger, user, node),
                    "id" => new AIMLTagHandlers.id(_logger, user, node),
                    "input" => new AIMLTagHandlers.input(_logger, user, request, node),
                    "javascript" => new AIMLTagHandlers.javascript(_logger, node),
                    "learn" => new AIMLTagHandlers.learn(_logger, bot, node),
                    "lowercase" => new AIMLTagHandlers.lowercase(_logger, _config.Locale, node),
                    "person" => new AIMLTagHandlers.Person(_logger, _config, query, request, node),
                    "person2" => new AIMLTagHandlers.Person2(_logger, _config, query, request, node),
                    "random" => new AIMLTagHandlers.random(_logger, node),
                    "sentence" => new AIMLTagHandlers.Sentence(_logger, _config, query, request, node),
                    "set" => new AIMLTagHandlers.set(_logger, _config, user, node),
                    "size" => new AIMLTagHandlers.size(_logger, _config.Size, node),
                    "sr" => new AIMLTagHandlers.Sr(_logger, bot, user, query, request, node),
                    "srai" => new AIMLTagHandlers.Srai(_logger, bot, user, request, node),
                    "star" => new AIMLTagHandlers.Star(_logger, query, request, node),
                    "system" => new AIMLTagHandlers.system(_logger, node),
                    "that" => new AIMLTagHandlers.that(_logger, user, request, node),
                    "thatstar" => new AIMLTagHandlers.Thatstar(_logger, query, request, node),
                    "think" => new AIMLTagHandlers.Think(_logger, node),
                    "topicstar" => new AIMLTagHandlers.Topicstar(_logger, query, request, node),
                    "uppercase" => new AIMLTagHandlers.uppercase(_logger, _config.Locale, node),
                    "version" => new AIMLTagHandlers.version(_logger, _config, node),
                    _ => null,
                };
            }
            return tagHandler;

        }

        /// <summary>
        /// Searches the CustomTag collection and processes the AIML if an appropriate tag handler is found
        /// </summary>
        /// <param name="user">the user who originated the request</param>
        /// <param name="query">the query that produced this node</param>
        /// <param name="request">the request from the user</param>
        /// <param name="result">the result to be sent to the user</param>
        /// <param name="node">the node to evaluate</param>
        /// <returns>the output string</returns>
        public AIMLTagHandler GetBespokeTags(User user, SubQuery query, Request request, Result result, XmlNode node)
        {
            if (_config.CustomTags.ContainsKey(node.Name.ToLower()))
            {
                TagHandler customTagHandler = _config.CustomTags[node.Name.ToLower()];

                AIMLTagHandler newCustomTag = customTagHandler.Instantiate(_config.LateBindingAssemblies);
                if (object.Equals(null, newCustomTag))
                {
                    return null;
                }
                else
                {
                    //newCustomTag.user = user;
                    //newCustomTag.query = query;
                    //newCustomTag.request = request;
                    //newCustomTag.result = result;
                    //newCustomTag.templateNode = node;
                    //newCustomTag.bot = this;
                    return newCustomTag;
                }
            }
            else
            {
                return null;
            }
        }




        /// <summary>
        /// Loads any custom tag handlers found in the dll referenced in the argument
        /// </summary>
        /// <param name="pathToDLL">the path to the dll containing the custom tag handling code</param>
        public void LoadCustomTagHandlers(string pathToDLL)
        {
            Assembly tagDLL = Assembly.LoadFrom(pathToDLL);
            Type[] tagDLLTypes = tagDLL.GetTypes();
            for (int i = 0; i < tagDLLTypes.Length; i++)
            {
                object[] typeCustomAttributes = tagDLLTypes[i].GetCustomAttributes(false);
                for (int j = 0; j < typeCustomAttributes.Length; j++)
                {
                    if (typeCustomAttributes[j] is CustomTagAttribute)
                    {
                        // We've found a custom tag handling class
                        // so store the assembly and store it away in the Dictionary<,> as a TagHandler class for 
                        // later usage

                        // store Assembly
                        if (!_config.LateBindingAssemblies.ContainsKey(tagDLL.FullName))
                        {
                            _config.LateBindingAssemblies.Add(tagDLL.FullName, tagDLL);
                        }

                        // create the TagHandler representation
                        TagHandler newTagHandler = new TagHandler
                        {
                            AssemblyName = tagDLL.FullName,
                            ClassName = tagDLLTypes[i].FullName,
                            TagName = tagDLLTypes[i].Name.ToLower()
                        };
                        if (_config.CustomTags.ContainsKey(newTagHandler.TagName))
                        {
                            throw new Exception("ERROR! Unable to add the custom tag: <" + newTagHandler.TagName + ">, found in: " + pathToDLL + " as a handler for this tag already exists.");
                        }
                        else
                        {
                            _config.CustomTags.Add(newTagHandler.TagName, newTagHandler);
                        }
                    }
                }
            }
        }

    }
}
