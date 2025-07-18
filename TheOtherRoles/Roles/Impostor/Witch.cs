namespace TheOtherRoles.Roles.Impostor;

public static class Witch
{
    public static PlayerControl witch;
    public static Color color = Palette.ImpostorRed;

    public static List<PlayerControl> futureSpelled = new();
    public static PlayerControl currentTarget;
    public static PlayerControl spellCastingTarget;
    public static float cooldown = 30f;
    public static float spellCastingDuration = 2f;
    public static float cooldownAddition = 10f;
    public static float currentCooldownAddition;
    public static bool canSpellAnyone;
    public static bool triggerBothCooldowns = true;
    public static bool witchVoteSavesTargets = true;
    public static bool witchWasGuessed;

    public static Sprite buttonSprite = new ResourceSprite("SpellButton.png");

    public static Sprite spelledOverlaySprite = new ResourceSprite("SpellButtonMeeting.png", 225f);

    public static void clearAndReload()
    {
        witch = null;
        futureSpelled.Clear();
        witchWasGuessed = false;
        currentTarget = spellCastingTarget = null;
        cooldown = CustomOptionHolder.witchCooldown.GetFloat();
        cooldownAddition = CustomOptionHolder.witchAdditionalCooldown.GetFloat();
        currentCooldownAddition = 0f;
        canSpellAnyone = CustomOptionHolder.witchCanSpellAnyone.GetBool();
        spellCastingDuration = CustomOptionHolder.witchSpellCastingDuration.GetFloat();
        triggerBothCooldowns = CustomOptionHolder.witchTriggerBothCooldowns.GetBool();
        witchVoteSavesTargets = CustomOptionHolder.witchVoteSavesTargets.GetBool();
    }
}
