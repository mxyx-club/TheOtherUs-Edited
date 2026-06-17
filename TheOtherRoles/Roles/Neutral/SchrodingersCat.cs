namespace TheOtherRoles.Roles.Neutral;

public class SchrodingersCat
{
    public static PlayerControl Player;
    public static PlayerControl currentTarget;
    public static Color color = Color.gray;

    public static Color stateColor => State switch
    {
        CatState.None => Color.gray,
        CatState.Crewmate => Palette.CrewmateBlue,
        CatState.Jackal => Jackal.color,
        CatState.Pavlovsowner => Pavlovsdogs.color,
        CatState.Werewolf => Werewolf.color,
        CatState.Juggernaut => Juggernaut.color,
        CatState.Pelican => Pelican.color,
        CatState.Phantom => Phantom.color,
        CatState.Infected => Infected.color,
        CatState.Arsonist => Arsonist.color,
        CatState.Impostor => Palette.ImpostorRed,
        _ => Color.gray,
    };

    public static int remainingChange => MaxChangeCount - ChangeCount;
    public static bool IsEvil => State is not CatState.Crewmate and not CatState.None;
    public static bool IsKiller => IsEvil && CanKill;
    public static string Name => $"SchrodingersCatRoles.{State}";

    public static int ChangeCount;
    public static CatState State = CatState.None;

    public static float Cooldown;
    public static bool CanKill;
    public static bool hasImpVision;
    public static bool TeamChanges;
    public static int MaxChangeCount;

    public static void ClearAndReload()
    {
        Player = null;
        State = CatState.None;
        ChangeCount = 0;
        CanKill = CustomOptionHolder.schrodingersCatCanKill.GetBool();
        Cooldown = CustomOptionHolder.schrodingersCatCooldown.GetFloat();
        hasImpVision = CustomOptionHolder.schrodingersCatHasImpVision.GetBool();
        TeamChanges = CustomOptionHolder.schrodingersCatTeamChanges.GetBool();
        MaxChangeCount = CustomOptionHolder.schrodingersCatMaxChangeCount.GetInt();
        MaxChangeCount = TeamChanges ? MaxChangeCount : 1;
    }

    public static bool InTeam(PlayerControl player, out Color color)
    {
        color = stateColor;

        return State switch
        {
            CatState.Impostor => player.IsImpostor(true),
            CatState.Jackal => Jackal.jackal.Any(x => x.PlayerId == player.PlayerId) || Jackal.Sidekick == player,
            CatState.Pavlovsowner => Pavlovsdogs.pavlovsdogs.Any(x => x.PlayerId == player.PlayerId) || Pavlovsdogs.pavlovsowner == player,
            CatState.Infected => Infected.Player.Any(x => x.PlayerId == player.PlayerId),
            CatState.Werewolf => Werewolf.werewolf == player,
            CatState.Juggernaut => Juggernaut.juggernaut == player,
            CatState.Phantom => Phantom.Player == player,
            CatState.Arsonist => Arsonist.arsonist == player,
            CatState.Pelican => Pelican.Player == player,
            _ => false,
        };
    }

    public enum CatState
    {
        None = 220,
        Crewmate,
        Impostor,
        Jackal,
        Infected,
        Pavlovsowner,
        Werewolf,
        Juggernaut,
        Phantom,
        Arsonist,
        Pelican,
    }

    public static Color getColor(CatState state) => state switch
    {
        CatState.None => Color.gray,
        CatState.Crewmate => Palette.CrewmateBlue,
        CatState.Jackal => Jackal.color,
        CatState.Pavlovsowner => Pavlovsdogs.color,
        CatState.Werewolf => Werewolf.color,
        CatState.Juggernaut => Juggernaut.color,
        CatState.Pelican => Pelican.color,
        CatState.Phantom => Phantom.color,
        CatState.Infected => Infected.color,
        CatState.Arsonist => Arsonist.color,
        CatState.Impostor => Palette.ImpostorRed,
        _ => Color.gray,
    };

}
