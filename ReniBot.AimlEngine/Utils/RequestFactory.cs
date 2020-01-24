using ReniBot.Common;

namespace ReniBot.AimlEngine.Utils
{
    public class RequestFactory
    {
        private readonly IUserRequestService _userRequestService;
        public RequestFactory(IUserRequestService userRequestService)
        {
            _userRequestService = userRequestService;
        }
        public  Request Create(string input, int userId)
        {
            int requestId = _userRequestService.Add(input, userId);
            Request request = new Request(input, userId);
            request.id = requestId;

            return request;
        }
    }
}
