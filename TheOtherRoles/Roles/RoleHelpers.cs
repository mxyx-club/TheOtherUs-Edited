using AmongUs.GameOptions;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles;

public enum RoleType
{
    Error = -1,
    Crewmate = 0,
    Impostor,
    Neutral,
    Modifier,
    Ghost,
    Special,
}

public enum RoleId
{
    DefaultRole,

    Impostor = 1,
    Morphling,
    WolfLord,
    Bomber,
    Poucher,
    Professional,
    Butcher,
    Mimic,
    Camouflager,
    Miner,
    Eraser,
    Vampire,
    Undertaker,
    Marionette,
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
    Gaoler,

    Survivor = 50,
    Amnisiac,
    Jester,
    Vulture,
    Lawyer,
    Executioner,
    Pursuer,
    PartTimer,
    Witness,
    Doomsayer,
    Arsonist,
    Jackal,
    Sidekick,
    Pavlovsowner,
    Pavlovsdogs,
    Infected,
    Werewolf,
    Swooper,
    Juggernaut,
    Pelican,
    Akujo,
    Thief,
    BandLeader,
    SchrodingersCat,
    Avenger,
    SoulSight,

    Crewmate = 100,
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
    Oracle,

    // Modifier ---
    Lover = 150,
    Assassin,
    Disperser,
    PoucherModifier,
    ProfessionalModifier,
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

    GhostEngineer = 200,
    Specter,
    Clog,
    Poltergeist,
}

public static class PlayerControlExtensions
{
    extension(PlayerControl player)
    {
        public bool IsUsingTransportation => player.inMovingPlat || player.onLadder;

        public bool IsLocalPlayer => player != null && player == PlayerControl.LocalPlayer;

        /// <summary>
        /// 假任务
        /// </summary>
        public bool HasFakeTasks()
        {
            return player == Werewolf.werewolf ||
                   player == Doomsayer.doomsayer ||
                   player == Juggernaut.juggernaut ||
                   player == Arsonist.arsonist ||
                   player == Witness.Player ||
                   player == PartTimer.partTimer ||
                   player == Akujo.akujo ||
                   player == Pelican.Player ||
                   player == Specter.Player ||
                   player == BandLeader.Player ||
                   player == Swooper.swooper ||
                   player == Lawyer.lawyer ||
                   player == Executioner.executioner ||
                   player == Vulture.vulture ||
                   player == SchrodingersCat.Player ||
                   player == Jackal.Sidekick ||
                   //player == Avenger.Player ||
                   player == Pavlovsdogs.pavlovsowner ||
                   Jester.Player.Any(x => x == player) ||
                   Jackal.jackal.Any(x => x == player) ||
                   Pursuer.Player.Any(x => x == player) ||
                   Survivor.Player.Any(x => x == player) ||
                   Infected.Player.Any(x => x == player) ||
                   Pavlovsdogs.pavlovsdogs.Any(x => x == player);
        }

        public bool CanUseSabotage()
        {
            var roleCouldUse = false;
            if (ModOption.disableSabotage) return false;
            else if (Jackal.canSabotage && (Jackal.jackal.Any(x => x.PlayerId == player.PlayerId) || player == Jackal.Sidekick))
                roleCouldUse = true;
            else if (Pavlovsdogs.canSabotage && (player == Pavlovsdogs.pavlovsowner || Pavlovsdogs.pavlovsdogs.Any(p => p == player)))
                roleCouldUse = true;
            else if (player.Data?.Role != null && player.Data.Role.IsImpostor)
                roleCouldUse = true;
            return roleCouldUse;
        }

