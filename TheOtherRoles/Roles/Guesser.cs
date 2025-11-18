using TheOtherRoles.Mode;


namespace TheOtherRoles.Roles;


public abstract class GuesserBase : RoleBase
{
    public abstract int Charges { get; set; }

    public GuesserBase(PlayerControl player, RoleInfo roleInfo, bool? hasTasks = null) : base(player, roleInfo, hasTasks) { }

    public override void Initialize() { }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        // Add Guesser Buttons
        if ((Charges > 0 || Player.Is(RoleId.Doomsayer)) && PlayerControl.LocalPlayer.IsAlive())
        {
            foreach (var pva in __instance.playerStates)
            {
                if (pva.AmDead || pva.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

                if (Player.TryGetRole<Eraser>(out var eraser) && eraser.alreadyErased.Contains(pva.TargetPlayerId)) continue;

                var template = pva.Buttons.transform.Find("CancelButton").gameObject;
                var targetBox = UObject.Instantiate(template, pva.transform);
                targetBox.name = "ShootButton";
                targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                var renderer = targetBox.GetComponent<SpriteRenderer>();
                renderer.sprite = HandleGuesser.targetSprite;
                var button = targetBox.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                button.OnClick.AddListener((Action)(() => Guesser.guesserOnClick(pva, __instance)));
            }
        }
    }



}

public static class Guesser
{
    public static bool isGuesser(byte playerId)
    {
        var player = PlayerById(playerId);
        if (player.GetRole() == RoleId.Vigilante) return true;
        if (player.GetModifier().Contains(RoleId.Assassin)) return true;
        return false;
    }

    public static void clear(byte playerId)
    {
        var player = PlayerById(playerId);
        if (player.TryGetModifier<Assassin>(out var assassin)) assassin.Destroy();
        if (player.TryGetRole<Vigilante>(out var vigilante)) vigilante.Destroy();
    }

    public static int remainingShots(byte playerId, bool shoot = false)
    {
        var player = PlayerById(playerId);
        if (player.TryGetRole<Vigilante>(out var vigilante))
        {
            return shoot ? Mathf.Max(0, --vigilante.Charges) : vigilante.Charges;
        }

        if (player.TryGetModifier<Assassin>(out var assassin))
        {
            return shoot ? Mathf.Max(0, --assassin.Charges) : assassin.Charges;
        }

        return 0;
    }

    public static bool CanMultipleShots(PlayerControl dyingTarget)
    {
        if (dyingTarget == PlayerControl.LocalPlayer)
            return false;

        if (ModOption.GameMode != CustomGameModes.Guesser)
        {
            if (dyingTarget.GetRole() == RoleId.Vigilante
                && HandleGuesser.remainingShots(PlayerControl.LocalPlayer.PlayerId) > 0
                && Vigilante.hasMultipleShotsPerMeeting) return true;
            else if (dyingTarget.GetModifier().Contains(RoleId.Vigilante)
                && HandleGuesser.remainingShots(PlayerControl.LocalPlayer.PlayerId) > 0
                && Assassin.assassinMultipleShotsPerMeeting) return true;
        }

        else if (HandleGuesser.isGuesser(PlayerControl.LocalPlayer.PlayerId)
            && HandleGuesser.remainingShots(PlayerControl.LocalPlayer.PlayerId) > 0
            && HandleGuesser.hasMultipleShotsPerMeeting) return true;

        return PlayerControl.LocalPlayer.TryGetRole<Doomsayer>(out var doomsayer) && Doomsayer.hasMultipleShotsPerMeeting && doomsayer.CanShoot;
    }

    public const int MaxOneScreenRole = 40;
    private static Dictionary<RoleType, List<Transform>> RoleButtons;
    private static Dictionary<RoleType, SpriteRenderer> RoleSelectButtons;
    public static RoleType currentTeamType;
    private static List<SpriteRenderer> PageButtons;
    public static GameObject guesserUI;
    public static PassiveButton guesserUIExitButton;
    public static int Page;
    public static byte guesserCurrentTarget;

