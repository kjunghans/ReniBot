using ReniBot.Entities;

namespace ReniBot.Common
{
    public interface IUserResultService
    {
        void Add(int duration, bool hasTimedOut, string rawOutput, int requestId, int userId);
        int Count(int userId);
        BotUserResult GetLast(int userId);
        string GetLastOutput(int userId);
        BotUserResult GetNResult(int index, int userId);
    }
}