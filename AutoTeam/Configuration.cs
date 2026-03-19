using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace AutoTeam;

[Config]
public class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "AutoTeam";

    [LocalizedPropertyName(CultureType.Chinese, "开启插件")]
    [LocalizedPropertyName(CultureType.English, "Enable")]
    public bool Enabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "组对应的队伍")]
    [LocalizedPropertyName(CultureType.English, "Group Team Mapping")]
    public Dictionary<string, string> GroupTeamMap { get; set; } = new();

    public string GetTeamForGroup(string groupName)
    {
        return this.GroupTeamMap.TryGetValue(groupName, out var team) ? team : "none-configured";
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