using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace ReniBot.AimlEngine.Utils
{
    public class AimlTagHandlerFactory
    {
        private readonly ILogger _logger;
        private readonly BotContext _context;
        private readonly Dictionary<string, AIMLTagHandler> _tags = new Dictionary<string, AIMLTagHandler>();

        public AimlTagHandlerFactory(ILogger logger, BotContext context)
        {
            _logger = logger;
            _context = context;
            LoadStandardTags();
        }

        private void LoadStandardTags()
        {
            _tags.Add("bot", new AIMLTagHandlers.bot(_logger, _context));
            _tags.Add("condition", new AIMLTagHandlers.condition(_logger, _context));
            _tags.Add("date", new AIMLTagHandlers.date(_logger, _context));
            _tags.Add("formal", new AIMLTagHandlers.formal(_logger, _context));
            _tags.Add("gender", new AIMLTagHandlers.Gender(_logger, _context));
            _tags.Add("get", new AIMLTagHandlers.Get(_logger, _context));
            _tags.Add("gossip", new AIMLTagHandlers.gossip(_logger, _context));
            _tags.Add("id", new AIMLTagHandlers.id(_logger, _context));
            _tags.Add("input", new AIMLTagHandlers.input(_logger, _context));
            _tags.Add("javascript", new AIMLTagHandlers.javascript(_logger, _context));
            _tags.Add("learn", new AIMLTagHandlers.learn(_logger, _context));
            _tags.Add("lowercase", new AIMLTagHandlers.lowercase(_logger, _context));
            _tags.Add("person", new AIMLTagHandlers.Person(_logger, _context));
            _tags.Add("person2", new AIMLTagHandlers.Person2(_logger, _context));
            _tags.Add("random", new AIMLTagHandlers.random(_logger, _context));
            _tags.Add("sentence", new AIMLTagHandlers.Sentence(_logger, _context));
            _tags.Add("set", new AIMLTagHandlers.set(_logger, _context));
            _tags.Add("size", new AIMLTagHandlers.size(_logger, _context));
            _tags.Add("sr", new AIMLTagHandlers.Sr(_logger, _context));
            _tags.Add("srai", new AIMLTagHandlers.Srai(_logger, _context));
            _tags.Add("star", new AIMLTagHandlers.Star(_logger, _context));
            _tags.Add("system", new AIMLTagHandlers.system(_logger, _context));
            _tags.Add("that", new AIMLTagHandlers.that(_logger, _context));
            _tags.Add("thatstar", new AIMLTagHandlers.Thatstar(_logger, _context));
            _tags.Add("think", new AIMLTagHandlers.Think(_logger, _context));
            _tags.Add("topicstar", new AIMLTagHandlers.Topicstar(_logger, _context));
            _tags.Add("uppercase", new AIMLTagHandlers.uppercase(_logger, _context));
            _tags.Add("version", new AIMLTagHandlers.version(_logger, _context));

        }

        public void RegisterTagHandler(string tagName, AIMLTagHandler tag)
        {
            _tags.Add(tagName, tag);
        }

        public AIMLTagHandler CreateTagHandler(string tagName)
        {
            AIMLTagHandler tagHandler;
            if (_tags.TryGetValue(tagName, out tagHandler))
                return tagHandler;
            else
                return null;

        }

        //TODO: Determine if custom tags is really necessary
        /// <summary>
        /// Searches the CustomTag collection and processes the AIML if an appropriate tag handler is found
        /// </summary>
        /// <param name="user">the user who originated the request</param>
        /// <param name="query">the query that produced this node</param>
        /// <param name="request">the request from the user</param>
        /// <param name="result">the result to be sent to the user</param>
        /// <param name="node">the node to evaluate</param>
        /// <returns>the output string</returns>
        //public AIMLTagHandler GetBespokeTags(User user, SubQuery query, Request request, Result result, XmlNode node)
        //{
        //    if (_config.CustomTags.ContainsKey(node.Name.ToLower()))
        //    {
        //        TagHandler customTagHandler = _config.CustomTags[node.Name.ToLower()];

        //        AIMLTagHandler newCustomTag = customTagHandler.Instantiate(_config.LateBindingAssemblies);
        //        if (object.Equals(null, newCustomTag))
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            //TODO: Need to figure out design pattern where only necessary objects are passed to object
        //            //newCustomTag.user = user;
        //            //newCustomTag.query = query;
        //            //newCustomTag.request = request;
        //            //newCustomTag.result = result;
        //            //newCustomTag.templateNode = node;
        //            //newCustomTag.bot = this;
        //            return newCustomTag;
        //        }
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}




        /// <summary>
        /// Loads any custom tag handlers found in the dll referenced in the argument
        /// </summary>
        /// <param name="pathToDLL">the path to the dll containing the custom tag handling code</param>
        //public void LoadCustomTagHandlers(string pathToDLL)
        //{
        //    Assembly tagDLL = Assembly.LoadFrom(pathToDLL);
        //    Type[] tagDLLTypes = tagDLL.GetTypes();
        //    for (int i = 0; i < tagDLLTypes.Length; i++)
        //    {
        //        object[] typeCustomAttributes = tagDLLTypes[i].GetCustomAttributes(false);
        //        for (int j = 0; j < typeCustomAttributes.Length; j++)
        //        {
        //            if (typeCustomAttributes[j] is CustomTagAttribute)
        //            {
        //                // We've found a custom tag handling class
        //                // so store the assembly and store it away in the Dictionary<,> as a TagHandler class for 
        //                // later usage

        //                // store Assembly
        //                if (!_config.LateBindingAssemblies.ContainsKey(tagDLL.FullName))
        //                {
        //                    _config.LateBindingAssemblies.Add(tagDLL.FullName, tagDLL);
        //                }

        //                // create the TagHandler representation
        //                TagHandler newTagHandler = new TagHandler
        //                {
        //                    AssemblyName = tagDLL.FullName,
        //                    ClassName = tagDLLTypes[i].FullName,
        //                    TagName = tagDLLTypes[i].Name.ToLower()
        //                };
        //                if (_config.CustomTags.ContainsKey(newTagHandler.TagName))
        //                {
        //                    throw new Exception("ERROR! Unable to add the custom tag: <" + newTagHandler.TagName + ">, found in: " + pathToDLL + " as a handler for this tag already exists.");
        //                }
        //                else
        //                {
        //                    _config.CustomTags.Add(newTagHandler.TagName, newTagHandler);
        //                }
        //            }
        //        }
        //    }
        //}

    }
}
