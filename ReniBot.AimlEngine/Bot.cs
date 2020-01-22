using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Net.Mail;

using ReniBot.AimlEngine.Utils;
using ReniBot.Service;
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
        private string _appKey;
        private BotConfiguration _config;
        private int _appId;
        private ILogger _logger;
        private IAimlLoader _loader;
        /// <summary>
        /// The "brain" of the bot
        /// </summary>
        private Node _graphmaster;
        private AimlTagHandlerFactory _tagFactory;


        /// <summary>
        /// Ctor
        /// </summary>
        public Bot(string appKey, BotConfiguration configuration, ILogger logger, IAimlLoader loader)
        {
            _appKey = appKey;
            _config = configuration;
            _logger = logger;
            _loader = loader;
            _tagFactory = new AimlTagHandlerFactory(logger, configuration);
        }


        public void initialize()
        {
            _appId = ApplicationService.GetApplicationIdFromKey(_appKey);
            if (_appId < 1)
                throw new Exception("Could not find application with that appKey");
            List<AimlDoc> docList = ApplicationService.GetAimlDocs(_appId);
            foreach (var aimlDoc in docList)
                loadAIMLFromXML(aimlDoc.XmlDoc, aimlDoc.name);

        }

        /// <summary>
        /// Loads AIML from .aiml files into the graphmaster "brain" of the bot
        /// </summary>
        public void loadAIMLFromFiles()
        {
           _graphmaster = _loader.LoadAIML(_config.PathToAIML);
        }

        /// <summary>
        /// Allows the bot to load a new XML version of some AIML
        /// </summary>
        /// <param name="newAIML">The XML document containing the AIML</param>
        /// <param name="filename">The originator of the XML document</param>
        public void loadAIMLFromXML(XmlDocument newAIML, string filename)
        {
            _graphmaster =  _loader.loadAIMLFromXML(newAIML, filename);

        }

        /// <summary>
        /// Given some raw input and a unique ID creates a response for a new user
        /// </summary>
        /// <param name="rawInput">the raw input</param>
        /// <param name="UserGUID">an ID for the new user (referenced in the result object)</param>
        /// <returns>the result to be output to the user</returns>
        public Result Chat(string rawInput, string UserGUID)
        {
            int userId = BotUserService.GetUserId(_appId, UserGUID);
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
                    string topic = BotUserService.GetTopic(request.userId);
                    UserResultService service = new UserResultService(request.userId);
                    string lastOutput = service.GetLastOutput();
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
                            string outputSentence = this.processNode(templateNode, query, request, result, new User(request.userId));
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
            UserResultService rservice = new UserResultService(request.userId);
            rservice.Add(result.Duration.Milliseconds, result.HasTimedOut, result.RawOutput, result.requestId, result.UserId);

            return result;
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
