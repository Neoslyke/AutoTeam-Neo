using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace AutoTeam;

[Config]
public class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "AutoTeam";

    public bool Enabled { get; set; } = true;

    public Dictionary<string, string> GroupTeamMap { get; set; } = new();

    public string GetTeamForGroup(string groupName)
    {
        return this.GroupTeamMap.TryGetValue(groupName, out var team) ? team : "none";
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