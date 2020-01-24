using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Net.Mail;
using ReniBot.AimlEngine.Utils;
using ReniBot.Common;
using ReniBot.Entities;
using Microsoft.Extensions.Logging;

namespace ReniBot.AimlEngine
{
    /// <summary>
    /// Encapsulates a bot. If no settings.xml file is found or referenced the bot will try to
    /// default to safe settings.
    /// </summary>
    public class Bot
    {
        public string AppKey { get; set; }
        private BotConfiguration _config;
        private int _appId;
        private ILogger _logger;
        /// <summary>
        /// The "brain" of the bot
        /// </summary>
        private Node _graphmaster;
        private AimlTagHandlerFactory _tagFactory;
        private IBotUserService _botUserService;
        private IUserResultService _userResultService;
        private readonly IAimlLoader _loader;
        private readonly IUserPredicateService _predicateService;
        private readonly IUserRequestService _requestService;

        /// <summary>
        /// Ctor
        /// </summary>
        public Bot(BotConfiguration configuration, ILogger logger, Node brain, IBotUserService botUserService, 
            IUserResultService resultService, IAimlLoader loader, IUserPredicateService predicateService, IUserRequestService requestService)
        {
            _config = configuration;
            _logger = logger;
            _graphmaster = brain;
            _tagFactory = new AimlTagHandlerFactory(logger, configuration);
            _botUserService = botUserService;
            _userResultService = resultService;
            _loader = loader;
            _predicateService = predicateService;
            _requestService = requestService;
        }


        /// <summary>
        /// Given some raw input and a unique ID creates a response for a new user
        /// </summary>
        /// <param name="rawInput">the raw input</param>
        /// <param name="UserGUID">an ID for the new user (referenced in the result object)</param>
        /// <returns>the result to be output to the user</returns>
        public Result Chat(string rawInput, string UserGUID)
        {
            int userId = _botUserService.GetUserId(_appId, UserGUID);
            Request request = new Request(rawInput, userId);
            return this.Chat(request);
        }

        /// <summary>
        /// Given a request containing user input, produces a result from the bot
        /// </summary>
        /// <param name="request">the request from the user</param>
        /// <returns>the result to be output to the user</returns>
        public Result Chat(Request request)
        {
            Result result = new Result(request);

            if (_config.isAcceptingUserInput)
            {
                // Normalize the input
                ReniBot.AimlEngine.Normalize.SplitIntoSentences splitter = new ReniBot.AimlEngine.Normalize.SplitIntoSentences(_config);
                string[] rawSentences = splitter.Transform(request.rawInput);
                foreach (string sentence in rawSentences)
                {
                    result.InputSentences.Add(sentence);
                    string topic = _botUserService.GetTopic(request.userId);
                    string lastOutput = _userResultService.GetLastOutput(request.userId);
                    string path = _loader.GeneratePath(sentence, lastOutput, topic, true);
                    result.NormalizedPaths.Add(path);
                }

                // grab the templates for the various sentences from the graphmaster
                foreach (string path in result.NormalizedPaths)
                {
                    Utils.SubQuery query = new SubQuery(path);
                    query.Template = _graphmaster.evaluate(path, query, request, MatchState.UserInput, new StringBuilder(), _config.TimeOut);
                    result.HasTimedOut = request.hasTimedOut;
                    result.SubQueries.Add(query);
                }

                // process the templates into appropriate output
                foreach (SubQuery query in result.SubQueries)
                {
                    if (query.Template.Length > 0)
                    {
                        try
                        {
                            XmlNode templateNode = AIMLTagHandler.getNode(query.Template);
                            string outputSentence = this.processNode(templateNode, query, request, result, new User(_botUserService, _predicateService, _userResultService, _requestService) { UserId = request.userId });
                            if (outputSentence.Length > 0)
                            {
                                result.OutputSentences.Add(outputSentence);
                            }
                        }
                        catch (Exception e)
                        {
                            if (_config.WillCallHome)
                            {
                                phoneHome(e.Message, request);
                            }
                            _logger.LogWarning("A problem was encountered when trying to process the input: " + request.rawInput + " with the template: \"" + query.Template + "\"");
                        }
                    }
                }
            }
            else
            {
                result.OutputSentences.Add(_config.NotAcceptingUserInputMessage);
            }

            // populate the Result object
            result.Duration = DateTime.Now - request.StartedOn;
            _userResultService.Add(result.Duration.Milliseconds, result.HasTimedOut, result.RawOutput, result.requestId, result.UserId);

            return result;
        }

