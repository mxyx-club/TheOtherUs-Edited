namespace TheOtherRoles.Roles.Neutral;

public class BandLeader
{
    public static PlayerControl Player;

    public static PlayerControl Keyboardist;
    public static PlayerControl Bassist;
    public static PlayerControl Drummer;

    public static Color color = new Color32(255, 192, 203, byte.MaxValue);
    public static PlayerControl currentTarget;

    public static PlayerControl[] Members => new[] { Keyboardist, Bassist, Drummer }.Where(x => x != null).ToArray();

    public static float killCooldown;
    public static float createCoolDown;

    public static WinnerFlags WinCondition = WinnerFlags.None;
    public static bool Formed;

    public static Sprite keyboardButton = new ResourceSprite("BandLeader.Keyboard.png");
    public static Sprite keyboardDel = new ResourceSprite("BandLeader.KeyboardDel.png");
    public static Sprite bassButton = new ResourceSprite("BandLeader.Guitar.png");
    public static Sprite bassDel = new ResourceSprite("BandLeader.GuitarDel.png");
    public static Sprite drumButton = new ResourceSprite("BandLeader.Drum.png");
    public static Sprite drumDel = new ResourceSprite("BandLeader.DrumDel.png");

    public static void BandLeaderFormed(byte winner, bool formed)
    {
        if (formed)
        {
            Formed = true;
            WinCondition = (WinnerFlags)winner;
            Message($"Band Leader Formed {(WinnerFlags)winner}");

            if (Members.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId) || Player.AmOwner)
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Player, "BandLeader.formed".Translate());
        }
        else
        {
            if (Members.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId) || Player.AmOwner)
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Player, "BandLeader.bad".Translate());
        }


    }

    public static void CreateBandMember(byte playerId, int role)
    {
        Message($"playerId: {playerId}, role: {role}");
        var player = PlayerById(playerId);
        if (playerId == byte.MaxValue) player = null;
        switch (role)
        {
            case 1:
                Keyboardist = player;
                Message($"Keyboardist: {Keyboardist?.Data?.PlayerName ?? "null"}");
                break;
            case 2:
                Bassist = player;
                Message($"Bassist: {Bassist?.Data?.PlayerName ?? "null"}");
                break;
            case 3:
                Drummer = player;
                Message($"Drummer: {Drummer?.Data?.PlayerName ?? "null"}");
                break;
            default:
                break;
        }
        HudManagerStartPatch.bandLeaderKeyboardistButton.Timer = HudManagerStartPatch.bandLeaderKeyboardistButton.MaxTimer = createCoolDown;
        HudManagerStartPatch.bandLeaderBassistButton.Timer = HudManagerStartPatch.bandLeaderBassistButton.MaxTimer = createCoolDown;
        HudManagerStartPatch.bandLeaderDrummerButton.Timer = HudManagerStartPatch.bandLeaderDrummerButton.MaxTimer = createCoolDown;
    }

    public static void ClearAndReload()
    {
        Player = null;
        Keyboardist = null;
        Bassist = null;
        Drummer = null;
        Formed = false;
        WinCondition = WinnerFlags.None;
        killCooldown = CustomOptionHolder.bandLeaderKillCooldown.GetFloat();
        createCoolDown = CustomOptionHolder.bandLeaderCreateCooldown.GetFloat();
    }

    public enum WinnerFlags
    {
        None,
        Crewmate,
        Neutral,
        Impostor
    }
}
