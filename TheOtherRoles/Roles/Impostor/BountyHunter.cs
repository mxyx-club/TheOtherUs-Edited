using TheOtherRoles.Objects;
using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles.Impostor;

public class BountyHunter : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(BountyHunter),
        (p) => new BountyHunter(p),
        RoleId.BountyHunter,
        RoleType.Impostor,
        "BountyHunter",
        color,
        101200,
        AddOptions
    );

    public BountyHunter(PlayerControl p) : base(p, roleInfo) { }

    public static CustomOption bountyHunterBountyDuration;
    public static CustomOption bountyHunterReducedCooldown;
    public static CustomOption bountyHunterPunishmentTime;
    public static CustomOption bountyHunterShowArrow;
    public static CustomOption bountyHunterArrowUpdateIntervall;
    public static CustomOption bountyHunterChangeTargetCooldown;

    public Arrow arrow;
    public PlayerControl bounty;
    public TextMeshPro cooldownText;
    public float arrowUpdateTimer;
    public float bountyUpdateTimer;

    public static float bountyDuration = 30f;
    public static float changeTargetCooldown = 30f;
    public static bool showArrow = true;
    public static float bountyKillCooldown;
    public static float punishmentTime = 15f;
    public static float arrowUpdateIntervall = 10f;

    public CustomButton bountyHunterChangeTarget;
    public static Sprite buttonSprite = new ResourceSprite("ChangePlayerButton.png", 150f);

    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        bountyHunterBountyDuration = CustomOption.Create(configId++, CustomOptionType.Impostor, "bountyHunterBountyDuration", 60f, 10f, 180f, 5f, roleInfo.RoleOption);
        bountyHunterReducedCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "bountyHunterReducedCooldown", 2.5f, 0f, 30f, 0.5f, roleInfo.RoleOption);
        bountyHunterPunishmentTime = CustomOption.Create(configId++, CustomOptionType.Impostor, "bountyHunterPunishmentTime", 10f, 0f, 60f, 2.5f, roleInfo.RoleOption);
        bountyHunterShowArrow = CustomOption.Create(configId++, CustomOptionType.Impostor, "bountyHunterShowArrow", true, roleInfo.RoleOption);
        bountyHunterArrowUpdateIntervall = CustomOption.Create(configId++, CustomOptionType.Impostor, "bountyHunterArrowUpdateIntervall", 0.5f, 0f, 15f, 0.5f, bountyHunterShowArrow);
        bountyHunterChangeTargetCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "bountyHunterChangeTargetCooldown", 30f, 15f, 90f, 2.5f, roleInfo.RoleOption);
    }

    public override void Initialize()
    {
        arrow = new Arrow(color);
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


        bountyDuration = bountyHunterBountyDuration.GetFloat();
        bountyKillCooldown = bountyHunterReducedCooldown.GetFloat();
        punishmentTime = bountyHunterPunishmentTime.GetFloat();
        showArrow = bountyHunterShowArrow.GetBool();
        arrowUpdateIntervall = bountyHunterArrowUpdateIntervall.GetFloat();
        changeTargetCooldown = bountyHunterChangeTargetCooldown.GetFloat();
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        if (Player.IsDead() || InMeeting)
        {
            if (arrow != null) UObject.Destroy(arrow.arrow);
            arrow = null;
            if (cooldownText != null && cooldownText.gameObject != null) UObject.Destroy(cooldownText.gameObject);
            cooldownText = null;
            bounty = null;
            foreach (var p in ModOption.playerIcons.Values)
            {
                if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
            }
            return;
        }

        arrowUpdateTimer -= Time.fixedDeltaTime;
        bountyUpdateTimer -= Time.fixedDeltaTime;

        if (bounty == null || bountyUpdateTimer <= 0f)
        {
            // Set new bounty
            bounty = null;
            arrowUpdateTimer = 0f; // Force arrow to update
            bountyUpdateTimer = bountyDuration;
            var possibleTargets = new List<PlayerControl>();
            foreach (PlayerControl p in PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsAlive() && !x.IsImpostor(true)))
                if ((p.GetModifier<Mini>()?.isGrownUp() != true) && p != Player.OtherLover()) possibleTargets.Add(p);
            if (possibleTargets.Count == 0) return;
            bounty = possibleTargets[rnd.Next(0, possibleTargets.Count)];
            if (bounty == null) return;

            // Show poolable player
            if (FastDestroyableSingleton<HudManager>.Instance != null && FastDestroyableSingleton<HudManager>.Instance.UseButton != null)
            {
                foreach (var pp in ModOption.playerIcons.Values) pp.gameObject.SetActive(false);
                if (ModOption.playerIcons.ContainsKey(bounty.PlayerId) &&
                    ModOption.playerIcons[bounty.PlayerId].gameObject != null)
                    ModOption.playerIcons[bounty.PlayerId].gameObject.SetActive(true);
            }
        }

        // Hide in meeting
        if (MeetingHud.Instance && ModOption.playerIcons.ContainsKey(bounty.PlayerId) &&
            ModOption.playerIcons[bounty.PlayerId].gameObject != null)
            ModOption.playerIcons[bounty.PlayerId].gameObject.SetActive(false);

        // Update Cooldown Text
        if (cooldownText != null)
        {
            cooldownText.text = Mathf
                .CeilToInt(Mathf.Clamp(bountyUpdateTimer, 0, bountyDuration)).ToString();
            cooldownText.gameObject.SetActive(!MeetingHud.Instance); // Show if not in meeting
        }

        // Update Arrow
        if (showArrow && bounty.IsAlive())
        {
            arrow ??= new Arrow(Color.red);
            if (arrowUpdateTimer <= 0f)
            {
                arrow.Update(bounty.transform.position);
                arrowUpdateTimer = arrowUpdateIntervall;
            }

            arrow.Update();
        }
    }

    public override void OnGameStart()
    {
        // Force Bounty Hunter to load a new Bounty when the Intro is over
        if (bounty != null)
        {
            bountyUpdateTimer = 0f;
            if (FastDestroyableSingleton<HudManager>.Instance != null)
            {
                cooldownText =
                    UObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText,
                        FastDestroyableSingleton<HudManager>.Instance.transform);
                cooldownText.alignment = TextAlignmentOptions.Center;
                cooldownText.transform.localPosition = IntroCutsceneOnDestroyPatch.bottomLeft + new Vector3(0f, -0.35f, -62f);
                cooldownText.transform.localScale = Vector3.one * 0.4f;
                cooldownText.gameObject.SetActive(true);
            }
        }

    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        if (Player.IsAlive()) bountyUpdateTimer = 0f;
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        // Set bountyHunter cooldown
        if (Player.IsAlive())
        {
            if (Info.Target == bounty)
            {
                Player.SetKillTimer(bountyKillCooldown);
                bountyUpdateTimer = 0f; // Force bounty update
            }
            else
            {
                Player.SetKillTimer(ModOption.KillCooldown + punishmentTime);
            }
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        bountyHunterChangeTarget.Destroy();
        bountyHunterChangeTarget = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        bountyHunterChangeTarget.Destroy();
        bountyHunterChangeTarget = new CustomButton(
            () =>
            {
                bounty = null;
                bountyHunterChangeTarget.Timer = bountyHunterChangeTarget.MaxTimer;
            },
            () =>
            {
                return Player.IsAlive() && Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove;
            },
            () => { bountyHunterChangeTarget.Timer = bountyHunterChangeTarget.MaxTimer; },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("ChangeTarget")
            )
        { MaxTimer = changeTargetCooldown };
    }
}
