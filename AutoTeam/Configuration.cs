using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace AutoTeam;

[Config]
public class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "AutoTeam";

    public bool Enable { get; set; } = true;

    public bool AutoAcceptRequest { get; set; } = true;

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