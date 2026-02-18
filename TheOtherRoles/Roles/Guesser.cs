using TheOtherRoles.Mode;
using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles;

public static class Guesser
{
    public static bool isGuesser(byte playerId)
    {
        if (Assassin.assassin.Any(item => item.PlayerId == playerId && Assassin.assassin != null)) return true;

        return Vigilante.vigilante != null && Vigilante.vigilante.PlayerId == playerId;
    }

    public static void clear(byte playerId)
    {
        if (Vigilante.vigilante != null && Vigilante.vigilante.PlayerId == playerId) Vigilante.vigilante = null;
        Assassin.assassin.RemoveAll(p => p.PlayerId == playerId);
    }

    public static int remainingShots(byte playerId, bool shoot = false)
    {
        if (Vigilante.vigilante != null && Vigilante.vigilante.PlayerId == playerId)
        {
            return shoot ? Mathf.Max(0, --Vigilante.remainingShotsNiceGuesser) : Vigilante.remainingShotsNiceGuesser;
        }

        if (Assassin.assassin != null && Assassin.assassin.Any(x => x.PlayerId == playerId))
        {
            return shoot ? Mathf.Max(0, --Assassin.remainingShotsEvilGuesser) : Assassin.remainingShotsEvilGuesser;
        }

        return 0;
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

    public static void guesserOnClick(int buttonTarget, MeetingHud __instance)
    {
        if (guesserUI != null || __instance.state is MeetingHud.VoteStates.Results or MeetingHud.VoteStates.Discussion) return;
        var targetId = __instance.playerStates[buttonTarget].TargetPlayerId;
        if (PlayerById(targetId) == Jailor.Jailed && Jailor.Player.IsAlive()) return;

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

        guesserCurrentTarget = __instance.playerStates[buttonTarget].TargetPlayerId;

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
        var teamCount = ModOption.AllowGuessModifier ? 4 : 3;
        for (int index = 0; index < teamCount; index++)
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
            Pagelabel.text = GetString(IsNext ? "GuesserUI.Next" : "GuesserUI.Last");
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
        foreach (RoleInfo roleInfo in RoleInfo.allRoleInfos)
        {
            if (roleInfo == null) continue; // Not guessable roles

            if (RoleRate.TryGetValue(roleInfo.roleId, out int isEnabled) && isEnabled == 0)
            {
                continue;
            }

            var guesserRole = PlayerControl.LocalPlayer.PlayerId == Vigilante.vigilante?.PlayerId ? RoleId.Vigilante : RoleId.Assassin;

            if (PlayerControl.LocalPlayer.PlayerId == Doomsayer.doomsayer?.PlayerId)
            {
                if (!Doomsayer.canGuessImpostor && roleInfo.roleType == RoleType.Impostor)
                    continue;
                if (!Doomsayer.canGuessNeutral && roleInfo.roleType == RoleType.Neutral)
                    continue;
            }

            switch (roleInfo.roleId)
            {
                case RoleId.Lover when !CustomOptionHolder.lovesCanBeGuessed.GetBool():
                    continue;
                case RoleId.Disperser when !CustomOptionHolder.disperserCanBeGuessed.GetBool():
                    continue;
                case RoleId.PoucherModifier when !CustomOptionHolder.poucherCanBeGuessed.GetBool():
                    continue;
                case RoleId.ProfessionalModifier when !CustomOptionHolder.professionalCanBeGuessed.GetBool():
                    continue;
                case RoleId.Vortox when !CustomOptionHolder.vortoxCanBeGuessed.GetBool():
                    continue;
                case RoleId.Specoality when !CustomOptionHolder.specoalityCanBeGuessed.GetBool():
                    continue;
                case RoleId.Bloody when !CustomOptionHolder.bloodyCanBeGuessed.GetBool():
                    continue;
                case RoleId.Tiebreaker when !CustomOptionHolder.tieCanBeGuessed.GetBool():
                    continue;
                case RoleId.Bait when !CustomOptionHolder.baitCanBeGuessed.GetBool():
                    continue;
                case RoleId.Aftermath when !CustomOptionHolder.aftermathCanBeGuessed.GetBool():
                    continue;
                case RoleId.Torch when !CustomOptionHolder.torchCanBeGuessed.GetBool():
                    continue;
                case RoleId.Sunglasses when !CustomOptionHolder.sunglassesCanBeGuesser.GetBool():
                    continue;
                case RoleId.Multitasker when !CustomOptionHolder.multitaskerCanBeGuessed.GetBool():
                    continue;
                case RoleId.Vip when !CustomOptionHolder.vipCanBeGuessed.GetBool():
                    continue;
                case RoleId.Slueth when !CustomOptionHolder.sluethCanBeGuessed.GetBool():
                    continue;
                case RoleId.Cursed when !CustomOptionHolder.cursedCanBeGuessed.GetBool():
                    continue;
                case RoleId.Watcher when !CustomOptionHolder.watcherCanBeGuessed.GetBool():
                    continue;
                case RoleId.Radar when !CustomOptionHolder.radarCanBeGuessed.GetBool():
                    continue;
                case RoleId.Tunneler when !CustomOptionHolder.tunnelerCanBeGuessed.GetBool():
                    continue;
                case RoleId.ButtonBarry when !CustomOptionHolder.buttonBarryCanBeGuessed.GetBool():
                    continue;
                case RoleId.Shifter when !CustomOptionHolder.shifterCanBeGuessed.GetBool():
                    continue;
                case RoleId.Assassin:
                    continue;
                case RoleId.LastImpostor:
                    continue;
                case RoleId.AntiTeleport:
                    continue;
                case RoleId.Flash:
                    continue;
                case RoleId.Mini:
                    continue;
                case RoleId.Giant:
                    continue;
                case RoleId.Blind:
                    continue;
                case RoleId.Indomitable:
                    continue;
                case RoleId.Chameleon:
                    continue;
            }

            if (roleInfo.roleType is not RoleType.Crewmate and not RoleType.Neutral and not RoleType.Impostor and not RoleType.Modifier)
                continue;

            // remove all roles that cannot spawn due to the settings from the ui.
            switch (roleInfo.roleId)
            {
                case RoleId.Spy when ModOption.NumImpostors <= 1:
                    continue;
                case RoleId.Poucher when Poucher.spawnModifier:
                    continue;
                case RoleId.Professional when Professional.spawnModifier:
                    continue;
                case RoleId.Crewmate when !Assassin.evilGuesserCanGuessCrewmate && guesserRole == RoleId.Assassin:
                    continue;
                case RoleId.Spy when PlayerControl.LocalPlayer.IsImpostor() && !Spy.EvilCanKillSpy:
                    continue;
                case RoleId.Mayor when Mayor.Revealed:
                    continue;
                case RoleId.WolfLord when WolfLord.Revealed:
                    continue;
                case RoleId.Vigilante when HandleGuesser.isGuesserGm || PlayerControl.LocalPlayer.PlayerId == Vigilante.vigilante?.PlayerId:
                    continue;
                case RoleId.Sidekick when !CustomOptionHolder.jackalCanCreateSidekick.GetBool():
                    continue;
                case RoleId.BandLeader:
                    continue;
                case RoleId.Avenger when !Avenger.IsGuessable:
                    continue;
                case RoleId.SchrodingersCat when !CustomOptionHolder.schrodingersCatIsGuessable.GetBool():
                    continue;
                case RoleId.Doomsayer when PlayerControl.LocalPlayer.PlayerId == Doomsayer.doomsayer?.PlayerId:
                    continue;
            }

            if (Snitch.snitch != null && Snitch.CanGuessIfTaksDone)
            {
                var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);
                var numberOfLeftTasks = playerTotal - playerCompleted;
                if (numberOfLeftTasks <= Snitch.taskCountForReveal && roleInfo.roleId == RoleId.Snitch) continue;
            }

            CreateRole(roleInfo);
        }
        #endregion

        void CreateRole(RoleInfo roleInfo = null)
        {
            if (roleInfo.roleType is RoleType.Ghost or RoleType.Special) return;
            RoleType team = roleInfo?.roleType ?? RoleType.Crewmate;
            //Color color = roleInfo?.color ?? Color.white;
            //RoleId role = roleInfo?.roleId ?? RoleId.Crewmate;

            if (roleInfo.roleId == RoleId.SchrodingersCat)
            {
                foreach (SchrodingersCat.CatState state in Enum.GetValues(typeof(SchrodingersCat.CatState)))
                {
                    if (CustomOptionHolder.schrodingersCatIsGuessable.GetSelection() == 1 && state == SchrodingersCat.CatState.None) continue;
                    CreateSchrodingerButton(team, state);
                }
                return;
            }
            void CreateSchrodingerButton(RoleType team, SchrodingersCat.CatState catState)
            {
                var buttonParent = new GameObject().transform;
                buttonParent.SetParent(container);
                var button = UObject.Instantiate(buttonTemplate, buttonParent);
                button.FindChild("ControllerHighlight").gameObject.SetActive(false);
                var buttonMask = UObject.Instantiate(maskTemplate, buttonParent);
                var label = UObject.Instantiate(textTemplate, button);
                button.GetComponent<SpriteRenderer>().sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;

                if (!RoleButtons.ContainsKey(team)) RoleButtons.Add(team, new());
                RoleButtons[team].Add(button);
                buttons.Add(button);

                int row = i[(int)team] / 5;
                int col = i[(int)team] % 5;
                buttonParent.localPosition = new Vector3(-3.47f + (1.75f * col), 1.5f - (0.45f * row), -200f);
                buttonParent.localScale = new Vector3(0.55f, 0.55f, 1f);

                label.text = catState == SchrodingersCat.CatState.None
                    ? Cs(roleInfo.color, roleInfo.Name)
                    : Cs(SchrodingersCat.getColor(catState), $"{roleInfo.Name}({GetString(catState.ToString())})");
                label.alignment = TextAlignmentOptions.Center;
                label.transform.localPosition = new Vector3(0, 0, label.transform.localPosition.z);
                label.transform.localScale *= 1.6f;
                label.autoSizeTextContainer = true;

                i[(int)team]++;

                button.GetComponent<PassiveButton>().OnClick.RemoveAllListeners();
                button.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() =>
                {
                    if (PlayerControl.LocalPlayer.IsDead()) return;

                    if (selectedButton != button)
                    {
                        selectedButton = button;
                        buttons.ForEach(x => x.GetComponent<SpriteRenderer>().color = x == selectedButton ? Color.red : Color.white);
                    }
                    else
                    {
                        var focusedTarget = PlayerById(__instance.playerStates[buttonTarget].TargetPlayerId);
                        CheckGuesserShoot(focusedTarget, (byte)catState, __instance);
                    }

                }));
            }
            var buttonParent = new GameObject().transform;
            buttonParent.SetParent(container);
            var button = UObject.Instantiate(buttonTemplate, buttonParent);
            button.FindChild("ControllerHighlight").gameObject.SetActive(false);
            var buttonMask = UObject.Instantiate(maskTemplate, buttonParent);
            var label = UObject.Instantiate(textTemplate, button);
            button.GetComponent<SpriteRenderer>().sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
            if (!RoleButtons.ContainsKey(team))
            {
                RoleButtons.Add(team, new());
            }
            RoleButtons[team].Add(button);
            buttons.Add(button);
            int row = i[(int)team] / 5;
            int col = i[(int)team] % 5;
            buttonParent.localPosition = new Vector3(-3.47f + (1.75f * col), 1.5f - (0.45f * row), -200f);
            buttonParent.localScale = new Vector3(0.55f, 0.55f, 1f);
            label.text = Cs(roleInfo.color, roleInfo.Name);
            label.alignment = TextAlignmentOptions.Center;
            label.transform.localPosition = new Vector3(0, 0, label.transform.localPosition.z);
            label.transform.localScale *= 1.6f;
            label.autoSizeTextContainer = true;
            int copiedIndex = i[(int)team];

