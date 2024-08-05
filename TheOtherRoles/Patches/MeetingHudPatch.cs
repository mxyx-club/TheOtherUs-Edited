using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.QuickChat;
using Hazel;
using Reactor.Utilities;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static TheOtherRoles.Options.MapOption;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Patches;

[HarmonyPatch]
internal class MeetingHudPatch
{
    private static bool[] selections;
    private static SpriteRenderer[] renderers;
    private static NetworkedPlayerInfo target;
    private static PassiveButton[] swapperButtonList;
    private static TextMeshPro meetingExtraButtonLabel;
    public static GameObject MeetingExtraButton;
    public static bool shookAlready;
    private static PlayerVoteArea swapped1;
    private static PlayerVoteArea swapped2;

    public static GameObject guesserUI;
    public static PassiveButton guesserUIExitButton;
    public static byte guesserCurrentTarget;


    private static void swapperOnClick(int i, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Results || Swapper.charges <= 0) return;
        if (__instance.playerStates[i].AmDead) return;

        var selectedCount = selections.Count(b => b);
        var renderer = renderers[i];

        switch (selectedCount)
        {
            case 0:
                renderer.color = Color.yellow;
                selections[i] = true;
                break;
            case 1 when selections[i]:
                renderer.color = Color.red;
                selections[i] = false;
                break;
            case 1:
                selections[i] = true;
                renderer.color = Color.yellow;
                meetingExtraButtonLabel.text = cs(Color.yellow, "确认换票");
                break;
            case 2 when !selections[i]:
                return;
            case 2:
                renderer.color = Color.red;
                selections[i] = false;
                meetingExtraButtonLabel.text = cs(Color.red, "确认换票");
                break;
        }
    }

    private static void swapperConfirm(MeetingHud __instance)
    {
        __instance.playerStates[0].Cancel(); // This will stop the underlying buttons of the template from showing up
        if (__instance.state == MeetingHud.VoteStates.Results) return;
        if (selections.Count(b => b) != 2) return;
        if (Swapper.charges <= 0 || Swapper.playerId1 != byte.MaxValue) return;

        PlayerVoteArea firstPlayer = null;
        PlayerVoteArea secondPlayer = null;
        for (var A = 0; A < selections.Length; A++)
        {
            if (selections[A])
            {
                if (firstPlayer == null)
                    firstPlayer = __instance.playerStates[A];
                else
                    secondPlayer = __instance.playerStates[A];
                renderers[A].color = Color.green;
            }
            else if (renderers[A] != null)
            {
                renderers[A].color = Color.gray;
            }

            swapperButtonList[A]?.OnClick.RemoveAllListeners(); // Swap buttons can't be clicked / changed anymore
        }

        if (firstPlayer != null && secondPlayer != null)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.SwapperSwap, SendOption.Reliable);
            writer.Write(firstPlayer.TargetPlayerId);
            writer.Write(secondPlayer.TargetPlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            RPCProcedure.swapperSwap(firstPlayer.TargetPlayerId, secondPlayer.TargetPlayerId);
            meetingExtraButtonLabel.text = cs(Color.green, "换票成功!");
            Swapper.charges--;
        }
    }

    public static void swapperCheckAndReturnSwap(MeetingHud __instance, byte dyingPlayerId)
    {
        // someone was guessed or dced in the meeting, check if this affects the swapper.
        if (Swapper.swapper == null || __instance.state == MeetingHud.VoteStates.Results) return;

        // reset swap.
        var reset = false;
        if (dyingPlayerId == Swapper.playerId1 || dyingPlayerId == Swapper.playerId2)
        {
            reset = true;
            Swapper.playerId1 = Swapper.playerId2 = byte.MaxValue;
        }

        // Only for the swapper: Reset all the buttons and charges value to their original state.
        if (CachedPlayer.LocalPlayer.PlayerControl != Swapper.swapper) return;

        // check if dying player was a selected player (but not confirmed yet)
        for (var i = 0; i < __instance.playerStates.Count; i++)
        {
            reset = reset || (selections[i] && __instance.playerStates[i].TargetPlayerId == dyingPlayerId);
            if (reset) break;
        }

        if (!reset) return;

        for (var i = 0; i < selections.Length; i++)
        {
            selections[i] = false;
            var playerVoteArea = __instance.playerStates[i];
            if (playerVoteArea.AmDead || (playerVoteArea.TargetPlayerId == Swapper.swapper.PlayerId && Swapper.canOnlySwapOthers)) continue;
            renderers[i].color = Color.red;
            Swapper.charges++;
            var copyI = i;
            swapperButtonList[i].OnClick.RemoveAllListeners();
            swapperButtonList[i].OnClick.AddListener((Action)(() => swapperOnClick(copyI, __instance)));
        }

        meetingExtraButtonLabel.text = cs(Color.red, "确认交换");
    }

    private static void mayorToggleVoteTwice(MeetingHud __instance)
    {
        __instance.playerStates[0].Cancel(); // This will stop the underlying buttons of the template from showing up
        if (__instance.state == MeetingHud.VoteStates.Results || Mayor.mayor.Data.IsDead) return;

        // Only accept changes until the mayor voted
        var mayorPVA = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == Mayor.mayor.PlayerId);
        if (Mayor.Revealed && mayorPVA != null && mayorPVA.DidVote)
        {
            SoundEffectsManager.play("fail");
            return;
        }
        Mayor.Revealed = !Mayor.Revealed;

        var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
            (byte)CustomRPC.MayorRevealed, SendOption.Reliable);
        writer.Write(Mayor.Revealed);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        meetingExtraButtonLabel.text = cs(Mayor.color, "揭示");
        Object.Destroy(MeetingExtraButton);

    }

    private static void guesserOnClick(int buttonTarget, MeetingHud __instance)
    {
        if (guesserUI != null || __instance.state is not (MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted)) return;
        __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(false));

        var PhoneUI = Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");
        var container = Object.Instantiate(PhoneUI, __instance.transform);
        container.transform.localPosition = new Vector3(0, 0, -5f);
        guesserUI = container.gameObject;

        var i = 0;
        var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
        var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
        var smallButtonTemplate = __instance.playerStates[0].Buttons.transform.Find("CancelButton");
        var textTemplate = __instance.playerStates[0].NameText;

        guesserCurrentTarget = __instance.playerStates[buttonTarget].TargetPlayerId;

        var exitButtonParent = new GameObject().transform;
        exitButtonParent.SetParent(container);
        var exitButton = Object.Instantiate(buttonTemplate.transform, exitButtonParent);
        var exitButtonMask = Object.Instantiate(maskTemplate, exitButtonParent);
        exitButton.gameObject.GetComponent<SpriteRenderer>().sprite =
            smallButtonTemplate.GetComponent<SpriteRenderer>().sprite;
        var transform = exitButtonParent.transform;
        transform.localPosition = new Vector3(2.725f, 2.1f, -5);
        transform.localScale = new Vector3(0.217f, 0.9f, 1);
        guesserUIExitButton = exitButton.GetComponent<PassiveButton>();
        guesserUIExitButton.OnClick.RemoveAllListeners();
        guesserUIExitButton.OnClick.AddListener((Action)(() =>
        {
            __instance.playerStates.ToList().ForEach(x =>
            {
                x.gameObject.SetActive(true);
                if (CachedPlayer.LocalPlayer.Data.IsDead && x.transform.FindChild("ShootButton") != null)
                    Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
            });
            Object.Destroy(container.gameObject);
        }));

        var buttons = new List<Transform>();
        Transform selectedButton = null;

        foreach (var roleInfo in RoleInfo.allRoleInfos)
        {
            var guesserRole = Guesser.niceGuesser != null && CachedPlayer.LocalPlayer.PlayerId == Guesser.niceGuesser.PlayerId
                    ? RoleId.NiceGuesser
                    : RoleId.EvilGuesser;

            if (Doomsayer.doomsayer != null && CachedPlayer.LocalPlayer.PlayerId == Doomsayer.doomsayer.PlayerId) guesserRole = RoleId.Doomsayer;

            switch (guesserRole)
            {
                case RoleId.Doomsayer when !Doomsayer.canGuessImpostor && roleInfo.isImpostor:
                case RoleId.Doomsayer when !Doomsayer.canGuessNeutral && roleInfo.isNeutral:
                    continue;
            }

            if (roleInfo.roleId == RoleId.Poucher && Poucher.spawnModifier) continue;
            if (Mayor.mayor != null && Mayor.Revealed && Mayor.Revealed && roleInfo.roleId == RoleId.Mayor) continue;

            if (allowModGuess && roleInfo.isModifier)
            {
                // Allow Guessing the following mods: Bait, TieBreaker, Bloody, and VIP
                if (roleInfo.roleId is
                    not RoleId.Aftermath and
                    not RoleId.Bait and
                    not RoleId.Tiebreaker and
                    not RoleId.Bloody and
                    not RoleId.Cursed and
                    not RoleId.Torch and
                    not RoleId.Slueth and
                    not RoleId.Watcher and
                    not RoleId.Radar and
                    not RoleId.Tunneler and
                    not RoleId.Multitasker and
                    not RoleId.Lover and
                    not RoleId.Vip) continue;
            }
            else if (roleInfo.isModifier)
            {
                continue;
            }

            if (roleInfo.roleId == guesserRole ||
                (!HandleGuesser.evilGuesserCanGuessSpy && guesserRole == RoleId.EvilGuesser &&
                 roleInfo.roleId == RoleId.Spy && !HandleGuesser.isGuesserGm) ||
                (!Guesser.evilGuesserCanGuessCrewmate && guesserRole == RoleId.EvilGuesser &&
                 roleInfo.roleId == RoleId.Crewmate)) continue; // Not guessable roles & modifier

            switch (HandleGuesser.isGuesserGm)
            {
                case true when roleInfo.roleId is RoleId.NiceGuesser or RoleId.EvilGuesser:
                case true when CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor &&
                               !HandleGuesser.evilGuesserCanGuessSpy && roleInfo.roleId == RoleId.Spy:
                    continue; // remove Guesser for guesser game mode
            }

            // remove all roles that cannot spawn due to the settings from the ui.
            var roleData = RoleManagerSelectRolesPatch.getRoleAssignmentData();
            switch (roleInfo.roleId)
            {
                case RoleId.Pursuer when CustomOptionHolder.lawyerSpawnRate.getSelection() == 0
                                      && CustomOptionHolder.executionerSpawnRate.getSelection() == 0:
                case RoleId.Spy when roleData.impostors.Count <= 1:
                    continue;
            }

            if (Snitch.snitch != null && HandleGuesser.guesserCantGuessSnitch)
            {
                var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);
                var numberOfLeftTasks = playerTotal - playerCompleted;
                if (numberOfLeftTasks <= Snitch.taskCountForReveal && roleInfo.roleId == RoleId.Snitch) continue;
            }


            var buttonParent = new GameObject().transform;
            buttonParent.SetParent(container);
            var button = Object.Instantiate(buttonTemplate, buttonParent);
            var buttonMask = Object.Instantiate(maskTemplate, buttonParent);
            var label = Object.Instantiate(textTemplate, button);
            button.GetComponent<SpriteRenderer>().sprite =
                ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
            buttons.Add(button);
            int row = i / 5, col = i % 5;
            buttonParent.localPosition = new Vector3(-3.47f + (1.55f * col), 1.5f - (0.35f * row), -5);
            buttonParent.localScale = new Vector3(0.45f, 0.45f, 1f);
            label.text = cs(roleInfo.color, roleInfo.name);
            label.alignment = TextAlignmentOptions.Center;
            label.transform.localPosition = new Vector3(0, 0, label.transform.localPosition.z);
            label.transform.localScale *= 1.7f;
            var copiedIndex = i;

            button.GetComponent<PassiveButton>().OnClick.RemoveAllListeners();
            if (!CachedPlayer.LocalPlayer.Data.IsDead &&
                !playerById(__instance.playerStates[buttonTarget].TargetPlayerId).Data.IsDead)
                button.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() =>
                {
                    if (selectedButton != button)
                    {
                        selectedButton = button;
                        buttons.ForEach(x =>
                            x.GetComponent<SpriteRenderer>().color = x == selectedButton ? Color.red : Color.white);
                    }
                    else
                    {
                        var focusedTarget = playerById(__instance.playerStates[buttonTarget].TargetPlayerId);
                        if
                        (
                            __instance.state is not (MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted)
                            || focusedTarget == null
                            || (HandleGuesser.remainingShots(CachedPlayer.LocalPlayer.PlayerId) <= 0
                                && HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerId))
                            || (CachedPlayer.LocalPlayer.PlayerControl == Doomsayer.doomsayer && !Doomsayer.CanShoot))
                            return;

                        if (!HandleGuesser.killsThroughShield && focusedTarget == Medic.shielded)
                        {
                            // Depending on the options, shooting the shielded player will not allow the guess, notifiy everyone about the kill attempt and close the window
                            __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                            Object.Destroy(container.gameObject);

                            var murderAttemptWriter = AmongUsClient.Instance.StartRpcImmediately(
                                CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShieldedMurderAttempt,
                                SendOption.Reliable);
                            AmongUsClient.Instance.FinishRpcImmediately(murderAttemptWriter);
                            RPCProcedure.shieldedMurderAttempt(0);
                            SoundEffectsManager.play("fail");
                            return;
                        }

                        if (focusedTarget == Indomitable.indomitable)
                        {
                            showFlash(new Color32(255, 197, 97, byte.MinValue));
                            __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                            Object.Destroy(container.gameObject);

                            var murderAttemptWriter = AmongUsClient.Instance.StartRpcImmediately(
                                CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShieldedMurderAttempt,
                                SendOption.Reliable);
                            AmongUsClient.Instance.FinishRpcImmediately(murderAttemptWriter);
                            RPCProcedure.shieldedMurderAttempt(0);
                            SoundEffectsManager.play("fail");
                            return;
                        }

                        var dyingTarget = CachedPlayer.LocalPlayer.PlayerControl;

                        var mainRoleInfo = RoleInfo.getRoleInfoForPlayer(focusedTarget, true);

                        if (mainRoleInfo == null) return;

                        foreach (var role in mainRoleInfo)
                        {
                            if (role.roleId == roleInfo.roleId)
                            {
                                dyingTarget = focusedTarget;
                                continue;
                            }
                        }

                        if (dyingTarget == CachedPlayer.LocalPlayer.PlayerControl == Doomsayer.doomsayer)
                            Doomsayer.CanShoot = false;

                        // Shoot player and send chat info if activated
                        var writer = AmongUsClient.Instance.StartRpcImmediately(
                            CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.GuesserShoot,
                            SendOption.Reliable);
                        writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                        writer.Write(dyingTarget.PlayerId);
                        writer.Write(focusedTarget.PlayerId);
                        writer.Write((byte)roleInfo.roleId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                        RPCProcedure.guesserShoot(CachedPlayer.LocalPlayer.PlayerId, dyingTarget.PlayerId,
                            focusedTarget.PlayerId, (byte)roleInfo.roleId);

                        // Reset the GUI
                        __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                        Object.Destroy(container.gameObject);
                        if (CanMultipleShots(dyingTarget))
                            __instance.playerStates.ToList().ForEach(x =>
                            {
                                if (x.TargetPlayerId == dyingTarget.PlayerId && x.transform.FindChild("ShootButton") != null)
                                    Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
                            });
                        else
                            __instance.playerStates.ToList().ForEach(x =>
                            {
                                if (x.transform.FindChild("ShootButton") != null)
                                    Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
                            });
                    }
                }));

            i++;
        }

        container.transform.localScale *= 0.75f;
    }

    private static void populateButtonsPostfix(MeetingHud __instance)
    {
        // Add Swapper Buttons
        var addSwapperButtons = Swapper.swapper != null && CachedPlayer.LocalPlayer.PlayerControl == Swapper.swapper &&
                                !Swapper.swapper.Data.IsDead;
        var addMayorButton = Mayor.mayor != null && CachedPlayer.LocalPlayer.PlayerControl == Mayor.mayor &&
                             !Mayor.mayor.Data.IsDead && !Mayor.Revealed;
        if (addSwapperButtons)
        {
            selections = new bool[__instance.playerStates.Length];
            renderers = new SpriteRenderer[__instance.playerStates.Length];
            swapperButtonList = new PassiveButton[__instance.playerStates.Length];

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];
                if (playerVoteArea.AmDead || (playerVoteArea.TargetPlayerId == Swapper.swapper.PlayerId &&
                                              Swapper.canOnlySwapOthers)) continue;

                var template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                var checkbox = Object.Instantiate(template, playerVoteArea.transform, true);
                checkbox.transform.position = template.transform.position;
                checkbox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                if (HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerId))
                    checkbox.transform.localPosition = new Vector3(-0.5f, 0.03f, -1.3f);
                var renderer = checkbox.GetComponent<SpriteRenderer>();
                renderer.sprite = Swapper.getCheckSprite();
                renderer.color = Color.red;

                if (Swapper.charges <= 0) renderer.color = Color.gray;

                var button = checkbox.GetComponent<PassiveButton>();
                swapperButtonList[i] = button;
                button.OnClick.RemoveAllListeners();
                var copiedIndex = i;
                button.OnClick.AddListener((Action)(() => swapperOnClick(copiedIndex, __instance)));

                selections[i] = false;
                renderers[i] = renderer;
            }
        }

        // Add meeting extra button, i.e. Swapper Confirm Button or Mayor Toggle Double Vote Button. Swapper Button uses ExtraButtonText on the Left of the Button. (Future meeting buttons can easily be added here)
        if (addSwapperButtons || addMayorButton)
        {
            var meetingUI = Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");

            var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
            var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
            var textTemplate = __instance.playerStates[0].NameText;
            var meetingExtraButtonParent = new GameObject().transform;
            meetingExtraButtonParent.SetParent(meetingUI);
            var meetingExtraButton = Object.Instantiate(buttonTemplate, meetingExtraButtonParent);
            MeetingExtraButton = meetingExtraButton.gameObject;

            var meetingExtraButtonMask = Object.Instantiate(maskTemplate, meetingExtraButtonParent);
            meetingExtraButtonLabel = Object.Instantiate(textTemplate, meetingExtraButton);
            meetingExtraButton.GetComponent<SpriteRenderer>().sprite =
                ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;

            meetingExtraButtonParent.localPosition = new Vector3(0, -2.225f, -5);
            meetingExtraButtonParent.localScale = new Vector3(0.55f, 0.55f, 1f);
            meetingExtraButtonLabel.alignment = TextAlignmentOptions.Center;
            meetingExtraButtonLabel.transform.localPosition =
                new Vector3(0, 0, meetingExtraButtonLabel.transform.localPosition.z);
            if (addSwapperButtons)
            {
                meetingExtraButtonLabel.transform.localScale *= 1.7f;
                meetingExtraButtonLabel.text = cs(Color.red, "确认交换");
            }
            else
            {
                var localScale = meetingExtraButtonLabel.transform.localScale;
                localScale = new Vector3(
                    localScale.x * 1.5f,
                    localScale.x * 1.7f,
                    localScale.x * 1.7f);
                meetingExtraButtonLabel.transform.localScale = localScale;
                meetingExtraButtonLabel.text = cs(Mayor.color, "揭示");
            }

            var passiveButton = meetingExtraButton.GetComponent<PassiveButton>();
            passiveButton.OnClick.RemoveAllListeners();
            if (!CachedPlayer.LocalPlayer.Data.IsDead)
            {
                if (addSwapperButtons)
                    passiveButton.OnClick.AddListener((Action)(() => swapperConfirm(__instance)));
                else
                    passiveButton.OnClick.AddListener((Action)(() => mayorToggleVoteTwice(__instance)));
            }

            meetingExtraButton.parent.gameObject.SetActive(false);
            __instance.StartCoroutine(Effects.Lerp(7.27f, new Action<float>(p =>
            {
                // Button appears delayed, so that its visible in the voting screen only!
                if ((int)p == 1) meetingExtraButton.parent.gameObject.SetActive(true);
            })));
        }

        var isGuesser = HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerId);

        // Add overlay for spelled players
        if (Witch.witch != null && Witch.futureSpelled != null)
        {
            foreach (PlayerVoteArea pva in __instance.playerStates)
            {
                if (Witch.futureSpelled.Any(x => x.PlayerId == pva.TargetPlayerId))
                {
                    var local = CachedPlayer.LocalPlayer.PlayerControl;
                    var rend = new GameObject().AddComponent<SpriteRenderer>();
                    rend.transform.SetParent(pva.transform);
                    rend.gameObject.layer = pva.Megaphone.gameObject.layer;
                    rend.transform.localPosition = new Vector3(-0.5f, -0.03f, -1f);
                    if (local == Swapper.swapper && (isGuesser || local.PlayerId == Mimic.mimic.PlayerId))
                    {
                        rend.transform.localPosition = new Vector3(-0.725f, -0.15f, -1f);
                    }
                    rend.sprite = Witch.getSpelledOverlaySprite();
                }
            }
        }

        // Add Guesser Buttons
        var GuesserRemainingShots = HandleGuesser.remainingShots(CachedPlayer.LocalPlayer.PlayerId);
        if (!isGuesser || CachedPlayer.LocalPlayer.Data.IsDead || GuesserRemainingShots <= 0) return;
        {
            Doomsayer.CanShoot = true;
            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];

                if (playerVoteArea.AmDead || playerVoteArea.TargetPlayerId == CachedPlayer.LocalPlayer.PlayerId) continue;

                if (!Eraser.canEraseGuess && CachedPlayer.LocalPlayer != null && CachedPlayer.LocalPlayer.PlayerControl == Eraser.eraser
                    && Eraser.alreadyErased.Contains(playerVoteArea.TargetPlayerId)) continue;

                if (Mayor.mayor != null && Mayor.Revealed && playerVoteArea.TargetPlayerId == Mayor.mayor.PlayerId) continue;

                if (CachedPlayer.LocalPlayer != null && CachedPlayer.LocalPlayer.PlayerControl == Specoality.specoality
                    && Specoality.canNoGuess != null && Specoality.canNoGuess.PlayerId == playerVoteArea.TargetPlayerId) continue;

                var template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                var targetBox = Object.Instantiate(template, playerVoteArea.transform);
                targetBox.name = "ShootButton";
                targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                var renderer = targetBox.GetComponent<SpriteRenderer>();
                renderer.sprite = HandleGuesser.getTargetSprite();
                var button = targetBox.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                var copiedIndex = i;
                button.OnClick.AddListener((Action)(() => guesserOnClick(copiedIndex, __instance)));
            }
        }
    }

    public static void updateMeetingText(MeetingHud __instance)
    {
        if (PlayerControl.LocalPlayer.Data.IsDead) return;

        if (MeetingHud.Instance.state is
            not MeetingHud.VoteStates.Voted and
            not MeetingHud.VoteStates.NotVoted and
            not MeetingHud.VoteStates.Discussion) return;
        var meetingInfoText = "";
        int numGuesses = HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerControl.PlayerId)
            && CachedPlayer.LocalPlayer.PlayerControl != Doomsayer.doomsayer
            ? HandleGuesser.remainingShots(CachedPlayer.LocalPlayer.PlayerControl.PlayerId) : 0;
        var local = CachedPlayer.LocalPlayer.PlayerControl;
        if (numGuesses > 0)
        {
            meetingInfoText = string.Format(getString("guesserGuessesLeft"), numGuesses);
        }

        if (local == Akujo.akujo && Akujo.timeLeft > 0)
            meetingInfoText = string.Format(getString("akujoTimeRemaining"), $"{TimeSpan.FromSeconds(Akujo.timeLeft):mm\\:ss}");

        if (local == Doomsayer.doomsayer)
            meetingInfoText = string.Format(getString("DoomsayerKilledToWin"), Doomsayer.killToWin - Doomsayer.killedToWin);

        if (local == Swapper.swapper)
            meetingInfoText = $"换票次数: {Swapper.charges}";

        if (meetingInfoText == "") return;

        __instance.TimerText.text = $"{meetingInfoText}\n{__instance.TimerText.text}";
    }

    [HarmonyPatch]
    public class ShowHost
    {
        private static TextMeshPro Text;
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        [HarmonyPostfix]

        public static void Setup(MeetingHud __instance)
        {
            if (AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame) return;

            __instance.ProceedButton.gameObject.transform.localPosition = new(-2.5f, 2.2f, 0);
            __instance.ProceedButton.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            __instance.ProceedButton.GetComponent<PassiveButton>().enabled = false;
            __instance.HostIcon.gameObject.SetActive(true);
            __instance.ProceedButton.gameObject.SetActive(true);
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        [HarmonyPostfix]

        public static void Postfix(MeetingHud __instance)
        {
            var host = GameData.Instance.GetHost();

            if (host != null)
            {
                PlayerMaterial.SetColors(host.DefaultOutfit.ColorId, __instance.HostIcon);
                if (Text == null) Text = __instance.ProceedButton.gameObject.GetComponentInChildren<TextMeshPro>();
                Text.text = $"{"Host".Translate()}: {host.PlayerName}";
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CheckForEndVoting))]
    private class MeetingCalculateVotesPatch
    {
        public static bool CheckVoted(PlayerVoteArea playerVoteArea)
        {
            if (playerVoteArea.AmDead || playerVoteArea.DidVote)
                return false;

            var playerInfo = GameData.Instance.GetPlayerById(playerVoteArea.TargetPlayerId);
            if (playerInfo == null)
                return false;

            return true;
        }
        private static Dictionary<byte, int> CalculateVotes(MeetingHud __instance)
        {
            var dictionary = new Dictionary<byte, int>();

            foreach (var playerVoteArea in __instance.playerStates)
            {
                if (playerVoteArea.VotedFor is 252 or 255 or 254) continue;
                var player = playerById(playerVoteArea.TargetPlayerId);
                if (player == null || player.Data == null || player.Data.IsDead ||
                    player.Data.Disconnected) continue;

                if (InfoSleuth.infoSleuth != null && playerVoteArea.TargetPlayerId == InfoSleuth.infoSleuth.PlayerId)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                        (byte)CustomRPC.InfoSleuthTarget, SendOption.Reliable);
                    writer.Write(playerVoteArea.VotedFor);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.infoSleuthTarget(playerVoteArea.VotedFor);
                }

                var additionalVotes = 1;
                if (Prosecutor.prosecutor != null && Prosecutor.prosecutor.PlayerId == playerVoteArea.TargetPlayerId)
                    additionalVotes = Prosecutor.ProsecuteThisMeeting ? 15 : 1;

                if (Mayor.mayor != null && Mayor.mayor.PlayerId == playerVoteArea.TargetPlayerId)
                    additionalVotes = Mayor.Revealed ? 3 : 1;

                if (Prosecutor.prosecutor != null && Prosecutor.ProsecuteThisMeeting && Prosecutor.prosecutor.PlayerId != playerVoteArea.TargetPlayerId)
                    additionalVotes = 0;

                if (dictionary.TryGetValue(playerVoteArea.VotedFor, out var currentVotes))
                    dictionary[playerVoteArea.VotedFor] = currentVotes + additionalVotes;
                else
                    dictionary[playerVoteArea.VotedFor] = additionalVotes;
            }

            // Swapper swap votes
            if (Swapper.swapper == null || Swapper.swapper.Data.IsDead) return dictionary;
            {
                swapped1 = null;
                swapped2 = null;
                foreach (var playerVoteArea in __instance.playerStates)
                {
                    if (playerVoteArea.TargetPlayerId == Swapper.playerId1) swapped1 = playerVoteArea;
                    if (playerVoteArea.TargetPlayerId == Swapper.playerId2) swapped2 = playerVoteArea;
                }

                if (swapped1 == null || swapped2 == null) return dictionary;

                dictionary.TryAdd(swapped1.TargetPlayerId, 0);
                dictionary.TryAdd(swapped2.TargetPlayerId, 0);

                (dictionary[swapped1.TargetPlayerId], dictionary[swapped2.TargetPlayerId]) = (
                    dictionary[swapped2.TargetPlayerId], dictionary[swapped1.TargetPlayerId]);
            }
            return dictionary;
        }


        private static bool Prefix(MeetingHud __instance)
        {
            if (!__instance.playerStates.All(ps => ps.AmDead || ps.DidVote)) return false;
            // If skipping is disabled, replace skipps/no-votes with self vote
            if (target == null && blockSkippingInEmergencyMeetings && noVoteIsSelfVote)
                foreach (var playerVoteArea in __instance.playerStates)
                    if (playerVoteArea.VotedFor == byte.MaxValue - 1)
                        playerVoteArea.VotedFor = playerVoteArea.TargetPlayerId; // TargetPlayerId

            var self = CalculateVotes(__instance);
            var max = self.MaxPair(out var tie);
            var exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault(v => !tie && v.PlayerId == max.Key && !v.IsDead);

            // TieBreaker 
            var potentialExiled = new List<NetworkedPlayerInfo>();
            var skipIsTie = false;
            if (self.Count > 0 && !Prosecutor.ProsecuteThisMeeting) // 阻止破平者在检察官会议中生效
            {
                Tiebreaker.isTiebreak = false;
                var maxVoteValue = self.Values.Max();
                PlayerVoteArea tb = null;
                if (Tiebreaker.tiebreaker != null)
                    tb = __instance.playerStates.ToArray().FirstOrDefault(x => x.TargetPlayerId == Tiebreaker.tiebreaker.PlayerId);

                var isTiebreakerSkip = tb == null || tb.VotedFor == 253 || (tb != null && tb.AmDead);

                foreach (var pair in self.Where(pair => pair.Value == maxVoteValue && !isTiebreakerSkip))
                {
                    if (pair.Key != 253)
                        potentialExiled.Add(GameData.Instance.AllPlayers.ToArray().FirstOrDefault(x => x.PlayerId == pair.Key));
                    else
                        skipIsTie = true;
                }
            }

            var array = new MeetingHud.VoterState[__instance.playerStates.Length];
            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];
                array[i] = new MeetingHud.VoterState
                {
                    VoterId = playerVoteArea.TargetPlayerId,
                    VotedForId = playerVoteArea.VotedFor
                };

                if (Tiebreaker.tiebreaker == null ||
                    playerVoteArea.TargetPlayerId != Tiebreaker.tiebreaker.PlayerId) continue;

                var tiebreakerVote = playerVoteArea.VotedFor;
                if (swapped1 != null && swapped2 != null)
                {
                    if (tiebreakerVote == swapped1.TargetPlayerId) tiebreakerVote = swapped2.TargetPlayerId;
                    else if (tiebreakerVote == swapped2.TargetPlayerId) tiebreakerVote = swapped1.TargetPlayerId;
                }

                if (potentialExiled.FindAll(x => x != null && x.PlayerId == tiebreakerVote).Count <= 0 ||
                    (potentialExiled.Count <= 1 && !skipIsTie)) continue;
                exiled = potentialExiled.ToArray().FirstOrDefault(v => v.PlayerId == tiebreakerVote);
                tie = false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetTiebreak,
                    SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.setTiebreak();
            }

            // RPCVotingComplete
            __instance.RpcVotingComplete(array, exiled, tie);
            return false;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
    public static class VotingComplete
    {
        public static void Postfix(MeetingHud __instance,
            [HarmonyArgument(0)] Il2CppStructArray<MeetingHud.VoterState> states,
            [HarmonyArgument(1)] NetworkedPlayerInfo exiled,
            [HarmonyArgument(2)] bool tie)
        {
            __instance.exiledPlayer = __instance.wasTie ? null : __instance.exiledPlayer;
            var exiledString = exiled == null ? "null" : exiled.PlayerName;
            Message($"被驱逐玩家: {exiledString}");
            Message($"是否平票: {tie}");
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
    private class MeetingHudBloopAVoteIconPatch
    {
        public static bool Prefix(MeetingHud __instance, NetworkedPlayerInfo voterPlayer, int index, Transform parent)
        {
            var spriteRenderer = Object.Instantiate(__instance.PlayerVotePrefab);
            var showVoteColors = !GameManager.Instance.LogicOptions.GetAnonymousVotes() ||
                                 (CachedPlayer.LocalPlayer.Data.IsDead && ghostsSeeVotes) ||
                                 (Prosecutor.prosecutor != null && Prosecutor.prosecutor == CachedPlayer.LocalPlayer.PlayerControl &&
                                  Prosecutor.canSeeVoteColors &&
                                  TasksHandler.taskInfo(CachedPlayer.LocalPlayer.Data).Item1 >=
                                  Prosecutor.tasksNeededToSeeVoteColors) ||
                                 (Watcher.watcher != null && CachedPlayer.LocalPlayer.PlayerControl == Watcher.watcher);
            if (showVoteColors && !Prosecutor.ProsecuteThisMeeting)
                PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
            else
                PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);

            var transform = spriteRenderer.transform;
            transform.SetParent(parent);
            transform.localScale = Vector3.zero;
            var component = parent.GetComponent<PlayerVoteArea>();
            if (component != null) spriteRenderer.material.SetInt(PlayerMaterial.MaskLayer, component.MaskLayer);

            __instance.StartCoroutine(Effects.Bloop(index * 0.3f, transform));
            parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
            return false;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateResults))]
    private class MeetingHudPopulateVotesPatch
    {
        private static bool Prefix(MeetingHud __instance, Il2CppStructArray<MeetingHud.VoterState> states)
        {
            // Swapper swap
            PlayerVoteArea swapped1 = null;
            PlayerVoteArea swapped2 = null;
            foreach (var playerVoteArea in __instance.playerStates)
            {
                if (playerVoteArea.TargetPlayerId == Swapper.playerId1) swapped1 = playerVoteArea;
                if (playerVoteArea.TargetPlayerId == Swapper.playerId2) swapped2 = playerVoteArea;
            }

            var doSwap = swapped1 != null && swapped2 != null && Swapper.swapper != null &&
                         !Swapper.swapper.Data.IsDead;
            if (doSwap)
            {
                var localPosition = swapped1.transform.localPosition;
                __instance.StartCoroutine(Effects.Slide3D(swapped1.transform, localPosition,
                    swapped2.transform.localPosition, 1.5f));
                __instance.StartCoroutine(Effects.Slide3D(swapped2.transform, swapped2.transform.localPosition,
                    localPosition, 1.5f));
            }


            __instance.TitleText.text = FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.MeetingVotingResults, new Il2CppReferenceArray<Il2CppSystem.Object>(0));

            var allNums = new Dictionary<int, int>();
            __instance.TitleText.text = Object.FindObjectOfType<TranslationController>().GetString(StringNames.MeetingVotingResults, []);

            var amountOfSkippedVoters = 0;
            var num = 0;
            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];
                var targetPlayerId = playerVoteArea.TargetPlayerId;
                allNums.Add(i, 0);

                playerVoteArea = doSwap switch
                {
                    // Swapper change playerVoteArea that gets the votes
                    true when playerVoteArea.TargetPlayerId == swapped1.TargetPlayerId => swapped2,
                    true when playerVoteArea.TargetPlayerId == swapped2.TargetPlayerId => swapped1,
                    _ => playerVoteArea
                };

                playerVoteArea.ClearForResults();
                var num2 = 0;
                var mayorVotesDisplayed = 0;
                for (var j = 0; j < states.Length; j++)
                {
                    if (Prosecutor.ProsecuteThisMeeting) continue;
                    var voterState = states[j];
                    var playerById = GameData.Instance.GetPlayerById(voterState.VoterId);
                    if (playerById == null)
                    {
                        Warn($"找不到投票者的玩家信息: {voterState.VoterId}");
                    }
                    else if (i == 0 && voterState.SkippedVote && !playerById.IsDead)
                    {
                        __instance.BloopAVoteIcon(playerById, num, __instance.SkippedVoting.transform);
                        num++;
                    }
                    else if (voterState.VotedForId == targetPlayerId && !playerById.IsDead)
                    {
                        __instance.BloopAVoteIcon(playerById, num2, playerVoteArea.transform);
                        num2++;
                    }

                    // Mayor vote, redo this iteration to place a second vote
                    if (Mayor.mayor == null || voterState.VoterId != (sbyte)Mayor.mayor.PlayerId
                        || mayorVotesDisplayed >= 2 || !Mayor.Revealed)
                    {
                        mayorVotesDisplayed = 0;
                        continue;
                    };
                    mayorVotesDisplayed++;
                    j--;
                }

                for (var stateIdx = 0; stateIdx < states.Length; stateIdx++)
                {
                    var voteState = states[stateIdx];
                    var playerInfo = GameData.Instance.GetPlayerById(voteState.VoterId);
                    if (Prosecutor.prosecutor == null) continue;
                    if (Prosecutor.prosecutor.Data.IsDead || Prosecutor.prosecutor.Data.Disconnected) continue;
                    if (Prosecutor.ProsecuteThisMeeting)
                    {
                        if (voteState.VoterId == Prosecutor.prosecutor.PlayerId)
                        {
                            if (playerInfo == null)
                            {
                                Error(string.Format("找不到投票者的玩家信息: {0}", voteState.VoterId));
                                Prosecutor.Prosecuted = true;
                            }
                            else if (i == 0 && voteState.SkippedVote)
                            {
                                __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                amountOfSkippedVoters += 6;
                                Prosecutor.Prosecuted = true;
                            }
                            else if (voteState.VotedForId == playerVoteArea.TargetPlayerId)
                            {
                                __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                allNums[i] += 6;
                                Prosecutor.Prosecuted = true;
                            }
                        }
                    }
                }

            }

            return false;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
    private class MeetingHudVotingCompletedPatch
    {
        private static void Postfix(MeetingHud __instance, [HarmonyArgument(0)] byte[] states,
            [HarmonyArgument(1)] NetworkedPlayerInfo exiled, [HarmonyArgument(2)] bool tie)
        {
            // Reset swapper values
            Swapper.playerId1 = byte.MaxValue;
            Swapper.playerId2 = byte.MaxValue;

            // Lovers, Lawyer & Pursuer save next to be exiled, because RPC of ending game comes before RPC of exiled
            Lovers.notAckedExiledIsLover = false;
            Lawyer.notAckedExiled = false;
            if (exiled != null)
            {
                Lovers.notAckedExiledIsLover = (Lovers.lover1 != null && Lovers.lover1.PlayerId == exiled.PlayerId) ||
                                               (Lovers.lover2 != null && Lovers.lover2.PlayerId == exiled.PlayerId);
                Lawyer.notAckedExiled = (Pursuer.pursuer != null && Pursuer.pursuer.Any(id => id.PlayerId == exiled.PlayerId)) ||
                                         (Lawyer.lawyer != null && Lawyer.target != null &&
                                          Lawyer.target.PlayerId == exiled.PlayerId && Lawyer.target != Jester.jester);
            }

            Camouflager.camoComms = false;

            // Mini
            if (!Mini.isGrowingUpInMeeting)
                Mini.timeOfGrowthStart = Mini.timeOfGrowthStart.Add(DateTime.UtcNow.Subtract(Mini.timeOfMeetingStart)).AddSeconds(10);

            // Snitch
            if (Snitch.snitch != null && !Snitch.needsUpdate && Snitch.snitch.Data.IsDead && Snitch.text != null) Object.Destroy(Snitch.text);

        }
    }

    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.Select))]
    private class PlayerVoteAreaSelectPatch
    {
        private static bool Prefix(MeetingHud __instance)
        {
            return !(CachedPlayer.LocalPlayer != null && HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerId) &&
                     guesserUI != null);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ServerStart))]
    private class MeetingServerStartPatch
    {
        private static void Postfix(MeetingHud __instance)
        {
            populateButtonsPostfix(__instance);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Deserialize))]
    private class MeetingDeserializePatch
    {
        private static void Postfix(MeetingHud __instance, [HarmonyArgument(0)] MessageReader reader,
            [HarmonyArgument(1)] bool initialState)
        {
            // Add swapper buttons
            if (initialState) populateButtonsPostfix(__instance);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    private class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo meetingTarget)
        {
            var roomTracker = FastDestroyableSingleton<HudManager>.Instance.roomTracker;
            var roomId = byte.MinValue;
            if (roomTracker != null && roomTracker.LastRoom != null) roomId = (byte)roomTracker.LastRoom.RoomId;

            // Resett Bait list
            Bait.active = new Dictionary<DeadPlayer, float>();
            // Save AntiTeleport position, if the player is able to move (i.e. not on a ladder or a gap thingy)
            if (CachedPlayer.LocalPlayer.PlayerPhysics.enabled && (CachedPlayer.LocalPlayer.PlayerControl.moveable
                                                                   || CachedPlayer.LocalPlayer.PlayerControl.inVent
                                                                   || HudManagerStartPatch.hackerVitalsButton.isEffectActive
                                                                   || HudManagerStartPatch.hackerAdminTableButton.isEffectActive
                                                                   || HudManagerStartPatch.securityGuardCamButton.isEffectActive
                                                                   || (Portal.isTeleporting &&
                                                                       Portal.teleportedPlayers.Last().playerId ==
                                                                       CachedPlayer.LocalPlayer.PlayerId)))
                if (!CachedPlayer.LocalPlayer.PlayerControl.inMovingPlat)
                    AntiTeleport.position = CachedPlayer.LocalPlayer.transform.position;

            // Medium meeting start time
            Medium.meetingStartTime = DateTime.UtcNow;
            // Mini
            Mini.timeOfMeetingStart = DateTime.UtcNow;
            Mini.ageOnMeetingStart = Mathf.FloorToInt(Mini.growingProgress() * 18);
            // Count meetings
            if (meetingTarget == null) meetingsCount++;
            // Reset vampire bitten
            Vampire.bitten = null;
            // Count meetings
            if (meetingTarget == null) meetingsCount++;
            // Save the meeting target
            target = meetingTarget;
            isRoundOne = false;

            // Blackmail target
            if (Blackmailer.blackmailed != null && Blackmailer.blackmailed == CachedPlayer.LocalPlayer.PlayerControl)
            {
                Coroutines.Start(BlackmailShhh());
            }

            // Add Portal info into Portalmaker Chat:
            if (Portalmaker.portalmaker != null &&
                (CachedPlayer.LocalPlayer.PlayerControl == Portalmaker.portalmaker || shouldShowGhostInfo()) &&
                !Portalmaker.portalmaker.Data.IsDead)
                if (Portal.teleportedPlayers.Count > 0)
                {
                    var msg = "星门使用日志:\n";
                    foreach (var entry in Portal.teleportedPlayers)
                    {
                        var timeBeforeMeeting = (float)(DateTime.UtcNow - entry.time).TotalMilliseconds / 1000;
                        msg += Portalmaker.logShowsTime ? $"{(int)timeBeforeMeeting} 秒前: " : "";
                        msg = msg + $"{entry.name} 使用了星门\n";
                    }

                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Portalmaker.portalmaker, $"{msg}");
                }

            // Add trapped Info into Trapper chat
            if (Trapper.trapper != null &&
                (CachedPlayer.LocalPlayer.PlayerControl == Trapper.trapper || shouldShowGhostInfo()) &&
                !Trapper.trapper.Data.IsDead)
            {
                if (Trap.traps.Any(x => x.revealed))
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Trapper.trapper, "陷阱日志:");
                foreach (var trap in Trap.traps)
                {
                    if (!trap.revealed) continue;
                    var message = $"陷阱 {trap.instanceId}: \n";
                    trap.trappedPlayer = trap.trappedPlayer.OrderBy(x => rnd.Next()).ToList();
                    message = trap.trappedPlayer.Aggregate(message, (current, p) => current + Trapper.infoType switch
                    {
                        0 => RoleInfo.GetRolesString(p, false, false, true) + "\n",
                        1 when isEvil(p) || p.Data.Role.IsImpostor => "邪恶职业 \n",
                        1 => "善良职业 \n",
                        _ => p.Data.PlayerName + "\n"
                    });

                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Trapper.trapper, $"{message}");
                }
            }

            Trapper.playersOnMap = new List<PlayerControl>();

            // Remove revealed traps
            Trap.clearRevealedTraps();

            Terrorist.clearBomb();

            // Reset zoomed out ghosts
            toggleZoom(true);

            // Stop all playing sounds
            SoundEffectsManager.stopAll();

            // Close In-Game Settings Display if open
            HudManagerUpdate.CloseSettings();

        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    private class MeetingHudUpdatePatch
    {
        public static Sprite Overlay => Blackmailer.getBlackmailOverlaySprite();

        private static void Postfix(MeetingHud __instance)
        {
            // Deactivate skip Button if skipping on emergency meetings is disabled
            if (target == null && blockSkippingInEmergencyMeetings)
                __instance.SkipVoteButton.gameObject.SetActive(false);

            if (__instance.state >= MeetingHud.VoteStates.Discussion)
                // Remove first kill shield
                firstKillPlayer = null;

            updateMeetingText(__instance);

            if (Blackmailer.blackmailer != null && Blackmailer.blackmailed != null)
            {
                // Blackmailer show overlay
                var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == Blackmailer.blackmailed.PlayerId);
                playerState.Overlay.gameObject.SetActive(true);
                playerState.Overlay.sprite = Overlay;
                if (__instance.state != MeetingHud.VoteStates.Animating && !Blackmailer.alreadyShook)
                {
                    Blackmailer.alreadyShook = true;
                    __instance.StartCoroutine(Effects.SwayX(playerState.transform));
                }
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    private class MeetingChatNotification
    {
        private static void Postfix(MeetingHud __instance)
        {
            var chat = FastDestroyableSingleton<HudManager>.Instance.Chat;
            var playerControl = CachedPlayer.LocalPlayer.PlayerControl;
            var num = (int)chat.timeSinceLastMessage;
            foreach (var allPlayer in CachedPlayer.AllPlayers)
            {
                PlayerControl playerControl2 = allPlayer;
                if (playerControl2 != playerControl || playerControl2.Data.IsDead || num != 0) continue;
                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetMeetingChatOverlay,
                    SendOption.Reliable);
                writer.Write(playerControl2.PlayerId);
                writer.Write(playerControl.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.setChatNotificationOverlay(playerControl.PlayerId, playerControl2.PlayerId);
                break;
            }
        }
    }

    [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.SetText))]
    public class BlockChatBlackmailed
    {
        public static bool Prefix(QuickChatMenu __instance)
        {
            if (Blackmailer.blackmailer != null && Blackmailer.blackmailed != null && Blackmailer.blackmailed == CachedPlayer.LocalPlayer.PlayerControl)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingHudStart
    {
        public static Sprite Letter => Blackmailer.getBlackmailOverlaySprite();

        public static void Postfix(MeetingHud __instance)
        {
            Message("会议开始");
            if (isNeutral(PlayerControl.LocalPlayer)) Message("PlayerIsNeutral");
            shookAlready = false;
            if (Blackmailer.blackmailed != null
                && Blackmailer.blackmailed.Data.PlayerId == CachedPlayer.LocalPlayer.PlayerId
                && !Blackmailer.blackmailed.Data.IsDead)
            {
                //Nothing here for now. What to do when local player who is blackmailed starts meeting
                //Coroutines.Start(BlackmailShhh());
                Coroutines.Start(BlackmailShhh());
            }

            if (InfoSleuth.infoSleuth != null && InfoSleuth.target != null && InfoSleuth.infoSleuth == CachedPlayer.LocalPlayer.PlayerControl)
            {
                string msg;
                var random = rnd.Next(2);
                var isNotCrew = isNeutral(InfoSleuth.target) || InfoSleuth.target.Data.Role.IsImpostor;
                var team = "的阵营是 " + teamString(InfoSleuth.target);
                var info = InfoSleuth.infoType switch
                {
                    0 => isNotCrew ? "不是船员" : "是船员",
                    1 => team,
                    _ => random == 0 ? isNotCrew ? "不是船员" : "是船员" : team,
                };

                msg = $"{InfoSleuth.target.Data.PlayerName} {info}";

                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(CachedPlayer.LocalPlayer.PlayerControl, $"{msg}");
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
                writer.Write(InfoSleuth.target.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.MediumInfo);
                writer.Write(msg);
                AmongUsClient.Instance.FinishRpcImmediately(writer);


                var writer1 = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.InfoSleuthNoTarget, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer1);
                RPCProcedure.infoSleuthNoTarget();
            }

        }
    }
}