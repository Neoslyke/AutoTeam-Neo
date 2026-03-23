using Newtonsoft.Json;
using TShockAPI;

namespace AutoTeam;

public class Configuration
{
    private static readonly string ConfigPath = Path.Combine(TShock.SavePath, "AutoTeam.json");

    [JsonProperty("Enable")]
    public bool Enable { get; set; } = true;

    [JsonProperty("AnnounceTeamJoin")]
    public bool AnnounceTeamJoin { get; set; } = true;

    [JsonProperty("GroupTeams")]
    public Dictionary<string, string> GroupTeams { get; set; } = new()
    {
        { "guest", "pink" },
        { "default", "blue" },
        { "owner", "red" },
        { "admin", "green" },
        { "vip", "none" }
    };

    public string GetTeamForGroup(string groupName)
    {
        if (GroupTeams.TryGetValue(groupName, out var team))
            return team;

        var key = GroupTeams.Keys.FirstOrDefault(k =>
            k.Equals(groupName, StringComparison.OrdinalIgnoreCase));

        return key != null ? GroupTeams[key] : "none";
    }

    public static Configuration Load()
    {
        try
        {
            if (!File.Exists(ConfigPath))
            {
                var config = new Configuration();
                config.Save();
                return config;
            }

            var json = File.ReadAllText(ConfigPath);
            return JsonConvert.DeserializeObject<Configuration>(json) ?? new Configuration();
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError($"[AutoTeam] Error loading config: {ex.Message}");
            return new Configuration();
        }
    }

    public void Save()
    {
        try
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError($"[AutoTeam] Error saving config: {ex.Message}");
        }
    }
}