        /// <summary>
        /// 管道技能相关
        /// </summary>
        public bool RoleCanUseVents()
        {
            var roleCouldUse = false;
            if (player.inVent) return true;

            if (Engineer.engineer != null && Engineer.engineer == player)
            {
                roleCouldUse = true;
            }
            else if (Werewolf.canUseVents && Werewolf.werewolf != null && Werewolf.werewolf == player)
            {
                roleCouldUse = true;
            }
            else if (Jackal.canUseVents && Jackal.jackal != null && Jackal.jackal.Any(x => x == player))
            {
                roleCouldUse = true;
            }
            else if (Jackal.canUseVents && Jackal.Sidekick != null && Jackal.Sidekick == player)
            {
                roleCouldUse = true;
            }
            else if ((Pavlovsdogs.canUseVents is 1 or 2) && Pavlovsdogs.pavlovsowner != null && Pavlovsdogs.pavlovsowner == player)
            {
                roleCouldUse = true;
            }
            else if ((Pavlovsdogs.canUseVents is 0 or 2) && Pavlovsdogs.pavlovsdogs != null && Pavlovsdogs.pavlovsdogs.Any(p => p == player))
            {
                roleCouldUse = true;
            }
            else if (Spy.canEnterVents && Spy.spy != null && Spy.spy == player)
            {
                roleCouldUse = true;
            }
            else if (Vulture.canUseVents && Vulture.vulture != null && Vulture.vulture == player)
            {
                roleCouldUse = true;
            }
            else if (Undertaker.dragedBody != null && !Undertaker.canDragAndVent && Undertaker.undertaker == player)
            {
                roleCouldUse = false;
            }
            else if (Thief.canUseVents && Thief.thief != null && Thief.thief == player)
            {
                roleCouldUse = true;
            }
            else if (Jester.Player != null && Jester.Player.Any(p => p == player) && Jester.canUseVents)
            {
                roleCouldUse = true;
            }
            else if (Juggernaut.juggernaut != null && Juggernaut.juggernaut == player && Juggernaut.canUseVents)
            {
                roleCouldUse = true;
            }
            else if (Pelican.Player != null && Pelican.Player == player && Pelican.CanUseVent)
            {
                roleCouldUse = true;
            }
            else if (Swooper.swooper != null && Swooper.swooper == player && Swooper.canUseVents)
            {
                roleCouldUse = true;
            }
            else if (Infected.Player != null && Infected.Player.Any(x => x == player) && Infected.canUseVents)
            {
                roleCouldUse = true;
            }
            else if (Avenger.Player != null && Avenger.Player == player && Avenger.canUseVents)
            {
                roleCouldUse = true;
            }
            else if (Werewolf.werewolf != null && Werewolf.werewolf == player)
            {
                if (CustomOptionHolder.werewolfCanUseVents.GetSelection() == 2) roleCouldUse = true;
                else if (CustomOptionHolder.werewolfCanUseVents.GetSelection() == 1 && Werewolf.canKill) roleCouldUse = true;
            }
            else if (player.Data?.Role != null && player.Data.Role.CanVent)
            {
                roleCouldUse = true;
            }
            if (Tunneler.tunneler != null && Tunneler.tunneler == player)
            {
                var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Tunneler.tunneler.Data);
                if (playerTotal - playerCompleted == 0 || Tunneler.NoTask) roleCouldUse = true;
            }

            return roleCouldUse;
        }

        public bool IsNeutral()
        {
            if (player == null) return false;
            var roleInfo = RoleInfo.getRoleInfoForPlayer(player, false, false).FirstOrDefault();
            return roleInfo != null && roleInfo.roleType == RoleType.Neutral;
        }

        public bool IsKiller()
        {
            return player != null && (player.IsImpostor() || player.IsKillerNeutral());
        }

        public bool IsKillerNeutral()
        {
            return player.IsNeutral() && (
                    player == Juggernaut.juggernaut ||
                    player == Werewolf.werewolf ||
                    player == Swooper.swooper ||
                    player == Arsonist.arsonist ||
                    (player == Avenger.Player && Avenger.CanFreeKill) ||
                    player == Pelican.Player ||
                    player == Jackal.Sidekick ||
                    player == Pavlovsdogs.pavlovsowner ||
                    Jackal.jackal.Any(x => x.PlayerId == player.PlayerId) ||
                    Infected.Player.Any(x => x.PlayerId == player.PlayerId) ||
                    Pavlovsdogs.pavlovsdogs.Any(x => x.PlayerId == player.PlayerId) ||
                    (player == SchrodingersCat.Player && SchrodingersCat.IsKiller)
                    );
        }

        public bool IsEvilNeutral()
        {
            return player != null && player.IsNeutral() && (
                    player == Jester.Player.Any(x => x.PlayerId == player.PlayerId) ||
                    player == Vulture.vulture ||
                    player == Lawyer.lawyer ||
                    player == Executioner.executioner ||
                    player == Witness.Player ||
                    (player == Avenger.Player && !Avenger.CanFreeKill) ||
                    player == Akujo.akujo ||
                    player == Doomsayer.doomsayer ||
                    player == Thief.thief ||
                    (player == SchrodingersCat.Player && SchrodingersCat.IsEvil)
                    );
        }

