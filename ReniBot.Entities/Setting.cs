namespace ReniBot.Entities
{
    public enum SettingType
    {
        GlobalSettings, Person2Substitutions, PersonSubstitutions,
        GenderSubstitutions, DefaultPredicates, Substitutions, Splitters
    };

    public class Setting
    {
        public SettingType type { get; set; }
        public string key { get; set; }
        public string val { get; set; }
        public int appId { get; set; }
    }
}
