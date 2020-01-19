using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReniBot.Service;

namespace ReniBot.AimlEngine.Utils
{
    public class RequestFactory
    {
        public static Request Create(string input, int userId)
        {
            int requestId = UserRequestService.Add(input, userId);
            Request request = new Request(input, userId);
            request.id = requestId;

            return request;
        }
    }
}
