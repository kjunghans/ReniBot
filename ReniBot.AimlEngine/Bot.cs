using Microsoft.Extensions.Logging;
using ReniBot.AimlEngine.Utils;
using ReniBot.Common;
using ReniBot.Entities;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Xml;

namespace ReniBot.AimlEngine
{
    /// <summary>
    /// Encapsulates a bot. If no settings.xml file is found or referenced the bot will try to
    /// default to safe settings.
    /// </summary>
    public class Bot: IBot
    {
        public string AppKey { get; set; }
        private BotConfiguration _config;
        private int _appId;
        private readonly ILogger _logger;
        /// <summary>
        /// The "brain" of the bot
        /// </summary>
        private readonly Node _graphmaster;
        private AimlTagHandlerFactory _tagFactory;
        private readonly IBotUserService _botUserService;
        private readonly IUserResultService _userResultService;
        private readonly IAimlLoader _loader;
        private readonly IUserPredicateService _predicateService;
        private readonly IUserRequestService _requestService;
        private readonly IApplicationService _applicationService;
        private readonly IBotConfigurationLoader _configurationLoader;
        private BotContext _context;

        /// <summary>
        /// Ctor
        /// </summary>
        public Bot(IBotConfigurationLoader configurationLoader, ILogger logger, IBotUserService botUserService,
            IUserResultService resultService, IAimlLoader loader, IUserPredicateService predicateService,
            IUserRequestService requestService, IApplicationService applicationService)
        {
            _logger = logger;
            _botUserService = botUserService;
            _userResultService = resultService;
            _loader = loader;
            _predicateService = predicateService;
            _requestService = requestService;
            _applicationService = applicationService;
            _configurationLoader = configurationLoader;
            _graphmaster = new Node();
        }

        public void Initialize()
        {
            _appId = _applicationService.GetApplicationIdFromKey(AppKey);
            if (_appId < 1)
                throw new Exception("Could not find application with that appKey");
            _config = _configurationLoader.loadSettings();
            _context = new BotContext() { Bot = this, Configuration = _config };
            _tagFactory = new AimlTagHandlerFactory(_logger, _context);
            List<AimlDoc> docList = _applicationService.GetAimlDocs(_appId);
            foreach (var aimlDoc in docList)
                Learn(aimlDoc.XmlDoc, aimlDoc.name);

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
            return Chat(request);
        }

        /// <summary>
        /// Given a request containing user input, produces a result from the bot
        /// </summary>
        /// <param name="request">the request from the user</param>
        /// <returns>the result to be output to the user</returns>
        public Result Chat(Request request)
        {
            Result result = new Result(request);
            _context.Request = request;
            _context.User = new User(_predicateService, _userResultService, _requestService) { UserId = request.UserId };

            if (_config.isAcceptingUserInput)
            {
                // Normalize the input
                ReniBot.AimlEngine.Normalize.SplitIntoSentences splitter = new ReniBot.AimlEngine.Normalize.SplitIntoSentences(_config);
                string[] rawSentences = splitter.Transform(request.RawInput);
                foreach (string sentence in rawSentences)
                {
                    result.InputSentences.Add(sentence);
                    string topic = _botUserService.GetTopic(request.UserId);
                    string lastOutput = _userResultService.GetLastOutput(request.UserId);
                    string path = _loader.GeneratePath(sentence, lastOutput, topic, true);
                    result.NormalizedPaths.Add(path);
                }

                // grab the templates for the various sentences from the graphmaster
                foreach (string path in result.NormalizedPaths)
                {
                    Utils.SubQuery query = new SubQuery(path);
                    query.Template = _graphmaster.Evaluate(path, query, request, MatchState.UserInput, new StringBuilder(), _config.TimeOut);
                    result.HasTimedOut = request.HasTimedOut;
                    result.SubQueries.Add(query);
                }

                // process the templates into appropriate output
                foreach (SubQuery query in result.SubQueries)
                {
                    if (query.Template.Length > 0)
                    {
                        try
                        {

                            XmlNode templateNode = AIMLTagHandler.GetNode(query.Template);
                            string outputSentence = ProcessNode(templateNode, query, request, result, _context.User);
                            if (outputSentence.Length > 0)
                            {
                                result.OutputSentences.Add(outputSentence);
                            }
                        }
                        catch (Exception e)
                        {
                            if (_config.WillCallHome)
                            {
                                PhoneHome(e.Message, request);
                            }
                            _logger.LogWarning("A problem was encountered when trying to process the input: " + request.RawInput + " with the template: \"" + query.Template + "\"");
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
            _userResultService.Add(result.Duration.Milliseconds, result.HasTimedOut, result.RawOutput, result.RequestId, result.UserId);

            return result;
        }

        public void Learn(XmlDocument doc, string filename)
        {
            _loader.SetGraphMaster(_graphmaster);
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
        private string ProcessNode(XmlNode node, SubQuery query, Request request, Result result, User user)
        {
            // check for timeout (to avoid infinite loops)
            if (request.StartedOn.AddMilliseconds(_config.TimeOut) < DateTime.Now)
            {
                //_logger.LogWarning("Request timeout. User: " + request.user.UserKey + " raw input: \"" + request.rawInput + "\" processing template: \""+query.Template+"\"");
                request.HasTimedOut = true;
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
                        templateResult.Append(ProcessNode(childNode, query, request, result, user));
                    }
                }
                return templateResult.ToString();
            }
            else
            {
                AIMLTagHandler tagHandler = _tagFactory.CreateTagHandler(tagName);
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
                                    childNode.InnerXml = ProcessNode(childNode, query, request, result, user);
                                }
                            }
                        }
                        return tagHandler.ProcessChange(node);
                    }
                    else
                    {
                        string resultNodeInnerXML = tagHandler.ProcessChange(node);
                        XmlNode resultNode = AIMLTagHandler.GetNode("<node>" + resultNodeInnerXML + "</node>");
                        if (resultNode.HasChildNodes)
                        {
                            StringBuilder recursiveResult = new StringBuilder();
                            // recursively check
                            foreach (XmlNode childNode in resultNode.ChildNodes)
                            {
                                recursiveResult.Append(ProcessNode(childNode, query, request, result, user));
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
        public void PhoneHome(string errorMessage, Request request)
        {
            MailAddress fromAddress = new MailAddress("donotreply@ReniBot.AimlEngine.com");
            MailAddress toAddress = new MailAddress(_config.AdminEmail);
            MailMessage msg = new MailMessage(fromAddress, toAddress)
            {
                Subject = "WARNING! ReniBot.AimlEngine has encountered a problem..."
            };
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
            message = message.Replace("*RAWINPUT*", request.RawInput);
            message = message.Replace("*USER*", request.UserId.ToString());
            StringBuilder paths = new StringBuilder();
            //foreach(string path in request.result.NormalizedPaths)
            //{
            //    paths.Append(path+Environment.NewLine);
            //}
            message = message.Replace("*PATHS*", paths.ToString());
            msg.Body = message;
            msg.IsBodyHtml = false;
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
