namespace ReniBot.Common
{
    public interface IBotUserService
    {
        int AddUser(int appId, string userKey, string topic = "*");
        string GetTopic(int userId);
        int GetUserId(int appId, string userKey);
        void UpdateTopic(int userId, string topic);
    }
}