using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace AutoTeam;

[Config]
public class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "AutoTeam";

    [LocalizedPropertyName(CultureType.English, "Enable")]
    public bool Enabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.English, "GroupTemp")]
    public Dictionary<string, string> GroupTeamMap { get; set; } = new();

    public string GetTeamForGroup(string groupName)
    {
        return this.GroupTeamMap.TryGetValue(groupName, out var team) ? team : GetString("none");
    }

    protected override void SetDefault()
    {
        this.GroupTeamMap = new Dictionary<string, string>
        {
            {"guest", "pink"},
            {"default", "blue"},
            {"owner", "red"},
            {"admin", "green"},
            {"vip", "none"}
        };
    }
}