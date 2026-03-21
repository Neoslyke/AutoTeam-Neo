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
        
        // Register hook for group changes
        ServerApi.Hooks.ServerChat.Register(this, OnChat);
        
        // Optional: Add command to manually set team for a player
        Commands.ChatCommands.Add(new Command("autoteam.set", SetTeamCommand, "setteam"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnJoin);
            ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == SetTeamCommand);
        }
        base.Dispose(disposing);
    }

    private void SetTeamCommand(CommandArgs args)
    {
        if (args.Parameters.Count < 2)
        {
            args.Player.SendErrorMessage("Usage: /setteam <player> <team>");
            args.Player.SendErrorMessage("Teams: none, red, green, blue, yellow, pink");
            return;
        }

        var playerName = args.Parameters[0];
        var teamName = args.Parameters[1].ToLower();
        
        var players = TSPlayer.FindByNameOrID(playerName);
        if (players.Count == 0)
        {
            args.Player.SendErrorMessage($"Player '{playerName}' not found.");
            return;
        }
        
        if (players.Count > 1)
        {
            args.Player.SendMultipleMatchError(players.Select(p => p.Name));
            return;
        }
        
        var targetPlayer = players[0];
        var teamIndex = GetTeamIndex(teamName);
        
        if (teamIndex == -1)
        {
            args.Player.SendErrorMessage($"Invalid team: {teamName}");
            args.Player.SendErrorMessage("Valid teams: none, red, green, blue, yellow, pink");
            return;
        }
        
        targetPlayer.SetTeam(teamIndex);
        args.Player.SendSuccessMessage($"Set {targetPlayer.Name}'s team to {teamName}.");
        targetPlayer.SendInfoMessage($"Your team has been set to {teamName}.");
    }

    private void OnChat(ServerChatEventArgs args)
    {
        // This hook can be used to detect group changes if needed
        // For now, we'll just use it to handle team updates on group changes
        if (args.Handled || args.Text.StartsWith("/"))
            return;
            
        // You could add logic here to check if a player's group changed
        // and update their team accordingly
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
        var teamName = Configuration.Instance.GetTeamForGroup(groupName);
        
        return teamName == "none" || string.IsNullOrEmpty(teamName);
    }

    private void SetTeam(TSPlayer player)
    {
        var groupName = player.Group.Name;
        var teamName = Configuration.Instance.GetTeamForGroup(groupName);
        var teamIndex = GetTeamIndex(teamName);

        if (teamIndex == -1)
        {
            player.SendErrorMessage($"[AutoTeam] Invalid team configuration for group '{groupName}': {teamName}");
            return;
        }

        if (player.Team != teamIndex)
        {
            player.SetTeam(teamIndex);

            if (teamIndex > 0)
            {
                var teamColor = Main.teamColor[teamIndex];
                var displayName = char.ToUpper(teamName[0]) + teamName.Substring(1);
                TSPlayer.All.SendMessage($"{player.Name} has joined the {displayName} team.", teamColor.R, teamColor.G, teamColor.B);
            }
            else
            {
                TSPlayer.All.SendMessage($"{player.Name} is no longer on a team.", 255, 255, 255);
            }
        }
    }

    private int GetTeamIndex(string teamName)
    {
        if (string.IsNullOrEmpty(teamName))
            return 0;
            
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