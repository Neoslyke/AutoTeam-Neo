using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AutoTeam;

[ApiVersion(2, 1)]
public class AutoTeam : TerrariaPlugin
{
    public override string Name => "AutoTeam";
    public override string Author => "Neoslyke, 十七, 肝帝熙恩";
    public override Version Version => new Version(2, 2, 0);
    public override string Description => "Automatically assigns players to teams based on their group.";

    public static Configuration Config { get; private set; } = new();

    public AutoTeam(Main game) : base(game) { }

    public override void Initialize()
    {
        Config = Configuration.Load();

        ServerApi.Hooks.NetGreetPlayer.Register(this, OnJoin);
        PlayerHooks.PlayerPostLogin += OnPostLogin;
        GeneralHooks.ReloadEvent += OnReload;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnJoin);
            PlayerHooks.PlayerPostLogin -= OnPostLogin;
            GeneralHooks.ReloadEvent -= OnReload;
        }
        base.Dispose(disposing);
    }

    private void OnReload(ReloadEventArgs args)
    {
        Config = Configuration.Load();
        args.Player?.SendSuccessMessage("[AutoTeam] Configuration reloaded.");
    }

    private async void OnJoin(GreetPlayerEventArgs args)
    {
        var who = args.Who;

        await Task.Delay(800);

        var player = TShock.Players[who];

        if (player == null || !player.Active)
            return;

        AssignTeam(player);
    }

    private void OnPostLogin(PlayerPostLoginEventArgs args)
    {
        var player = args.Player;

        if (player == null || !player.Active)
            return;

        AssignTeam(player);
    }

    private void AssignTeam(TSPlayer player)
    {
        if (!Config.Enable)
            return;

        var groupName = player.Group?.Name;
        if (string.IsNullOrEmpty(groupName))
            return;

        var teamName = Config.GetTeamForGroup(groupName);
        if (string.IsNullOrEmpty(teamName) || teamName == "none")
            return;

        var teamIndex = GetTeamIndex(teamName);
        if (teamIndex == -1)
            return;

        // Don't reassign if already on the correct team
        if (player.Team == teamIndex)
            return;

        player.SetTeam(teamIndex);

        if (Config.AnnounceTeamJoin)
        {
            var teamColor = Main.teamColor[teamIndex];
            var displayName = char.ToUpper(teamName[0]) + teamName.Substring(1);
            TSPlayer.All.SendMessage($"{player.Name} has joined the {displayName} team.", teamColor.R, teamColor.G, teamColor.B);
        }
    }

    private static int GetTeamIndex(string teamName)
    {
        if (string.IsNullOrEmpty(teamName))
            return 0;

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