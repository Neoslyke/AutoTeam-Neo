using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using Newtonsoft.Json;

namespace AutoTeam;

[Config]
public class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "AutoTeam";

    [JsonProperty("Enabled")]
    public bool Enabled { get; set; } = true;

    [JsonProperty("AutoAcceptRequest")]
    public bool AutoAcceptRequest { get; set; } = true;

    [JsonProperty("GroupTemp")]
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