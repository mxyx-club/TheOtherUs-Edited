using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TheOtherRoles.Patches;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Roles;

public static class Guesser
{
    public static PlayerControl niceGuesser;

    public static List<PlayerControl> evilGuesser = new();
    public static Color color = new Color32(255, 255, 0, byte.MaxValue);

    public static bool guesserCantGuessSnitch;
    public static bool showInfoInGhostChat = true;
    public static bool killsThroughShield = true;
    public static int remainingShotsNiceGuesser = 2;
    public static bool hasMultipleShotsPerMeeting;

    public static int remainingShotsEvilGuesser = 2;
    public static bool assassinMultipleShotsPerMeeting;
    public static bool assassinKillsThroughShield = true;
    public static bool evilGuesserCanGuessSpy = true;
    public static bool evilGuesserCanGuessCrewmate = true;

    public static bool isGuesser(byte playerId)
    {
        if (evilGuesser.Any(item => item.PlayerId == playerId && evilGuesser != null)) return true;

        return niceGuesser != null && niceGuesser.PlayerId == playerId;
    }

    public static void clear(byte playerId)
    {
        if (niceGuesser != null && niceGuesser.PlayerId == playerId) niceGuesser = null;
        foreach (var item in evilGuesser.Where(item => item.PlayerId == playerId && evilGuesser != null))
            evilGuesser = null;
    }

    public static int remainingShots(byte playerId, bool shoot = false)
    {
        var result = remainingShotsEvilGuesser;
        if (niceGuesser != null && niceGuesser.PlayerId == playerId)
        {
            result = remainingShotsNiceGuesser;
            if (shoot) remainingShotsNiceGuesser = Mathf.Max(0, remainingShotsNiceGuesser - 1);
        }
        else if (shoot)
        {
            remainingShotsEvilGuesser = Mathf.Max(0, remainingShotsEvilGuesser - 1);
        }
        return result;
    }

    public static void clearAndReload()
    {
        niceGuesser = null;
        evilGuesser = new List<PlayerControl>();

        guesserCantGuessSnitch = CustomOptionHolder.guesserCantGuessSnitchIfTaksDone.getBool();
        showInfoInGhostChat = CustomOptionHolder.guesserShowInfoInGhostChat.getBool();
        killsThroughShield = CustomOptionHolder.guesserKillsThroughShield.getBool();
        remainingShotsNiceGuesser = Mathf.RoundToInt(CustomOptionHolder.guesserNumberOfShots.getFloat());
        hasMultipleShotsPerMeeting = CustomOptionHolder.guesserHasMultipleShotsPerMeeting.getBool();

        remainingShotsEvilGuesser = Mathf.RoundToInt(CustomOptionHolder.modifierAssassinNumberOfShots.getFloat());
        assassinMultipleShotsPerMeeting = CustomOptionHolder.modifierAssassinMultipleShotsPerMeeting.getBool();
        assassinKillsThroughShield = CustomOptionHolder.modifierAssassinKillsThroughShield.getBool();
        evilGuesserCanGuessSpy = CustomOptionHolder.guesserEvilCanKillSpy.getBool();
        evilGuesserCanGuessCrewmate = CustomOptionHolder.guesserEvilCanKillCrewmate.getBool();
    }

