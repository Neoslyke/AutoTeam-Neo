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
    public Dictionary<string, string> GroupTemp { get; set; } = new();

    public string GetTeamForGroup(string groupName)
    {
        return this.GroupTemp.TryGetValue(groupName, out var team) ? team : "none";
    }

    protected override void SetDefault()
    {
        this.GroupTemp = new Dictionary<string, string>
        {
            {"guest", "pink"},
            {"default", "blue"},
            {"owner", "red"},
            {"admin", "green"},
            {"vip", "none"}
        };
    }
}