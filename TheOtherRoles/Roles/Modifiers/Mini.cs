using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles.Modifier;

public class Mini : ModifierBase
{
    public static Color color = Color.yellow;

    public static readonly RoleInfo roleinfo = new(
        typeof(Mini),
        (p) => new Mini(p),
        RoleId.Mini,
        "Mini",
        color,
        402100,
        AddOptions
    );

    public static readonly List<Mini> AllMini = new();

    public static IEnumerable<PlayerControl> UngrownMinis => AllMini.Where(m => !m.isGrownUp()).Select(m => m.Player);

    public Mini(PlayerControl p) : base(p, roleinfo)
    {
        AllMini.Add(this);
        CustomRoleManager.OnFixedUpdateOthers.Add(OnFixedUpdate);
    }

    public const float defaultColliderRadius = 0.2233912f;
    public const float defaultColliderOffset = 0.3636057f;
    public float ageOnMeetingStart;

    public static float growingUpDuration = 400f;
    public static bool isGrowingUpInMeeting = true;
    public static bool triggerMiniLose;

    public DateTime timeOfGrowthStart = DateTime.UtcNow;
    public DateTime timeOfMeetingStart = DateTime.UtcNow;

    public static CustomOption modifierMiniGrowingUpDuration;
    public static CustomOption modifierMiniGrowingUpInMeeting;

    public static float Multiplier => PlayerControl.LocalPlayer.TryGetModifier<Mini>(out var mini) && mini.isGrownUp() ? 0.66f : 2f;

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierMiniGrowingUpDuration = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierMiniGrowingUpDuration", 400f, 100f, 1500f, 25f, roleinfo.RoleOption);
        modifierMiniGrowingUpInMeeting = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierMiniGrowingUpInMeeting", true, roleinfo.RoleOption);
    }

    public float growingProgress()
    {
        var timeSinceStart = (float)(DateTime.UtcNow - timeOfGrowthStart).TotalMilliseconds;
        return Mathf.Clamp(timeSinceStart / (growingUpDuration * 1000), 0f, 1f);
    }

    public bool isGrownUp() => growingProgress() == 1f;

    public override void OnDestroy()
    {
        base.OnDestroy();
        AllMini.Remove(this);
    }

    public override void Initialize()
    {
        triggerMiniLose = false;
        growingUpDuration = modifierMiniGrowingUpDuration.GetFloat();
        isGrowingUpInMeeting = modifierMiniGrowingUpInMeeting.GetBool();
        timeOfGrowthStart = DateTime.UtcNow;
    }

    public override void OnGameStart()
    {
        timeOfGrowthStart = DateTime.UtcNow;
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        timeOfMeetingStart = DateTime.UtcNow;
        ageOnMeetingStart = Mathf.FloorToInt(growingProgress() * 18);
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        if (Camouflager.CamoTimer > 0f || MushroomSabotageActive ||
            (Player.TryGetRole<Morphling>(out var morphling) && morphling.morphTimer > 0f) ||
            (Player.TryGetRole<Ninja>(out var ninja) && ninja.isInvisable) || SurveillanceMinigamePatch.nightVisionIsActive ||
            (Player.TryGetRole<Swooper>(out var swooper) && swooper.IsInvisable) ||
            (Player.TryGetRole<Jackal>(out var jackal) && jackal.IsInvisable) || isActiveCamoComms) return;

        var growing = growingProgress();
        var suffix = "";
        if (growing != 1f)
            suffix = " <color=#FAD934FF>(" + Mathf.FloorToInt(growing * 18) + ")</color>";
        if (!isGrowingUpInMeeting && MeetingHud.Instance != null && ageOnMeetingStart != 0 &&
            !(ageOnMeetingStart >= 18))
            suffix = " <color=#FAD934FF>(" + ageOnMeetingStart + ")</color>";

        Player.cosmetics.nameText.text += suffix;
        if (MeetingHud.Instance != null)
            foreach (var pva in MeetingHud.Instance.playerStates)
                if (pva.NameText != null && Player.PlayerId == pva.TargetPlayerId)
                    pva.NameText.text += suffix;

        if (Player.TryGetRole<Morphling>(out var result) && result.morphTimer > 0f)
            result.Player.cosmetics.nameText.text += suffix;
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        var multiplier = isGrownUp() ? 0.66f : 2f;
        PlayerControl.LocalPlayer.SetKillTimer(ModOption.KillCooldown * multiplier);

        if (!isGrowingUpInMeeting) timeOfGrowthStart = timeOfGrowthStart.Add(DateTime.UtcNow.Subtract(timeOfMeetingStart)).AddSeconds(10);

        if (exiled != null && Player.PlayerId == exiled.PlayerId && !isGrownUp() && Player.IsCrew())
        {
            triggerMiniLose = true;
            return;
        }
    }

    public override bool BeforeMurderPlayer(MurderInfo Info)
    {
        if (Info.Target == Player && !isGrownUp()) return false;
        return true;
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        // Mini Set Impostor Mini kill timer (Due to mini being a modifier, all "SetKillTimers" must have happened before this!)
        if (Info.Killer == Player && Player == PlayerControl.LocalPlayer)
        {
            var multiplier = isGrownUp() ? 0.66f : 2f;
            PlayerControl.LocalPlayer.SetKillTimer(ModOption.KillCooldown * multiplier);
        }

    }
}
