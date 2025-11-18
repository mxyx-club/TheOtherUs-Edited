using AmongUs.GameOptions;
using TheOtherRoles.Patches;
using static TheOtherRoles.Roles.CustomRoleManager;

namespace TheOtherRoles.Roles;
#nullable enable

public static class RoleHelpers
{
    public static bool CanSeeGhostInfo
    {
        get
        {
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Ended) return true;
            else if (PlayerControl.LocalPlayer.IsAlive()) { field = false; return false; }
            else return field;
        }
        set
        {
            if (PlayerControl.LocalPlayer.IsAlive()) field = false;
            else if (PlayerControl.LocalPlayer == Specter.Player) field = false;
            else if (Specter.Player.GetPartner() == PlayerControl.LocalPlayer) field = false;
            else field = value;
        }
    }

    public static PlayerControl? PlayerById(byte? id)
    {
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            if (player.PlayerId == id) return player;

        return null;
    }

    public static PlayerControl? PlayerByName(string name)
    {
        if (name.IsNullOrWhiteSpace()) return null;
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            if (player?.Data?.PlayerName == name) return player;
        return null;
    }

    public static bool Is(this PlayerControl player, RoleId roleId)
    {
        if (player == null) return false;

        var roleBase = player.GetRoleBase();
        if (roleBase?.RoleId == roleId)
            return true;

        if (roleBase is Mimic { TargetRole: not null } mimic && mimic.TargetRole.RoleId == roleId)
            return true;

        if (AllActiveModifier.TryGetValue(player.PlayerId, out var modifiers))
        {
            foreach (var modifier in modifiers)
            {
                if (modifier.RoleId == roleId)
                    return true;
            }
        }

        return false;
    }

    public static T? As<T>(this IRoleInfo? role) where T : class, IRoleInfo => role as T;

    public static T? GetRoleOfType<T>(this PlayerControl player) where T : class, IRoleInfo
    {
        if (player == null) return null;

        return player.GetRoleBase() as T ?? player.GetModifierBase()?.OfType<T>().FirstOrDefault();
    }

    public static T? GetRole<T>(this PlayerControl player) where T : RoleBase
    {
        if (player == null) return null;
        return player?.GetRoleBase() as T;
    }

    public static T? GetModifier<T>(this PlayerControl player) where T : ModifierBase
    {
        if (!AllActiveModifier.TryGetValue(player.PlayerId, out var abilities)) return null;
        return abilities.OfType<T>().FirstOrDefault();
    }

    public static IEnumerable<IRoleInfo> GetRoles(RoleId roleId)
    {
        return AllRoles.Where(x => x.RoleId == roleId);
    }

    public static IEnumerable<T> GetRoles<T>() where T : IRoleInfo
    {
        return AllRoles.OfType<T>();
    }

    public static IEnumerable<ModifierBase> GetModifier(RoleId roletype)
    {
        return AllActiveModifier.Values.SelectMany(x => x).Where(x => x.RoleId == roletype);
    }

    public static bool TryGetRole<T>(this PlayerControl? player, out T result) where T : RoleBase
    {
#pragma warning disable CS8625
        result = null;

        if (player == null || !AllActiveRoles.TryGetValue(player.PlayerId, out var role))
            return false;

        if (role is T matchedRole)
        {
            result = matchedRole;
            return true;
        }
        return result != null;
    }

    public static bool TryGetModifier<T>(this PlayerControl? player, out T result) where T : ModifierBase
    {
        result = null;

        if (player == null || !AllActiveModifier.TryGetValue(player.PlayerId, out var abilities))
            return false;

        foreach (var ability in abilities)
        {
            if (ability is T matchedAbility)
            {
                result = matchedAbility;
                return true;
            }
        }
        return false;
    }

    public static bool IsNeutral(this PlayerControl player)
    {
        if (player.GetRoleBase() is INeutral) return true;
        return false;
    }

    public static bool IsKiller(this PlayerControl player)
    {
        return player != null && (player.IsImpostor() || IsKillerNeutral(player));
    }

    public static bool IsBenignNeutral(this PlayerControl player)
    {
        if (player.GetRoleBase() is INeutral neutral)
        {
            return neutral.NeutralType == NeutralType.Benign;
        }
        return false;
    }

    public static bool IsKillerNeutral(this PlayerControl player)
    {
        if (player.GetRoleBase() is INeutral neutral)
        {
            return neutral.NeutralType == NeutralType.Kill;
        }
        return false;
    }

    public static bool IsEvilNeutral(this PlayerControl player)
    {
        if (player.GetRoleBase() is INeutral neutral)
        {
            return neutral.NeutralType == NeutralType.Evil;
        }
        return false;
    }

    public static bool IsCrew(this PlayerControl player)
    {
        return player != null && !player.Data.Role.IsImpostor && !player.IsNeutral();
    }

    public static bool IsImpostor(this PlayerControl player, bool AndSpy = false, bool AndCat = false)
    {
        if (player == null) return false;
        return player.Data.Role.IsImpostor
            || (AndSpy && player.Is(RoleId.Spy) == player)
            || (AndCat && player.TryGetRole<SchrodingersCat>(out var cat) && cat?.State == SchrodingersCat.CatState.Impostor);
    }

    public static RoleId GetRole(this PlayerControl player)
    {
        if (player == null) return RoleId.DefaultRole;
        if (AllActiveRoles.TryGetValue(player.PlayerId, out var roleBase))
        {
            return roleBase.RoleId;
        }
        return player.IsImpostor() ? RoleId.Impostor : RoleId.Crewmate;
    }

    public static List<RoleId> GetModifier(this PlayerControl player)
    {
        if (player == null) return [];
        List<RoleId> abilitys = [];
        if (AllActiveModifier.TryGetValue(player.PlayerId, out var ability))
        {
            ability.ForEach(x => abilitys.Add(x.RoleId));
        }
        return abilitys ?? [];
    }

    public static RoleInfo? GetRoleInfo(this PlayerControl player)
    {
        if (player == null) return null;
        var info = player.GetRoleBase()?.RoleInfo;
        info ??= player.IsImpostor() ? Vanilla.Impostor.roleInfo : Vanilla.Crewmate.roleInfo;
        return info;
    }

    public static Color GetRoleColor(this RoleId role)
    {
        var roleInfo = CustomRoleManager.GetRoleInfo(role);
        if (roleInfo != null) return roleInfo.Color;
        return Palette.CrewmateBlue;
    }

    public static RoleType GetRoleTeam(this RoleId role)
    {
        var roleInfo = CustomRoleManager.GetRoleInfo(role);
        if (roleInfo != null) return roleInfo.RoleType;
        return RoleType.Special;
    }

    public static bool CheckAndDoVetKill(PlayerControl killer, PlayerControl target)
    {
        var shouldVetKill = target.TryGetRole<Veteran>(out var veteran);
        if (veteran != null && veteran.alertActive)
        {
            var writer = StartRPC(CustomRPC.CustomMurderPlayer);
            writer.Write(target.PlayerId);
            writer.Write(killer.PlayerId);
            writer.Write(true);
            writer.Write(false);
            writer.Write((byte)CustomDeathReason.Veteran);
            writer.EndRPC();
            MurderPlayer(target, killer, true, false, CustomDeathReason.Veteran);
        }

        return shouldVetKill;
    }

    public static bool CheckMurderPlayer(PlayerControl killer, PlayerControl target)
    {
        var info = new MurderInfo(killer, target);
        return CustomRoleManager.CheckMurderPlayer(info);
    }

    public static void CustomMurderPlayer(PlayerControl killer, PlayerControl target, bool showAnimation = true, bool force = false,
        CustomDeathReason deathReason = CustomDeathReason.Null) => MurderPlayer(killer, target, showAnimation, force, deathReason);

    public static bool RpcCustomMurderPlayer(
        PlayerControl killer, PlayerControl target, bool showAnimation = true, bool force = false,
        CustomDeathReason deathReason = CustomDeathReason.Null)
    {
        var flag = true;
        if (!force)
        {
            flag = CheckMurderPlayer(killer, target);
        }

        if (!flag) return false;

        var writer = StartRPC(CustomRPC.CustomMurderPlayer);
        writer.Write(killer.PlayerId);
        writer.Write(target.PlayerId);
        writer.Write(showAnimation);
        writer.Write(flag);
        writer.Write((byte)deathReason);
        writer.EndRPC();
        MurderPlayer(killer, target, showAnimation, flag, deathReason);

        return flag;
    }

    public static bool IsAlive(this PlayerControl? player)
    {
        return player != null && !player.Data.Disconnected && !player.Data.IsDead;
    }

    public static bool IsDead(this PlayerControl? player)
    {
        return player == null || player.Data.Disconnected || player.Data.IsDead;
    }

    public static bool IsLover(this PlayerControl? player)
    {
        if (player == null) return false;
        if (player.TryGetModifier<Lovers>(out var lover))
        {
            return lover?.IsLover(player) ?? false;
        }
        return false;
    }

    public static PlayerControl? OtherLover(this PlayerControl player)
    {
        if (player == null) return null;
        if (player.TryGetModifier<Lovers>(out var lover))
        {
            return lover?.OtherLover(player);
        }
        return null;
    }

    public static PlayerControl? GetPartner(this PlayerControl player)
    {
        var akujo = Akujo.Players.FirstOrDefault(x => x.isAkujoTeam(player));
        if (akujo != null) return akujo.OtherLover(player);
        return Lovers.bothDie ? player.OtherLover() : null;
    }

    public static bool PowerCrewIsAlive()
    {
        // This functions blocks the game from ending if specified crewmate roles are alive
        if (!CustomOptionHolder.blockGameEnd.GetBool()) return false;

        foreach (var role in AllActiveRoles.Values)
        {
            if (role is IPowerCrew && role.Player.IsAlive())
            {
                return true;
            }
        }

        return false;
    }

    public static bool HasFakeTasks(this PlayerControl player)
    {
        return player.GetRoleBase()?.HasTasks == true;
    }

    public static bool HasImpVision(GameData.PlayerInfo data)
    {
        var player = PlayerById(data?.PlayerId);
        var role = player?.GetRoleBase();
        if (player == null || role == null) return false;

        return role?.HasImpVision == true;
    }

    public static bool CanUseVents(this PlayerControl player)
    {
        if (player.inVent) return true;
        var role = player.GetRoleBase();
        return role?.CanUseVent == true;
    }

    public static bool CanUseSabotage(this PlayerControl player)
    {
        var roleCouldUse = false;
        if (ModOption.disableSabotage) return false;
        if (Jackal.canSabotage && (player.Is(RoleId.Jackal) || player.Is(RoleId.Sidekick)) && !ModOption.disableSabotage)
            roleCouldUse = true;
        if (Pavlovsowner.canSabotage && (player.Is(RoleId.Pavlovsowner) || player.Is(RoleId.Pavlovsdogs)) && !ModOption.disableSabotage)
            roleCouldUse = true;
        if (player.Data?.Role != null && player.Data.Role.IsImpostor)
            roleCouldUse = true;
        return roleCouldUse;
    }

    public static void setPlayerNameColor(PlayerControl p, Color color)
    {
        p.cosmetics.nameText.color = color.SetAlpha(Chameleon.visibility(p.PlayerId));
        if (MeetingHud.Instance != null)
        {
            foreach (var player in MeetingHud.Instance.playerStates)
                if (player.NameText != null && p.PlayerId == player.TargetPlayerId)
                    player.NameText.color = color;
        }
    }

    public static IEnumerable<DeadBody> AllDeadBodies()
    {
        //Componentで探すよりタグで探す方が相当はやい
        var bodies = GameObject.FindGameObjectsWithTag("DeadBody");
        for (int i = 0; i < bodies.Count; i++) yield return bodies[i].GetComponent<DeadBody>();
    }

    public static DeadBody? GetDeadBody(byte id)
    {
        return AllDeadBodies().FirstOrDefault((p) => p.ParentId == id);
    }

    public static PlayerControl SetTarget(object? untarget = null, bool onlyCrewmates = false,
        bool targetInVents = false, float distances = 0f, PlayerControl? targetingPlayer = null)
    {
        return PlayerControlFixedUpdatePatch.SetTarget(onlyCrewmates, targetInVents, untarget, KillDistances: distances, targetingPlayer: targetingPlayer);
    }

    public static void SetPlayerOutline(PlayerControl target, Color color)
    {
        if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) return;

        color = color.SetAlpha(Chameleon.visibility(target.PlayerId));

        target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
        target.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", color);
    }

    public static PlayerControl? ImpostorSetTarget()
    {
        PlayerControl target;
        if (GetRoles(RoleId.Spy).Any())
        {
            target = Spy.impostorsCanKillAnyone ? SetTarget(null, false, true) : SetTarget(new RoleId[] { RoleId.Spy }, true, true);
        }
        else
        {
            target = SetTarget(null, true, true);
        }

        if (target == null) return null;
        if (SchrodingersCat.AllCat.Any(x => x.State == SchrodingersCat.CatState.Impostor && x.Player == target)) return null;
        if (Mini.UngrownMinis.Contains(target)) return null;

        SetPlayerOutline(target, Palette.ImpostorRed);
        return target;
    }

    public static void turnToImpostor(PlayerControl? player)
    {
        if (player == null) return;
        player.Data.Role.TeamType = RoleTeamTypes.Impostor;
        SetRoleType(player, RoleTypes.Impostor);
        RPCProcedure.setRole(player.PlayerId, (byte)RoleId.Impostor);
        player.SetKillTimer(ModOption.KillCooldown);

        Message("PROOF I AM IMP VANILLA ROLE: " + player.Data.Role.IsImpostor);

        foreach (var player2 in PlayerControl.AllPlayerControls)
            if (player2.Data.Role.IsImpostor && PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                player.cosmetics.nameText.color = Palette.ImpostorRed;
    }

    public static void SetRoleType(PlayerControl player, RoleTypes roleType)
    {
        try
        {
            if (player == null || player.Data == null) return;
            var data = player.Data;
            if (data.Role)
            {
                data.Role.Deinitialize(player);
                UObject.Destroy(data.Role.gameObject);
            }
            if (RoleManager.Instance == null) return;
            var roleBehaviour = UObject.Instantiate(RoleManager.Instance.AllRoles.First(r => r.Role == roleType), GameData.Instance.transform);
            roleBehaviour.Initialize(player);
            player.Data.Role = roleBehaviour;
            player.Data.RoleType = roleType;
            roleBehaviour.AdjustTasks(player);
        }
        catch (Exception e)
        {
            Error(e);
        }
    }



    public static List<RoleId[]> blockedRolePairings = new();

    public static void blockRole()
    {
        blockedRolePairings.Clear();

        if (Pavlovsowner.pavlovsownerAndJackalAsWell.GetBool())
            blockedRolePairings.Add([RoleId.Jackal, RoleId.Pavlovsowner]);
        if (Executioner.executionerPromotesToLawyer.GetBool())
            blockedRolePairings.Add([RoleId.Executioner, RoleId.Lawyer]);

        if (Jester.canDragDeadBody)
            blockedRolePairings.Add([RoleId.Jester, RoleId.Undertaker]);

        blockedRolePairings.Add([RoleId.Vampire, RoleId.Warlock, RoleId.Witch]);
        blockedRolePairings.Add([RoleId.Vulture, RoleId.Cleaner, RoleId.Pelican]);
        blockedRolePairings.Add([RoleId.Ninja, RoleId.Swooper]);
        blockedRolePairings.Add([RoleId.Gunsmith, RoleId.Berserker, RoleId.BountyHunter, RoleId.WolfLord]);
    }

    public static void clearAndReloadRoles()
    {
        // Gamemodes
        HandleGuesser.clearAndReload();

        GhostRole.ClearAndReload();
        blockRole();
        CanSeeGhostInfo = false;
    }

    public static bool IsKillerNeutral(RoleId role)
    {
        var info = CustomRoleManager.GetRoleInfo(role);
        if (info != null && info.RoleType == RoleType.Neutral && info.IsKiller)
        {
            return true;
        }
        return false;
    }
}


