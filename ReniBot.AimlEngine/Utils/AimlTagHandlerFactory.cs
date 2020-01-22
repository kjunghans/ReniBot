using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ReniBot.AimlEngine.Utils
{
    public class AimlTagHandlerFactory
    {
        private readonly ILogger _logger;
        private BotConfiguration _config;

        public AimlTagHandlerFactory(ILogger logger, BotConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public AIMLTagHandler CreateTagHandler(string tagName, Bot bot, User user, SubQuery query, Request request, Result result, XmlNode node)
        {
            AIMLTagHandler tagHandler = null;
            tagHandler = this.getBespokeTags(user, query, request, result, node);
            if (object.Equals(null, tagHandler))
            {
                switch (tagName)
                {
                    case "bot":
                        tagHandler = new AIMLTagHandlers.bot(_logger, _config.GlobalSettings, node);
                        break;
                    case "condition":
                        tagHandler = new AIMLTagHandlers.condition(_logger, user, node);
                        break;
                    case "date":
                        tagHandler = new AIMLTagHandlers.date(_logger, _config.Locale, node);
                        break;
                    case "formal":
                        tagHandler = new AIMLTagHandlers.formal(_logger,  node);
                        break;
                    case "gender":
                        tagHandler = new AIMLTagHandlers.gender(_logger, _config, user, query, request, result, node);
                        break;
                    case "get":
                        tagHandler = new AIMLTagHandlers.get(_logger, _config, user, node);
                        break;
                    case "gossip":
                        tagHandler = new AIMLTagHandlers.gossip(_logger, user, node);
                        break;
                    case "id":
                        tagHandler = new AIMLTagHandlers.id(_logger, user, node);
                        break;
                    case "input":
                        tagHandler = new AIMLTagHandlers.input(_logger, user,  request, node);
                        break;
                    case "javascript":
                        tagHandler = new AIMLTagHandlers.javascript(_logger,  node);
                        break;
                    case "learn":
                        tagHandler = new AIMLTagHandlers.learn(_logger, bot, node);
                        break;
                    case "lowercase":
                        tagHandler = new AIMLTagHandlers.lowercase(_logger, _config.Locale, node);
                        break;
                    case "person":
                        tagHandler = new AIMLTagHandlers.person(_logger, _config, user, query, request, result, node);
                        break;
                    case "person2":
                        tagHandler = new AIMLTagHandlers.person2(_logger, _config, user, query, request, result, node);
                        break;
                    case "random":
                        tagHandler = new AIMLTagHandlers.random(_logger, node);
                        break;
                    case "sentence":
                        tagHandler = new AIMLTagHandlers.sentence(_logger, _config, user, query, request, result, node);
                        break;
                    case "set":
                        tagHandler = new AIMLTagHandlers.set(_logger, _config, user, node);
                        break;
                    case "size":
                        tagHandler = new AIMLTagHandlers.size(_logger, _config.Size, node);
                        break;
                    case "sr":
                        tagHandler = new AIMLTagHandlers.sr(_logger, bot, user, query, request, result, node);
                        break;
                    case "srai":
                        tagHandler = new AIMLTagHandlers.srai(_logger, bot,  user, request, node);
                        break;
                    case "star":
                        tagHandler = new AIMLTagHandlers.star(_logger, user, query, request, result, node);
                        break;
                    case "system":
                        tagHandler = new AIMLTagHandlers.system(_logger, node);
                        break;
                    case "that":
                        tagHandler = new AIMLTagHandlers.that(_logger, user, request, node);
                        break;
                    case "thatstar":
                        tagHandler = new AIMLTagHandlers.thatstar(_logger, user, query, request, result, node);
                        break;
                    case "think":
                        tagHandler = new AIMLTagHandlers.think(_logger, user, query, request, result, node);
                        break;
                    case "topicstar":
                        tagHandler = new AIMLTagHandlers.topicstar(_logger, bot, user, query, request, result, node);
                        break;
                    case "uppercase":
                        tagHandler = new AIMLTagHandlers.uppercase(_logger, _config.Locale, node);
                        break;
                    case "version":
                        tagHandler = new AIMLTagHandlers.version(_logger, _config, node);
                        break;
                    default:
                        tagHandler = null;
                        break;
                }

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
        public AIMLTagHandler getBespokeTags(User user, SubQuery query, Request request, Result result, XmlNode node)
        {
            if (_config.CustomTags.ContainsKey(node.Name.ToLower()))
            {
                TagHandler customTagHandler = (TagHandler)_config.CustomTags[node.Name.ToLower()];

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
        public void loadCustomTagHandlers(string pathToDLL)
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
                        TagHandler newTagHandler = new TagHandler();
                        newTagHandler.AssemblyName = tagDLL.FullName;
                        newTagHandler.ClassName = tagDLLTypes[i].FullName;
                        newTagHandler.TagName = tagDLLTypes[i].Name.ToLower();
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
