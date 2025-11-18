using static TheOtherRoles.Options.CustomOption;

namespace TheOtherRoles.Roles;

public class RoleInfo
{
    public static IReadOnlyList<RoleInfo> AllRoleInfo => _AllRoleInfo;
    private static readonly List<RoleInfo> _AllRoleInfo = new();

    public Type RoleObjectType { get; }
    public string RoleObjectTypeName { get; }
    public string NameKey { get; }
    public Color Color { get; }
    public RoleId RoleId { get; }
    public RoleType RoleType { get; }
    public int ConfigId { get; }
    public bool CanAssign { get; }
    public int MaxPlayer { get; }
    public bool IsHidden { get; }
    public bool IsKiller { get; }
    public Func<PlayerControl, RoleBase> CreateRoleInstance { get; }
    public Func<PlayerControl, ModifierBase> CreateAbilityInstance { get; }
    public CustomOptionType OptionType { get; }
    public string Name => NameKey.Translate();

    public CustomOption RoleOption { get; private set; }
    public CustomOption PlayerCountOption { get; private set; }
    public Action OptionCreater { get; private set; }
    public int AssignSelection => CanAssign ? RoleOption?.GetSelection() ?? 0 : 0;
    public int PlayerCount
    {
        get
        {
            if (AssignSelection == 0) return 0;
            if (RoleDraft.isEnabled) return AssignSelection > 0 ? 1 : 0;
            return PlayerCountOption?.GetInt() ?? (AssignSelection > 0 ? 1 : 0);
        }
    }

    public string IntroDescription => GetString(NameKey + "IntroDesc");
    public string ShortDescription => GetString(NameKey + "ShortDesc");
    public string FullDescription => GetString(NameKey + "FullDesc");

    public RoleInfo(
        Type RoleObjectType,
        Func<PlayerControl, RoleBase> CreateRoleInstance,
        RoleId RoleId,
        RoleType Type,
        string NameKey,
        Color32 Color,
        int ConfigId,
        Action OptionCreator = null,
        bool CanAssign = true,
        bool Hidden = false,
        int MaxPlayer = 15,
        bool IsKiller = false
        )
    {
        this.RoleObjectType = RoleObjectType;
        this.Color = Color;
        this.RoleId = RoleId;
        this.NameKey = NameKey;
        this.ConfigId = ConfigId;
        this.RoleType = Type;
        this.CreateRoleInstance = CreateRoleInstance;
        this.OptionCreater = OptionCreator;
        this.CanAssign = CanAssign;
        this.IsHidden = Hidden;
        this.MaxPlayer = MaxPlayer;
        this.IsKiller = IsKiller;
        OptionType = this.RoleType switch
        {
            RoleType.Impostor => CustomOptionType.Impostor,
            RoleType.Crewmate => CustomOptionType.Crewmate,
            RoleType.Neutral => CustomOptionType.Neutral,
            RoleType.Modifier => CustomOptionType.Modifiers,
            _ => CustomOptionType.General
        };
        _AllRoleInfo.Add(this);
        CustomRoleManager.AllRolesInfo.Add(RoleId, this);
    }

    public RoleInfo(
        Type RoleObjectType,
        Func<PlayerControl, ModifierBase> CreateRoleInstance,
        RoleId RoleId,
        string NameKey,
        Color32 Color,
        int ConfigId,
        Action OptionCreator = null,
        bool CanAssign = true,
        bool Hidden = false,
        int MaxPlayer = 15
        )
    {
        this.RoleObjectType = RoleObjectType;
        this.Color = Color;
        this.RoleId = RoleId;
        this.NameKey = NameKey;
        this.ConfigId = ConfigId;
        this.RoleType = RoleType.Modifier;
        this.CreateAbilityInstance = CreateRoleInstance;
        this.OptionCreater = OptionCreator;
        this.CanAssign = CanAssign;
        this.IsHidden = Hidden;
        this.MaxPlayer = MaxPlayer;
        OptionType = this.RoleType switch
        {
            RoleType.Impostor => CustomOptionType.Impostor,
            RoleType.Crewmate => CustomOptionType.Crewmate,
            RoleType.Neutral => CustomOptionType.Neutral,
            RoleType.Modifier => CustomOptionType.Modifiers,
            _ => CustomOptionType.General
        };
        _AllRoleInfo.Add(this);
        CustomRoleManager.AllRolesInfo.Add(RoleId, this);
    }

    public RoleBase CreateInstance(PlayerControl player)
    {
        if (CreateRoleInstance != null) return CreateRoleInstance(player);
        return Activator.CreateInstance(RoleObjectType, player as object) as RoleBase;
    }

    public ModifierBase CreateModifier(PlayerControl player)
    {
        if (CreateAbilityInstance != null) return CreateAbilityInstance(player);
        return Activator.CreateInstance(RoleObjectType, player as object) as ModifierBase;
    }

