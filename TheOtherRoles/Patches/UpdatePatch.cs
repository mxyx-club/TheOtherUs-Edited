using AmongUs.GameOptions;
using Rewired;
using TheOtherRoles.Objects;

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
        {
            foreach (var player in MeetingHud.Instance.playerStates)
                if (player.NameText != null && p.PlayerId == player.TargetPlayerId)
                    player.NameText.color = color;
        }
    }

    private static void updateBlindReport()
    {
        if (Blind.blind != null && PlayerControl.LocalPlayer == Blind.blind)
            DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(false);
        // Sadly the report button cannot be hidden due to preventing R to report
    }

    public static void updatePlayerInfo()
    {
        if (!InGame) return;
        var local = PlayerControl.LocalPlayer;
        foreach (PlayerControl p in PlayerControl.AllPlayerControls)
        {
            var playerVoteArea = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == p.PlayerId);
            if (playerVoteArea != null && playerVoteArea.ColorBlindName.gameObject.active)
            {
                playerVoteArea.ColorBlindName.transform.localPosition = new Vector3(-0.93f, -0.2f, -0.1f);
                playerVoteArea.ColorBlindName.fontSize *= 1.75f;
            }
            p.cosmetics.nameText.transform.parent.SetLocalZ(-0.0001f);

            bool teamSeeRoles = (Lawyer.lawyerKnowsRole && local == Lawyer.lawyer && p == Lawyer.target) ||
                 (PartTimer.knowsRole && local == PartTimer.partTimer && p == PartTimer.target) ||
                 (local == PartTimer.target && p == PartTimer.partTimer) ||
                 (Akujo.knowsRoles && local == Akujo.akujo && (p == Akujo.honmei || Akujo.keeps.Any(x => x.PlayerId == p.PlayerId))) ||
                 (ModOption.impostorSeeRoles && Spy.spy == null && local.IsImpostor() && p.IsImpostor()) ||
                 (BandLeader.Player == p && BandLeader.Members.Any(x => x.PlayerId == local.PlayerId)) ||
                 (Jackal.jackal.Any(x => x.PlayerId == local.PlayerId) && p.PlayerId == Jackal.Sidekick?.PlayerId) ||
                 (Jackal.Sidekick == local && Jackal.jackal.Any(x => x.PlayerId == p.PlayerId)) ||
                 (SchrodingersCat.InTeam(local, out _) && p == SchrodingersCat.Player);

            bool reported = ((local == Slueth.slueth && Slueth.reported.Any(x => x.PlayerId == p.PlayerId)) ||
                             (local == Poucher.poucher && Poucher.killed.Any(x => x.PlayerId == p.PlayerId))) && p.IsDead();

            bool revealed = (Mayor.mayor == p && Mayor.Revealed) || (WolfLord.Player == p && WolfLord.Revealed);

            if (p == local || local.Data.IsDead || teamSeeRoles || reported || revealed)
            {
                var mainRole = RoleInfo.GetRolesString(p, true, false, false, false);
                var allRoleText = RoleInfo.GetRolesString(p, true, true, true, true);
                if (p.IsDead() && CanSeeRoleInfo) allRoleText += $" - {RoleInfo.GetDeathReasonString(p)}";

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
                else if (local.IsAlive() && Mayor.mayor == p && Mayor.Revealed)
                {
                    meetingInfoText = cs(Mayor.color, "Mayor".Translate());
                }
                else if (local.IsAlive() && WolfLord.Player == p && WolfLord.Revealed)
                {
                    meetingInfoText = cs(WolfLord.color, "WolfLord".Translate());
                }
                else if (teamSeeRoles && local.IsAlive())
                {
                    meetingInfoText = playerInfoText = mainRole;
                }
                else if (reported && local.IsAlive())
                {
                    meetingInfoText = playerInfoText = mainRole;
                }
                else
                {
                    if (CanSeeRoleInfo)
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

    private static void setBasePlayerOutlines()
    {
        var local = PlayerControl.LocalPlayer;
        foreach (PlayerControl target in PlayerControl.AllPlayerControls)
        {
            if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) continue;

            var isMorphedMorphling = target == Morphling.morphling && Morphling.morphTarget != null && Morphling.morphTimer > 0f;
            var hasVisibleShield = false;
            var color = Medic.shieldedColor;
            if (!isCamoComms && Camouflager.camouflageTimer <= 0f && !MushroomSabotageActive &&
                Medic.shielded != null && ((target == Medic.shielded && !isMorphedMorphling) ||
                (isMorphedMorphling && Morphling.morphTarget == Medic.shielded)))
            {
                hasVisibleShield = Medic.showShielded == 0 || CanSeeRoleInfo // Everyone or Ghost info
                    || (Medic.showShielded == 1 && (local == Medic.shielded || local == Medic.medic)) // Shielded + Medic
                    || (Medic.showShielded == 2 && local == Medic.medic); // Medic only

                // Make shield invisible till after the next meeting if the option is set (the medic can already see the shield)
                hasVisibleShield = hasVisibleShield && (Medic.meetingAfterShielding || !Medic.showShieldAfterMeeting ||
                    local == Medic.medic || CanSeeRoleInfo);
            }

            if (BodyGuard.guarded.IsAlive() && target == BodyGuard.guarded &&
                (CanSeeRoleInfo || local == BodyGuard.bodyguard || (local == BodyGuard.guarded && BodyGuard.showShielded)))
            {
                hasVisibleShield = true;
                color = new Color32(205, 150, 100, byte.MaxValue);
            }

            if (!isCamoComms && Camouflager.camouflageTimer <= 0f && !MushroomSabotageActive &&
                ModOption.firstKillPlayer != null && ModOption.shieldFirstKill &&
                ((target == ModOption.firstKillPlayer && !isMorphedMorphling) ||
                 (isMorphedMorphling && Morphling.morphTarget == ModOption.firstKillPlayer)))
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

    private static void setNameColors()
    {
        var local = PlayerControl.LocalPlayer;
        var localRole = RoleInfo.getRoleInfoForPlayer(local, false).FirstOrDefault();
        var allPlayer = PlayerControl.AllPlayerControls;
        setPlayerNameColor(local, localRole.color);

        if (Sheriff.Player != null && Sheriff.Player.Any(x => x == local))
        {
            foreach (var p in Sheriff.Player) setPlayerNameColor(p, Sheriff.color);
            if (Sheriff.Deputy != null && Sheriff.knowsSheriff) setPlayerNameColor(Sheriff.Deputy, Sheriff.color);
        }
        if (Sheriff.Deputy != null && Sheriff.Deputy == local)
        {
            setPlayerNameColor(Sheriff.Deputy, Sheriff.color);
            foreach (var p in Sheriff.Player) setPlayerNameColor(p, Sheriff.color);
        }

        if (Prophet.prophet != null && Prophet.prophet == local)
        {
            setPlayerNameColor(Prophet.prophet, Prophet.color);
            if (Prophet.examined != null && local.IsAlive()) // Reset the name tags when Prophet is dead
            {
                foreach (var p in Prophet.examined)
                {
                    setPlayerNameColor(p.Key, p.Value ^ Vortox.Reversal ? Palette.ImpostorRed : Color.green);
                }
            }
        }

        if (Executioner.executioner != null && local == Executioner.executioner && Executioner.target != null)
        {
            setPlayerNameColor(Executioner.target, Executioner.color);
        }

        if (Lawyer.lawyer != null && local == Lawyer.lawyer && Lawyer.target != null)
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

        if (Grenadier.Player != null && ((local.IsImpostor() && Grenadier.indicatorsMode)
            || local == Grenadier.Player || CanSeeRoleInfo))
        {
            foreach (var p in Grenadier.controls)
            {
                if (p != local && !p.IsImpostor()) setPlayerNameColor(p, Color.black);
            }
        }

        if (SchrodingersCat.Player != null && (SchrodingersCat.Player == local || CanSeeRoleInfo))
        {
            setPlayerNameColor(SchrodingersCat.Player, SchrodingersCat.stateColor);
            foreach (var p in allPlayer)
            {
                if (SchrodingersCat.InTeam(p, out var color))
                {
                    setPlayerNameColor(p, color);
                }
            }
        }

        if (Jackal.jackal != null && Jackal.jackal.Any(x => x.PlayerId == local.PlayerId))
        {
            // Jackal can see his sidekick
            foreach (var p in Jackal.jackal) setPlayerNameColor(p, Jackal.color);
            if (Jackal.Sidekick != null) setPlayerNameColor(Jackal.Sidekick, Jackal.color);

            if (SchrodingersCat.State == SchrodingersCat.CatState.Jackal)
                setPlayerNameColor(SchrodingersCat.Player, Jackal.color);
        }

        // No else if here, as a Lover of team Jackal needs the colors
        if (Jackal.Sidekick != null && Jackal.Sidekick == local)
        {
            // Sidekick can see the jackal
            setPlayerNameColor(Jackal.Sidekick, Jackal.color);
            foreach (var p in Jackal.jackal) setPlayerNameColor(p, Jackal.color);

            if (SchrodingersCat.State == SchrodingersCat.CatState.Jackal)
                setPlayerNameColor(SchrodingersCat.Player, Jackal.color);
        }

        if (Werewolf.canUseVents && Werewolf.werewolf != null && Werewolf.werewolf == local)
        {
            if (SchrodingersCat.State == SchrodingersCat.CatState.Werewolf)
                setPlayerNameColor(SchrodingersCat.Player, Werewolf.color);
        }

        if (Juggernaut.juggernaut != null && Juggernaut.juggernaut == local)
        {
            if (SchrodingersCat.State == SchrodingersCat.CatState.Juggernaut)
                setPlayerNameColor(SchrodingersCat.Player, Juggernaut.color);
        }

        if (Pelican.Player != null && Pelican.Player == local)
        {
            if (SchrodingersCat.State == SchrodingersCat.CatState.Pelican)
                setPlayerNameColor(SchrodingersCat.Player, Pelican.color);
        }

        if (Swooper.swooper != null && Swooper.swooper == local)
        {
            if (SchrodingersCat.State == SchrodingersCat.CatState.Swooper)
                setPlayerNameColor(SchrodingersCat.Player, Swooper.color);
        }

        if (Arsonist.arsonist != null && Arsonist.arsonist == local)
        {
            if (SchrodingersCat.State == SchrodingersCat.CatState.Arsonist)
                setPlayerNameColor(SchrodingersCat.Player, Arsonist.color);
        }

        if (SchrodingersCat.Player != null && PlayerControl.LocalPlayer.IsImpostor())
        {
            if (SchrodingersCat.State == SchrodingersCat.CatState.Impostor)
                setPlayerNameColor(SchrodingersCat.Player, Palette.ImpostorRed);
        }

        if (Pavlovsdogs.pavlovsowner != null && Pavlovsdogs.pavlovsowner == local)
        {
            setPlayerNameColor(Pavlovsdogs.pavlovsowner, Pavlovsdogs.color);
            foreach (var p in Pavlovsdogs.pavlovsdogs) setPlayerNameColor(p, Pavlovsdogs.color);
            if (SchrodingersCat.State == SchrodingersCat.CatState.Pavlovsowner)
                setPlayerNameColor(SchrodingersCat.Player, Pavlovsdogs.color);
        }

        if (Pavlovsdogs.pavlovsdogs != null && Pavlovsdogs.pavlovsdogs.Any(p => p == local))
        {
            foreach (var p in Pavlovsdogs.pavlovsdogs) setPlayerNameColor(p, Pavlovsdogs.color);
            if (Pavlovsdogs.pavlovsowner != null) setPlayerNameColor(Pavlovsdogs.pavlovsowner, Pavlovsdogs.color);
            if (SchrodingersCat.State == SchrodingersCat.CatState.Pavlovsowner)
                setPlayerNameColor(SchrodingersCat.Player, Pavlovsdogs.color);
        }

        if (Snitch.snitch != null)
        {
            var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);
            int numberOfTasks = playerTotal - playerCompleted;

            bool forImp = local.IsImpostor();
            bool forKillerTeam = Snitch.Team == Snitch.includeNeutralTeam.KillNeutral && isKillerNeutral(local);
            bool forEvilTeam = Snitch.Team == Snitch.includeNeutralTeam.EvilNeutral && isEvilNeutral(local);
            bool forNeutraTeam = Snitch.Team == Snitch.includeNeutralTeam.AllNeutral && local.IsNeutral();

            if (numberOfTasks <= Snitch.taskCountForReveal && Snitch.snitch.IsAlive())
            {
                foreach (PlayerControl p in allPlayer)
                {
                    if (forImp || forKillerTeam || forEvilTeam || forNeutraTeam)
                    {
                        setPlayerNameColor(Snitch.snitch, Snitch.color);
                    }
                }
            }

            if (numberOfTasks == 0 && Snitch.seeInMeeting && Snitch.snitch.IsAlive())
            {
                foreach (PlayerControl p in allPlayer)
                {
                    bool TargetsImp = p.Data.Role.IsImpostor;
                    bool TargetsKillerTeam = Snitch.Team == Snitch.includeNeutralTeam.KillNeutral && isKillerNeutral(p);
                    bool TargetsEvilTeam = Snitch.Team == Snitch.includeNeutralTeam.EvilNeutral && isEvilNeutral(p);
                    bool TargetsNeutraTeam = Snitch.Team == Snitch.includeNeutralTeam.AllNeutral && p.IsNeutral();
                    var targetsRole = RoleInfo.getRoleInfoForPlayer(p, false, false).FirstOrDefault();
                    if (local == Snitch.snitch && (TargetsImp || TargetsKillerTeam || TargetsEvilTeam || TargetsNeutraTeam))
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
        if (Spy.spy != null && local.Data.Role.IsImpostor) setPlayerNameColor(Spy.spy, Spy.color);
    }

    private static void setNameTags()
    {
        var local = PlayerControl.LocalPlayer;
        var allPlayerStates = MeetingHud.Instance?.playerStates;
        // Lovers
        if (Lovers.lover1 != null && Lovers.lover2 != null &&
            (Lovers.lover1 == local || Lovers.lover2 == local))
        {
            var suffix = cs(Lovers.color, " ♥");
            Lovers.lover1.cosmetics.nameText.text += suffix;
            Lovers.lover2.cosmetics.nameText.text += suffix;

            if (MeetingHud.Instance != null)
            {
                foreach (var player in allPlayerStates)
                    if (Lovers.lover1.PlayerId == player.TargetPlayerId || Lovers.lover2.PlayerId == player.TargetPlayerId)
                        player.NameText.text += suffix;
            }
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
                foreach (PlayerVoteArea player in allPlayerStates)
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

        if (PartTimer.partTimer != null && PartTimer.target != null && (local == PartTimer.partTimer || local == PartTimer.target || CanSeeRoleInfo))
        {
            var suffix = cs(PartTimer.color, " ★");
            PartTimer.partTimer.cosmetics.nameText.text += suffix;
            PartTimer.target.cosmetics.nameText.text += suffix;

            if (MeetingHud.Instance != null)
                foreach (var player in allPlayerStates)
                    if (PartTimer.partTimer.PlayerId == player.TargetPlayerId || PartTimer.target.PlayerId == player.TargetPlayerId)
                        player.NameText.text += suffix;
        }

        if (BandLeader.Player != null)
        {
            var suffix1 = cs(BandLeader.color, "(K)");
            var suffix2 = cs(BandLeader.color, "(B)");
            var suffix3 = cs(BandLeader.color, "(D)");
            var isKeyboardist = local == BandLeader.Player || BandLeader.Keyboardist == local || BandLeader.Formed || CanSeeRoleInfo;
            var isBassist = local == BandLeader.Player || BandLeader.Bassist == local || BandLeader.Formed || CanSeeRoleInfo;
            var isDrummer = local == BandLeader.Player || BandLeader.Drummer == local || BandLeader.Formed || CanSeeRoleInfo;
            if (local == BandLeader.Player || local.IsDead() || BandLeader.Members.Any(x => x == local))
            {
                if (BandLeader.Keyboardist != null && isKeyboardist)
                    BandLeader.Keyboardist.cosmetics.nameText.text += suffix1;
                if (BandLeader.Bassist != null && isBassist)
                    BandLeader.Bassist.cosmetics.nameText.text += suffix2;
                if (BandLeader.Drummer != null && isDrummer)
                    BandLeader.Drummer.cosmetics.nameText.text += suffix3;

                if (MeetingHud.Instance != null)
                {
                    foreach (var player in allPlayerStates)
                    {
                        if (isKeyboardist && BandLeader.Keyboardist?.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix1;
                        if (isBassist && BandLeader.Bassist?.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix2;
                        if (isDrummer && BandLeader.Drummer?.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix3;
                    }
                }
            }
        }

        var localIsArsonist = Arsonist.arsonist != null && Arsonist.dousedPlayers != null && Arsonist.arsonist == local;
        var localIsDead = Arsonist.arsonist != null && Arsonist.dousedPlayers != null && CanSeeRoleInfo;
        if (localIsArsonist || localIsDead)
        {
            var suffix = cs(Arsonist.color, " ♨");
            foreach (var target in Arsonist.dousedPlayers)
            {
                target.cosmetics.nameText.text += suffix;
            }

            if (MeetingHud.Instance != null)
                foreach (var target in allPlayerStates)
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
                foreach (var player in allPlayerStates)
                    if (player.TargetPlayerId == Lawyer.target.PlayerId)
                        player.NameText.text += suffix;
        }

        var localIsExecutioner = Executioner.executioner != null && Executioner.target != null && Executioner.executioner == local;
        if (localIsExecutioner && Executioner.executioner.IsAlive())
        {
            var suffix = cs(Executioner.color, " §");
            Executioner.target.cosmetics.nameText.text += suffix;

            if (MeetingHud.Instance != null)
                foreach (var player in allPlayerStates)
                    if (player.TargetPlayerId == Executioner.target.PlayerId)
                        player.NameText.text += suffix;
        }

        // Display lighter / darker color for all alive players
        if (PlayerControl.LocalPlayer != null && MeetingHud.Instance != null && ModOption.showLighterDarker)
        {
            foreach (var player in allPlayerStates)
            {
                var target = playerById(player.TargetPlayerId);
                if (target != null) player.NameText.text += $" ({(isLighterColor(target) ? "浅" : "深")})";
            }
        }

        // Add medic shield info:
        if (MeetingHud.Instance != null && Medic.medic != null && Medic.shielded != null && Medic.shieldVisible(Medic.shielded))
        {
            foreach (PlayerVoteArea player in allPlayerStates)
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

        if (Medic.shielded.IsDead() || Medic.medic == null || Medic.medic.IsDead()) Medic.shielded = null;
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

    public static void evilTrapperUpdate()
    {
        try
        {
            if (PlayerControl.LocalPlayer == EvilTrapper.evilTrapper && KillTrap.traps.Count != 0 && !KillTrap.hasTrappedPlayer() && !EvilTrapper.meetingFlag)
            {
                foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    foreach (var trap in KillTrap.traps)
                    {
                        if (DateTime.UtcNow.Subtract(trap.Value.placedTime).TotalSeconds < EvilTrapper.extensionTime) continue;
                        if (trap.Value.isActive || p.Data.IsDead || p.inVent || EvilTrapper.meetingFlag) continue;
                        var p1 = p.transform.localPosition;
                        Dictionary<GameObject, byte> listActivate = new();
                        var p2 = trap.Value.killtrap.transform.localPosition;
                        var distance = Vector3.Distance(p1, p2);
                        if (distance < EvilTrapper.trapRange)
                        {
                            TMP_Text text;
                            RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                            GameObject gameObject = UObject.Instantiate(roomTracker.gameObject);
                            UObject.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                            gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                            gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
                            gameObject.transform.localScale = Vector3.one * 2f;
                            text = gameObject.GetComponent<TMP_Text>();
                            text.text = string.Format(GetString("trapperGotTrapText"), p.Data.PlayerName);
                            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(3f, new Action<float>((p) =>
                            {
                                if (p == 1f && text != null && text.gameObject != null)
                                {
                                    UObject.Destroy(text.gameObject);
                                }
                            })));
                            var writer = StartRPC(CustomRPC.ActivateTrap);
                            writer.Write(trap.Key);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            writer.Write(p.PlayerId);
                            writer.EndRPC();
                            RPCProcedure.activateTrap(trap.Key, EvilTrapper.evilTrapper.PlayerId, p.PlayerId);
                            break;
                        }
                    }
                }
            }

            if (PlayerControl.LocalPlayer == EvilTrapper.evilTrapper && KillTrap.hasTrappedPlayer() && !EvilTrapper.meetingFlag)
            {
                // トラップにかかっているプレイヤーを救出する
                foreach (var trap in KillTrap.traps)
                {
                    if (trap.Value.killtrap == null || !trap.Value.isActive) return;
                    Vector3 p1 = trap.Value.killtrap.transform.position;
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.PlayerId == trap.Value.target.PlayerId || player.Data.IsDead || player.inVent || player == EvilTrapper.evilTrapper) continue;
                        Vector3 p2 = player.transform.position;
                        float distance = Vector3.Distance(p1, p2);
                        if (distance < 0.5)
                        {
                            var writer = StartRPC(CustomRPC.DisableTrap);
                            writer.Write(trap.Key);
                            writer.EndRPC();
                            RPCProcedure.disableTrap(trap.Key);
                        }
                    }

                }
            }
        }
        catch (NullReferenceException e)
        {
            Warn(e.Message);
        }
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
        if (Vampire.vampire.IsAlive() && Vampire.vampire == PlayerControl.LocalPlayer)
            enabled = false;
        if (Berserker.Player.IsAlive() && Berserker.Player == PlayerControl.LocalPlayer)
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
        else if (PlayerControl.LocalPlayer.roleCanUseVents() && !__instance.ImpostorVentButton.isActiveAndEnabled)
        {
            __instance.ImpostorVentButton.Show();

        }
        if (ReInput.players.GetPlayer(0).GetButtonDown(RewiredConsts.Action.UseVent) &&
            !PlayerControl.LocalPlayer.Data.Role.IsImpostor && PlayerControl.LocalPlayer.roleCanUseVents())
        {
            __instance.ImpostorVentButton.DoClick();
        }

    }

    private static void updateUseButton(HudManager __instance)
    {
        if (MeetingHud.Instance) __instance.UseButton.Hide();
    }

    private static void updateSabotageButton(HudManager __instance)
    {
        if (PlayerControl.LocalPlayer.IsDead() && CustomOptionHolder.deadImpsBlockSabotage.GetBool()) __instance.SabotageButton.Hide();
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
        DeadBody[] array = UObject.FindObjectsOfType<DeadBody>();
        foreach (var body in array.Where(x => x.ParentId == Giant.giant.PlayerId))
        {
            try
            {
                body.transform.localScale = new Vector3(Giant.size, Giant.size, 1f);
            }
            catch { }
        }
    }

    private static void detectiveUpdateFootPrints()
    {
        if (Detective.detective.IsAlive() && Detective.detective == PlayerControl.LocalPlayer && !InMeeting)
        {
            Detective.timer -= Time.fixedDeltaTime;
            if (Detective.timer <= 0f)
            {
                Detective.timer = Detective.footprintIntervall;
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    if (player != null && player != PlayerControl.LocalPlayer && player.IsAlive() && !player.inVent)
                        FootprintHolder.Instance.MakeFootprint(player);
            }
        }
    }

    private static void bountyHunterUpdate()
    {
        if (BountyHunter.bountyHunter == null || PlayerControl.LocalPlayer != BountyHunter.bountyHunter) return;

        if (BountyHunter.bountyHunter.Data.IsDead || InMeeting)
        {
            if (BountyHunter.arrow != null) UObject.Destroy(BountyHunter.arrow.arrow);
            BountyHunter.arrow = null;
            if (BountyHunter.cooldownText != null && BountyHunter.cooldownText.gameObject != null) UObject.Destroy(BountyHunter.cooldownText.gameObject);
            BountyHunter.cooldownText = null;
            BountyHunter.bounty = null;
            foreach (PoolablePlayer p in ModOption.playerIcons.Values)
            {
                if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
            }
            return;
        }

        BountyHunter.arrowUpdateTimer -= Time.fixedDeltaTime;
        BountyHunter.bountyUpdateTimer -= Time.fixedDeltaTime;

        if ((BountyHunter.bounty == null || BountyHunter.bountyUpdateTimer <= 0f) && !InMeeting)
        {
            // Set new bounty
            BountyHunter.bounty = null;
            BountyHunter.arrowUpdateTimer = 0f; // Force arrow to update
            BountyHunter.bountyUpdateTimer = BountyHunter.bountyDuration;
            var possibleTargets = new List<PlayerControl>();
            foreach (PlayerControl p in PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsAlive() && !x.IsImpostor(true)))
                if ((p != Mini.mini || Mini.isGrownUp()) && p != Lovers.otherLover(BountyHunter.bountyHunter))
                    possibleTargets.Add(p);
            if (possibleTargets.Count == 0) return;
            BountyHunter.bounty = possibleTargets[rnd.Next(0, possibleTargets.Count)];
            if (BountyHunter.bounty == null) return;

            // Ghost Info
            var writer = StartRPC(CustomRPC.ShareGhostInfo);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write((byte)RPCProcedure.GhostInfoTypes.BountyTarget);
            writer.Write(BountyHunter.bounty.PlayerId);
            writer.EndRPC();

            // Show poolable player
            if (FastDestroyableSingleton<HudManager>.Instance?.UseButton != null)
            {
                foreach (var pp in ModOption.playerIcons.Values) pp.gameObject.SetActive(false);
                if (BountyHunter.bounty != null && !InMeeting
                    && ModOption.playerIcons.ContainsKey(BountyHunter.bounty.PlayerId)
                    && ModOption.playerIcons[BountyHunter.bounty.PlayerId]?.gameObject != null)
                {
                    ModOption.playerIcons[BountyHunter.bounty.PlayerId].gameObject.SetActive(true);
                }
            }
        }

        // Update Cooldown Text
        if (BountyHunter.cooldownText != null)
        {
            BountyHunter.cooldownText.text = Mathf.CeilToInt(Mathf.Clamp(BountyHunter.bountyUpdateTimer, 0, BountyHunter.bountyDuration)).ToString();
            BountyHunter.cooldownText.gameObject.SetActive(!MeetingHud.Instance); // Show if not in meeting
        }

        // Update Arrow
        if (BountyHunter.showArrow && BountyHunter.bounty.IsAlive())
        {
            BountyHunter.arrow ??= new Arrow(Color.red);
            if (BountyHunter.arrowUpdateTimer <= 0f)
            {
                BountyHunter.arrow.Update(BountyHunter.bounty.transform.position);
                BountyHunter.arrowUpdateTimer = BountyHunter.arrowUpdateIntervall;
            }

            BountyHunter.arrow.Update();
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

        if (!InGame || IsHideNSeek) return;

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

        // Update player outlines
        setBasePlayerOutlines();

        // Update Player Info
        updatePlayerInfo();

        // Ninja
        NinjaTrace.UpdateAll();
        // yoyo
        Silhouette.UpdateAll();
        // BountyHunter
        bountyHunterUpdate();
        // Detective
        detectiveUpdateFootPrints();
        // EvilTrapper
        evilTrapperUpdate();
        // Trapper
        Trap.Update();
        // Bomber
        Bomb.update();
        // Vampire
        Garlic.UpdateAll();
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
                pet.Visible = ((PlayerControl.LocalPlayer.IsDead() && target.IsDead()) || target.IsAlive()) && !target.inVent;
        }
    }
}