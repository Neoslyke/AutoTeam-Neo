using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace AutoTeam;

[Config]
public class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "AutoTeam";

    [LocalizedPropertyName(CultureType.English, "Enable")]
    [LocalizedPropertyName(CultureType.Chinese, "启用")]
    public bool Enabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.English, "GroupTeams")]
    [LocalizedPropertyName(CultureType.Chinese, "组队配置")]
    public Dictionary<string, string> GroupTeams { get; set; } = new();

    public string GetTeamForGroup(string groupName)
    {
        if (this.GroupTeams.TryGetValue(groupName, out var team))
            return team;
            
        // Try case-insensitive lookup
        var key = this.GroupTeams.Keys.FirstOrDefault(k => 
            k.Equals(groupName, StringComparison.OrdinalIgnoreCase));
            
        return key != null ? this.GroupTeams[key] : "none";
    }

    protected override void SetDefault()
    {
        this.GroupTeams = new Dictionary<string, string>
        {
            {"guest", "pink"},
            {"default", "blue"},
            {"owner", "red"},
            {"admin", "green"},
            {"vip", "none"},
        };
    }
}