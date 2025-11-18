namespace TheOtherRoles.Roles.Neutral;

public class Pavlovsdogs : RoleBase, INeutral
{
    public static Color color = new Color32(244, 169, 106, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Pavlovsdogs),
        (p) => new Pavlovsdogs(p),
        RoleId.Pavlovsdogs,
        RoleType.Neutral,
        "Pavlovsdogs",
        color,
        206400,
        null,
        false,
        true,
        IsKiller: true
    );

    public Pavlovsdogs(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Kill;
    public override bool? CanUseVent => Pavlovsowner.pavlovsownerCanUseVents.GetSelection() is 1 or 3;
    public override bool? HasImpVision => Pavlovsowner.pavlovsownerHasImpostorVision.GetBool();
    public override bool? IsKiller => true;

    public PlayerControl currentTarget;
    public float deathTime;

    public static float cooldown = 30f;
    public static bool enableRampage;
    public static float rampageKillCooldown;
    public static int rampageDeathTime;

    public CustomButton pavlovsdogsKillButton;

    public override void Initialize()
    {
        currentTarget = null;
        cooldown = Pavlovsowner.pavlovsownerKillCooldown.GetFloat();

        enableRampage = Pavlovsowner.pavlovsownerRampage.GetBool();
        rampageKillCooldown = Pavlovsowner.pavlovsownerRampageKillCooldown.GetFloat();
        rampageDeathTime = Pavlovsowner.pavlovsownerRampageDeathTime.GetInt();
    }

    public override void CleanUp(HudManager __instance)
    {
        pavlovsdogsKillButton?.Destroy();
        pavlovsdogsKillButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        pavlovsdogsKillButton?.Destroy();
        pavlovsdogsKillButton = new CustomButton(
            () =>
            {
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, currentTarget)) return;
                if (enableRampage)
                {
                    deathTime = rampageDeathTime;
                    pavlovsdogsKillButton.MaxTimer = Player.IsDead() ? rampageKillCooldown : cooldown;
                }
                pavlovsdogsKillButton.Timer = pavlovsdogsKillButton.MaxTimer;
                currentTarget = null;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsAlive() && PlayerControl.LocalPlayer == Player;
            },
            () =>
            {
                if (enableRampage && Player.IsDead() && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    deathTime -= Time.deltaTime;
                    if (pavlovsdogsKillButton.ButtonTitle != null)
                        pavlovsdogsKillButton.ButtonTitle.text = string.Format("SerialKillerSuicideText".Translate(), (int)deathTime + 1);

                    if (deathTime <= 0)
                        PlayerControl.LocalPlayer.RpcMurderPlayer(PlayerControl.LocalPlayer, true);
                }

                var untargetablePlayers = new List<PlayerControl>();
                untargetablePlayers.AddRange(GetRoles(RoleId.Pavlovsowner)?.Select(x => x.Player));
                untargetablePlayers.AddRange(GetRoles(RoleId.Pavlovsdogs)?.Select(x => x.Player));
                untargetablePlayers.AddRange(Mini.UngrownMinis);
                currentTarget = SetTarget(untarget: untargetablePlayers);
                SetPlayerOutline(currentTarget, Palette.ImpostorRed);

                pavlovsdogsKillButton.showTargetNameOnButton(currentTarget, GetString("killButtonText")); return currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                if (enableRampage)
                {
                    deathTime = rampageDeathTime;
                    pavlovsdogsKillButton.MaxTimer = Player.IsDead() ? rampageKillCooldown : cooldown;
                }
                pavlovsdogsKillButton.Timer = pavlovsdogsKillButton.MaxTimer;
            },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            ModInputManager.modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        )
        {
            MaxTimer = cooldown,
        };
        // PavlovsdogKillSelfText -> pavlovsdogsKillButton
    }
}
