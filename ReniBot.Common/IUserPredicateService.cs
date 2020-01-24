namespace ReniBot.Common
{
    public interface IUserPredicateService
    {
        void addSetting(string key, string value);
        string grabSetting(string key);
        void removeSetting(string key);
    }
}