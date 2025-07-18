namespace TheOtherRoles.Roles.Impostor;

public static class Blackmailer
{
    public static PlayerControl Player;
    public static Color color = Palette.ImpostorRed;
    public static Color blackmailedColor = Palette.White;

    public static bool alreadyShook;
    public static PlayerControl blackmailed;
    public static PlayerControl currentTarget;
    public static float cooldown = 30f;
    public static Sprite blackmailButtonSprite = new ResourceSprite("BlackmailerBlackmailButton.png");
    public static Sprite overlaySprite = new ResourceSprite("BlackmailerOverlay.png", 100);

    public static IEnumerator BlackmailShhh()
    {
        //Helpers.showFlash(new Color32(49, 28, 69, byte.MinValue), 3f, "Blackmail", false, 0.75f);
        yield return HudManager.Instance.CoFadeFullScreen(Color.clear, new Color(0f, 0f, 0f, 0.98f));
        var TempPosition = HudManager.Instance.shhhEmblem.transform.localPosition;
        var TempDuration = HudManager.Instance.shhhEmblem.HoldDuration;
        HudManager.Instance.shhhEmblem.transform.localPosition = new Vector3(
            HudManager.Instance.shhhEmblem.transform.localPosition.x,
            HudManager.Instance.shhhEmblem.transform.localPosition.y,
            HudManager.Instance.FullScreen.transform.position.z + 1f);
        HudManager.Instance.shhhEmblem.TextImage.text = GetString("BlackmailShhhText");
        HudManager.Instance.shhhEmblem.HoldDuration = 3f;
        yield return HudManager.Instance.ShowEmblem(true);
        HudManager.Instance.shhhEmblem.transform.localPosition = TempPosition;
        HudManager.Instance.shhhEmblem.HoldDuration = TempDuration;
        yield return HudManager.Instance.CoFadeFullScreen(new Color(0f, 0f, 0f, 0.98f), Color.clear);
        yield return null;
    }

    public static void clearAndReload()
    {
        Player = null;
        currentTarget = null;
        blackmailed = null;
        alreadyShook = false;
        cooldown = CustomOptionHolder.blackmailerCooldown.GetFloat();
    }
}
