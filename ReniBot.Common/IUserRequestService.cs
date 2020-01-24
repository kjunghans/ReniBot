using ReniBot.Entities;

namespace ReniBot.Common
{
    public interface IUserRequestService
    {
        BotUserRequest GetRequestById(int id);

        int Add(string rawInput, int userId);
    }
}