using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using InnerNet;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Objects;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
internal class HudManagerUpdatePatch
{
    private static readonly Dictionary<byte, (string name, Color color)> TagColorDict = new();

    private static void resetNameTagsAndColors()
    {
        var localPlayer = CachedPlayer.LocalPlayer.PlayerControl;
        var myData = CachedPlayer.LocalPlayer.Data;
        var amImpostor = myData.Role.IsImpostor;
        var morphTimerNotUp = Morphling.morphTimer > 0f;
        var morphTargetNotNull = Morphling.morphTarget != null;

        var dict = TagColorDict;
        dict.Clear();

        foreach (var data in GameData.Instance.AllPlayers.GetFastEnumerator())
        {
            var player = data.Object;
            var text = data.PlayerName;
            Color color;
            if (player)
            {
                var playerName = text;
                if (morphTimerNotUp && morphTargetNotNull && Morphling.morphling == player)
                    playerName = Morphling.morphTarget.Data.PlayerName;
                var nameText = player.cosmetics.nameText;

                nameText.text = hidePlayerName(localPlayer, player) ? "" : playerName;
                nameText.color = color = amImpostor && data.Role.IsImpostor ? Palette.ImpostorRed : Color.white;
                nameText.color = nameText.color.SetAlpha(Chameleon.visibility(player.PlayerId));
            }
            else
            {
                color = Color.white;
            }


            dict.Add(data.PlayerId, (text, color));
        }

        if (MeetingHud.Instance != null)
            foreach (var playerVoteArea in MeetingHud.Instance.playerStates)
            {
                var data = dict[playerVoteArea.TargetPlayerId];
                var text = playerVoteArea.NameText;
                text.text = data.name;
                text.color = data.color;
            }
    }

    private static void setPlayerNameColor(PlayerControl p, Color color)
    {
        p.cosmetics.nameText.color = color.SetAlpha(Chameleon.visibility(p.PlayerId));
        if (MeetingHud.Instance != null)
            foreach (var player in MeetingHud.Instance.playerStates)
                if (player.NameText != null && p.PlayerId == player.TargetPlayerId)
                    player.NameText.color = color;
    }

    private static void updateBlindReport()
    {
        if (Blind.blind != null && CachedPlayer.LocalPlayer.PlayerControl == Blind.blind)
            DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(false);
        // Sadly the report button cannot be hidden due to preventing R to report
    }

