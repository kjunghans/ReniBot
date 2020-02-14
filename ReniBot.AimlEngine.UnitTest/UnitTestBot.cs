using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ReniBot.AimlEngine.Utils;
using ReniBot.Common;
using ReniBot.Entities;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Xunit;

namespace ReniBot.AimlEngine.UnitTest
{
    public class UnitTestBot
    {
        private IApplicationService GetMockAppService()
        {
            var mockApplicationService = new Mock<IApplicationService>();
            mockApplicationService.Setup(x => x.GetApplicationIdFromKey(It.IsAny<string>())).Returns(1);
            List<AimlDoc> docList = new List<AimlDoc>();
            AimlDoc doc = new AimlDoc
            {
                appId = 1
            };
            string name = "AI.aiml";
            string path = "./" + name;
            doc.document = File.ReadAllText(path);
            doc.name = name;
            docList.Add(doc);
            mockApplicationService.Setup(x => x.GetAimlDocs(It.IsAny<int>())).Returns(docList);

            return mockApplicationService.Object;
        }

        private Bot CreateBot()
        {
            var serviceProvider = new ServiceCollection()
                 .AddLogging()
                 .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();
            var logger = factory.CreateLogger<Bot>();

            var mockUserService = new Mock<IBotUserService>();
            mockUserService.Setup(x => x.GetTopic(It.IsAny<int>())).Returns("");

            var mockResultService = new Mock<IUserResultService>();
            mockResultService.Setup(x => x.GetLastOutput(It.IsAny<int>())).Returns("");

            var mockPredicateService = new Mock<IUserPredicateService>();

            var mockRequestService = new Mock<IUserRequestService>();


            IBotConfigurationLoader configurationLoader = new ConfigurationLoader();
            IBotUserService botUserService = mockUserService.Object;
            IUserResultService resultService = mockResultService.Object;
            IUserPredicateService predicateService = mockPredicateService.Object;
            IUserRequestService requestService = mockRequestService.Object;
            IApplicationService applicationService = GetMockAppService();

            SettingsDictionary substitutions = new SettingsDictionary();
            Normalize.ApplySubstitutions substitutor = new Normalize.ApplySubstitutions(substitutions);
            Regex strippers = new Regex("[^0-9a-zA-Z]");
            Normalize.StripIllegalCharacters stripper = new Normalize.StripIllegalCharacters(strippers);
            IAimlLoader loader = new AIMLLoader(logger, substitutor, stripper, true, 1000);
            Bot bot = new Bot(configurationLoader, logger, botUserService, resultService, loader, predicateService, requestService, applicationService);
            return bot;

        }

        [Fact]
        public void TestBasicChat()
        {
            int userId = 1;
            Bot bot = CreateBot();
            bot.Initialize();
            string expectedResult = "Because I am a piece of software.";
            var result = bot.Chat(new Request("Why do you live in a computer?", userId));
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result.Output);
        }

        [Fact]
        public void TestRandomTag()
        {
            int userId = 1;
            Bot bot = CreateBot();
            bot.Initialize();
            List<string> expectedResults = new List<string>();
            expectedResults.Add("Sure is!");
            expectedResults.Add("It can be fun at times...");
            expectedResults.Add("Yes");
            expectedResults.Add("I enjoy being a computer.");

            var result = bot.Chat(new Request("Is it cool to be a computer?", userId));
            Assert.NotNull(result);
            bool foundMatch = false;
            foreach(string expectedResult in expectedResults)
            {
                if (expectedResult.Equals(result.Output))
                    foundMatch = true;
            }
            Assert.True(foundMatch);

        }

        [Fact]
        public void TestBotTag()
        {
            int userId = 1;
            Bot bot = CreateBot();
            bot.Initialize();
            string expectedResult = "As I am a autonomous computer program I do not have that bodily function.";
            var result = bot.Chat(new Request("Your piss stinks", userId));
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result.Output);
        }

        [Fact]
        public void TestSetTag()
        {
            int userId = 1;
            Bot bot = CreateBot();
            bot.Initialize();
            string expectedResult = "Computers cannot die as we're not really alive.";
            var result = bot.Chat(new Request("Computers should die", userId));
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result.Output);
        }

    }
}
