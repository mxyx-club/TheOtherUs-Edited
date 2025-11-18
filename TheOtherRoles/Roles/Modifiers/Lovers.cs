namespace TheOtherRoles.Roles.Modifier;

public class Lovers : ModifierBase
{
    public static Color color = new Color32(232, 57, 185, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Lovers),
        (p) => new Lovers(p),
        RoleId.Lover,
        "Lover",
        color,
        401600
    );

    public Lovers(PlayerControl p) : base(p, roleinfo) { }

    public PlayerControl Lover;
    public bool notAckedExiledIsLover;

    public static int ImpLoverRate;
    public static bool bothDie = true;
    public static bool enableChat = true;

    public static CustomOption modifierLoverImpLoverRate;
    public static CustomOption modifierLoverBothDie;
    public static CustomOption modifierLoverEnableChat;

    public override RoleType[] RemoveTeam { get; set; } = [];
    public new bool IsAlive => Player.IsAlive() && Lover.IsAlive() && !notAckedExiledIsLover;

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierLoverImpLoverRate = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierLoverImpLoverRate", CustomOptionHolder.rates, roleinfo.RoleOption);
        modifierLoverBothDie = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierLoverBothDie", true, roleinfo.RoleOption);
        modifierLoverEnableChat = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierLoverEnableChat", true, roleinfo.RoleOption);
    }

    public bool IsLover(PlayerControl player) => player != null && (player == Player || player == Lover);


    public bool IsKillerLover() => Player.IsKiller() || Lover.IsKiller();

    public PlayerControl OtherLover(PlayerControl player)
    {
        if (player == null) return null;
        if (player == Player) return Lover;
        if (player == Lover) return Player;
        return null;
    }

    public bool hasAliveKillingLover(PlayerControl player)
    {
        return IsLover(player) && IsAlive && IsKillerLover();
    }

    public override void Initialize()
    {
        Lover = null;
        notAckedExiledIsLover = false;
        ImpLoverRate = modifierLoverImpLoverRate.GetInt();
        bothDie = modifierLoverBothDie.GetBool();
        enableChat = modifierLoverEnableChat.GetBool();
    }

    public override bool SeeRoleTag(PlayerControl seen, PlayerControl seer, out string tag)
    {
        if ((IsLover(seen) && seer == OtherLover(seen)) || CanSeeGhostInfo)
        {
            tag = Cs(color, " ♥");
            return true;
        }
        tag = string.Empty;
        return false;
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (IsLover(Info.Target))
        {
            var otherLover = OtherLover(Info.Target);
            if (otherLover.IsAlive() && bothDie)
                CustomMurderPlayer(otherLover, otherLover, false, true, CustomDeathReason.LoverSuicide);
        }
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        if (OtherLover(exiled?.Object) != null)
        {
            var otherLover = OtherLover(exiled.Object);
            if (otherLover.IsAlive() && bothDie)
            {
                otherLover.Exiled();
                PlayerData.SetDeathReason(otherLover, CustomDeathReason.LoverSuicide);
            }
        }
    }
}
