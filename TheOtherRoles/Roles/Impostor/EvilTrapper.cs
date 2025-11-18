using Il2CppSystem.Xml.Schema;
using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class EvilTrapper : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(EvilTrapper),
        (p) => new EvilTrapper(p),
        RoleId.EvilTrapper,
        RoleType.Impostor,
        "EvilTrapper",
        color,
        103000,
        AddOptions
    );

    public EvilTrapper(PlayerControl p) : base(p, roleInfo) { }

    public PlayerControl trapTarget;
    public TMP_Text text;
    public bool isTrapKill;

    public static float minDistance;
    public static int numTrap;
    public static float extensionTime;
    public static float cooldown;
    public static float killTimer;
    public static float trapRange;
    public static float maxDistance;
    public static float penaltyTime;
    public static float bonusTime;

    public static CustomOption evilTrapperNumTrap;
    public static CustomOption evilTrapperExtensionTime;
    public static CustomOption evilTrapperCooldown;
    public static CustomOption evilTrapperKillTimer;
    public static CustomOption evilTrapperTrapRange;
    public static CustomOption evilTrapperMaxDistance;
    public static CustomOption evilTrapperPenaltyTime;
    public static CustomOption evilTrapperBonusTime;

    public CustomButton evilTrapperSetTrapButton;
    public static Sprite trapButtonSprite = new ResourceSprite("TrapperButton.png");
    public static RemoteProcess<(PlayerControl player, Vector3 position)> PlaceTrap = new("PlaceTrap", (data, _) =>
    {
        var pos = Vector3.zero;
        pos.x = data.position.x;
        pos.y = data.position.y - 0.2f;
        var killtrap = new KillTrap(data.player, pos);
    });
    public static RemoteProcess<(PlayerControl player, bool active)> ClearAllTraps = new("ClearAllTraps", (data, _) =>
    // public static void ClearAllTraps(PlayerControl trapper, bool active)
    {
        var traps = KillTrap.AllObjects.Where(x => x.trapper == data.player && (data.active || !x.isTriggered)).ToArray();
        foreach (var t in traps)
        {
            t?.Destroy();
        }
    });

    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        evilTrapperNumTrap = CustomOption.Create(configId++, CustomOptionType.Impostor, "evilTrapperNumTrap", 3f, 1f, 10f, 1f, roleInfo.RoleOption);
        evilTrapperExtensionTime = CustomOption.Create(configId++, CustomOptionType.Impostor, "evilTrapperExtensionTime", 5f, 2f, 10f, 0.5f, roleInfo.RoleOption);
        evilTrapperCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "evilTrapperCooldown", 15f, 10f, 60f, 2.5f, roleInfo.RoleOption);
        evilTrapperKillTimer = CustomOption.Create(configId++, CustomOptionType.Impostor, "evilTrapperKillTimer", 5f, 1f, 30f, 1f, roleInfo.RoleOption);
        evilTrapperTrapRange = CustomOption.Create(configId++, CustomOptionType.Impostor, "evilTrapperTrapRange", 1f, 0.25f, 2f, 0.125f, roleInfo.RoleOption);
        evilTrapperMaxDistance = CustomOption.Create(configId++, CustomOptionType.Impostor, "evilTrapperMaxDistance", 10f, 0f, 20f, 0.25f, roleInfo.RoleOption);
        evilTrapperPenaltyTime = CustomOption.Create(configId++, CustomOptionType.Impostor, "evilTrapperPenaltyTime", 0f, 0f, 30f, 0.5f, roleInfo.RoleOption);
        evilTrapperBonusTime = CustomOption.Create(configId++, CustomOptionType.Impostor, "evilTrapperBonusTime", 10f, 0f, 15f, 0.5f, roleInfo.RoleOption);
    }

    public static void setTrap()
    {
        var pos = PlayerControl.LocalPlayer.transform.position;
        PlaceTrap.Invoke((PlayerControl.LocalPlayer, pos));
    }

    public override void Initialize()
    {
        trapTarget = null;
        numTrap = evilTrapperNumTrap.GetInt();
        extensionTime = evilTrapperExtensionTime.GetFloat();
        killTimer = evilTrapperKillTimer.GetFloat();
        cooldown = evilTrapperCooldown.GetFloat();
        maxDistance = evilTrapperMaxDistance.GetFloat();
        trapRange = evilTrapperTrapRange.GetFloat();
        penaltyTime = evilTrapperPenaltyTime.GetFloat();
        bonusTime = evilTrapperBonusTime.GetFloat();
        text = null;
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        try
        {
            if (trapTarget.IsAlive() && text == null)
            {
                RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                GameObject gameObject = UObject.Instantiate(roomTracker.gameObject);
                UObject.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
                gameObject.transform.localScale = Vector3.one * 2f;
                text ??= gameObject.GetComponent<TMP_Text>();
                text.text = string.Format(GetString("trapperGotTrapText"), trapTarget.Data.PlayerName);

            }
            if (text != null) UObject.Destroy(text.gameObject);
        }
        catch (NullReferenceException e)
        {
            Warn(e.Message);
        }
    }
    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (Player != null && PlayerControl.LocalPlayer == Player && Info.Killer == Player)
        {
            if (KillTrap.isTrapped(Info.Target) && !isTrapKill)  // トラップにかかっている対象をキルした場合のボーナス
            {
                Player.killTimer = ModOption.KillCooldown - bonusTime;
                evilTrapperSetTrapButton.Timer = cooldown - bonusTime;
            }
            else if (KillTrap.isTrapped(Info.Target) && isTrapKill)  // トラップキルした場合のペナルティ
            {
                Player.killTimer = ModOption.KillCooldown;
                evilTrapperSetTrapButton.Timer = cooldown;
            }
            else // トラップにかかっていない対象を通常キルした場合はペナルティーを受ける
            {
                Player.killTimer = ModOption.KillCooldown + penaltyTime;
                evilTrapperSetTrapButton.Timer = cooldown + penaltyTime;
            }
            if (!isTrapKill)
            {
                ClearAllTraps.Invoke((PlayerControl.LocalPlayer, false));
            }
            isTrapKill = false;
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        evilTrapperSetTrapButton?.Destroy();
        evilTrapperSetTrapButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        evilTrapperSetTrapButton?.Destroy();
        evilTrapperSetTrapButton = new CustomButton(
            () =>
            { // ボタンが押された時に実行
                if (!PlayerControl.LocalPlayer.CanMove || KillTrap.hasTrappedPlayer()) return;
                setTrap();
                evilTrapperSetTrapButton.Timer = evilTrapperSetTrapButton.MaxTimer;
            },
            () =>
            { //ボタン有効になる条件
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            { //ボタンが使える条件
                return PlayerControl.LocalPlayer.CanMove && !KillTrap.hasTrappedPlayer();
            },
            () =>
            { //ミーティング終了時
                evilTrapperSetTrapButton.Timer = evilTrapperSetTrapButton.MaxTimer;
            },
            trapButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("PlaceTrapText")
        )
        {
            MaxTimer = cooldown,
        };
    }
}
