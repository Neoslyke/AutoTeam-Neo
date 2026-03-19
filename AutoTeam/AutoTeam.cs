using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static TShockAPI.GetDataHandlers;

namespace AutoTeam;

[ApiVersion(2, 1)]
public class AutoTeam : LazyPlugin
{
    public override string Author => "Modified by Neoslyke (original by 十七 / 肝帝熙恩)";
    public override Version Version => new Version(2, 4, 10);
    public override string Description => "Automatically assigns players to teams based on their group";
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    
    public AutoTeam(Main game) : base(game) { }

    public override void Initialize()
    {
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnJoin);
        PlayerHooks.PlayerPostLogin += this.OnLogin;
        GetDataHandlers.PlayerTeam += this.Team;

        Commands.ChatCommands.Add(new Command("autoteam.toggle", this.TogglePlugin, "autoteam", "at"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnJoin);
            PlayerHooks.PlayerPostLogin -= this.OnLogin;
            GetDataHandlers.PlayerTeam -= this.Team;
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.TogglePlugin);
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

    private void Team(object? sender, PlayerTeamEventArgs args)
    {
        if (args.Player == null)
            return;

        if (this.ShouldSkipAutoTeam(args.Player))
            return;

        this.SetTeam(args.Player);
        args.Handled = true;
    }

    private void OnJoin(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null)
            return;

        if (this.ShouldSkipAutoTeam(player))
            return;

        this.SetTeam(player);
    }

    private void OnLogin(PlayerPostLoginEventArgs args)
    {
        if (args.Player == null)
            return;

        if (this.ShouldSkipAutoTeam(args.Player))
            return;

        this.SetTeam(args.Player);
    }

    private bool ShouldSkipAutoTeam(TSPlayer player)
    {
        if (!Configuration.Instance.Enabled)
            return true;

        if (player.Group == null || player.Group.HasPermission("noautoteam"))
            return true;

        var groupName = player.Group.Name;
        return Configuration.Instance.GetTeamForGroup(groupName) == "none-configured";
    }

    private void SetTeam(TSPlayer player)
    {
        var groupName = player.Group.Name;
        var teamName = Configuration.Instance.GetTeamForGroup(groupName);

        var teamIndex = this.GetTeamIndex(teamName);

        if (teamIndex != -1)
        {
            player.SetTeam(teamIndex);
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
            "none" => 0,
            "red" => 1,
            "green" => 2,
            "blue" => 3,
            "yellow" => 4,
            "pink" => 5,
            _ => -1,
        };
    }
}