        public bool IsCrew(bool AndCat = false)
        {
            if (player == null) return false;
            return (!player.IsImpostor() && !IsNeutral(player))
                || (AndCat && SchrodingersCat.Player == player && SchrodingersCat.State == SchrodingersCat.CatState.Crewmate);
        }

        public bool IsImpostor(bool AndSpy = false, bool AndCat = false)
        {
            if (player == null) return false;
            return player?.Data?.Role?.IsImpostor == true
                || (AndSpy && Spy.spy == player)
                || (AndCat && SchrodingersCat.Player == player && SchrodingersCat.State == SchrodingersCat.CatState.Impostor);
        }

        public bool CanUseMeetingAbility()
        {
            if (player.IsDead()) return true;
            if (Blackmailer.Player.IsAlive() && Blackmailer.blackmailed == player) return false;
            if (Jailor.Player.IsAlive() && Jailor.Jailed == player) return false;
            return true;
        }

        public bool IsAlive()
        {
            return player != null && !player.Data.Disconnected && !player.Data.IsDead;
        }

        public bool IsDead()
        {
            return player == null || player.Data.Disconnected || player.Data.IsDead;
        }

        public void clearAllTasks()
        {
            if (player == null) return;
            foreach (var playerTask in player.myTasks.GetFastEnumerator())
            {
                playerTask.OnRemove();
                UObject.Destroy(playerTask.gameObject);
            }

            player.myTasks.Clear();

            if (player.Data != null && player.Data.Tasks != null)
                player.Data.Tasks.Clear();
        }

        public void SetKillTimerUnchecked(float time, float max = float.NegativeInfinity)
        {
            if (max == float.NegativeInfinity) max = time;

            player.killTimer = time;
            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(time, max);
        }

        public PlayerControl GetPartner()
        {
            return Akujo.otherLover(player) ?? Lovers.otherLover(player);
        }

        public void NoCheckStartMeeting(GameData.PlayerInfo target, bool force = false)
        {
            if (InMeeting) return;

            if (AmongUsClient.Instance.AmHost)
            {
                handleVampireBiteOnBodyReport();
                handleBomberExplodeOnBodyReport();

                MeetingRoomManager.Instance.AssignSelf(player, target);
                DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(player);
                player.RpcStartMeeting(target);
            }
        }

        public void ModRevive(bool cleanBody = true, bool setPos = true)
        {
            if (player == null) return;

            DeadBody[] array = UObject.FindObjectsOfType<DeadBody>();

            for (var i = 0; i < array.Length; i++)
            {
                if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == player.PlayerId)
                {
                    if (setPos) player.NetTransform.RpcSnapTo(array[i].transform.position);
                    if (cleanBody) UObject.Destroy(array[i].gameObject);
                    break;
                }
            }

            player?.Revive();
        }

        public void CustomExiled(PlayerControl killer = null, bool noCheckLover = false)
        {
            ExilePlayerPatch.NoCheckLover = noCheckLover;
            ExilePlayerPatch.Killer = killer;
            Message($"NoCheckLover: {noCheckLover}", "Custom");
            player.Exiled();
        }

        public void RpcExiled()
        {
            var writer = StartRPC(CustomRPC.Exiled);
            writer.Write(player.PlayerId);
            writer.EndRPC();
            player.Exiled();
        }

        public RoleType GetRoleType()
        {
            if (player == null) return RoleType.Error;

            if (player.IsCrew(true)) return RoleType.Crewmate;
            else if (player.IsImpostor(false, true)) return RoleType.Impostor;
            else if (player.IsNeutral()) return RoleType.Neutral;
            return RoleType.Error;
        }

        public void SetRoleType(RoleTypes roleType)
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
    }
}