    private static void guesserSelectRole(RoleType Role, bool SetPage = true)
    {
        currentTeamType = Role;
        if (SetPage) Page = 1;
        foreach (var RoleButton in RoleButtons)
        {
            int index = 0;
            RoleButtons.TryGetValue(RoleButton.Key, out var RoleButtonList);
            RoleButtonList ??= new();
            foreach (var RoleBtn in RoleButtonList)
            {
                if (RoleBtn == null) continue;
                index++;
                if (index <= (Page - 1) * MaxOneScreenRole) { RoleBtn.gameObject.SetActive(false); continue; }
                if ((Page * MaxOneScreenRole) < index) { RoleBtn.gameObject.SetActive(false); continue; }
                RoleBtn.gameObject.SetActive(RoleButton.Key == Role);
            }
        }
        foreach (var RoleButton in RoleSelectButtons)
        {
            if (RoleButton.Value == null) continue;
            RoleButton.Value.color = new(0, 0, 0, RoleButton.Key == Role ? 1 : 0.25f);
        }
    }

    public static void guesserOnClick(PlayerVoteArea pva, MeetingHud __instance)
    {
        if (guesserUI != null || !(__instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted or MeetingHud.VoteStates.Discussion))
            return;

        Page = 1;
        RoleButtons = new();
        RoleSelectButtons = new();
        PageButtons = new();

        __instance.playerStates.ForEach(x => x.gameObject.SetActive(false));

        Transform PhoneUI = UObject.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");
        Transform container = UObject.Instantiate(PhoneUI, __instance.transform);
        container.transform.localPosition = new Vector3(0, 0, -5f);
        guesserUI = container.gameObject;
        container.transform.localScale *= 0.75f;

        List<int> i = [0, 0, 0, 0];
        var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
        var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
        var smallButtonTemplate = __instance.playerStates[0].Buttons.transform.Find("CancelButton");
        var textTemplate = __instance.playerStates[0].NameText;

        guesserCurrentTarget = pva.TargetPlayerId;

        var exitButtonParent = new GameObject().transform;
        exitButtonParent.SetParent(container);
        var exitButton = UObject.Instantiate(buttonTemplate.transform, exitButtonParent);
        var exitButtonMask = UObject.Instantiate(maskTemplate, exitButtonParent);
        exitButton.gameObject.GetComponent<SpriteRenderer>().sprite = smallButtonTemplate.GetComponent<SpriteRenderer>().sprite;
        var transform = exitButtonParent.transform;
        transform.localPosition = new Vector3(3f, 2.1f, -5);
        transform.localScale = new Vector3(0.217f, 0.9f, 1);
        guesserUIExitButton = exitButton.GetComponent<PassiveButton>();
        guesserUIExitButton.OnClick.RemoveAllListeners();
        guesserUIExitButton.OnClick.AddListener((Action)(() =>
        {
            __instance.playerStates.ForEach(x =>
            {
                x.gameObject.SetActive(true);
                if (PlayerControl.LocalPlayer.Data.IsDead && x.transform.FindChild("ShootButton") != null)
                    UObject.Destroy(x.transform.FindChild("ShootButton").gameObject);
            });
            UObject.Destroy(container.gameObject);
        }));

        var buttons = new List<Transform>();
        Transform selectedButton = null;

        // From SuperNewRoles
        for (int index = 1; index <= 3; index++)
        {
            Transform TeambuttonParent = new GameObject().transform;
            TeambuttonParent.SetParent(container);
            Transform Teambutton = UObject.Instantiate(buttonTemplate, TeambuttonParent);
            Teambutton.FindChild("ControllerHighlight").gameObject.SetActive(false);
            Transform TeambuttonMask = UObject.Instantiate(maskTemplate, TeambuttonParent);
            TextMeshPro Teamlabel = UObject.Instantiate(textTemplate, Teambutton);
            //Teambutton.GetComponent<SpriteRenderer>().sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
            RoleSelectButtons.Add((RoleType)index, Teambutton.GetComponent<SpriteRenderer>());
            TeambuttonParent.localPosition = new(-2.75f + (index * 1.75f), 2.225f, -200);
            TeambuttonParent.localScale = new(0.55f, 0.55f, 1f);
            Teamlabel.color = getTeamColor((RoleType)index);
            //Info($"{Teamlabel.color} {(RoleTeam)index}");
            Teamlabel.text = GetString(((RoleType)index is RoleType.Crewmate ? "Crewmate" : ((RoleType)index).ToString()) + "RolesText");
            Teamlabel.alignment = TextAlignmentOptions.Center;
            Teamlabel.transform.localPosition = new Vector3(0, 0, Teamlabel.transform.localPosition.z);
            Teamlabel.transform.localScale *= 1.6f;
            Teamlabel.autoSizeTextContainer = true;

            static void CreateTeamButton(Transform Teambutton, RoleType type)
            {
                Teambutton.GetComponent<PassiveButton>().OnClick.AddListener((UnityEngine.Events.UnityAction)(() =>
                {
                    guesserSelectRole(type);
                    ReloadPage();
                }));
            }
            if (!PlayerControl.LocalPlayer.Data.IsDead) CreateTeamButton(Teambutton, (RoleType)index);
        }

        static void ReloadPage()
        {
            PageButtons[0].color = new(1, 1, 1, 1f);
            PageButtons[1].color = new(1, 1, 1, 1f);
            if (RoleButtons.Count != 0)
            {
                return;
            }
            else if (((RoleButtons[currentTeamType].Count / MaxOneScreenRole) +
                (RoleButtons[currentTeamType].Count % MaxOneScreenRole != 0 ? 1 : 0)) < Page)
            {
                Page -= 1;
                PageButtons[1].color = new(1, 1, 1, 0.1f);
            }
            else if (((RoleButtons[currentTeamType].Count / MaxOneScreenRole) +
                (RoleButtons[currentTeamType].Count % MaxOneScreenRole != 0 ? 1 : 0)) < Page + 1)
            {
                PageButtons[1].color = new(1, 1, 1, 0.1f);
            }
            if (Page <= 1)
            {
                Page = 1;
                PageButtons[0].color = new(1, 1, 1, 0.1f);
            }
            guesserSelectRole(currentTeamType, false);
            Info("Page:" + Page);
        }

        static void CreatePage(bool IsNext, MeetingHud __instance, Transform container)
        {
            var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
            var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
            var smallButtonTemplate = __instance.playerStates[0].Buttons.transform.Find("CancelButton");
            var textTemplate = __instance.playerStates[0].NameText;
            Transform PagebuttonParent = new GameObject().transform;
            PagebuttonParent.SetParent(container);
            Transform Pagebutton = UObject.Instantiate(buttonTemplate, PagebuttonParent);
            Pagebutton.FindChild("ControllerHighlight").gameObject.SetActive(false);
            Transform PagebuttonMask = UObject.Instantiate(maskTemplate, PagebuttonParent);
            TextMeshPro Pagelabel = UObject.Instantiate(textTemplate, Pagebutton);
            //Pagebutton.GetComponent<SpriteRenderer>().sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
            PagebuttonParent.localPosition = IsNext ? new(3.535f, -2.2f, -200) : new(-3.475f, -2.2f, -200);
            PagebuttonParent.localScale = new(0.55f, 0.55f, 1f);
            Pagelabel.color = Color.white;
            Pagelabel.text = GetString(IsNext ? "下一页" : "上一页");
            Pagelabel.alignment = TextAlignmentOptions.Center;
            Pagelabel.transform.localPosition = new Vector3(0, 0, Pagelabel.transform.localPosition.z);
            Pagelabel.transform.localScale *= 1.6f;
            Pagelabel.autoSizeTextContainer = true;
            if (!IsNext && Page <= 1) Pagebutton.GetComponent<SpriteRenderer>().color = new(1, 1, 1, 0.1f);
            Pagebutton.GetComponent<PassiveButton>().OnClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                Info("翻页");
                if (IsNext) Page += 1;
                else Page -= 1;
                ReloadPage();
            }));
            PageButtons.Add(Pagebutton.GetComponent<SpriteRenderer>());
        }
        if (!PlayerControl.LocalPlayer.Data.IsDead)
        {
            CreatePage(false, __instance, container);
            CreatePage(true, __instance, container);
        }

        int ind = 0;

        #region 职业排除规则
        foreach (var roleInfo in CustomRoleManager.AllRolesInfo.Values)
        {
            if (roleInfo == null) continue; // Not guessable roles

            if (roleInfo.AssignSelection > 0)
            {
                continue;
            }

            var guesserRole = PlayerControl.LocalPlayer.GetRole();

            if (PlayerControl.LocalPlayer.Is(RoleId.Doomsayer))
            {
                if (!Doomsayer.canGuessImpostor && roleInfo.RoleType == RoleType.Impostor)
                    continue;
                if (!Doomsayer.canGuessNeutral && roleInfo.RoleType == RoleType.Neutral)
                    continue;
            }

            if (roleInfo.RoleType is not RoleType.Crewmate and not RoleType.Neutral and not RoleType.Impostor)
                continue;

            // remove all roles that cannot spawn due to the settings from the ui.
            switch (roleInfo.RoleId)
            {
                case RoleId.Spy when ModOption.NumImpostors <= 1:
                    continue;
                case RoleId.Poucher when PoucherA.spawnModifier:
                    continue;
                case RoleId.Crewmate when !Assassin.evilGuesserCanGuessCrewmate && guesserRole == RoleId.Assassin:
                    continue;
                case RoleId.Spy when PlayerControl.LocalPlayer.IsImpostor() && !HandleGuesser.evilGuesserCanGuessSpy:
                    continue;
                case RoleId.Vigilante when HandleGuesser.isGuesserGm || PlayerControl.LocalPlayer.Is(RoleId.Vigilante):
                    continue;
                case RoleId.Sidekick when !Jackal.jackalCanCreateSidekick.GetBool():
                    continue;
                case RoleId.BandLeader:
                    continue;
                case RoleId.SchrodingersCat when !SchrodingersCat.IsGuessable:
                    continue;
                case RoleId.Doomsayer when guesserRole == RoleId.Doomsayer:
                    continue;
            }

            CreateRole(roleInfo);
        }
        #endregion

        void CreateRole(RoleInfo roleInfo = null)
        {
            if (roleInfo.RoleType is RoleType.Ghost or RoleType.Special) return;
            RoleType team = roleInfo?.RoleType ?? RoleType.Crewmate;
            //Color color = roleInfo?.color ?? Color.white;
            //RoleId role = roleInfo?.roleId ?? RoleId.Crewmate;

            var buttonParent = new GameObject().transform;
            buttonParent.SetParent(container);
            var button = UObject.Instantiate(buttonTemplate, buttonParent);
            button.FindChild("ControllerHighlight").gameObject.SetActive(false);
            var buttonMask = UObject.Instantiate(maskTemplate, buttonParent);
            var label = UObject.Instantiate(textTemplate, button);
            button.GetComponent<SpriteRenderer>().sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
            //button.GetComponent<SpriteRenderer>().sprite = FastDestroyableSingleton<HatManager>.Instance.GetNamePlateById("nameplate_NoPlate")?.viewData?.viewData?.Image;
            if (!RoleButtons.ContainsKey(team)) RoleButtons.Add(team, new());
            RoleButtons[team].Add(button);
            buttons.Add(button);
            int row = i[(int)team] / 5;
            int col = i[(int)team] % 5;
            buttonParent.localPosition = new Vector3(-3.47f + (1.75f * col), 1.5f - (0.45f * row), -200f);
            buttonParent.localScale = new Vector3(0.55f, 0.55f, 1f);
            label.text = Cs(roleInfo.Color, roleInfo.Name);
            label.alignment = TextAlignmentOptions.Center;
            label.transform.localPosition = new Vector3(0, 0, label.transform.localPosition.z);
            label.transform.localScale *= 1.6f;
            label.autoSizeTextContainer = true;
            int copiedIndex = i[(int)team];

            button.GetComponent<PassiveButton>().OnClick.RemoveAllListeners();
            if (!PlayerControl.LocalPlayer.Data.IsDead) button.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() =>
            {
                if (selectedButton != button)
                {
                    selectedButton = button;
                    buttons.ForEach(x => x.GetComponent<SpriteRenderer>().color = x == selectedButton ? Color.red : Color.white);
                }
                else
                {
                    var target = PlayerControl.LocalPlayer;
                    var guessed = PlayerById(pva.TargetPlayerId);
                    var mainRoleInfo = guessed.GetRole();

                    if (__instance.state is not (MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted)
                        || guessed == null
                        || (HandleGuesser.remainingShots(PlayerControl.LocalPlayer.PlayerId) <= 0
                            && HandleGuesser.isGuesser(PlayerControl.LocalPlayer.PlayerId))
                        || (PlayerControl.LocalPlayer.TryGetRole<Doomsayer>(out var doomsayer) && !doomsayer.CanShoot))
                        return;

                    /*if (!HandleGuesser.killsThroughShield && guessed == Medic.shielded)
                    {
                        // Depending on the options, shooting the shielded player will not allow the guess, notifiy everyone about the kill attempt and close the window
                        __instance.playerStates.ForEach(x => x.gameObject.SetActive(true));
                        Object.Destroy(container.gameObject);

                        var murderAttemptWriter = StartRPC(CustomRPC.ShieldedMurderAttempt);
                        murderAttemptWriter.EndRPC();
                        RPCProcedure.shieldedMurderAttempt(0);
                        SoundEffectsManager.play("fail");
                        return;
                    }*/

                    if (guessed.Is(RoleId.Indomitable))
                    {
                        showFlash(new Color32(255, 197, 97, byte.MinValue));
                        __instance.playerStates.ForEach(x => x.gameObject.SetActive(true));
                        UObject.Destroy(container.gameObject);

                        SoundEffectsManager.play("fail");
                        seedGuessChat(PlayerControl.LocalPlayer, target, (byte)roleInfo.RoleId);
                        return;
                    }

                    if (mainRoleInfo == roleInfo.RoleId)
                    {
                        target = guessed;
                    }

                    if (PlayerControl.LocalPlayer.TryGetModifier<Specoality>(out var specoality) && specoality.linearfunction > 0)
                    {
                        if (PlayerControl.LocalPlayer.IsAlive() && guessed != target)
                        {
                            if (guesserUI != null) guesserUIExitButton.OnClick.Invoke();

                            Coroutines.Start(showFlashCoroutine(Color.red, 1f, 0.3f));
                            specoality.linearfunction--;
                            SoundEffectsManager.play("fail");
                            __instance.playerStates.ForEach(x =>
                            {
                                if (x.TargetPlayerId == guessed.PlayerId && x.transform.FindChild("ShootButton") != null)
                                {
                                    UObject.Destroy(x.transform.FindChild("ShootButton").gameObject);
                                }
                            });
                            return;
                        }
                    }

                    // Shoot player and send chat info if activated
                    var writer = StartRPC(CustomRPC.GuesserShoot);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(target.PlayerId);
                    writer.Write(guessed.PlayerId);
                    writer.Write((byte)roleInfo.RoleId);
                    writer.EndRPC();
                    guesserShoot(PlayerControl.LocalPlayer.PlayerId, target.PlayerId, guessed.PlayerId, (byte)roleInfo.RoleId);

                    // Reset the GUI
                    __instance.playerStates.ForEach(x => x.gameObject.SetActive(true));
                    UObject.Destroy(container.gameObject);
                    if (CanMultipleShots(target))
                    {
                        __instance.playerStates.ForEach(x =>
                        {
                            if (x.TargetPlayerId == target.PlayerId && x.transform.FindChild("ShootButton") != null)
                                UObject.Destroy(x.transform.FindChild("ShootButton").gameObject);
                        });
                    }
                    else
                    {
                        __instance.playerStates.ForEach(x =>
                        {
                            if (x.transform.FindChild("ShootButton") != null)
                                UObject.Destroy(x.transform.FindChild("ShootButton").gameObject);
                        });
                    }
                }
            }));
            i[(int)team]++;
            ind++;
        }
        guesserSelectRole(RoleType.Crewmate);
        ReloadPage();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId">猜测者</param>
    /// <param name="targetId">实际死亡玩家</param>
    /// <param name="guessedId">被猜测玩家</param>
    /// <param name="guessedRoleId">猜测的职业</param>
    public static void guesserShoot(byte playerId, byte targetId, byte guessedId, byte guessedRoleId)
    {
        var player = PlayerById(playerId);
        var target = PlayerById(targetId);
        var guessed = PlayerById(guessedId);
        if (target == null) return;

        var partner = target.GetPartner();
        var partnertRole = partner.GetRoleBase();
        var targetRole = target.GetRoleBase();

        if (targetRole is Mayor mayor && mayor.Revealed && (RoleId)guessedRoleId == RoleId.Mayor)
        {
            return;
        }

        if (player.TryGetRole<Doomsayer>(out var doomsayer) && doomsayer.CanShoot)
        {
            var roleInfo = CustomRoleManager.AllRolesInfo.Values.FirstOrDefault(x => (byte)x.RoleId == guessedRoleId);
            if (!doomsayer.Player.Data.IsDead && guessedId == targetId)
            {
                doomsayer.killedToWin++;
                if (doomsayer.killedToWin >= Doomsayer.killToWin) doomsayer.triggerDoomsayerrWin = true;
                if (guesserUI != null) guesserUIExitButton.OnClick.Invoke();
            }
            else
            {
                seedGuessChat(player, guessed, guessedRoleId);
                return;
            }
        }

        bool lawyerDiedAdditionally = false;
        if (player.TryGetRole<Lawyer>(out var lawyer) && lawyer.Target != null && lawyer.Target.PlayerId == target.PlayerId)
        {
            // Lawyer guessed client.
            if (player.AmOwner)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(player.Data, player.Data);
            }

            player.Exiled();
            lawyerDiedAdditionally = true;
            PlayerData.SetDeathReason(player, CustomDeathReason.LawyerSuicide, player);
        }

        byte partnerId = partner != null ? partner.PlayerId : targetId;

        target.Exiled();
        PlayerData.SetDeathReason(target, CustomDeathReason.Guess, player);
        if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(target.KillSfx, false, 0.8f);

        if (MeetingHud.Instance)
        {
            ExtendMeetingTime(CustomOptionHolder.guessExtendmeetingTime.GetFloat());

            foreach (var pva in MeetingHud.Instance.playerStates)
            {
                bool shouldClearVote = CustomOptionHolder.guessReVote.GetBool()
                    || pva.VotedFor == targetId || pva.VotedFor == partnerId
                    || (lawyerDiedAdditionally && PlayerById(pva.TargetPlayerId).Is(RoleId.Lawyer));

                if (shouldClearVote)
                {
                    pva.UnsetVote();
                    var voteAreaPlayer = PlayerById(pva.TargetPlayerId);
                    if (voteAreaPlayer?.AmOwner == false) continue;
                    MeetingHud.Instance.ClearVote();
                }
            }
            if (AmongUsClient.Instance.AmHost) MeetingHud.Instance.CheckForEndVoting();
        }

        HandleGuesser.remainingShots(playerId, true);

        if (FastDestroyableSingleton<HudManager>.Instance != null && player != null)
        {
            if (PlayerControl.LocalPlayer == target)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(player.Data, target.Data);
            }
            else if (partner != null && PlayerControl.LocalPlayer == partner)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(partner.Data, partner.Data);
            }
        }

        // remove shoot button from targets for all guessers and close their guesserUI
        if (GuesserGM.isGuesser(PlayerControl.LocalPlayer.PlayerId) && PlayerControl.LocalPlayer != player && !PlayerControl.LocalPlayer.Data.IsDead &&
            GuesserGM.remainingShots(PlayerControl.LocalPlayer.PlayerId) > 0 && MeetingHud.Instance)
        {
            MeetingHud.Instance.playerStates.ToList().ForEach(x =>
            {
                if (x.TargetPlayerId == target.PlayerId && x.transform.FindChild("ShootButton") != null)
                    UObject.Destroy(x.transform.FindChild("ShootButton")?.gameObject);
            });

            if (partner != null)
            {
                MeetingHud.Instance.playerStates.ToList().ForEach(x =>
                {
                    if (x.TargetPlayerId == partner.PlayerId && x.transform.FindChild("ShootButton") != null)
                        UObject.Destroy(x.transform.FindChild("ShootButton")?.gameObject);
                });
            }

            if (lawyerDiedAdditionally)
            {
                MeetingHud.Instance.playerStates.ToList().ForEach((Action<PlayerVoteArea>)(x =>
                {
                    if (RoleHelpers.Is(PlayerById(x.TargetPlayerId), RoleId.Lawyer) && x.transform.FindChild("ShootButton") != null)
                        UObject.Destroy(x.transform.FindChild("ShootButton")?.gameObject);
                }));
            }
        }

        if (guesserUI != null && guesserUIExitButton != null) guesserUIExitButton.OnClick.Invoke();
        if (player != null && guessed != null) seedGuessChat(player, guessed, guessedRoleId);
        if (PlayerControl.LocalPlayer.TryGetRole<WolfLord>(out var wolfLord) && !wolfLord.Revealed && PlayerControl.LocalPlayer == player) WolfLord.ClearButton();
    }

    public static void seedGuessChat(PlayerControl player, PlayerControl target, byte role)
    {
        if (PlayerControl.LocalPlayer.IsDead() && PlayerControl.LocalPlayer != Specter.Player)
        {
            var roleInfo = CustomRoleManager.AllRolesInfo.Values.FirstOrDefault(x => (byte)x.RoleId == role);
            var msg = $"{player.Data.PlayerName} 赌怪猜测 {target.Data.PlayerName} 是 {roleInfo?.Name ?? ""}!";
            if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
            {
                _ = new LateTask(() => { FastDestroyableSingleton<HudManager>.Instance!.Chat.AddChat(player, msg); }, 0.1f, "Guess Chat");
            }
        }
    }
}