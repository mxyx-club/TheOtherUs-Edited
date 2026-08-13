namespace TheOtherRoles.Roles.Crewmate;

public class Dreamcatcher
{
    public static PlayerControl Player;
    public static Color color = new Color32(175, 107, 204, byte.MaxValue);

    public static PlayerControl CurrentTarget;
    public static PlayerControl Dreamed;
    public static PlayerControl LastDreamed;
    public static bool ShieldUsed;

    public static float DreamCooldown;
    public static bool DreamShieldOnce;
    //public static bool CheckDreamTargetAbilityl;

    public static Sprite dreamButtonSprite = new ResourceSprite("DreamcatcherDreamButton.png");
    public static Sprite killButtonSprite = new ResourceSprite("DreamcatcherKillButton.png");

    public static void ClearAndReload()
    {
        Player = null;
        CurrentTarget = null;
        Dreamed = null;
        LastDreamed = null;
        ShieldUsed = false;

        DreamCooldown = CustomOptionHolder.dreamcatcherDreamCooldown.GetFloat();
        DreamShieldOnce = CustomOptionHolder.dreamcatcherDreamShieldOnce.GetBool();
        //CheckDreamTargetAbilityl = CustomOptionHolder.dreamcatcherCheckDreamTargetAbility.GetBool();
    }

    public static void SetDreamer(PlayerControl playerControl, bool killed)
    {
        if (killed)
        {
            LastDreamed = null;
            Dreamed = null;
            CurrentTarget = null;
            return;
        }
        LastDreamed = Dreamed;
        Dreamed = playerControl;
    }
}
