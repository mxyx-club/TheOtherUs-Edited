using System.Collections.Generic;
using TheOtherRoles.Patches;
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public class SchrodingersCat
{
    public static PlayerControl Player;
    public static PlayerControl currentTarget;
    public static Color color => State switch
    {
        CatState.None => Color.gray,
        CatState.Crewmate => Palette.CrewmateBlue,
        CatState.Jackal => Jackal.color,
        CatState.Pavlovsowner => Pavlovsdogs.color,
        CatState.Werewolf => Werewolf.color,
        CatState.Juggernaut => Juggernaut.color,
        CatState.Pelican => Pelican.color,
        CatState.Swooper => Swooper.color,
        CatState.Arsonist => Arsonist.color,
        CatState.Impostor => Palette.ImpostorRed,
        _ => Color.gray,
    };

    public static int remainingChange => TeamChanges ? MaxChangeCount - ChangeCount : 0;
    public static bool IsEvil => State is not CatState.Crewmate and not CatState.None;

    public static int ChangeCount;
    public static CatState State = CatState.None;

    public static float Cooldown;
    public static bool CanKill;
    public static bool hasImpVision;
    public static bool TeamChanges;
    public static int MaxChangeCount;
    public static bool IsGuessable;

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
        IsGuessable = CustomOptionHolder.schrodingersCatIsGuessable.GetBool();
    }

    public static PlayerControl SetTarget()
    {
        if (Player == null || State == CatState.None || State == CatState.Crewmate) return null;

        List<PlayerControl> untarget = null;

        foreach (var p in PlayerControl.AllPlayerControls)
        {
            switch (State)
            {
                case CatState.Jackal when Jackal.Sidekick == p || Jackal.jackal.Contains(p):
                    untarget.TryAdd(p);
                    continue;
                case CatState.Pavlovsowner when Pavlovsdogs.pavlovsowner == p && Pavlovsdogs.pavlovsdogs.Contains(p):
                    untarget.TryAdd(p);
                    continue;
                case CatState.Werewolf when Werewolf.werewolf == p:
                    untarget.TryAdd(p);
                    continue;
                case CatState.Juggernaut when Juggernaut.juggernaut == p:
                    untarget.TryAdd(p);
                    continue;
                case CatState.Pelican when Pelican.Player == p:
                    untarget.TryAdd(p);
                    continue;
                case CatState.Swooper when Swooper.swooper == p:
                    untarget.TryAdd(p);
                    continue;
                case CatState.Arsonist when Arsonist.arsonist == p:
                    untarget.TryAdd(p);
                    continue;
            }
        }
        return PlayerControlFixedUpdatePatch.SetTarget(State == CatState.Impostor, untargetablePlayers: untarget);
    }

    public static bool InTeam(PlayerControl player, out Color color)
    {
        color = SchrodingersCat.color;

        return State switch
        {
            CatState.Impostor => player.IsImpostor(),
            CatState.Jackal => Jackal.jackal.Contains(player) || Jackal.Sidekick == player,
            CatState.Pavlovsowner => Pavlovsdogs.pavlovsdogs.Contains(player) || Pavlovsdogs.pavlovsowner == player,
            CatState.Werewolf => Werewolf.werewolf == player,
            CatState.Juggernaut => Juggernaut.juggernaut == player,
            CatState.Swooper => Swooper.swooper == player,
            CatState.Arsonist => Arsonist.arsonist == player,
            CatState.Pelican => Pelican.Player == player,
            _ => false,
        };
    }

    public enum CatState
    {
        None,
        Crewmate,
        Impostor,
        Jackal,
        Pavlovsowner,
        Werewolf,
        Juggernaut,
        Swooper,
        Arsonist,
        Pelican,
    }
}