        public void Learn(XmlDocument doc, string filename)
        {
            _loader.loadAIMLFromXML(doc, filename);
        }

        /// <summary>
        /// Recursively evaluates the template nodes returned from the bot
        /// </summary>
        /// <param name="node">the node to evaluate</param>
        /// <param name="query">the query that produced this node</param>
        /// <param name="request">the request from the user</param>
        /// <param name="result">the result to be sent to the user</param>
        /// <param name="user">the user who originated the request</param>
        /// <returns>the output string</returns>
        private string processNode(XmlNode node, SubQuery query, Request request, Result result, User user)
        {
            // check for timeout (to avoid infinite loops)
            if (request.StartedOn.AddMilliseconds(_config.TimeOut) < DateTime.Now)
            {
                //_logger.LogWarning("Request timeout. User: " + request.user.UserKey + " raw input: \"" + request.rawInput + "\" processing template: \""+query.Template+"\"");
                request.hasTimedOut = true;
                return string.Empty;
            }
                        
            // process the node
            string tagName = node.Name.ToLower();
            if (tagName == "template")
            {
                StringBuilder templateResult = new StringBuilder();
                if (node.HasChildNodes)
                {
                    // recursively check
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        templateResult.Append(this.processNode(childNode, query, request, result, user));
                    }
                }
                return templateResult.ToString();
            }
            else
            {
                AIMLTagHandler tagHandler = _tagFactory.CreateTagHandler(tagName, this, user, query, request, result, node);
                if (object.Equals(null, tagHandler))
                {
                    return node.InnerText;
                }
                else
                {
                    if (tagHandler.isRecursive)
                    {
                        if (node.HasChildNodes)
                        {
                            // recursively check
                            foreach (XmlNode childNode in node.ChildNodes)
                            {
                                if (childNode.NodeType != XmlNodeType.Text)
                                {
                                    childNode.InnerXml = this.processNode(childNode, query, request, result, user);
                                }
                            }
                        }
                        return tagHandler.Transform();
                    }
                    else
                    {
                        string resultNodeInnerXML = tagHandler.Transform();
                        XmlNode resultNode = AIMLTagHandler.getNode("<node>" + resultNodeInnerXML + "</node>");
                        if (resultNode.HasChildNodes)
                        {
                            StringBuilder recursiveResult = new StringBuilder();
                            // recursively check
                            foreach (XmlNode childNode in resultNode.ChildNodes)
                            {
                                recursiveResult.Append(this.processNode(childNode, query, request, result, user));
                            }
                            return recursiveResult.ToString();
                        }
                        else
                        {
                            return resultNode.InnerXml;
                        }
                    }
                }
            }
        }

 
        /// <summary>
        /// Attempts to send an email to the botmaster at the AdminEmail address setting with error messages
        /// resulting from a query to the bot
        /// </summary>
        /// <param name="errorMessage">the resulting error message</param>
        /// <param name="request">the request object that encapsulates all sorts of useful information</param>
        public void phoneHome(string errorMessage, Request request)
        {
            MailAddress fromAddress = new MailAddress("donotreply@ReniBot.AimlEngine.com");
            MailAddress toAddress = new MailAddress(_config.AdminEmail);
            MailMessage msg = new MailMessage(fromAddress, toAddress);
            msg.Subject = "WARNING! ReniBot.AimlEngine has encountered a problem...";
            string message = @"Dear Botmaster,

This is an automatically generated email to report errors with your bot.

At *TIME* the bot encountered the following error:

""*MESSAGE*""

whilst processing the following input:

""*RAWINPUT*""

from the user with an id of: *USER*

The normalized paths generated by the raw input were as follows:

*PATHS*

Please check your AIML!

Regards,

The ReniBot.AimlEngine program.
";
            message = message.Replace("*TIME*", DateTime.Now.ToString());
            message = message.Replace("*MESSAGE*", errorMessage);
            message = message.Replace("*RAWINPUT*", request.rawInput);
            message = message.Replace("*USER*", request.userId.ToString());
            StringBuilder paths = new StringBuilder();
            //foreach(string path in request.result.NormalizedPaths)
            //{
            //    paths.Append(path+Environment.NewLine);
            //}
            message = message.Replace("*PATHS*", paths.ToString());
            msg.Body = message;
            msg.IsBodyHtml=false;
            try
            {
                if (msg.To.Count > 0)
                {
                    SmtpClient client = new SmtpClient();
                    client.Send(msg);
                }
            }
            catch
            {
                // if we get here then we can't really do much more
            }
        }
    }
}