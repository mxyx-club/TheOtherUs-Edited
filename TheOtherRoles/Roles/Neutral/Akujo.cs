namespace TheOtherRoles.Roles.Neutral;

public static class Akujo
{
    public static Color color = new Color32(142, 69, 147, byte.MaxValue);
    public static PlayerControl akujo;
    public static PlayerControl honmei;
    public static List<PlayerControl> keeps = new();
    public static PlayerControl currentTarget;
    public static DateTime startTime;

    public static float timeLimit = 1300f;
    public static bool knowsRoles = true;
    public static bool honmeiCannotFollowWin;
    public static bool honmeiOptimizeWin;
    public static int timeLeft;
    public static bool forceKeeps;
    public static int keepsLeft;
    public static int numKeeps;

    public static Sprite honmeiSprite = new ResourceSprite("AkujoHonmeiButton.png");
    public static Sprite keepSprite = new ResourceSprite("AkujoKeepButton.png");

    public static bool IsKillerLover()
    {
        return honmei.IsAlive() && honmei.IsKiller();
    }

    public static bool isAkujoTeam(PlayerControl player)
    {
        return player != null && (player == akujo || player == honmei);
    }

    public static PlayerControl otherLover(PlayerControl player)
    {
        if (akujo == null || honmei == null) return null;
        if (player == akujo) return honmei;
        if (player == honmei) return akujo;
        return null;
    }

    public static void breakLovers(PlayerControl target)
    {
        if (Lovers.isLover(target))
        {
            var otherLover = Lovers.otherLover(target);
            if (otherLover != null)
            {
                Lovers.clearAndReload();
                otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                PlayerData.SetDeathReason(otherLover, CustomDeathReason.LoveStolen, akujo);
            }
        }
    }

    public static void clearAndReload()
    {
        akujo = null;
        honmei = null;
        keeps.Clear();
        currentTarget = null;
        startTime = DateTime.UtcNow;
        timeLimit = CustomOptionHolder.akujoTimeLimit.GetFloat();
        forceKeeps = CustomOptionHolder.akujoForceKeeps.GetBool();
        knowsRoles = CustomOptionHolder.akujoKnowsRoles.GetBool();
        honmeiCannotFollowWin = CustomOptionHolder.akujoHonmeiCannotFollowWin.GetBool();
        honmeiOptimizeWin = CustomOptionHolder.akujoHonmeiOptimizeWin.GetBool();
        timeLeft = (int)Math.Ceiling(timeLimit - (DateTime.UtcNow - startTime).TotalSeconds);
        numKeeps = Math.Min(CustomOptionHolder.akujoNumKeeps.GetInt(), PlayerControl.AllPlayerControls.Count - 2);
        keepsLeft = numKeeps;
    }
}
