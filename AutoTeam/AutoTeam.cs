using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using System.Threading.Tasks;

namespace AutoTeam;

[ApiVersion(2, 1)]
public class AutoTeam : LazyPlugin
{
    public override string Name => "AutoTeam";
    public override string Author => "Neoslyke, 十七，肝帝熙恩";
    public override Version Version => new Version(2, 1, 0);
    public override string Description => "Automatically assigns players to teams based on their group";
    
    public AutoTeam(Main game) : base(game) { }

    public override void Initialize()
    {
        ServerApi.Hooks.NetGreetPlayer.Register(this, OnJoin);

        Commands.ChatCommands.Add(new Command("autoteam.toggle", TogglePlugin, "autoteam", "at"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnJoin);
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == TogglePlugin);
        }
        base.Dispose(disposing);
    }

    private void TogglePlugin(CommandArgs args)
    {
        Configuration.Instance.Enabled = !Configuration.Instance.Enabled;

        var status = Configuration.Instance.Enabled ? "enabled" : "disabled";
        args.Player.SendSuccessMessage($"AutoTeam plugin is now {status}");

        Configuration.Save();
    }

    private void OnJoin(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];

        if (player == null)
            return;

        Task.Run(async () =>
        {
            await Task.Delay(800);

            if (player == null || ShouldSkipAutoTeam(player))
                return;

            SetTeam(player);
        });
    }

    private bool ShouldSkipAutoTeam(TSPlayer player)
    {
        if (!Configuration.Instance.Enabled)
            return true;

        if (player.Group == null || player.Group.HasPermission("noautoteam"))
            return true;

        var groupName = player.Group.Name;
        return Configuration.Instance.GetTeamForGroup(groupName) == "none";
    }

    private void SetTeam(TSPlayer player)
    {
        var groupName = player.Group.Name;
        var teamName = Configuration.Instance.GetTeamForGroup(groupName);

        var teamIndex = GetTeamIndex(teamName);

        if (teamIndex != -1)
        {

            if (player.Team != teamIndex)
            {
                player.SetTeam(teamIndex);
            }

            player.SendInfoMessage($"Your team has been set to {teamName}.");
        }
        else
        {
            player.SendInfoMessage($"Invalid team configuration: {teamName}");
        }
    }

    private int GetTeamIndex(string teamName)
    {
        return teamName.ToLower() switch
        {
            "none" or "无队伍" => 0,
            "red" or "红队" => 1,
            "green" or "绿队" => 2,
            "blue" or "蓝队" => 3,
            "yellow" or "黄队" => 4,
            "pink" or "粉队" => 5,
            _ => -1,
        };
    }
}