public static class RoleHelpers
{
    public static bool CanSeeGhostInfo
    {
        get
        {
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Ended) return true;
            else if (PlayerControl.LocalPlayer.IsAlive()) return false;
            else if (PlayerControl.LocalPlayer == Specter.Player) return false;
            else if (Specter.Player.GetPartner() == PlayerControl.LocalPlayer) return false;
            else return field;
        }
        set => field = !PlayerControl.LocalPlayer.IsAlive() && PlayerControl.LocalPlayer != Specter.Player && Specter.Player.GetPartner() != PlayerControl.LocalPlayer && value;
    }

    public static PlayerControl ImpostorSetTarget()
    {
        List<PlayerControl> untargetablePlayers = [];

        if (SchrodingersCat.Player.IsAlive() && SchrodingersCat.State == SchrodingersCat.CatState.Impostor) untargetablePlayers.Add(SchrodingersCat.Player);

        PlayerControl target;

        if (Spy.spy != null)
        {
            if (Spy.impostorsCanKillAnyone)
            {
                target = SetTarget(null, false, inVented: ModOption.ImpCanKillInVent);
            }
            else
            {
                untargetablePlayers.Add(Spy.spy);
                target = SetTarget(untargetablePlayers, true, inVented: ModOption.ImpCanKillInVent);
            }
        }
        else
        {
            target = SetTarget(untargetablePlayers, true, inVented: ModOption.ImpCanKillInVent);
        }

        SetPlayerOutline(target, Palette.ImpostorRed);
        return target;
    }

    public static bool CustomMurderPlayer(
        PlayerControl killer,
        PlayerControl target,
        bool showAnimation = true,
        bool force = false,
        CustomDeathReason deathReason = CustomDeathReason.Null)
    {
        if (!InGame) return false;
        if (!force && !CheckMurderPlayer(killer, target)) return false;

        if (deathReason == CustomDeathReason.Null) deathReason = target == killer ? CustomDeathReason.Suicide : CustomDeathReason.Kill;
        KillAnimationCoPerformKillPatch.hideNextAnimation = !showAnimation;
        killer.MurderPlayer(target, MurderResultFlags.Succeeded);
        PlayerData.SetDeathReason(target, deathReason, killer);
        return true;
    }

    public static bool RpcCustomMurderPlayer(
        PlayerControl killer,
        PlayerControl target,
        bool showAnimation = true,
        bool force = false,
        CustomDeathReason deathReason = CustomDeathReason.Null)
    {
        if (!InGame) return false;
        if (!force && !CheckMurderPlayer(killer, target)) return false;

        var writer = StartRPC(CustomRPC.CustomMurderPlayer);
        writer.Write(killer.PlayerId);
        writer.Write(target.PlayerId);
        writer.Write(showAnimation);
        writer.Write(true);
        writer.Write((byte)deathReason);
        writer.EndRPC();
        CustomMurderPlayer(killer, target, showAnimation, true, deathReason);
        return true;
    }

    public static bool CheckUseAbility(PlayerControl player, PlayerControl target)
    {
        if (Veteran.veteran == target && Veteran.alertActive)
        {
            if (ModOption.EnableOtherLog) Message(" [Kill Fail] Veteran Active!", "CheckMurderPlayer");
            RpcCustomMurderPlayer(target, player);
            return true;
        }

        return false;
    }

    public static bool AllCrewDead()
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (!player.IsDead() && player.IsCrew())
                return false;
        }
        return true;
    }

    public static bool CheckMurderPlayer(PlayerControl killer, PlayerControl target)
    {
        if (killer == null || target == null) return false;
        if (killer == target) return true;
        if (IsHideNSeek) return true;
        // Block impostor not fully grown mini kill
        if (Mini.mini != null && target == Mini.mini && !Mini.isGrownUp) return false;

        if (CheckUseAbility(killer, target)) return false;

        // Handle first kill attempt
        if (ModOption.shieldFirstKill && ModOption.firstKillPlayer == target)
            return false;

        if (Pursuer.blankedList.Any(x => x == killer.PlayerId))
        {
            var writer = StartRPC(CustomRPC.SetBlanked);
            writer.Write(killer.PlayerId);
            writer.Write(true);
            writer.EndRPC();
            RPCProcedure.SetBlanked(killer.PlayerId, true);
            CustomButton.SetKillTimer();
            if (ModOption.EnableOtherLog) Message(" [Kill Fail] Blanked");
            return false;
        }

        if (BodyGuard.bodyguard != null && target == BodyGuard.guarded && BodyGuard.bodyguard.IsAlive())
        {
            target.ShowFailedMurder();
            // Kill the Killer
            RpcCustomMurderPlayer(BodyGuard.bodyguard, killer, false, true);
            // Kill the BodyGuard
            RpcCustomMurderPlayer(killer, BodyGuard.bodyguard, false, true);

            var writer3 = StartRPC(CustomRPC.ShowBodyGuardFlash);
            writer3.EndRPC();
            RPCProcedure.showBodyGuardFlash();
            return false;
        }

        if (Medic.shielded != null && Medic.shielded == target)
        {
            var writer = StartRPC(CustomRPC.ShieldedMurderAttempt);
            writer.Write(killer.PlayerId);
            writer.EndRPC();
            RPCProcedure.shieldedMurderAttempt(killer.PlayerId);

            CustomButton.SetKillTimer();
            target.ShowFailedMurder();
            if (ModOption.EnableOtherLog) Message(" [Kill Fail] Medic Shielded!");
            return false;
        }

        if (Survivor.Player != null && Survivor.Player.Any(x => x.PlayerId == target.PlayerId) && Survivor.VestActive(target.PlayerId))
        {
            CustomButton.SetKillTimer(Survivor.vestResetCooldown);
            target.ShowFailedMurder();
            if (ModOption.EnableOtherLog) Message(" [Kill Fail] Survivor Vest!");
            return false;
        }

        if (Cursed.cursed != null && Cursed.cursed == target && killer.Data.Role.IsImpostor)
        {
            var writer = StartRPC(CustomRPC.CursedTurn);
            writer.Write(target.PlayerId);
            writer.EndRPC();
            Cursed.TurnToImpostor(target.PlayerId);

            CustomButton.SetKillTimer();
            if (ModOption.EnableOtherLog) Message(" [Kill Fail] Cursed TurnToImpostor!");
            return false;
        }

        if (target.IsUsingTransportation)
            return false;

        if (killer == EvilTrapper.evilTrapper)
        {
            KillTrap.ClearAllTraps(killer, false);
        }

        return true;
    }

    public static List<RoleId[]> blockedRolePairings = new();

    public static void blockRole()
    {
        blockedRolePairings.Clear();

        blockedRolePairings.Add([RoleId.Vampire, RoleId.Warlock, RoleId.Witch]);

        if (CustomOptionHolder.onlyOneNeutralTeam.GetBool())
        {
            blockedRolePairings.Add([RoleId.Jackal, RoleId.Pavlovsowner, RoleId.Infected]);
        }
        if (Executioner.promotesToLawyer)
        {
            blockedRolePairings.Add([RoleId.Executioner, RoleId.Lawyer]);
        }

        if (Jester.canDragDeadBody)
        {
            blockedRolePairings.Add([RoleId.Jester, RoleId.Undertaker]);
        }

        blockedRolePairings.Add([RoleId.Vulture, RoleId.Cleaner, RoleId.Pelican]);

        blockedRolePairings.Add([RoleId.Ninja, RoleId.Swooper]);

        blockedRolePairings.Add([RoleId.Gunsmith, RoleId.Berserker, RoleId.BountyHunter, RoleId.WolfLord]);

        blockedRolePairings.Add([RoleId.Mayor, RoleId.Prosecutor]);

        blockedRolePairings.Add([RoleId.Prophet, RoleId.Oracle]);
    }

    public static Dictionary<RoleId, int> RoleRate = new();

    public static void ResetRoleSelection()
    {
        RoleRate.Clear();
        RoleRate.AddRange(new()
        {
            { RoleId.Sheriff, CustomOptionHolder.sheriffSpawnRate.GetSelection() },
            { RoleId.Deputy, CustomOptionHolder.deputySpawnRate.GetSelection() },
            { RoleId.BodyGuard, CustomOptionHolder.bodyGuardSpawnRate.GetSelection() },
            { RoleId.Balancer, CustomOptionHolder.balancerSpawnRate.GetSelection() },
            { RoleId.Detective, CustomOptionHolder.detectiveSpawnRate.GetSelection() },
            { RoleId.Engineer, CustomOptionHolder.engineerSpawnRate.GetSelection() },
            { RoleId.Hacker, CustomOptionHolder.hackerSpawnRate.GetSelection() },
            { RoleId.InfoSleuth, CustomOptionHolder.infoSleuthSpawnRate.GetSelection() },
            { RoleId.Jumper, CustomOptionHolder.jumperSpawnRate.GetSelection() },
            { RoleId.Mayor, CustomOptionHolder.mayorSpawnRate.GetSelection() },
            { RoleId.Medic, CustomOptionHolder.medicSpawnRate.GetSelection() },
            { RoleId.Medium, CustomOptionHolder.mediumSpawnRate.GetSelection() },
            { RoleId.Portalmaker, CustomOptionHolder.portalmakerSpawnRate.GetSelection() },
            { RoleId.Prophet, CustomOptionHolder.prophetSpawnRate.GetSelection() },
            { RoleId.Prosecutor, CustomOptionHolder.prosecutorSpawnRate.GetSelection() },
            { RoleId.SecurityGuard, CustomOptionHolder.securityGuardSpawnRate.GetSelection() },
            { RoleId.Seer, CustomOptionHolder.seerSpawnRate.GetSelection() },
            { RoleId.Snitch, CustomOptionHolder.snitchSpawnRate.GetSelection() },
            { RoleId.Spy, CustomOptionHolder.spySpawnRate.GetSelection() },
            { RoleId.Swapper, CustomOptionHolder.swapperSpawnRate.GetSelection() },
            { RoleId.Tracker, CustomOptionHolder.trackerSpawnRate.GetSelection() },
            { RoleId.Trapper, CustomOptionHolder.trapperSpawnRate.GetSelection() },
            { RoleId.Veteran, CustomOptionHolder.veteranSpawnRate.GetSelection() },
            { RoleId.Vigilante, CustomOptionHolder.guesserSpawnRate.GetSelection() },
            { RoleId.Redemptor, CustomOptionHolder.redemptorSpawnRate.GetSelection() },
            { RoleId.Jailor, CustomOptionHolder.jailorSpawnRate.GetSelection() },

            { RoleId.WolfLord, CustomOptionHolder.wolfLordSpawnRate.GetSelection() },
            { RoleId.Blackmailer, CustomOptionHolder.blackmailerSpawnRate.GetSelection() },
            { RoleId.Bomber, CustomOptionHolder.bomberSpawnRate.GetSelection() },
            { RoleId.BountyHunter, CustomOptionHolder.bountyHunterSpawnRate.GetSelection() },
            { RoleId.Butcher, CustomOptionHolder.butcherSpawnRate.GetSelection() },
            { RoleId.Camouflager, CustomOptionHolder.camouflagerSpawnRate.GetSelection() },
            { RoleId.Cleaner, CustomOptionHolder.cleanerSpawnRate.GetSelection() },
            { RoleId.Eraser, CustomOptionHolder.eraserSpawnRate.GetSelection() },
            { RoleId.Marionette, CustomOptionHolder.marionetteSpawnRate.GetSelection() },
            { RoleId.EvilTrapper, CustomOptionHolder.evilTrapperSpawnRate.GetSelection() },
            { RoleId.Gambler, CustomOptionHolder.gamblerSpawnRate.GetSelection() },
            { RoleId.Mimic, CustomOptionHolder.mimicSpawnRate.GetSelection() },
            { RoleId.Miner, CustomOptionHolder.minerSpawnRate.GetSelection() },
            { RoleId.Morphling, CustomOptionHolder.morphlingSpawnRate.GetSelection() },
            { RoleId.Ninja, CustomOptionHolder.ninjaSpawnRate.GetSelection() },
            { RoleId.Poucher, CustomOptionHolder.poucherSpawnRate.GetSelection() },
            { RoleId.Terrorist, CustomOptionHolder.terroristSpawnRate.GetSelection() },
            { RoleId.Trickster, CustomOptionHolder.tricksterSpawnRate.GetSelection() },
            { RoleId.Undertaker, CustomOptionHolder.undertakerSpawnRate.GetSelection() },
            { RoleId.Vampire, CustomOptionHolder.vampireSpawnRate.GetSelection() },
            { RoleId.Warlock, CustomOptionHolder.warlockSpawnRate.GetSelection() },
            { RoleId.Witch, CustomOptionHolder.witchSpawnRate.GetSelection() },
            { RoleId.Yoyo, CustomOptionHolder.yoyoSpawnRate.GetSelection() },
            { RoleId.Grenadier, CustomOptionHolder.grenadierSpawnRate.GetSelection() },
            { RoleId.Gunsmith, CustomOptionHolder.gunsmithSpawnRate.GetSelection() },
            { RoleId.Berserker, CustomOptionHolder.berserkerSpawnRate.GetSelection() },
            { RoleId.Gaoler,CustomOptionHolder.gaolerSpawnRate.GetSelection() },

            { RoleId.Akujo, CustomOptionHolder.akujoSpawnRate.GetSelection() },
            { RoleId.Amnisiac, CustomOptionHolder.amnisiacSpawnRate.GetSelection() },
            { RoleId.Arsonist, CustomOptionHolder.arsonistSpawnRate.GetSelection() },
            { RoleId.Doomsayer, CustomOptionHolder.doomsayerSpawnRate.GetSelection() },
            { RoleId.Executioner, CustomOptionHolder.executionerSpawnRate.GetSelection() },
            { RoleId.Jackal, CustomOptionHolder.jackalSpawnRate.GetSelection() },
            { RoleId.Sidekick, CustomOptionHolder.jackalSpawnRate.GetSelection() },
            { RoleId.Jester, CustomOptionHolder.jesterSpawnRate.GetSelection() },
            { RoleId.Juggernaut, CustomOptionHolder.juggernautSpawnRate.GetSelection() },
            { RoleId.Pelican, CustomOptionHolder.pelicanSpawnRate.GetSelection() },
            { RoleId.Lawyer, CustomOptionHolder.lawyerSpawnRate.GetSelection() },
            { RoleId.PartTimer, CustomOptionHolder.partTimerSpawnRate.GetSelection() },
            { RoleId.Pavlovsowner, CustomOptionHolder.pavlovsownerSpawnRate.GetSelection() },
            { RoleId.Pavlovsdogs, CustomOptionHolder.pavlovsownerSpawnRate.GetSelection() },
            { RoleId.Survivor, CustomOptionHolder.survivorSpawnRate.GetSelection() },
            { RoleId.Swooper, CustomOptionHolder.swooperSpawnRate.GetSelection() },
            { RoleId.Thief, CustomOptionHolder.thiefSpawnRate.GetSelection() },
            { RoleId.Vulture, CustomOptionHolder.vultureSpawnRate.GetSelection() },
            { RoleId.Witness, CustomOptionHolder.witnessSpawnRate.GetSelection() },
            { RoleId.Werewolf, CustomOptionHolder.werewolfSpawnRate.GetSelection() },
            { RoleId.BandLeader, CustomOptionHolder.bandLeaderSpawnRate.GetSelection() },
            { RoleId.SchrodingersCat, CustomOptionHolder.schrodingersCatSpawnRate.GetSelection() },
            { RoleId.Pursuer, CustomOptionHolder.lawyerSpawnRate.GetSelection() + CustomOptionHolder.executionerSpawnRate.GetSelection() },
            { RoleId.Infected, CustomOptionHolder.infectedSpawnRate.GetSelection() },
            { RoleId.Avenger, CustomOptionHolder.modifierLover.GetSelection()  },

            { RoleId.Lover, CustomOptionHolder.modifierLover.GetSelection() },
            { RoleId.Aftermath, CustomOptionHolder.modifierAftermath.GetSelection() },
            { RoleId.AntiTeleport, CustomOptionHolder.modifierAntiTeleport.GetSelection() },
            { RoleId.Assassin, CustomOptionHolder.modifierAssassin.GetSelection() },
            { RoleId.Bait, CustomOptionHolder.modifierBait.GetSelection() },
            { RoleId.Blind, CustomOptionHolder.modifierBlind.GetSelection() },
            { RoleId.Bloody, CustomOptionHolder.modifierBloody.GetSelection() },
            { RoleId.ButtonBarry, CustomOptionHolder.modifierButtonBarry.GetSelection() },
            { RoleId.Chameleon, CustomOptionHolder.modifierChameleon.GetSelection() },
            { RoleId.Cursed, CustomOptionHolder.modifierCursed.GetSelection() },
            { RoleId.Disperser, CustomOptionHolder.modifierDisperser.GetSelection() },
            { RoleId.Flash, CustomOptionHolder.modifierFlash.GetSelection() },
            { RoleId.Giant, CustomOptionHolder.modifierGiant.GetSelection() },
            { RoleId.Indomitable, CustomOptionHolder.modifierIndomitable.GetSelection() },
            { RoleId.LastImpostor, CustomOptionHolder.modifierLastImpostor.GetSelection() },
            { RoleId.Mini, CustomOptionHolder.modifierMini.GetSelection() },
            { RoleId.Multitasker, CustomOptionHolder.modifierMultitasker.GetSelection() },
            { RoleId.Radar, CustomOptionHolder.modifierRadar.GetSelection() },
            { RoleId.Shifter, CustomOptionHolder.modifierShifter.GetSelection() },
            { RoleId.Slueth, CustomOptionHolder.modifierSlueth.GetSelection() },
            { RoleId.Specoality, CustomOptionHolder.modifierSpecoality.GetSelection() },
            { RoleId.Tiebreaker, CustomOptionHolder.modifierTieBreaker.GetSelection() },
            { RoleId.Torch, CustomOptionHolder.modifierTorch.GetSelection() },
            { RoleId.Tunneler, CustomOptionHolder.modifierTunneler.GetSelection() },
            { RoleId.Vip, CustomOptionHolder.modifierVip.GetSelection() },
            { RoleId.Watcher, CustomOptionHolder.modifierWatcher.GetSelection()}
        });
    }

    public static void clearAndReloadRoles()
    {
        Vigilante.clearAndReload();
        Jester.clearAndReload();
        Mayor.clearAndReload();
        Prosecutor.clearAndReload();
        Portalmaker.clearAndReload();
        Jailor.ClearAndReload();
        Infected.clearAndReload();
        Poucher.clearAndReload();
        Professional.clearAndReload();
        Mimic.clearAndReload();
        Engineer.clearAndReload();
        Sheriff.clearAndReload();
        InfoSleuth.clearAndReload();
        Gambler.clearAndReload();
        Butcher.clearAndReload();
        Amnisiac.clearAndReload();
        Detective.clearAndReload();
        Werewolf.clearAndReload();
        BodyGuard.clearAndReload();
        Veteran.clearAndReload();
        Medic.clearAndReload();
        Shifter.clearAndReload();
        Swapper.clearAndReload();
        Lovers.clearAndReload();
        Seer.clearAndReload();
        Glitch.clearAndReload();
        Camouflager.clearAndReload();
        Hacker.clearAndReload();
        Tracker.clearAndReload();
        Vampire.clearAndReload();
        Snitch.clearAndReload();
        Jackal.clearAndReload();
        Pavlovsdogs.clearAndReload();
        Eraser.clearAndReload();
        Spy.clearAndReload();
        Trickster.clearAndReload();
        Cleaner.clearAndReload();
        Undertaker.clearAndReload();
        Warlock.clearAndReload();
        SecurityGuard.clearAndReload();
        Arsonist.clearAndReload();
        BountyHunter.clearAndReload();
        Vulture.clearAndReload();
        Alchemyst.clearAndReload();
        Bomber.clearAndReload();
        Lawyer.clearAndReload();
        Executioner.clearAndReload();
        Pursuer.clearAndReload();
        Witch.clearAndReload();
        Jumper.clearAndReload();
        Prophet.clearAndReload();
        Marionette.ClearAndReload();
        Ninja.clearAndReload();
        Blackmailer.clearAndReload();
        Thief.clearAndReload();
        Miner.clearAndReload();
        Trapper.clearAndReload();
        Terrorist.clearAndReload();
        Juggernaut.clearAndReload();
        SoulSight.ClearAndReload();
        Doomsayer.clearAndReload();
        Swooper.clearAndReload();
        Balancer.clearAndReload();
        Akujo.clearAndReload();
        Yoyo.clearAndReload();
        EvilTrapper.clearAndReload();
        Survivor.clearAndReload();
        PartTimer.clearAndReload();
        Grenadier.clearAndReload();
        Witness.ClearAndReload();
        WolfLord.ClearAndReload();
        Pelican.clearAndReload();
        Redemptor.ClearAndReload();
        BandLeader.ClearAndReload();
        SchrodingersCat.ClearAndReload();
        Gunsmith.ClearAndReload();
        Berserker.ClearAndReload();
        Avenger.ClearAndReload();
        Oracle.ClearAndReload();
        Gaoler.ClearAndReload();

        // Modifier
        Assassin.clearAndReload();
        Aftermath.clearAndReload();
        Bait.clearAndReload();
        Bloody.clearAndReload();
        AntiTeleport.clearAndReload();
        Tiebreaker.clearAndReload();
        Sunglasses.clearAndReload();
        Torch.clearAndReload();
        Flash.clearAndReload();
        Blind.clearAndReload();
        Watcher.clearAndReload();
        Radar.clearAndReload();
        Tunneler.clearAndReload();
        Multitasker.clearAndReload();
        Disperser.clearAndReload();
        Mini.clearAndReload();
        Giant.clearAndReload();
        Indomitable.clearAndReload();
        Slueth.clearAndReload();
        Cursed.clearAndReload();
        Vip.clearAndReload();
        Chameleon.clearAndReload();
        ButtonBarry.clearAndReload();
        LastImpostor.clearAndReload();
        Specoality.clearAndReload();
        Vortox.ClearAndReload();

        Poltergeist.ClearAndReload();
        GhostEngineer.ClearAndReload();
        Clog.ClearAndReload();
        Specter.ClearAndReload();

        // Gamemodes
        HandleGuesser.clearAndReload();

        Lovers.SetAvengerLover();
        Jackal.SetSwoop();

        blockRole();
        ResetRoleSelection();
        CanSeeGhostInfo = false;
    }

}

