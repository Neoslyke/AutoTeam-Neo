using LazyAPI.ConfigFiles;

namespace AutoTeam;

public class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "AutoTeam";

    public bool Enable { get; set; } = true;

    public Dictionary<string, string> GroupTeams { get; set; } = new();

    public string GetTeamForGroup(string groupName)
    {
        if (this.GroupTeams.TryGetValue(groupName, out var team))
            return team;
            
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