    private static void setNameColors()
    {
        var localPlayer = CachedPlayer.LocalPlayer.PlayerControl;
        var localRole = RoleInfo.getRoleInfoForPlayer(localPlayer, false).FirstOrDefault();
        setPlayerNameColor(localPlayer, localRole.color);

        if (Sheriff.sheriff != null && Sheriff.sheriff == localPlayer)
        {
            setPlayerNameColor(Sheriff.sheriff, Sheriff.color);
            if (Deputy.deputy != null && Deputy.knowsSheriff) setPlayerNameColor(Deputy.deputy, Sheriff.color);
        }
        if (Deputy.deputy != null && Deputy.deputy == localPlayer)
        {
            setPlayerNameColor(Deputy.deputy, Deputy.color);
            if (Sheriff.sheriff != null && Deputy.knowsSheriff) setPlayerNameColor(Sheriff.sheriff, Sheriff.color);
        }

        if (Prophet.prophet != null && Prophet.prophet == localPlayer)
        {
            setPlayerNameColor(Prophet.prophet, Prophet.color);
            if (Prophet.examined != null && !localPlayer.Data.IsDead) // Reset the name tags when Prophet is dead
            {
                foreach (var p in Prophet.examined)
                {
                    setPlayerNameColor(p.Key, p.Value ? Palette.ImpostorRed : Color.green);
                }
            }
        }

        else if (Jackal.jackal != null && Jackal.jackal == localPlayer)
        {
            // Jackal can see his sidekick
            setPlayerNameColor(Jackal.jackal, Jackal.color);
            if (Sidekick.sidekick != null) setPlayerNameColor(Sidekick.sidekick, Jackal.color);
            if (Jackal.fakeSidekick != null) setPlayerNameColor(Jackal.fakeSidekick, Jackal.color);
        }

        if (Executioner.executioner != null && localPlayer == Executioner.executioner)
        {
            setPlayerNameColor(Executioner.target, Color.grey);
        }

        if (Snitch.snitch != null)
        {
            var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);
            int numberOfTasks = playerTotal - playerCompleted;
            var snitchIsDead = Snitch.snitch.Data.IsDead;

            bool forImp = localPlayer.Data.Role.IsImpostor;
            bool forKillerTeam = Snitch.Team == Snitch.includeNeutralTeam.KillNeutral && isKiller(localPlayer);
            bool forEvilTeam = Snitch.Team == Snitch.includeNeutralTeam.EvilNeutral && isEvil(localPlayer);
            bool forNeutraTeam = Snitch.Team == Snitch.includeNeutralTeam.AllNeutral && isNeutral(localPlayer);
            if (numberOfTasks <= Snitch.taskCountForReveal)
            {
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (forImp || forKillerTeam || forEvilTeam || forNeutraTeam)
                    {
                        setPlayerNameColor(Snitch.snitch, Snitch.color);
                    }
                }
            }
            if (numberOfTasks == 0 && Snitch.seeInMeeting && !snitchIsDead)
            {
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    bool TargetsImp = p.Data.Role.IsImpostor;
                    bool TargetsKillerTeam = Snitch.Team == Snitch.includeNeutralTeam.KillNeutral && isKiller(p);
                    bool TargetsEvilTeam = Snitch.Team == Snitch.includeNeutralTeam.EvilNeutral && isEvil(p);
                    bool TargetsNeutraTeam = Snitch.Team == Snitch.includeNeutralTeam.AllNeutral && isNeutral(p);
                    var targetsRole = RoleInfo.getRoleInfoForPlayer(p, false).FirstOrDefault();
                    if (localPlayer == Snitch.snitch && (TargetsImp || TargetsKillerTeam || TargetsEvilTeam || TargetsNeutraTeam))
                    {
                        if (Snitch.teamNeutraUseDifferentArrowColor)
                        {
                            setPlayerNameColor(p, targetsRole.color);
                        }
                        else
                        {
                            setPlayerNameColor(p, Palette.ImpostorRed);
                        }
                    }
                }
            }
        }

        // No else if here, as a Lover of team Jackal needs the colors
        if (Sidekick.sidekick != null && Sidekick.sidekick == localPlayer)
        {
            // Sidekick can see the jackal
            setPlayerNameColor(Sidekick.sidekick, Sidekick.color);
            if (Jackal.jackal != null) setPlayerNameColor(Jackal.jackal, Jackal.color);
        }

        // No else if here, as the Impostors need the Spy name to be colored
        if (Spy.spy != null && localPlayer.Data.Role.IsImpostor) setPlayerNameColor(Spy.spy, Spy.color);
        if (Sidekick.sidekick != null && Sidekick.wasTeamRed && localPlayer.Data.Role.IsImpostor)
            setPlayerNameColor(Sidekick.sidekick, Spy.color);
        if (Jackal.jackal != null && Jackal.wasTeamRed && localPlayer.Data.Role.IsImpostor)
            setPlayerNameColor(Jackal.jackal, Spy.color);

        // Crewmate roles with no changes: Mini
        // Impostor roles with no changes: Morphling, Camouflager, Vampire, Godfather, Eraser, Janitor, Cleaner, Warlock, BountyHunter,  Witch and Mafioso
    }

    private static void setNameTags()
    {
        // Lovers
        if (Lovers.lover1 != null && Lovers.lover2 != null &&
            (Lovers.lover1 == CachedPlayer.LocalPlayer.PlayerControl ||
             Lovers.lover2 == CachedPlayer.LocalPlayer.PlayerControl))
        {
            var suffix = cs(Lovers.color, " ♥");
            Lovers.lover1.cosmetics.nameText.text += suffix;
            Lovers.lover2.cosmetics.nameText.text += suffix;

            if (MeetingHud.Instance != null)
                foreach (var player in MeetingHud.Instance.playerStates)
                    if (Lovers.lover1.PlayerId == player.TargetPlayerId ||
                        Lovers.lover2.PlayerId == player.TargetPlayerId)
                        player.NameText.text += suffix;
        }
        if (Akujo.akujo != null && (Akujo.keeps != null || Akujo.honmei != null))
        {
            if (Akujo.keeps != null)
            {
                foreach (PlayerControl p in Akujo.keeps)
                {
                    if (CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo) p.cosmetics.nameText.text += cs(Color.gray, " ♥");
                    if (CachedPlayer.LocalPlayer.PlayerControl == p)
                    {
                        Akujo.akujo.cosmetics.nameText.text += cs(Akujo.color, " ♥");
                        p.cosmetics.nameText.text += cs(Akujo.color, " ♥");
                    }
                }
            }
            if (Akujo.honmei != null)
            {
                if (CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo) Akujo.honmei.cosmetics.nameText.text += cs(Akujo.color, " ♥");
                if (CachedPlayer.LocalPlayer.PlayerControl == Akujo.honmei)
                {
                    Akujo.akujo.cosmetics.nameText.text += cs(Akujo.color, " ♥");
                    Akujo.honmei.cosmetics.nameText.text += cs(Akujo.color, " ♥");
                }
            }

            if (MeetingHud.Instance != null)
            {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                {
                    if (player.TargetPlayerId == Akujo.akujo.PlayerId && ((Akujo.honmei != null && Akujo.honmei == CachedPlayer.LocalPlayer.PlayerControl) || (Akujo.keeps != null && Akujo.keeps.Any(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerControl.PlayerId))))
                        player.NameText.text += cs(Akujo.color, " ♥");
                    if (CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo)
                    {
                        if (player.TargetPlayerId == Akujo.honmei?.PlayerId) player.NameText.text += cs(Akujo.color, " ♥");
                        if (Akujo.keeps != null && Akujo.keeps.Any(x => x.PlayerId == player.TargetPlayerId)) player.NameText.text += cs(Color.gray, " ♥");
                    }
                }
            }
        }
        // Lawyer or Prosecutor
        var localIsLawyer = Lawyer.lawyer != null && Lawyer.target != null && Lawyer.lawyer == PlayerControl.LocalPlayer;
        var localIsKnowingTarget = Lawyer.lawyer != null && Lawyer.target != null && Lawyer.targetKnows && Lawyer.target == PlayerControl.LocalPlayer;
        if (localIsLawyer || (localIsKnowingTarget && !Lawyer.lawyer.Data.IsDead))
        {
            var suffix = cs(Lawyer.color, " §");
            Lawyer.target.cosmetics.nameText.text += suffix;

            if (MeetingHud.Instance != null)
                foreach (var player in MeetingHud.Instance.playerStates)
                    if (player.TargetPlayerId == Lawyer.target.PlayerId)
                        player.NameText.text += suffix;
        }

        var localIsExecutioner = Executioner.executioner != null && Executioner.target != null && Executioner.executioner == PlayerControl.LocalPlayer;
        if (localIsExecutioner && !Executioner.executioner.Data.IsDead)
        {
            var suffix = cs(Executioner.color, " §");
            Executioner.target.cosmetics.nameText.text += suffix;

            if (MeetingHud.Instance != null)
                foreach (var player in MeetingHud.Instance.playerStates)
                    if (player.TargetPlayerId == Executioner.target.PlayerId)
                        player.NameText.text += suffix;
        }

        // Former Thief
        if (Thief.formerThief != null && (Thief.formerThief == CachedPlayer.LocalPlayer.PlayerControl ||
                                          CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead))
        {
            var suffix = cs(Thief.color, " $");
            Thief.formerThief.cosmetics.nameText.text += suffix;
            if (MeetingHud.Instance != null)
                foreach (var player in MeetingHud.Instance.playerStates)
                    if (player.TargetPlayerId == Thief.formerThief.PlayerId)
                        player.NameText.text += suffix;
        }

        // Display lighter / darker color for all alive players
        if (CachedPlayer.LocalPlayer != null && MeetingHud.Instance != null && MapOption.showLighterDarker)
            foreach (var player in MeetingHud.Instance.playerStates)
            {
                var target = playerById(player.TargetPlayerId);
                if (target != null) player.NameText.text += $" ({(isLighterColor(target) ? "浅" : "深")})";
            }
        // Add medic shield info:
        if (MeetingHud.Instance != null && Medic.medic != null && Medic.shielded != null && Medic.shieldVisible(Medic.shielded))
        {
            foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                if (player.TargetPlayerId == Medic.shielded.PlayerId)
                {
                    player.NameText.text = cs(Medic.color, "[") + player.NameText.text + cs(Medic.color, "]");
                    // player.HighlightedFX.color = Medic.color;
                    // player.HighlightedFX.enabled = true;
                }
        }
    }

    private static void updateShielded()
    {
        if (Medic.shielded == null) return;

        if (Medic.shielded.Data.IsDead || Medic.medic == null || Medic.medic.Data.IsDead) Medic.shielded = null;
    }

    private static void timerUpdate()
    {
        var dt = Time.deltaTime;
        Hacker.hackerTimer -= dt;
        Trickster.lightsOutTimer -= dt;
        Tracker.corpsesTrackingTimer -= dt;
        Ninja.invisibleTimer -= dt;
        Swooper.swoopTimer -= dt;
        HideNSeek.timer -= dt;
        foreach (var key in Deputy.handcuffedKnows.Keys)
            Deputy.handcuffedKnows[key] -= dt;
    }

    public static void miniUpdate()
    {
        if (Mini.mini == null || Camouflager.camouflageTimer > 0f || MushroomSabotageActive() ||
            (Mini.mini == Morphling.morphling && Morphling.morphTimer > 0f) ||
            (Mini.mini == Ninja.ninja && Ninja.isInvisble) || SurveillanceMinigamePatch.nightVisionIsActive ||
            (Mini.mini == Swooper.swooper && Swooper.isInvisable) || isActiveCamoComms()) return;

        var growingProgress = Mini.growingProgress();
        var scale = (growingProgress * 0.35f) + 0.35f;
        var suffix = "";
        if (growingProgress != 1f)
            suffix = " <color=#FAD934FF>(" + Mathf.FloorToInt(growingProgress * 18) + ")</color>";
        if (!Mini.isGrowingUpInMeeting && MeetingHud.Instance != null && Mini.ageOnMeetingStart != 0 &&
            !(Mini.ageOnMeetingStart >= 18))
            suffix = " <color=#FAD934FF>(" + Mini.ageOnMeetingStart + ")</color>";

        Mini.mini.cosmetics.nameText.text += suffix;
        if (MeetingHud.Instance != null)
            foreach (var player in MeetingHud.Instance.playerStates)
                if (player.NameText != null && Mini.mini.PlayerId == player.TargetPlayerId)
                    player.NameText.text += suffix;

        if (Morphling.morphling != null && Morphling.morphTarget == Mini.mini && Morphling.morphTimer > 0f)
            Morphling.morphling.cosmetics.nameText.text += suffix;
    }

    private static void updateImpostorKillButton(HudManager __instance)
    {
        if (!CachedPlayer.LocalPlayer.Data.Role.IsImpostor) return;
        if (MeetingHud.Instance)
        {
            __instance.KillButton.Hide();
            return;
        }

        var enabled = true;
        if (Vampire.vampire != null && Vampire.vampire == CachedPlayer.LocalPlayer.PlayerControl)
            enabled = false;
        else if (Cultist.cultist != null && Cultist.cultist == CachedPlayer.LocalPlayer.PlayerControl &&
                 Cultist.needsFollower) enabled = false;

        if (enabled) __instance.KillButton.Show();
        else __instance.KillButton.Hide();

        if (Deputy.handcuffedKnows.ContainsKey(CachedPlayer.LocalPlayer.PlayerId) &&
            Deputy.handcuffedKnows[CachedPlayer.LocalPlayer.PlayerId] > 0) __instance.KillButton.Hide();
    }

    private static void updateReportButton(HudManager __instance)
    {
        if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
        if ((Deputy.handcuffedKnows.ContainsKey(CachedPlayer.LocalPlayer.PlayerId) &&
             Deputy.handcuffedKnows[CachedPlayer.LocalPlayer.PlayerId] > 0) ||
            MeetingHud.Instance) __instance.ReportButton.Hide();
        else if (!__instance.ReportButton.isActiveAndEnabled) __instance.ReportButton.Show();
    }

    private static void updateVentButton(HudManager __instance)
    {
        if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
        if ((Deputy.handcuffedKnows.ContainsKey(CachedPlayer.LocalPlayer.PlayerId) &&
             Deputy.handcuffedKnows[CachedPlayer.LocalPlayer.PlayerId] > 0) ||
            MeetingHud.Instance) __instance.ImpostorVentButton.Hide();
        else if (CachedPlayer.LocalPlayer.PlayerControl.roleCanUseVents() &&
                 !__instance.ImpostorVentButton.isActiveAndEnabled) __instance.ImpostorVentButton.Show();
    }

    private static void updateUseButton(HudManager __instance)
    {
        if (MeetingHud.Instance) __instance.UseButton.Hide();
    }

    private static void updateSabotageButton(HudManager __instance)
    {
        if (MeetingHud.Instance || MapOption.gameMode == CustomGamemodes.HideNSeek ||
            MapOption.gameMode == CustomGamemodes.PropHunt) __instance.SabotageButton.Hide();
        if (PlayerControl.LocalPlayer.Data.IsDead && CustomOptionHolder.deadImpsBlockSabotage.getBool()) __instance.SabotageButton.Hide();
    }

    private static void updateMapButton(HudManager __instance)
    {
        if (Trapper.trapper == null || !(CachedPlayer.LocalPlayer.PlayerId == Trapper.trapper.PlayerId) ||
            __instance == null || __instance.MapButton.HeldButtonSprite == null) return;
        __instance.MapButton.HeldButtonSprite.color = Trapper.playersOnMap.Any() ? Trapper.color : Color.white;
    }

    private static void Postfix(HudManager __instance)
    {
        var player = PlayerControl.LocalPlayer;
        if (player == null) return;
        //壁抜け
        if (Input.GetKeyDown(KeyCode.LeftControl))
            if ((AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started ||
                 AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                && player.CanMove)
                player.Collider.offset = new Vector2(0f, 127f);
        //壁抜け解除
        if (player.Collider.offset.y == 127f)
            if (!Input.GetKey(KeyCode.LeftControl) || AmongUsClient.Instance.IsGameStarted)
                player.Collider.offset = new Vector2(0f, -0.3636f);
        if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started ||
            GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

        CustomButton.HudUpdate();
        resetNameTagsAndColors();
        setNameColors();
        updateShielded();
        setNameTags();

        // Impostors
        updateImpostorKillButton(__instance);
        // Timer updates
        timerUpdate();
        // Mini
        miniUpdate();

        // Deputy Sabotage, Use and Vent Button Disabling
        updateReportButton(__instance);
        updateVentButton(__instance);
        // Meeting hide buttons if needed (used for the map usage, because closing the map would show buttons)
        updateSabotageButton(__instance);
        updateUseButton(__instance);
        updateBlindReport();
        updateMapButton(__instance);
        if (!MeetingHud.Instance) __instance.AbilityButton?.Update();

        // Fix dead player's pets being visible by just always updating whether the pet should be visible at all.
        foreach (PlayerControl target in CachedPlayer.AllPlayers)
        {
            var pet = target.GetPet();
            if (pet != null)
                pet.Visible = ((PlayerControl.LocalPlayer.Data.IsDead && target.Data.IsDead) || !target.Data.IsDead) &&
                              !target.inVent;
        }
    }
}