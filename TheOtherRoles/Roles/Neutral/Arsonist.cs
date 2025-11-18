using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles.Neutral;

public class Arsonist : RoleBase, INeutral
{
    public static Color color = new Color32(238, 112, 46, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Arsonist),
        (p) => new Arsonist(p),
        RoleId.Arsonist,
        RoleType.Neutral,
        "Arsonist",
        color,
        206500,
        AddOptions,
        IsKiller: true
    );

    public Arsonist(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Kill;

    public static float cooldown = 30f;
    public static float duration = 3f;
    public static bool igniteCooldownRemoved;

    public PlayerControl currentTarget;
    public PlayerControl currentTarget2;
    public PlayerControl douseTarget;
    public List<PlayerControl> dousedPlayers = new();

    public static CustomOption arsonistCooldown;
    public static CustomOption arsonistDuration;
    public static CustomOption arsonistIgniteCdRemoved;

    public CustomButton arsonistButton;
    public CustomButton arsonistKillButton;
    public static Sprite douseSprite = new ResourceSprite("DouseButton.png");
    public static Sprite igniteSprite = new ResourceSprite("IgniteButton.png");
    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        arsonistCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "arsonistCooldown", 12.5f, 5f, 60f, 2.5f, roleinfo.RoleOption);
        arsonistDuration = CustomOption.Create(configId++, CustomOptionType.Neutral, "arsonistDuration", 0.25f, 0f, 10f, 0.125f, roleinfo.RoleOption);
        arsonistIgniteCdRemoved = CustomOption.Create(configId++, CustomOptionType.Neutral, "arsonistIgniteCdRemoved", false, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        Player = null;
        currentTarget = null;
        currentTarget2 = null;
        douseTarget = null;
        dousedPlayers = new List<PlayerControl>();
        foreach (var p in ModOption.playerIcons.Values)
            if (p != null && p.gameObject != null)
                p.gameObject.SetActive(false);

        cooldown = arsonistCooldown.GetFloat();
        duration = arsonistDuration.GetFloat();
        igniteCooldownRemoved = arsonistIgniteCdRemoved.GetBool();
    }

    public override bool SeeRoleTag(PlayerControl seen, PlayerControl seer, out string tag)
    {
        if ((Player == seen || CanSeeGhostInfo) && dousedPlayers.Contains(seer))
        {
            tag = Cs(color, " ♨");
            return true;
        }

        tag = string.Empty;
        return false;
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {

        // Arsonist deactivate dead poolable players
        if (Player != null && Player == PlayerControl.LocalPlayer)
        {
            var visibleCounter = 0;
            var newBottomLeft = IntroCutsceneOnDestroyPatch.bottomLeft;
            var BottomLeft = newBottomLeft + new Vector3(-0.25f, -0.25f, 0);
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                if (!ModOption.playerIcons.ContainsKey(p.PlayerId)) continue;
                if (p.Data.IsDead || p.Data.Disconnected)
                {
                    ModOption.playerIcons[p.PlayerId].gameObject.SetActive(false);
                }
                else
                {
                    ModOption.playerIcons[p.PlayerId].transform.localPosition =
                        newBottomLeft + (Vector3.right * visibleCounter * 0.35f);
                    visibleCounter++;
                }
            }
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        arsonistButton?.Destroy();
        arsonistKillButton?.Destroy();
        arsonistButton = null;
        arsonistKillButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Arsonist button (涂油)
        arsonistButton?.Destroy();
        arsonistButton = new CustomButton(
            () =>
            {
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;
                douseTarget = currentTarget;
                arsonistButton.HasEffect = true;
                SoundEffectsManager.play("arsonistDouse");
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                List<PlayerControl> untargetables;
                if (douseTarget != null)
                {
                    untargetables = new();
                    foreach (var cachedPlayer in PlayerControl.AllPlayerControls)
                        if (cachedPlayer.PlayerId != douseTarget.PlayerId)
                            untargetables.Add(cachedPlayer);
                }
                else
                {
                    untargetables = dousedPlayers;
                }

                currentTarget = SetTarget(untarget: untargetables, distances: 0.5f);
                if (currentTarget != null) SetPlayerOutline(currentTarget, color);

                arsonistButton.showTargetNameOnButton(currentTarget, GetString("DouseText"));

                if (arsonistButton.isEffectActive && douseTarget != currentTarget)
                {
                    douseTarget = null;
                    arsonistButton.Timer = 0f;
                    arsonistButton.isEffectActive = false;
                }

                return PlayerControl.LocalPlayer.CanMove && currentTarget != null;
            },
            () =>
            {
                arsonistButton.Timer = arsonistButton.MaxTimer;
                arsonistButton.isEffectActive = false;
                douseTarget = null;
            },
            douseSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            true,
            duration,
            () =>
            {
                if (douseTarget != null) dousedPlayers.Add(douseTarget);

                arsonistButton.Timer = arsonistButton.MaxTimer;
                arsonistKillButton.Timer = arsonistKillButton.MaxTimer == 0 ? 1f : arsonistKillButton.MaxTimer;
                foreach (var p in dousedPlayers)
                    if (ModOption.playerIcons.ContainsKey(p.PlayerId))
                        ModOption.playerIcons[p.PlayerId].setSemiTransparent(false);

                // Ghost Info
                var writer = StartRPC(CustomRPC.ShareGhostInfo);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.ArsonistDouse);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(douseTarget.PlayerId);
                writer.EndRPC();

                douseTarget = null;
            },
            buttonText: GetString("DouseText")
        )
        {
            MaxTimer = cooldown,
            EffectDuration = duration,
        };

        // Arsonist button (点火)
        arsonistKillButton?.Destroy();
        arsonistKillButton = new CustomButton(
            () =>
            {
                foreach (PlayerControl p in dousedPlayers.Where(p => p.IsAlive()))
                {
                    RpcCustomMurderPlayer(Player, p, false, true, CustomDeathReason.Arson);
                }
                arsonistKillButton.Timer = arsonistKillButton.MaxTimer;
                arsonistButton.Timer = arsonistButton.MaxTimer;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() && dousedPlayers.Count > 0;
            },
            () =>
            {
                currentTarget2 = SetTarget(distances: 0.5f);
                var cankill = false;
                if (currentTarget2 && dousedPlayers.Any(x => x == currentTarget2))
                {
                    SetPlayerOutline(currentTarget2, color);
                    arsonistKillButton.showTargetNameOnButton(currentTarget2, GetString("IgniteText"));
                    cankill = true;
                }

                return PlayerControl.LocalPlayer.CanMove && cankill;
            },
            () =>
            {
                var count = PlayerControl.AllPlayerControls.ToList().Count(p => p.IsAlive() && p.IsKiller() && p != Player);

                if (count == 0 && igniteCooldownRemoved) arsonistKillButton.Timer = arsonistKillButton.MaxTimer = 0f;
                else arsonistKillButton.Timer = arsonistKillButton.MaxTimer = arsonistButton.MaxTimer;
            },
            igniteSprite,
            __instance,
            __instance.KillButton,
            ModInputManager.modKillInput.keyCode,
            buttonText: GetString("IgniteText")
        )
        {
            MaxTimer = cooldown,
        };
    }
}