    public void CreateOption()
    {
        if (RoleOption != null) return;

        if (CanAssign)
        {
            RoleOption = Create(ConfigId + 1, OptionType, Cs(Color, NameKey), CustomOptionHolder.rates, null, true);
            CustomRoleSpawnChances.TryAdd(RoleOption.RoleId, RoleOption);

            if (MaxPlayer > 1)
            {
                PlayerCountOption = Create(ConfigId + 2, OptionType, "RolesCount", 1, 1, MaxPlayer, 1, RoleOption, isHidden: () => RoleDraft.isEnabled);
                CustomRoleCounts.TryAdd(RoleOption.RoleId, PlayerCountOption);
            }
        }

        OptionCreater?.Invoke();
    }

    public static string GetRolesString(PlayerControl p, bool useColors, bool showModifier = true, bool showGhostInfo = true, bool onlyGhostRole = false)
    {

        string roleName = p.GetRoleInfo().Name;

        /*if (onlyGhostRole)
        {
            var ghostRoleInfo = getRoleInfoForPlayer(p, false, true).FirstOrDefault(x => x.roleType == RoleType.Ghost);

            if (p.Data.IsDead && ghostRoleInfo != null)
            {
                roleName = p.GetRoleInfo().Name;
            }
        }*/

        /*if (Lawyer.target != null && p.PlayerId == Lawyer.target.PlayerId && PlayerControl.LocalPlayer != Lawyer.target)
            roleName += useColors ? cs(Lawyer.color, " §") : " §";

        if (Executioner.target != null && p.PlayerId == Executioner.target.PlayerId && PlayerControl.LocalPlayer != Executioner.target)
            roleName += useColors ? cs(Executioner.color, " §") : " §";

        if (p.Is(RoleId.Jackal) && Jackal.canSwoop)
            roleName += "JackalIsSwooperInfo".Translate();

        if (HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(p.PlayerId) && p != Doomsayer.doomsayer)
            roleName += "GuessserGMInfo".Translate();

        if (showGhostInfo && p != null)
        {
            if (p == Shifter.shifter && (PlayerControl.LocalPlayer == Shifter.shifter || ShowGhostInfo) && Shifter.futureShift != null)
                roleName += cs(Color.yellow, " ← " + Shifter.futureShift.Data.PlayerName);
            if (p == Vulture.vulture && (PlayerControl.LocalPlayer == Vulture.vulture || ShowGhostInfo))
                roleName += cs(Vulture.color, string.Format("roleInfoRemaining".Translate(), Vulture.numberToWin - Vulture.eatenBodies));
            if (p == Witness.Player && (PlayerControl.LocalPlayer == Witness.Player || ShowGhostInfo))
                roleName += cs(Witness.color, string.Format("roleInfoRemaining".Translate(), Witness.exileToWin - Witness.exiledCount));

            if (ShowGhostInfo)
            {
                if (Eraser.futureErased.Any(x => x == p))
                    roleName = cs(Color.gray, "(被抹除) ") + roleName;
                if (Vampire.vampire != null && !Vampire.vampire.Data.IsDead && Vampire.bitten == p && !p.Data.IsDead)
                    roleName = cs(Vampire.color, $"(被吸血 {(int)HudManagerStartPatch.vampireKillButton.Timer + 1}) ") + roleName;
                if (CustomRoleManager.handcuffedPlayers.Any(x => x == p.PlayerId))
                    roleName = cs(Color.gray, "(被上拷) ") + roleName;
                if (CustomRoleManager.handcuffedKnows.ContainsKey(p.PlayerId)) // Active cuff
                    roleName = cs(Sheriff.color, "(被上拷) ") + roleName;
                if (p == Warlock.curseVictim)
                    roleName = cs(Warlock.color, "(被下咒) ") + roleName;
                if (p == Ninja.ninjaMarked)
                    roleName = cs(Ninja.color, "(被标记) ") + roleName;
                if (p == Thief.formerThief)
                    roleName += cs(Thief.color, " (窃)");
                if (Pursuer.blankedList.Contains(p) && !p.Data.IsDead)
                    roleName = cs(Pursuer.color, "(被塞空包弹) ") + roleName;
                if (Witch.futureSpelled.Any(x => x == p) && !MeetingHud.Instance) // This is already displayed in meetings!
                    roleName = cs(Witch.color, "☆ ") + roleName;
                if (BountyHunter.bounty == p && BountyHunter.bountyHunter.IsAlive())
                    roleName = cs(BountyHunter.color, "(被悬赏) ") + roleName;
                if (p == Arsonist.arsonist)
                    roleName += cs(Arsonist.color, $" (剩余 {PlayerControl.AllPlayerControls
                        .Count(x => x != Arsonist.arsonist && x.IsAlive() &&
                        !Arsonist.dousedPlayers.Any(y => y.PlayerId == x.PlayerId))} )");
                if (Akujo.keeps.Any(x => x.PlayerId == p.PlayerId))
                    roleName = cs(Color.gray, "(备胎) ") + roleName;
                if (p == Akujo.honmei)
                    roleName = cs(Akujo.color, "(真爱) ") + roleName;
            }
        }*/

        return roleName;
    }

    public static string getRoleDescription(string name)
    {
        foreach (var roleInfo in AllRoleInfo)
        {
            if (roleInfo.Name == name) return $"{name}: \n{$"{roleInfo.NameKey}FullDesc".Translate()}";
        }
        return null;
    }
}
