using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using AmongUs.GameOptions;
using InnerNet;
using TheOtherRoles.Buttons;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
internal class HudManagerUpdatePatch
{
    private static readonly Dictionary<byte, (string name, Color color)> TagColorDict = new();

    private static void resetNameTagsAndColors()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        var myData = PlayerControl.LocalPlayer.Data;
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
                var nameText = player.cosmetics.nameText;
                if (morphTimerNotUp && morphTargetNotNull && Morphling.morphling == player)
                    playerName = Morphling.morphTarget.Data.PlayerName;

                nameText.text = hidePlayerName(localPlayer, player) ? "" : playerName;
                if (DataManager.Settings.Accessibility.ColorBlindMode)
                {
                    player.cosmetics.colorBlindText.gameObject.SetActive(!hidePlayerName(localPlayer, player));
                }

                player.cosmetics.colorBlindText.gameObject.transform.SetLocalZ(0.0001f);
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
                var (name, color) = dict[playerVoteArea.TargetPlayerId];
                var text = playerVoteArea.NameText;
                text.text = name;
                text.color = color;
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
        if (Blind.blind != null && PlayerControl.LocalPlayer == Blind.blind)
            DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(false);
        // Sadly the report button cannot be hidden due to preventing R to report
    }

    private static void setNameColors()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        var localRole = RoleInfo.getRoleInfoForPlayer(localPlayer, false).FirstOrDefault();
        setPlayerNameColor(localPlayer, localRole.color);

        if (Sheriff.Player != null && Sheriff.Player.Any(x => x == localPlayer))
        {
            foreach (var p in Sheriff.Player) setPlayerNameColor(p, Sheriff.color);
            if (Sheriff.Deputy != null && Sheriff.knowsSheriff) setPlayerNameColor(Sheriff.Deputy, Sheriff.color);
        }
        if (Sheriff.Deputy != null && Sheriff.Deputy == localPlayer)
        {
            setPlayerNameColor(Sheriff.Deputy, Sheriff.color);
            foreach (var p in Sheriff.Player) setPlayerNameColor(p, Sheriff.color);
        }

        if (Prophet.prophet != null && Prophet.prophet == localPlayer)
        {
            setPlayerNameColor(Prophet.prophet, Prophet.color);
            if (Prophet.examined != null && !localPlayer.Data.IsDead) // Reset the name tags when Prophet is dead
            {
                foreach (var p in Prophet.examined)
                {
                    setPlayerNameColor(p.Key, p.Value ^ Vortox.Reversal ? Palette.ImpostorRed : Color.green);
                }
            }
        }

        if (Executioner.executioner != null && localPlayer == Executioner.executioner && Executioner.target != null)
        {
            setPlayerNameColor(Executioner.target, Executioner.color);
        }

        if (Lawyer.lawyer != null && localPlayer == Lawyer.lawyer && Lawyer.target != null)
        {
            setPlayerNameColor(Lawyer.target, RoleInfo.getRoleInfoForPlayer(Lawyer.target, false)?.FirstOrDefault()?.color ?? Color.white);
        }

        if (Mayor.mayor != null && Mayor.Revealed)
        {
            setPlayerNameColor(Mayor.mayor, Mayor.color);
        }

        if (WolfLord.Player != null && WolfLord.Revealed)
        {
            setPlayerNameColor(WolfLord.Player, WolfLord.color);
        }

        if (Grenadier.Player != null && ((localPlayer.isImpostor() && Grenadier.indicatorsMode)
            || localPlayer == Grenadier.Player || shouldShowGhostInfo()))
        {
            foreach (var p in Grenadier.controls)
            {
                if (p != localPlayer && !p.isImpostor()) setPlayerNameColor(p, Color.black);
            }
        }

        if (Jackal.jackal != null && Jackal.jackal.Any(x => x == localPlayer))
        {
            // Jackal can see his sidekick
            foreach (var p in Jackal.jackal) setPlayerNameColor(p, Jackal.color);
            if (Jackal.Sidekick != null) setPlayerNameColor(Jackal.Sidekick, Jackal.color);
        }

        // No else if here, as a Lover of team Jackal needs the colors
        if (Jackal.Sidekick != null && Jackal.Sidekick == localPlayer)
        {
            // Sidekick can see the jackal
            setPlayerNameColor(Jackal.Sidekick, Jackal.color);
            foreach (var p in Jackal.jackal) setPlayerNameColor(p, Jackal.color);
        }

        if (Pavlovsdogs.pavlovsowner != null && Pavlovsdogs.pavlovsowner == localPlayer)
        {
            setPlayerNameColor(Pavlovsdogs.pavlovsowner, Pavlovsdogs.color);
            foreach (var p in Pavlovsdogs.pavlovsdogs) setPlayerNameColor(p, Pavlovsdogs.color);
        }

        if (Pavlovsdogs.pavlovsdogs != null && Pavlovsdogs.pavlovsdogs.Any(p => p == localPlayer))
        {
            foreach (var p in Pavlovsdogs.pavlovsdogs) setPlayerNameColor(p, Pavlovsdogs.color);
            if (Pavlovsdogs.pavlovsowner != null) setPlayerNameColor(Pavlovsdogs.pavlovsowner, Pavlovsdogs.color);
        }

        if (Snitch.snitch != null)
        {
            var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);
            int numberOfTasks = playerTotal - playerCompleted;

            bool forImp = localPlayer.Data.Role.IsImpostor;
            bool forKillerTeam = Snitch.Team == Snitch.includeNeutralTeam.KillNeutral && isKillerNeutral(localPlayer);
            bool forEvilTeam = Snitch.Team == Snitch.includeNeutralTeam.EvilNeutral && isEvilNeutral(localPlayer);
            bool forNeutraTeam = Snitch.Team == Snitch.includeNeutralTeam.AllNeutral && localPlayer.isNeutral();

            if (numberOfTasks <= Snitch.taskCountForReveal && Snitch.snitch.IsAlive())
            {
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (forImp || forKillerTeam || forEvilTeam || forNeutraTeam)
                    {
                        setPlayerNameColor(Snitch.snitch, Snitch.color);
                    }
                }
            }

            if (numberOfTasks == 0 && Snitch.seeInMeeting && Snitch.snitch.IsAlive())
            {
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    bool TargetsImp = p.Data.Role.IsImpostor;
                    bool TargetsKillerTeam = Snitch.Team == Snitch.includeNeutralTeam.KillNeutral && isKillerNeutral(p);
                    bool TargetsEvilTeam = Snitch.Team == Snitch.includeNeutralTeam.EvilNeutral && isEvilNeutral(p);
                    bool TargetsNeutraTeam = Snitch.Team == Snitch.includeNeutralTeam.AllNeutral && p.isNeutral();
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

        // No else if here, as the Impostors need the Spy name to be colored
        if (Spy.spy != null && localPlayer.Data.Role.IsImpostor) setPlayerNameColor(Spy.spy, Spy.color);
    }

    private static void setNameTags()
    {
        var local = PlayerControl.LocalPlayer;
        // Lovers
        if (Lovers.lover1 != null && Lovers.lover2 != null &&
            (Lovers.lover1 == local || Lovers.lover2 == local))
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
                    if (local == Akujo.akujo) p.cosmetics.nameText.text += cs(Color.gray, " ♥");
                    if (local == p)
                    {
                        Akujo.akujo.cosmetics.nameText.text += cs(Akujo.color, " ♥");
                        p.cosmetics.nameText.text += cs(Akujo.color, " ♥");
                    }
                }
            }
            if (Akujo.honmei != null)
            {
                if (local == Akujo.akujo) Akujo.honmei.cosmetics.nameText.text += cs(Akujo.color, " ♥");
                if (local == Akujo.honmei)
                {
                    Akujo.akujo.cosmetics.nameText.text += cs(Akujo.color, " ♥");
                    Akujo.honmei.cosmetics.nameText.text += cs(Akujo.color, " ♥");
                }
            }

            if (MeetingHud.Instance != null)
            {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                {
                    if (player.TargetPlayerId == Akujo.akujo.PlayerId && ((Akujo.honmei != null && Akujo.honmei == local) || (Akujo.keeps != null && Akujo.keeps.Any(x => x.PlayerId == local.PlayerId))))
                        player.NameText.text += cs(Akujo.color, " ♥");
                    if (local == Akujo.akujo)
                    {
                        if (player.TargetPlayerId == Akujo.honmei?.PlayerId) player.NameText.text += cs(Akujo.color, " ♥");
                        if (Akujo.keeps != null && Akujo.keeps.Any(x => x.PlayerId == player.TargetPlayerId)) player.NameText.text += cs(Color.gray, " ♥");
                    }
                }
            }
        }

        // Parttimer
        if (PartTimer.partTimer != null && PartTimer.target != null && (local == PartTimer.partTimer || local == PartTimer.target || shouldShowGhostInfo()))
        {
            var suffix = cs(PartTimer.color, " ★");
            PartTimer.partTimer.cosmetics.nameText.text += suffix;
            PartTimer.target.cosmetics.nameText.text += suffix;

            if (MeetingHud.Instance != null)
                foreach (var player in MeetingHud.Instance.playerStates)
                    if (PartTimer.partTimer.PlayerId == player.TargetPlayerId || PartTimer.target.PlayerId == player.TargetPlayerId)
                        player.NameText.text += suffix;
        }

        var localIsArsonist = Arsonist.arsonist != null && Arsonist.dousedPlayers != null && Arsonist.arsonist == local;
        var localIsDead = Arsonist.arsonist != null && Arsonist.dousedPlayers != null && shouldShowGhostInfo();
        if (localIsArsonist || localIsDead)
        {
            var suffix = cs(Arsonist.color, " ♨");
            foreach (var target in Arsonist.dousedPlayers)
            {
                target.cosmetics.nameText.text += suffix;
            }

            if (MeetingHud.Instance != null)
                foreach (var target in MeetingHud.Instance.playerStates)
                    if (Arsonist.dousedPlayers.Any(p => p.PlayerId == target.TargetPlayerId))
                        target.NameText.text += suffix;
        }


        // Lawyer or Prosecutor
        var localIsLawyer = Lawyer.lawyer != null && Lawyer.target != null && Lawyer.lawyer == local;
        var localIsKnowingTarget = Lawyer.lawyer != null && Lawyer.target != null && Lawyer.targetKnows && Lawyer.target == local;
        if (localIsLawyer || (localIsKnowingTarget && Lawyer.lawyer.IsAlive()))
        {
            var suffix = cs(Lawyer.color, " §");
            Lawyer.target.cosmetics.nameText.text += suffix;

            if (MeetingHud.Instance != null)
                foreach (var player in MeetingHud.Instance.playerStates)
                    if (player.TargetPlayerId == Lawyer.target.PlayerId)
                        player.NameText.text += suffix;
        }

        var localIsExecutioner = Executioner.executioner != null && Executioner.target != null && Executioner.executioner == local;
        if (localIsExecutioner && Executioner.executioner.IsAlive())
        {
            var suffix = cs(Executioner.color, " §");
            Executioner.target.cosmetics.nameText.text += suffix;

            if (MeetingHud.Instance != null)
                foreach (var player in MeetingHud.Instance.playerStates)
                    if (player.TargetPlayerId == Executioner.target.PlayerId)
                        player.NameText.text += suffix;
        }

        // Display lighter / darker color for all alive players
        if (PlayerControl.LocalPlayer != null && MeetingHud.Instance != null && ModOption.showLighterDarker)
        {
            foreach (var player in MeetingHud.Instance.playerStates)
            {
                var target = playerById(player.TargetPlayerId);
                if (target != null) player.NameText.text += $" ({(isLighterColor(target) ? "浅" : "深")})";
            }
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
        Jackal.swoopTimer -= dt;
        Swooper.swoopTimer -= dt;
        foreach (var key in Sheriff.handcuffedKnows.Keys)
            Sheriff.handcuffedKnows[key] -= dt;
    }

    public static void miniUpdate()
    {
        if (Mini.mini == null || Camouflager.camouflageTimer > 0f || MushroomSabotageActive ||
            (Mini.mini == Morphling.morphling && Morphling.morphTimer > 0f) ||
            (Mini.mini == Ninja.ninja && Ninja.isInvisable) || SurveillanceMinigamePatch.nightVisionIsActive ||
            (Mini.mini == Swooper.swooper && Swooper.isInvisable) ||
            (Jackal.jackal.Any(x => x == Mini.mini) && Jackal.isInvisable) || isActiveCamoComms) return;

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
        if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;
        if (MeetingHud.Instance)
        {
            __instance.KillButton.Hide();
            return;
        }

        var enabled = true;
        if (Vampire.vampire != null && Vampire.vampire == PlayerControl.LocalPlayer)
            enabled = false;

        if (enabled) __instance.KillButton.Show();
        else __instance.KillButton.Hide();

        if (Sheriff.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) &&
            Sheriff.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] > 0) __instance.KillButton.Hide();
    }

    private static void updateReportButton(HudManager __instance)
    {
        if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
        if ((Sheriff.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) &&
             Sheriff.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] > 0) ||
            MeetingHud.Instance) __instance.ReportButton.Hide();
        else if (!__instance.ReportButton.isActiveAndEnabled) __instance.ReportButton.Show();
    }

    private static void updateVentButton(HudManager __instance)
    {
        if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
        if ((Sheriff.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) &&
             Sheriff.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] > 0) ||
            MeetingHud.Instance) __instance.ImpostorVentButton.Hide();
        else if (PlayerControl.LocalPlayer.roleCanUseVents() &&
                 !__instance.ImpostorVentButton.isActiveAndEnabled) __instance.ImpostorVentButton.Show();
    }

    private static void updateUseButton(HudManager __instance)
    {
        if (MeetingHud.Instance) __instance.UseButton.Hide();
    }

    private static void updateSabotageButton(HudManager __instance)
    {
        if (PlayerControl.LocalPlayer.Data.IsDead && CustomOptionHolder.deadImpsBlockSabotage.GetBool()) __instance.SabotageButton.Hide();
    }

    private static void updateMapButton(HudManager __instance)
    {
        if (Trapper.trapper == null || !(PlayerControl.LocalPlayer.PlayerId == Trapper.trapper.PlayerId) ||
            __instance == null || __instance.MapButton.HeldButtonSprite == null) return;
        __instance.MapButton.HeldButtonSprite.color = Trapper.playersOnMap.Any() ? Trapper.color : Color.white;
    }

    public static void updateGiantSize(HudManager __instance)
    {
        if (Giant.giant == null) return;
        DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
        foreach (var body in array.Where(x => x.ParentId == Giant.giant.PlayerId))
        {
            try
            {
                body.transform.localScale = new Vector3(Giant.size, Giant.size, 1f);
            }
            catch { }
        }
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
        updateGiantSize(__instance);
        updateBlindReport();
        updateMapButton(__instance);
        if (!MeetingHud.Instance) __instance.AbilityButton?.Update();

        if (Specter.Player != null && PlayerControl.LocalPlayer == Specter.Player && InGame && !InMeeting)
        {
            __instance.ShadowQuad?.gameObject?.SetActive(true);
        }

        // Fix dead player's pets being visible by just always updating whether the pet should be visible at all.
        foreach (PlayerControl target in PlayerControl.AllPlayerControls)
        {
            var pet = target.GetPet();
            if (pet != null)
                pet.Visible = ((PlayerControl.LocalPlayer.Data.IsDead && target.Data.IsDead) || !target.Data.IsDead) && !target.inVent;
        }
    }
}