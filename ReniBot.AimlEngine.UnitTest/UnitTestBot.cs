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
            AimlDoc doc = new AimlDoc();
            doc.appId = 1;
            string name = "AI.aiml";
            string path = "./" + name;
            doc.document = File.ReadAllText(path);
            doc.name = name;
            docList.Add(doc);
            mockApplicationService.Setup(x => x.GetAimlDocs(It.IsAny<int>())).Returns(docList);

            return mockApplicationService.Object;
        }

        [Fact]
        public void TestChat()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();
            var logger = factory.CreateLogger<Bot>();
            int userId = 1;

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
            string inputString = "";
            Normalize.ApplySubstitutions substitutor = new Normalize.ApplySubstitutions(substitutions, inputString);
            Regex strippers = new Regex("[^0-9a-zA-Z]");
            Normalize.StripIllegalCharacters stripper = new Normalize.StripIllegalCharacters(strippers);
            IAimlLoader loader = new AIMLLoader(logger, substitutor, stripper, true, 1000);
            Bot bot = new Bot(configurationLoader, logger, botUserService, resultService, loader, predicateService, requestService, applicationService);
            bot.Initialize();
            string expectedResult = "Because I am a piece of software.";
            var result = bot.Chat(new Request("Why do you live in a computer?", userId));
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result.Output);
        }
    }
}
