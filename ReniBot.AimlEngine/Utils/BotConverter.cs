using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReniBot.Entities;
using ReniBot.Service;

namespace ReniBot.AimlEngine.Utils
{
    public class BotConverter
    {
        public static Result Convert(BotUserResult input)
        {
            BotUserRequest userRequest = UserRequestService.GetRequestById(input.requestId);
            Result output = new Result(Convert(userRequest), input.rawOutput, new TimeSpan(0,0,0,0,input.Duration),
                input.hasTimedOut);

            return output;
        }

        public static Request Convert(BotUserRequest input)
        {
            Request output = new Request()
            {
                userId = input.userId,
                hasTimedOut = false,
                rawInput = input.rawInput,
                StartedOn = input.startedOn,
                id = input.id
            };
            return output;
        }
    }
}
