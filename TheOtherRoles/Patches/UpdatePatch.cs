using AmongUs.GameOptions;
using Rewired;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
internal class HudManagerUpdatePatch
{
    private static readonly Dictionary<byte, (string name, Color color)> TagColorDict = new();

    public static void UpdatePlayerInfo()
    {
        if (!InGame) return;
        var local = PlayerControl.LocalPlayer;
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p?.Data == null || p == local) continue;
            var playerVoteArea = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == p.PlayerId);
            if (playerVoteArea != null && playerVoteArea.ColorBlindName.gameObject.active)
            {
                playerVoteArea.ColorBlindName.transform.localPosition = new Vector3(-0.93f, -0.2f, -0.1f);
                playerVoteArea.ColorBlindName.fontSize *= 1.75f;
            }
            p.cosmetics.nameText.transform.parent.SetLocalZ(-0.0001f);

            bool teamSeeRoles = CustomRoleManager.AllRoles.OfType<RoleBase>().Any(x => x.SeeRoleName(local, p));

            if (p == local || local.Data.IsDead || teamSeeRoles)
            {
                var mainRole = RoleInfo.GetRolesString(p, true, false, false, false);
                var allRoleText = RoleInfo.GetRolesString(p, true, true, true, true);
                if (p.IsDead() && CanSeeGhostInfo) allRoleText += $" - {PlayerData.GetDeathReasonString(p)}";

                var playerInfoTransform = p.cosmetics.nameText.transform.parent.FindChild("Info");
                var playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TextMeshPro>() : null;
                if (playerInfo == null)
                {
                    playerInfo = UObject.Instantiate(p.cosmetics.nameText, p.cosmetics.nameText.transform.parent);
                    playerInfo.transform.localPosition += Vector3.up * 0.225f;
                    playerInfo.fontSize *= 0.8f;
                    playerInfo.gameObject.name = "Info";
                    playerInfo.color = playerInfo.color.SetAlpha(1f);
                }

                var meetingInfoTransform = playerVoteArea != null ? playerVoteArea.NameText.transform.parent.FindChild("Info") : null;
                var meetingInfo = meetingInfoTransform != null ? meetingInfoTransform.GetComponent<TextMeshPro>() : null;

                if (meetingInfo == null && playerVoteArea != null)
                {
                    meetingInfo = UObject.Instantiate(playerVoteArea.NameText, playerVoteArea.NameText.transform.parent);
                    meetingInfo.transform.localPosition += Vector3.down * 0.2f;
                    meetingInfo.fontSize *= 0.64f;
                    meetingInfo.gameObject.name = "Info";
                }

                // Set player name higher to align in middle
                if (meetingInfo != null && playerVoteArea != null)
                {
                    var playerName = playerVoteArea.NameText;
                    playerName.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
                }

                var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p.Data);
                var taskInfo = tasksTotal > 0 ? $"<color=#FAD934FF>({tasksCompleted}/{tasksTotal})</color>" : "";

                var playerInfoText = "";
                var meetingInfoText = "";
                if (p == local)
                {
                    if (p.Data.IsDead) mainRole = allRoleText;
                    playerInfoText = $"{mainRole}";
                    if (HudManager.Instance.TaskPanel != null)
                    {
                        var tabText = HudManager.Instance.TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TextMeshPro>();
                        tabText.SetText(string.Format("tasksNum".Translate(), taskInfo));
                    }
                    meetingInfoText = $"{allRoleText} {taskInfo}".Trim();
                }
                else if (teamSeeRoles && local.IsAlive())
                {
                    meetingInfoText = playerInfoText = mainRole;
                }
                else
                {
                    if (CanSeeGhostInfo)
                    {
                        playerInfoText = $"{allRoleText} {taskInfo}".Trim();
                        meetingInfoText = playerInfoText;
                    }
                }

                playerInfo.text = playerInfoText;
                playerInfo.gameObject.SetActive(p.Visible);
                if (meetingInfo != null)
                {
                    meetingInfo.text = MeetingHud.Instance.state == MeetingHud.VoteStates.Results ? "" : meetingInfoText;
                }
            }
            else
            {
                if (local != p && p != null)
                {
                    var playerInfoTransform = p.cosmetics.nameText.transform.parent.FindChild("Info");
                    var playerInfo = playerInfoTransform?.GetComponent<TextMeshPro>();
                    if (playerInfo != null) playerInfo.text = "";
                }
            }
        }
    }

    private static void SetNameColors()
    {
        if (!InGame) return;

        var local = PlayerControl.LocalPlayer;
        if (local == null) return;

        var localRole = local.GetRoleInfo();
        setPlayerNameColor(local, localRole.Color);

        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p?.Data == null || p == local) continue;

            bool canSeeColor = false;
            Color roleColor = Color.white;

            foreach (var role in CustomRoleManager.AllRoles.OfType<RoleBase>())
            {
                if (role.SeeRoleColor(local, p, out var tempColor))
                {
                    canSeeColor = true;
                    roleColor = tempColor;
                    break;
                }
            }
            if (canSeeColor)
                setPlayerNameColor(p, roleColor);
        }
    }

    private static void resetNameTagsAndColors()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        var myData = PlayerControl.LocalPlayer.Data;
        var amImpostor = myData.Role.IsImpostor;
        //var morphTimerNotUp = Morphling.morphTimer > 0f;
        //var morphTargetNotNull = Morphling.morphTarget != null;

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
                //if (morphTimerNotUp && morphTargetNotNull && Morphling.morphling == player)
                //playerName = Morphling.morphTarget.Data.PlayerName;

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

    private static void setNameTags()
    {
        var local = PlayerControl.LocalPlayer;

        foreach (var medic in Medic.AllMedic)
        {
            // Add medic shield info:
            if (MeetingHud.Instance != null && medic.shielded != null && medic.shieldVisible(medic.shielded))
            {
                foreach (PlayerVoteArea player in MeetingHud.Instance?.playerStates)
                {
                    if (player.TargetPlayerId == medic.shielded.PlayerId)
                    {
                        player.NameText.text = Cs(Medic.color, "[") + player.NameText.text + Cs(Medic.color, "]");
                        // player.HighlightedFX.color = Medic.color;
                        // player.HighlightedFX.enabled = true;
                    }
                }
            }
        }

        foreach (var target in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (target?.Data == null) continue;

            bool canSeeColor = false;
            string roleTag = string.Empty;

            foreach (var role in CustomRoleManager.AllRoles.OfType<RoleBase>())
            {
                if (role.SeeRoleTag(local, target, out var tempTag))
                {
                    canSeeColor = true;
                    roleTag = tempTag;
                    break;
                }
            }
            if (canSeeColor)
            {
                target.cosmetics.nameText.text += roleTag;
                if (MeetingHud.Instance != null)
                {
                    foreach (var pva in MeetingHud.Instance?.playerStates)
                    {
                        if (pva.TargetPlayerId == target.PlayerId)
                            pva.NameText.text += Cs(Akujo.color, " ♥");
                    }
                }
            }
        }

        // Display lighter / darker color for all alive players
        if (PlayerControl.LocalPlayer != null && InMeeting && ModOption.showLighterDarker)
        {
            foreach (var pva in MeetingHud.Instance.playerStates)
            {
                var target = PlayerById(pva.TargetPlayerId);
                if (target != null) pva.NameText.text += $" ({(IsLightColor(target) ? "浅" : "深")})";
            }
        }

    }

    private static void setBasePlayerOutlines()
    {
        if (isActiveCamoComms || MushroomSabotageActive || InMeeting) return;
        var local = PlayerControl.LocalPlayer;
        foreach (PlayerControl target in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) continue;

            var isMorphedMorphling = target.TryGetRole<Morphling>(out var morphling) && morphling.morphTarget != null && morphling.morphTimer > 0f;
            var hasVisibleShield = false;
            var color = Medic.shieldedColor;

            if (target == null || target == local) continue;

            foreach (var role in CustomRoleManager.AllActiveRoles.Values)
            {
                if (role.SetBasePlayerOutlines(local, target, out var tempColor))
                {
                    color = tempColor;
                    break;
                }
            }

            foreach (var medic in Medic.AllMedic)
            {
                if (medic == null || medic.shielded == null) continue;
                if (target == medic.shielded)
                {
                    hasVisibleShield = true;
                    color = Medic.color;
                    break;
                }
                hasVisibleShield = true;
                color = Medic.color;
                break;
            }

            if (ModOption.firstKillPlayer != null && ModOption.shieldFirstKill
                && ((target == ModOption.firstKillPlayer && !isMorphedMorphling)
                || (isMorphedMorphling && morphling.sampledTarget == ModOption.firstKillPlayer)))
            {
                hasVisibleShield = true;
                color = Color.blue;
            }

            if (hasVisibleShield)
            {
                target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
                target.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", color);
            }
            else
            {
                target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
            }
        }
    }

    private static void timerUpdate()
    {
        var dt = Time.deltaTime;
        Trickster.lightsOutTimer -= dt;
        Tracker.corpsesTrackingTimer -= dt;
        foreach (var key in CustomRoleManager.handcuffedKnows.Keys)
            CustomRoleManager.handcuffedKnows[key] -= dt;
    }

    private static void updateImpostorKillButton(HudManager __instance)
    {
        if (!PlayerControl.LocalPlayer.IsImpostor()) return;
        if (MeetingHud.Instance)
        {
            __instance.KillButton.Hide();
            return;
        }

        var enabled = true;
        if (PlayerControl.LocalPlayer.Is(RoleId.Vampire))
            enabled = false;

        if (enabled) __instance.KillButton.Show();
        else __instance.KillButton.Hide();

        if (CustomRoleManager.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) &&
        CustomRoleManager.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] > 0) __instance.KillButton.Hide();
    }

    private static void updateReportButton(HudManager __instance)
    {
        if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
        if ((CustomRoleManager.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) &&
         CustomRoleManager.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] > 0) ||
        MeetingHud.Instance) __instance.ReportButton.Hide();
        else if (!__instance.ReportButton.isActiveAndEnabled) __instance.ReportButton.Show();
    }

    private static void updateVentButton(HudManager __instance)
    {
        if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
        if ((CustomRoleManager.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) &&
         CustomRoleManager.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] > 0) ||
        MeetingHud.Instance) __instance.ImpostorVentButton.Hide();
        else if (PlayerControl.LocalPlayer.CanUseVents() && !__instance.ImpostorVentButton.isActiveAndEnabled)
        {
            __instance.ImpostorVentButton.Show();

        }
        if (ReInput.players.GetPlayer(0).GetButtonDown(RewiredConsts.Action.UseVent) &&
        !PlayerControl.LocalPlayer.Data.Role.IsImpostor && PlayerControl.LocalPlayer.CanUseVents())
        {
            __instance.ImpostorVentButton.DoClick();
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
        CustomRoleManager.OnHudUpdate(__instance);
        resetNameTagsAndColors();
        SetNameColors();
        setNameTags();

        // Impostors
        updateImpostorKillButton(__instance);
        // Timer updates
        timerUpdate();

        // Update player outlines
        setBasePlayerOutlines();

        // Update Player Info
        UpdatePlayerInfo();

        // Deputy Sabotage, Use and Vent Button Disabling
        updateReportButton(__instance);
        updateVentButton(__instance);
        if (!MeetingHud.Instance)
            __instance.AbilityButton?.Update();
        // 在会议中隐藏使用按钮
        if (MeetingHud.Instance)
            __instance.UseButton.Hide();
        // 如果红狼已死亡，且不允许死亡玩家破坏时，则隐藏按钮
        if (PlayerControl.LocalPlayer.IsImpostor() && CustomOptionHolder.deadImpsBlockSabotage.GetBool())
            __instance.SabotageButton.Hide();
        // 为怨灵显示阴影层
        if (Specter.Player != null && PlayerControl.LocalPlayer == Specter.Player && InGame && !InMeeting)
            __instance.ShadowQuad?.gameObject?.SetActive(true);

        // Fix dead player's pets being visible by just always updating whether the pet should be visible at all.
        foreach (PlayerControl target in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (target.GetPet() != null)
                target.GetPet().Visible = ((PlayerControl.LocalPlayer.IsDead() && target.IsDead()) || target.IsAlive()) && !target.inVent;
        }
    }
}