            button.GetComponent<PassiveButton>().OnClick.RemoveAllListeners();
            button.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() =>
            {
                if (PlayerControl.LocalPlayer.IsDead()) return;

                if (selectedButton != button)
                {
                    selectedButton = button;
                    buttons.ForEach(x => x.GetComponent<SpriteRenderer>().color = x == selectedButton ? Color.red : Color.white);
                }
                else
                {
                    var focusedTarget = PlayerById(__instance.playerStates[buttonTarget].TargetPlayerId);
                    CheckGuesserShoot(focusedTarget, (byte)roleInfo.roleId, __instance);
                }
            }));
            i[(int)team]++;
            ind++;
        }
        guesserSelectRole(RoleType.Crewmate);
        ReloadPage();
    }


    public static void CheckGuesserShoot(PlayerControl target, byte roleId, MeetingHud __instance)
    {
        var dyingTarget = PlayerControl.LocalPlayer;
        var mainRoleInfo = RoleInfo.getRoleInfoForPlayer(target, true);

        if (__instance.state is MeetingHud.VoteStates.Discussion or MeetingHud.VoteStates.Results
            || target == null
            || PlayerControl.LocalPlayer.IsDead()
            || (HandleGuesser.remainingShots(PlayerControl.LocalPlayer.PlayerId) <= 0 && HandleGuesser.isGuesser(PlayerControl.LocalPlayer.PlayerId))
            || (PlayerControl.LocalPlayer == Doomsayer.doomsayer && !Doomsayer.CanShoot))
            return;

        if (!PlayerControl.LocalPlayer.CanUseMeetingAbility() || dyingTarget == Jailor.Jailed) return;

        if (Medic.GuessShield && target == Medic.shielded)
        {
            // Depending on the options, shooting the shielded player will not allow the guess, notifiy everyone about the kill attempt and close the window
            __instance.playerStates.ForEach(x => x.gameObject.SetActive(true));
            if (guesserUI != null) guesserUIExitButton.OnClick.Invoke();

            var murderAttemptWriter = StartRPC(CustomRPC.ShieldedMurderAttempt);
            murderAttemptWriter.EndRPC();
            RPCProcedure.shieldedMurderAttempt(0);
            SoundEffectsManager.play("fail");
            seedGuessChat(PlayerControl.LocalPlayer, target, roleId, true, "但是对方被法医保护了！");
            return;
        }
        if (target == Indomitable.indomitable)
        {
            Coroutines.Start(showFlashCoroutine(new(255, 197, 97), 1.25f, 0.4f));
            __instance.playerStates.ForEach(x => x.gameObject.SetActive(true));
            if (guesserUI != null) guesserUIExitButton.OnClick.Invoke();
            seedGuessChat(PlayerControl.LocalPlayer, target, roleId, true, "但是对方是不屈者！");
            return;
        }

        if (target.IsDead()) return;

        if (mainRoleInfo == null) return;

        foreach (var role in mainRoleInfo)
        {
            if (roleId >= (byte)SchrodingersCat.CatState.None && role.roleId == RoleId.SchrodingersCat)
            {
                if ((byte)SchrodingersCat.State == roleId)
                {
                    dyingTarget = target;
                }
                continue;
            }
            else if (role.roleId == (RoleId)roleId)
            {
                dyingTarget = target;
                continue;
            }
        }

        if (target == Oracle.Confesser && Oracle.CanNotGuessConfess && Oracle.Player.IsAlive())
        {
            if (guesserUI != null) guesserUIExitButton.OnClick.Invoke();
            Coroutines.Start(showFlashCoroutine(Oracle.color, 1.25f, 0.4f));
            seedGuessChat(PlayerControl.LocalPlayer, target, roleId, true, "但是对方被神谕者保护了！");
            return;
        }

        if (Specoality.specoality != null && PlayerControl.LocalPlayer == Specoality.specoality && Specoality.linearfunction > 0)
        {
            if (Specoality.specoality.IsAlive() && target != dyingTarget)
            {
                if (guesserUI != null) guesserUIExitButton.OnClick.Invoke();
                seedGuessChat(PlayerControl.LocalPlayer, target, roleId, true, "但猜测错误");

                Coroutines.Start(showFlashCoroutine(Color.red, 1.25f, 0.4f));
                Specoality.linearfunction--;
                SoundEffectsManager.play("fail");

                __instance.playerStates.ForEach(x =>
                {
                    if (x.TargetPlayerId == target.PlayerId && x.transform.FindChild("ShootButton") != null)
                    {
                        UObject.Destroy(x.transform.FindChild("ShootButton").gameObject);
                    }
                });
                return;
            }
        }

        // Shoot player and send chat info if activated
        var writer = StartRPC(CustomRPC.GuesserShoot);
        writer.Write(PlayerControl.LocalPlayer.PlayerId);   // 猜测者
        writer.Write(dyingTarget.PlayerId);                 // 实际死亡玩家
        writer.Write(target.PlayerId);                      // 猜测的玩家
        writer.Write(roleId);                               // 猜测的职业
        writer.EndRPC();
        guesserShoot(PlayerControl.LocalPlayer.PlayerId, dyingTarget.PlayerId, target.PlayerId, roleId);

        // Reset the GUI
        __instance.playerStates.ForEach(x => x.gameObject.SetActive(true));
        guesserUI?.gameObject?.Destroy();
        if (HandleGuesser.CanMultipleShots(dyingTarget))
        {
            foreach (var pva in __instance.playerStates)
            {
                var button = pva.transform.FindChild("ShootButton");

                if (pva.TargetPlayerId == dyingTarget.PlayerId && button != null)
                    button?.gameObject?.Destroy();
            }
        }
        else
        {
            foreach (var pva in __instance.playerStates)
            {
                pva.transform.FindChild("ShootButton")?.gameObject?.Destroy();
            }
        }
    }

    public static void guesserShoot(byte killerId, byte dyingTargetId, byte guessedTargetId, byte guessedRoleId)
    {
        var dyingTarget = PlayerById(dyingTargetId);
        var guessedTarget = PlayerById(guessedTargetId);
        var guesser = PlayerById(killerId);
        if (dyingTarget == null) return;

        var dyingPartner = Akujo.otherLover(dyingTarget);

        // Lawyer shouldn't be exiled with the client for guesses
        if (Lawyer.target != null && (dyingTarget == Lawyer.target || dyingPartner == Lawyer.target))
            Lawyer.targetWasGuessed = true;

        if (Executioner.target != null && (dyingTarget == Executioner.target || dyingPartner == Executioner.target))
            Executioner.targetWasGuessed = true;

        if (Witch.witch != null && (dyingTarget == Witch.witch || dyingPartner == Witch.witch))
            Witch.witchWasGuessed = true;

        if (Thief.thief != null && Thief.thief.PlayerId == killerId && Thief.canStealWithGuess)
        {
            if (Thief.thief.IsAlive() && Thief.tiefCanKill(dyingTarget))
                Thief.StealsRole(dyingTarget.PlayerId);
        }

        if (Doomsayer.doomsayer != null && Doomsayer.doomsayer == guesser && Doomsayer.canGuess)
        {
            if (Doomsayer.doomsayer.IsAlive() && guessedTargetId == dyingTargetId)
            {
                Doomsayer.killedToWin++;
                if (Doomsayer.killedToWin >= Doomsayer.killToWin) Doomsayer.triggerDoomsayerrWin = true;
                if (guesserUI != null) guesserUIExitButton.OnClick.Invoke();
            }
            else
            {
                Doomsayer.CanShoot = false;
                seedGuessChat(guesser, guessedTarget, guessedRoleId);
                MeetingHud.Instance.playerStates.ForEach(x =>
                {
                    if (x.TargetPlayerId == Lawyer.lawyer?.PlayerId && x.transform.FindChild("ShootButton") != null)
                        UObject.Destroy(x.transform.FindChild("ShootButton")?.gameObject);
                });
                return;
            }
        }

        bool lawyerDiedAdditionally = false;
        if (Lawyer.lawyer != null && Lawyer.lawyer.PlayerId == killerId && Lawyer.target != null && Lawyer.target.PlayerId == dyingTargetId)
        {
            // Lawyer guessed client.
            if (PlayerControl.LocalPlayer == Lawyer.lawyer)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(Lawyer.lawyer.Data, Lawyer.lawyer.Data);
            }

            Lawyer.lawyer.Exiled();
            lawyerDiedAdditionally = true;
            PlayerData.SetDeathReason(Lawyer.lawyer, CustomDeathReason.LawyerSuicide, guesser);
        }

        byte partnerId = dyingPartner != null ? dyingPartner.PlayerId : dyingTargetId;
        dyingTarget.CustomExiled(guesser);

        var reason = dyingTarget == guesser ? CustomDeathReason.GuessFail : CustomDeathReason.GuessSuccess;
        PlayerData.SetDeathReason(dyingTarget, reason, guesser);

        if (dyingTarget == Balancer.currentTarget) Balancer.currentTarget = null;

        if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(dyingTarget.KillSfx, false, 0.8f);

        if (MeetingHud.Instance)
        {
            ExtendMeetingTime(CustomOptionHolder.guessExtendmeetingTime.GetFloat());
            MeetingHudPatch.swapperCheckAndReturnSwap(MeetingHud.Instance, dyingTargetId);

            foreach (var pva in MeetingHud.Instance.playerStates)
            {
                bool shouldClearVote = CustomOptionHolder.guessReVote.GetBool()
                    || pva.VotedFor == dyingTargetId || pva.VotedFor == partnerId
                    || (lawyerDiedAdditionally && Lawyer.lawyer?.PlayerId == pva.TargetPlayerId);

                if (shouldClearVote || Jailor.Jailed?.AmOwner == true || Blackmailer.blackmailed?.AmOwner == true)
                {
                    pva.UnsetVote();
                    var voteAreaPlayer = PlayerById(pva.TargetPlayerId);
                    if (voteAreaPlayer?.AmOwner == false) continue;
                    MeetingHud.Instance.ClearVote();
                }
            }
            if (AmongUsClient.Instance.AmHost) MeetingHud.Instance.CheckForEndVoting();
        }

        HandleGuesser.remainingShots(killerId, true);

        if (FastDestroyableSingleton<HudManager>.Instance != null && guesser != null)
        {
            if (PlayerControl.LocalPlayer == dyingTarget)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(guesser.Data, dyingTarget.Data);
            }
            else if (dyingPartner != null && PlayerControl.LocalPlayer == dyingPartner)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(dyingPartner.Data, dyingPartner.Data);
            }
        }

        // remove shoot button from targets for all guessers and close their guesserUI
        if (GuesserGM.isGuesser(PlayerControl.LocalPlayer.PlayerId) && PlayerControl.LocalPlayer != guesser && !PlayerControl.LocalPlayer.Data.IsDead &&
            GuesserGM.remainingShots(PlayerControl.LocalPlayer.PlayerId) > 0 && MeetingHud.Instance)
        {
            MeetingHud.Instance.playerStates.ToList().ForEach(x =>
            {
                if (x.TargetPlayerId == dyingTarget.PlayerId && x.transform.FindChild("ShootButton") != null)
                    UObject.Destroy(x.transform.FindChild("ShootButton")?.gameObject);
            });

            if (dyingPartner != null)
            {
                MeetingHud.Instance.playerStates.ToList().ForEach(x =>
                {
                    if (x.TargetPlayerId == dyingPartner.PlayerId && x.transform.FindChild("ShootButton") != null)
                        UObject.Destroy(x.transform.FindChild("ShootButton")?.gameObject);
                });
            }

            if (lawyerDiedAdditionally)
            {
                MeetingHud.Instance.playerStates.ForEach(x =>
                {
                    if (x.TargetPlayerId == Lawyer.lawyer?.PlayerId && x.transform.FindChild("ShootButton") != null)
                        UObject.Destroy(x.transform.FindChild("ShootButton")?.gameObject);
                });
            }
        }

        if (guesserUI != null && guesserUIExitButton != null) guesserUIExitButton.OnClick.Invoke();
        if (guesser != null && guessedTarget != null) seedGuessChat(guesser, guessedTarget, guessedRoleId);
        if (WolfLord.Player == guesser && !WolfLord.Revealed && PlayerControl.LocalPlayer == guesser) WolfLord.WolfLord_Patch.ClearButton();
    }

    public static void seedGuessChat(PlayerControl guesser, PlayerControl guessedTarget, byte guessedRoleId, bool rpcSend = false, string text = "")
    {
        if (CanSeeGhostInfo || PlayerControl.LocalPlayer == guesser || ModOption.DebugMode)
        {
            var msg = "";
            if (guessedRoleId >= (byte)SchrodingersCat.CatState.None)
            {
                var state = (SchrodingersCat.CatState)guessedRoleId;

                var name = state == SchrodingersCat.CatState.None
                    ? RoleInfo.schrodingersCat.Name
                    : $"{RoleInfo.schrodingersCat.Name}({GetString(state.ToString())})";
                msg = string.Format(GetString("GuesserUI.GuessChat"), guesser.Data.PlayerName, guessedTarget.Data.PlayerName, name);
            }
            else
            {
                var roleInfo = RoleInfo.RoleInfoById.GetValueOrDefault((RoleId)guessedRoleId);
                msg = string.Format(GetString("GuesserUI.GuessChat"), guesser.Data.PlayerName, guessedTarget.Data.PlayerName, roleInfo?.Name);
            }

            msg += $"\n{text}";

            if (FastDestroyableSingleton<HudManager>.Instance)
            {
                _ = new LateTask(() =>
                {
                    ChatControllerPatch.CurrentChatType = ChatControllerPatch.ChatTypes.GuesserMessage;
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(guesser, msg);

                }, 0.1f, "Guesser Chat");
            }

            if (rpcSend && PlayerControl.LocalPlayer == guesser)
            {
                var writer = StartRPC(CustomRPC.GuesserMessage);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(guessedTarget.PlayerId);
                writer.Write(guessedRoleId);
                writer.EndRPC();
            }
        }
    }
}