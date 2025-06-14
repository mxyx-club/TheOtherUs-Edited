using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles;

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
    Default,

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
    Escapist,
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
    Jailor,

    Survivor,
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

    Crewmate,
    Vigilante,
    Mayor,
    Prosecutor,
    Portalmaker,
    Engineer,
    Sheriff,
    Deputy,
    Hunter,
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

    // Modifier ---
    Lover,
    Assassin,
    Disperser,
    PoucherModifier,
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
    Poltergeist,
}

public static class RoleHelpers
{
    private static bool _CanSeeRoleInfo;
    public static bool CanSeeRoleInfo
    {
        get
        {
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Ended) return true;
            else if (PlayerControl.LocalPlayer.IsAlive()) return false;
            else if (PlayerControl.LocalPlayer == Specter.Player) return false;
            else if (Specter.Player.getPartner() == PlayerControl.LocalPlayer) return false;
            else return _CanSeeRoleInfo;
        }
        set
        {
            if (PlayerControl.LocalPlayer.IsAlive()) _CanSeeRoleInfo = false;
            else if (PlayerControl.LocalPlayer == Specter.Player) _CanSeeRoleInfo = false;
            else if (Specter.Player.getPartner() == PlayerControl.LocalPlayer) _CanSeeRoleInfo = false;
            else _CanSeeRoleInfo = value;
        }
    }

    public static void CustomMurderPlayer(
        PlayerControl killer,
        PlayerControl target,
        bool showAnimation = true,
        CustomDeathReason deathReason = CustomDeathReason.NULL)
    {
        if (deathReason == CustomDeathReason.NULL) deathReason = target == killer ? CustomDeathReason.Suicide : CustomDeathReason.Kill;
        KillAnimationCoPerformKillPatch.hideNextAnimation = !showAnimation;
        killer.MurderPlayer(target, MurderResultFlags.Succeeded);
        GameHistory.OverrideDeathReasonAndKiller(target, deathReason, killer);
    }

    public static bool RpcCustomMurderPlayer(
        PlayerControl killer,
        PlayerControl target,
        bool showAnimation = true,
        bool force = false,
        CustomDeathReason deathReason = CustomDeathReason.NULL)
    {
        if (!force && !CheckMurderPlayer(killer, target))
            return false;

        var writer = StartRPC(CustomRPC.CustomMurderPlayer);
        writer.Write(killer.PlayerId);
        writer.Write(target.PlayerId);
        writer.Write(showAnimation);
        writer.Write((byte)deathReason);
        writer.EndRPC();
        CustomMurderPlayer(killer, target, showAnimation, deathReason);
        return true;
    }

    public static bool CheckMurderPlayer(PlayerControl killer, PlayerControl target)
    {
        if (killer == null || target == null) return false;
        if (killer == target) return true;
        if (IsHideNSeek) return true;
        // Block impostor not fully grown mini kill
        if (Mini.mini != null && target == Mini.mini && !Mini.isGrownUp()) return false;

        // Handle first kill attempt
        if (ModOption.shieldFirstKill && ModOption.firstKillPlayer == target)
            return false;

        if (CheckUseAbility(killer, target)) return false;

        if (Pursuer.blankedList.Any(x => x.PlayerId == killer.PlayerId))
        {
            var writer = StartRPC(CustomRPC.SetBlanked);
            writer.Write(killer.PlayerId);
            writer.Write(false);
            writer.EndRPC();
            RPCProcedure.SetBlanked(killer.PlayerId, false);
            CustomButton.SetKillTimer();
            return false;
        }

        if (BodyGuard.bodyguard != null && target == BodyGuard.guarded && BodyGuard.bodyguard.IsAlive())
        {
            // Kill the Killer
            var writer = StartRPC(CustomRPC.CustomMurderPlayer);
            writer.Write(BodyGuard.bodyguard.PlayerId);
            writer.Write(killer.PlayerId);
            writer.Write(false);
            writer.EndRPC();
            CustomMurderPlayer(BodyGuard.bodyguard, killer, false);

            // Kill the BodyGuard
            var writer2 = StartRPC(CustomRPC.CustomMurderPlayer);
            writer2.Write(killer.PlayerId);
            writer2.Write(BodyGuard.bodyguard.PlayerId);
            writer2.Write(false);
            writer2.EndRPC();
            CustomMurderPlayer(killer, BodyGuard.bodyguard, false);

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
            SoundEffectsManager.play("fail");
            return false;
        }


        if (Survivor.Player != null && Survivor.Player.Any(x => x.PlayerId == target.PlayerId) && Survivor.vestActive)
        {
            CustomButton.SetKillTimer(Survivor.vestResetCooldown);
            SoundEffectsManager.play("fail");
            return false;
        }

        if (Cursed.cursed != null && Cursed.cursed == target && killer.Data.Role.IsImpostor)
        {
            turnToImpostorRPC(target);

            CustomButton.SetKillTimer();
            return false;
        }

        if (target.isUsingTransportation())
            return false;

        return true;
    }



    public static List<RoleId[]> blockedRolePairings = new();

    public static void blockRole()
    {
        blockedRolePairings.Clear();

        blockedRolePairings.Add([RoleId.Vampire, RoleId.Warlock, RoleId.Witch]);

        if (CustomOptionHolder.pavlovsownerAndJackalAsWell.GetBool())
        {
            blockedRolePairings.Add([RoleId.Jackal, RoleId.Pavlovsowner]);
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
            { RoleId.Hunter, CustomOptionHolder.hunterSpawnRate.GetSelection() },
            { RoleId.Jailor, CustomOptionHolder.jailorSpawnRate.GetSelection() },

            { RoleId.WolfLord, CustomOptionHolder.wolfLordSpawnRate.GetSelection() },
            { RoleId.Blackmailer, CustomOptionHolder.blackmailerSpawnRate.GetSelection() },
            { RoleId.Bomber, CustomOptionHolder.bomberSpawnRate.GetSelection() },
            { RoleId.BountyHunter, CustomOptionHolder.bountyHunterSpawnRate.GetSelection() },
            { RoleId.Butcher, CustomOptionHolder.butcherSpawnRate.GetSelection() },
            { RoleId.Camouflager, CustomOptionHolder.camouflagerSpawnRate.GetSelection() },
            { RoleId.Cleaner, CustomOptionHolder.cleanerSpawnRate.GetSelection() },
            { RoleId.Eraser, CustomOptionHolder.eraserSpawnRate.GetSelection() },
            { RoleId.Escapist, CustomOptionHolder.escapistSpawnRate.GetSelection() },
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
        Hunter.ClearAndReload();
        Jailor.ClearAndReload();
        Infected.clearAndReload();
        Poucher.clearAndReload();
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
        Morphling.clearAndReload();
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
        Medium.clearAndReload();
        Bomber.clearAndReload();
        Lawyer.clearAndReload();
        Executioner.clearAndReload();
        Pursuer.clearAndReload();
        Witch.clearAndReload();
        Jumper.clearAndReload();
        Prophet.clearAndReload();
        Escapist.clearAndReload();
        Ninja.clearAndReload();
        Blackmailer.clearAndReload();
        Thief.clearAndReload();
        Miner.clearAndReload();
        Trapper.clearAndReload();
        Terrorist.clearAndReload();
        Juggernaut.clearAndReload();
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
        Specter.ClearAndReload();

        // Gamemodes
        HandleGuesser.clearAndReload();

        blockRole();
        ResetRoleSelection();
        CanSeeRoleInfo = false;
    }

}

