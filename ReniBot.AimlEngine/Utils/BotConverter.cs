using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReniBot.Entities;
using ReniBot.Common;

namespace ReniBot.AimlEngine.Utils
{
    public class BotConverter
    {
        private readonly IUserRequestService _userRequestService;

        public BotConverter(IUserRequestService userRequestService)
        {
            _userRequestService = userRequestService;
        }

        public  Result Convert(BotUserResult input)
        {
            BotUserRequest userRequest = _userRequestService.GetRequestById(input.requestId);
            Result output = new Result(Convert(userRequest), input.rawOutput, new TimeSpan(0,0,0,0,input.Duration),
                input.hasTimedOut);

            return output;
        }

        public  Request Convert(BotUserRequest input)
        {
            Request output = new Request()
            {
                UserId = input.userId,
                HasTimedOut = false,
                RawInput = input.rawInput,
                StartedOn = input.startedOn,
                Id = input.id
            };
            return output;
        }
    }
}