    public const int MaxOneScreenRole = 40;
    private static Dictionary<RoleTeam, List<Transform>> RoleButtons;
    private static Dictionary<RoleTeam, SpriteRenderer> RoleSelectButtons;
    public static RoleTeam currentTeamType;
    private static List<SpriteRenderer> PageButtons;
    public static GameObject guesserUI;
    public static PassiveButton guesserUIExitButton;
    public static int Page;
    public static byte guesserCurrentTarget;
    static void guesserSelectRole(RoleTeam Role, bool SetPage = true)
    {
        currentTeamType = Role;
        if (SetPage) Page = 1;
        foreach (var RoleButton in RoleButtons)
        {
            int index = 0;
            foreach (var RoleBtn in RoleButton.Value)
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
        if (guesserUI != null || !(__instance.state is MeetingHud.VoteStates.Voted
            or MeetingHud.VoteStates.NotVoted or MeetingHud.VoteStates.Discussion)) return;

        Page = 1;
        RoleButtons = new();
        RoleSelectButtons = new();
        PageButtons = new();

        __instance.playerStates.ForEach(x => x.gameObject.SetActive(false));

        Transform PhoneUI = Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");
        Transform container = Object.Instantiate(PhoneUI, __instance.transform);
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
        var exitButton = Object.Instantiate(buttonTemplate.transform, exitButtonParent);
        var exitButtonMask = Object.Instantiate(maskTemplate, exitButtonParent);
        exitButton.gameObject.GetComponent<SpriteRenderer>().sprite = smallButtonTemplate.GetComponent<SpriteRenderer>().sprite;
        var transform = exitButtonParent.transform;
        transform.localPosition = new Vector3(3f, 2.1f, -5);
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
        var teamCount = MapOption.allowModGuess ? 4 : 3;
        // From SuperNewRoles
        for (int index = 0; index < teamCount; index++)
        {
            Transform TeambuttonParent = new GameObject().transform;
            TeambuttonParent.SetParent(container);
            Transform Teambutton = Object.Instantiate(buttonTemplate, TeambuttonParent);
            Teambutton.FindChild("ControllerHighlight").gameObject.SetActive(false);
            Transform TeambuttonMask = Object.Instantiate(maskTemplate, TeambuttonParent);
            TextMeshPro Teamlabel = Object.Instantiate(textTemplate, Teambutton);
            //Teambutton.GetComponent<SpriteRenderer>().sprite = FastDestroyableSingleton<HatManager>.Instance.GetNamePlateById("nameplate_NoPlate")?.viewData?.viewData?.Image;
            RoleSelectButtons.Add((RoleTeam)index, Teambutton.GetComponent<SpriteRenderer>());
            TeambuttonParent.localPosition = new(-2.75f + (index * 1.75f), 2.225f, -200);
            TeambuttonParent.localScale = new(0.55f, 0.55f, 1f);
            Teamlabel.color = getTeamColor((RoleTeam)index);
            //Info($"{Teamlabel.color} {(RoleTeam)index}");
            Teamlabel.text = getString(((RoleTeam)index is RoleTeam.Crewmate ? "Crewmate" : ((RoleTeam)index).ToString()) + "RolesText");
            Teamlabel.alignment = TextAlignmentOptions.Center;
            Teamlabel.transform.localPosition = new Vector3(0, 0, Teamlabel.transform.localPosition.z);
            Teamlabel.transform.localScale *= 1.6f;
            Teamlabel.autoSizeTextContainer = true;
            static void CreateTeamButton(Transform Teambutton, RoleTeam type)
            {
                Teambutton.GetComponent<PassiveButton>().OnClick.AddListener((UnityEngine.Events.UnityAction)(() =>
                {
                    guesserSelectRole(type);
                    ReloadPage();
                }));
            }
            if (!PlayerControl.LocalPlayer.Data.IsDead) CreateTeamButton(Teambutton, (RoleTeam)index);
        }

        static void ReloadPage()
        {
            PageButtons[0].color = new(1, 1, 1, 1f);
            PageButtons[1].color = new(1, 1, 1, 1f);
            if ((RoleButtons[currentTeamType].Count / MaxOneScreenRole + (RoleButtons[currentTeamType].Count % MaxOneScreenRole != 0 ? 1 : 0)) < Page)
            {
                Page -= 1;
                PageButtons[1].color = new(1, 1, 1, 0.1f);
            }
            else if ((RoleButtons[currentTeamType].Count / MaxOneScreenRole + (RoleButtons[currentTeamType].Count % MaxOneScreenRole != 0 ? 1 : 0)) < Page + 1)
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
            Transform Pagebutton = Object.Instantiate(buttonTemplate, PagebuttonParent);
            Pagebutton.FindChild("ControllerHighlight").gameObject.SetActive(false);
            Transform PagebuttonMask = Object.Instantiate(maskTemplate, PagebuttonParent);
            TextMeshPro Pagelabel = Object.Instantiate(textTemplate, Pagebutton);
            // Pagebutton.GetComponent<SpriteRenderer>().sprite = FastDestroyableSingleton<HatManager>.Instance.GetNamePlateById("nameplate_NoPlate")?.viewData?.viewData?.Image;
            PagebuttonParent.localPosition = IsNext ? new(3.535f, -2.2f, -200) : new(-3.475f, -2.2f, -200);
            PagebuttonParent.localScale = new(0.55f, 0.55f, 1f);
            Pagelabel.color = Color.white;
            Pagelabel.text = getString(IsNext ? "下一页" : "上一页");
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

            var guesserRole = niceGuesser != null && CachedPlayer.LocalPlayer.PlayerId == niceGuesser.PlayerId
                ? RoleId.NiceGuesser
                : RoleId.EvilGuesser;

            if (Doomsayer.doomsayer != null && CachedPlayer.LocalPlayer.PlayerId == Doomsayer.doomsayer.PlayerId) guesserRole = RoleId.Doomsayer;

            switch (guesserRole)
            {
                case RoleId.Doomsayer when !Doomsayer.canGuessImpostor && roleInfo.roleTeam == RoleTeam.Impostor:
                case RoleId.Doomsayer when !Doomsayer.canGuessNeutral && roleInfo.roleTeam == RoleTeam.Neutral:
                    continue;
            }

            if (Mayor.mayor != null && Mayor.Revealed && Mayor.Revealed && roleInfo.roleId == RoleId.Mayor) continue;

            if (roleInfo.roleId == RoleId.Poucher) continue;

            if (roleInfo.roleTeam == RoleTeam.Modifier)
            {
                // Allow Guessing the following mods: Bait, TieBreaker, Bloody, and VIP
                if (MapOption.allowModGuess && !roleInfo.isGuessable) continue;
            }

            if (roleInfo.roleId == guesserRole ||
                (!HandleGuesser.evilGuesserCanGuessSpy && guesserRole == RoleId.EvilGuesser &&
                roleInfo.roleId == RoleId.Spy && !HandleGuesser.isGuesserGm) ||
                (!evilGuesserCanGuessCrewmate && guesserRole == RoleId.EvilGuesser &&
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
            CreateRole(roleInfo);
        }
        #endregion

        void CreateRole(RoleInfo roleInfo = null)
        {
            RoleTeam team = roleInfo?.roleTeam ?? RoleTeam.Crewmate;
            Color color = roleInfo?.color ?? Color.white;
            string NameKey = roleInfo?.name ?? "Crewmate";
            RoleId role = roleInfo?.roleId ?? RoleId.Crewmate;

            Transform buttonParent = new GameObject().transform;
            buttonParent.SetParent(container);
            Transform button = Object.Instantiate(buttonTemplate, buttonParent);
            button.FindChild("ControllerHighlight").gameObject.SetActive(false);
            Transform buttonMask = Object.Instantiate(maskTemplate, buttonParent);
            TextMeshPro label = Object.Instantiate(textTemplate, button);
            // button.GetComponent<SpriteRenderer>().sprite = FastDestroyableSingleton<HatManager>.Instance.GetNamePlateById("nameplate_NoPlate")?.viewData?.viewData?.Image;
            if (!RoleButtons.ContainsKey(team))
            {
                RoleButtons.Add(team, new());
            }
            RoleButtons[team].Add(button);
            buttons.Add(button);
            int row = i[(int)team] / 5;
            int col = i[(int)team] % 5;
            buttonParent.localPosition = new Vector3(-3.47f + 1.75f * col, 1.5f - 0.45f * row, -200f);
            buttonParent.localScale = new Vector3(0.55f, 0.55f, 1f);
            label.text = cs(roleInfo.color, roleInfo.name);
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
                        __instance.playerStates.ForEach(x => x.gameObject.SetActive(true));
                        Object.Destroy(container.gameObject);

                        var murderAttemptWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                            (byte)CustomRPC.ShieldedMurderAttempt, SendOption.Reliable);
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

                        var murderAttemptWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                            (byte)CustomRPC.ShieldedMurderAttempt, SendOption.Reliable);
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

                    // Shoot player and send chat info if activated
                    var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                        (byte)CustomRPC.GuesserShoot, SendOption.Reliable);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer.Write(dyingTarget.PlayerId);
                    writer.Write(focusedTarget.PlayerId);
                    writer.Write((byte)roleInfo.roleId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.guesserShoot(CachedPlayer.LocalPlayer.PlayerId, dyingTarget.PlayerId, focusedTarget.PlayerId, (byte)roleInfo.roleId);

                    // Reset the GUI
                    __instance.playerStates.ForEach(x => x.gameObject.SetActive(true));
                    Object.Destroy(container.gameObject);
                    if (CanMultipleShots(dyingTarget))
                    {
                        __instance.playerStates.ForEach(x =>
                        {
                            if (x.TargetPlayerId == dyingTarget.PlayerId && x.transform.FindChild("ShootButton") != null)
                                Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
                        });
                    }
                    else
                    {
                        __instance.playerStates.ToList().ForEach(x =>
                        {
                            if (x.transform.FindChild("ShootButton") != null)
                                Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
                        });
                    }
                }
            }));
            i[(int)team]++;
            ind++;
        }
        guesserSelectRole(RoleTeam.Crewmate);
        ReloadPage();

        #region
        /*
        foreach (var roleInfo in RoleInfo.allRoleInfos)
        {
            var guesserRole = niceGuesser != null && CachedPlayer.LocalPlayer.PlayerId == niceGuesser.PlayerId
                    ? RoleId.NiceGuesser
                    : RoleId.EvilGuesser;

            if (Doomsayer.doomsayer != null && CachedPlayer.LocalPlayer.PlayerId == Doomsayer.doomsayer.PlayerId) guesserRole = RoleId.Doomsayer;

            switch (guesserRole)
            {
                case RoleId.Doomsayer when !Doomsayer.canGuessImpostor && roleInfo.roleTeam == RoleTeam.Impostor:
                case RoleId.Doomsayer when !Doomsayer.canGuessNeutral && roleInfo.roleTeam == RoleTeam.Neutral:
                    continue;
            }

            if (roleInfo.roleId == RoleId.Poucher && Poucher.spawnModifier) continue;
            if (Mayor.mayor != null && Mayor.Revealed && Mayor.Revealed && roleInfo.roleId == RoleId.Mayor) continue;

            if (MapOption.allowModGuess && roleInfo.isModifier)
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
                (!evilGuesserCanGuessCrewmate && guesserRole == RoleId.EvilGuesser &&
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

                            var murderAttemptWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                                (byte)CustomRPC.ShieldedMurderAttempt, SendOption.Reliable);
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

                            var murderAttemptWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                                (byte)CustomRPC.ShieldedMurderAttempt, SendOption.Reliable);
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
                        __instance.playerStates.ForEach(x => x.gameObject.SetActive(true));
                        Object.Destroy(container.gameObject);
                        if (CanMultipleShots(dyingTarget))
                        {
                            __instance.playerStates.ForEach(x =>
                            {
                                if (x.TargetPlayerId == dyingTarget.PlayerId && x.transform.FindChild("ShootButton") != null)
                                    Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
                            });
                        }
                        else
                        {
                            __instance.playerStates.ToList().ForEach(x =>
                            {
                                if (x.transform.FindChild("ShootButton") != null)
                                    Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
                            });
                        }
                    }
                }));

            i++;
        }*/
        #endregion
    }

}