public enum RoleType
{
    Crewmate,
    Impostor,
    Neutral,
    Modifier,
    Ghost,
    Special,
}

public enum RoleId
{
    DefaultRole,

    // Impostor
    Impostor,
    Morphling,
    WolfLord,
    Bomber,
    Poucher,
    Butcher,
    Mimic,
    Camouflager,
    Miner,
    Eraser,
    Vampire,
    Undertaker,
    Warlock,
    Trickster,
    BountyHunter,
    Cleaner,
    Terrorist,
    Blackmailer,
    Witch,
    Ninja,
    Yoyo,
    EvilTrapper,
    Gambler,
    Grenadier,
    Gunsmith,
    Berserker,
    Marionette,

    // Neutral
    Amnisiac,      // 200100
    Survivor,
    Pursuer,
    PartTimer,
    BandLeader,
    SchrodingersCat,  // 200600
    Jester,
    Vulture,
    Lawyer,
    Executioner,
    Doomsayer,
    Akujo,
    Thief,
    Witness,       // 203800
    Jackal,
    Sidekick,
    Pavlovsowner,
    Pavlovsdogs,
    Arsonist,
    Werewolf,
    Swooper,
    Juggernaut,
    Pelican,       // 206900
    Infected,

    // Crewmate
    Crewmate,
    Vigilante,
    Mayor,
    Prosecutor,
    Portalmaker,
    Engineer,
    Sheriff,
    Deputy,
    BodyGuard,
    Jumper,
    Detective,
    Veteran,
    Medic,
    Swapper,
    Seer,
    Hacker,
    Tracker,
    Snitch,
    Prophet,
    InfoSleuth,
    Spy,
    SecurityGuard,
    Medium,
    Trapper,
    Balancer,
    Redemptor,
    Jailor,

    // Modifier
    Lover = 150,
    Assassin,
    Disperser,
    PoucherA,
    Vortox,
    Specoality,
    LastImpostor,
    Bloody,
    AntiTeleport,
    Tiebreaker,
    Bait,
    Aftermath,
    Flash,
    Torch,
    Sunglasses,
    Multitasker,
    Mini,
    Giant,
    Vip,
    Indomitable,
    Slueth,
    Cursed,
    Blind,
    Watcher,
    Radar,
    Tunneler,
    ButtonBarry,
    Chameleon,
    Shifter,

    // Ability
    Guesser,

    // Ghost
    GhostEngineer = 200,
    Poltergeist,
    Specter,

    // Special
    GM,
}