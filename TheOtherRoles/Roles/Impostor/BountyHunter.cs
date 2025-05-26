using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Impostor;

public static class BountyHunter
{
    public static PlayerControl bountyHunter;
    public static Color color = Palette.ImpostorRed;

    public static Arrow arrow;
    public static float bountyDuration = 30f;
    public static bool showArrow = true;
    public static float bountyKillCooldown;
    public static float punishmentTime = 15f;
    public static float arrowUpdateIntervall = 10f;
    public static float changeTargetCooldown = 30f;

    public static float arrowUpdateTimer;
    public static float bountyUpdateTimer;
    public static PlayerControl bounty;
    public static TextMeshPro cooldownText;

    public static Sprite buttonSprite = new ResourceSprite("ChangePlayerButton.png", 150f);

    public static void clearAndReload()
    {
        arrow = new Arrow(color);
        bountyHunter = null;
        bounty = null;
        arrowUpdateTimer = 0f;
        bountyUpdateTimer = 0f;
        if (arrow != null && arrow.arrow != null) UObject.Destroy(arrow.arrow);
        arrow = null;
        if (cooldownText != null && cooldownText.gameObject != null) UObject.Destroy(cooldownText.gameObject);
        cooldownText = null;
        foreach (var p in ModOption.playerIcons.Values)
            if (p != null && p.gameObject != null)
                p.gameObject.SetActive(false);


        bountyDuration = CustomOptionHolder.bountyHunterBountyDuration.GetFloat();
        bountyKillCooldown = CustomOptionHolder.bountyHunterReducedCooldown.GetFloat();
        punishmentTime = CustomOptionHolder.bountyHunterPunishmentTime.GetFloat();
        showArrow = CustomOptionHolder.bountyHunterShowArrow.GetBool();
        arrowUpdateIntervall = CustomOptionHolder.bountyHunterArrowUpdateIntervall.GetFloat();
        changeTargetCooldown = CustomOptionHolder.bountyHunterChangeTargetCooldown.GetFloat();
    }
}
