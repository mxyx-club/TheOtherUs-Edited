using UnityEngine.UI;

namespace TheOtherRoles.Modules;

public static class LobbyRoleInfo
{
    public static GameObject RolesSummaryUI { get; set; }
    private static TextMeshPro infoButtonText;
    private static TextMeshPro infoTitleText;

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class RoleSummaryButtonHudUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (!LobbyBehaviour.Instance || AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started) return;
            try
            {
                if (HudManagerStartPatch.roleSummaryButton == null) HudManagerStartPatch.createRoleSummaryButton(__instance);
                //if (HudManagerStartPatch.roleSummaryButton.Timer > 0f) HudManagerStartPatch.roleSummaryButton.Timer = 0f;
                HudManagerStartPatch.roleSummaryButton.Update();
                HudManagerStartPatch.gameModeButton.Update();
            }
            catch { }
        }
    }

    public static void RoleSummaryOnClick()
    {
        if (RolesSummaryUI != null)
        {
            UObject.Destroy(RolesSummaryUI);
            RolesSummaryUI = null;
            return;
        }

        var container = new GameObject("RoleSummaryMenuContainer").AddComponent<SpriteRenderer>();
        container.sprite = new ResourceSprite("LobbyRoleInfo.TeamScreen.png", 110f);
        container.transform.SetParent(HudManager.Instance.transform);
        container.gameObject.transform.SetLocalZ(-200);
        container.transform.localPosition = new Vector3(0, -0.2f, -50f);
        container.transform.localScale = new Vector3(.75f, .7f, 1f);
        container.gameObject.layer = 5;

        RolesSummaryUI = container.gameObject;

        var buttonTemplate = HudManager.Instance.SettingsButton.transform;
        var textTemplate = HudManager.Instance.TaskPanel.taskText;

        var newtitle = UObject.Instantiate(textTemplate, container.transform);
        newtitle.text = GetString("lobbyInfoSummary");
        newtitle.color = Color.white;
        newtitle.outlineWidth = 0.05f;
        newtitle.transform.localPosition = new Vector3(1.05f, 0.17f, -2f);
        newtitle.transform.localScale = Vector3.one * 2.5f;

        // 添加退出按钮
        var exitButtonTransform = UObject.Instantiate(buttonTemplate, container.transform);
        exitButtonTransform.name = "RolesSummaryUIExit";
        exitButtonTransform.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
        exitButtonTransform.GetComponent<SpriteRenderer>().sprite = new ResourceSprite("ExitButton.png", 135f);
        exitButtonTransform.localPosition = new Vector3(4.4f, 1.3f, -5);
        exitButtonTransform.localScale = new Vector3(1f, 1.05f, 1f);

        var exitButton = exitButtonTransform.GetComponent<PassiveButton>();
        var exitOnClick = exitButton.OnClick = new Button.ButtonClickedEvent();
        exitOnClick.AddListener((Action)(() =>
        {
            UObject.Destroy(RolesSummaryUI);
        }));

        List<Transform> buttons = new();

        foreach (RoleType teamId in Enum.GetValues(typeof(RoleType)))
        {
            if (teamId is RoleType.Special or RoleType.Error) continue;

            var buttonTransform = UObject.Instantiate(buttonTemplate, container.transform);
            buttonTransform.name = teamId.ToString() + "Button";
            buttonTransform.GetComponent<BoxCollider2D>().size = new Vector2(2.5f, 0.55f);
            buttonTransform.GetComponent<SpriteRenderer>().sprite = new ResourceSprite("TheOtherRoles.Resources.LobbyRoleInfo.RolePlate.png", 215f);
            buttons.Add(buttonTransform);
            buttonTransform.localPosition = new Vector3(0, 2.2f - ((buttons.Count - 1) * 1f), -5);
            buttonTransform.localScale = new Vector3(2f, 1.5f, 1f);

            var label = UObject.Instantiate(textTemplate, buttonTransform);
            label.text = Cs(getTeamColor(teamId), GetString(teamId.ToString() + "RolesText"));
            label.alignment = TextAlignmentOptions.Center;
            label.transform.localPosition = new Vector3(0, 0, label.transform.localPosition.z);
            label.transform.localScale = new Vector3(1.4f, 2.2f, 1f);

            var button = buttonTransform.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            var onClick = button.OnClick = new Button.ButtonClickedEvent();
            onClick.AddListener((Action)(() =>
            {
                UObject.Destroy(container.gameObject);
                roleInfosOnclick(teamId);
            }));

            button.OnMouseOver.RemoveAllListeners();
            button.OnMouseOver.AddListener((Action)(() =>
            {
                buttonTransform.GetComponent<SpriteRenderer>().color = Color.yellow;
            }));

            button.OnMouseOut.RemoveAllListeners();
            button.OnMouseOut.AddListener((Action)(() =>
            {
                buttonTransform.GetComponent<SpriteRenderer>().color = Color.white;
            }));
        }
    }

    public static void roleInfosOnclick(RoleType teamId)
    {
        var container = new GameObject("RoleListMenuContainer").AddComponent<SpriteRenderer>();
        container.sprite = new ResourceSprite("LobbyRoleInfo.RoleListScreen.png", 110f);
        container.transform.SetParent(HudManager.Instance.transform);
        container.transform.localPosition = new Vector3(0, 0.12f, -75f);
        container.transform.localScale = new Vector3(.7f, .7f, 1f);
        container.gameObject.layer = 5;
        RolesSummaryUI = container.gameObject;

        var buttonTemplate = HudManager.Instance.SettingsButton.transform;
        var textTemplate = HudManager.Instance.TaskPanel.taskText;

        var newtitle = UObject.Instantiate(textTemplate, container.transform);
        newtitle.text = GetString(teamId.ToString() + "RolesText");
        newtitle.outlineWidth = 0.01f;
        newtitle.transform.localPosition = new Vector3(0f, 2.8f, -2f);
        newtitle.transform.localScale = Vector3.one * 2.5f;

        // 添加退出按钮
        var exitButtonTransform = UObject.Instantiate(buttonTemplate, container.transform);
        exitButtonTransform.name = "RoleListExit";
        exitButtonTransform.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
        exitButtonTransform.GetComponent<SpriteRenderer>().sprite = new ResourceSprite("ExitButton.png", 135f);
        exitButtonTransform.localPosition = new Vector3(5.8f, 0.5f, -5);
        exitButtonTransform.localScale = new Vector3(1f, 1f, 1f);

        var exitButton = exitButtonTransform.GetComponent<PassiveButton>();
        var exitOnClick = exitButton.OnClick = new Button.ButtonClickedEvent();
        exitOnClick.AddListener((Action)(() =>
        {
            UObject.Destroy(RolesSummaryUI);
        }));

        // 添加返回按钮
        var backButtonTransform = UObject.Instantiate(buttonTemplate, container.transform);
        backButtonTransform.name = "RoleListBack";
        backButtonTransform.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
        backButtonTransform.GetComponent<SpriteRenderer>().sprite = new ResourceSprite("BackButton.png", 135f);
        backButtonTransform.localPosition = new Vector3(5.8f, 1.5f, -5);
        backButtonTransform.localScale = new Vector3(1f, 1f, 1f);

        var backButton = backButtonTransform.GetComponent<PassiveButton>();
        var backOnClick = backButton.OnClick = new Button.ButtonClickedEvent();
        backOnClick.AddListener((Action)(() =>
        {
            UObject.Destroy(container.gameObject);
            _ = new LateTask(RoleSummaryOnClick, 0.05f);
        }));

        List<Transform> buttons = new();
        var count = 0;
        var gameStarted = AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started;
        foreach (var roleInfo in RoleInfo.allRoleInfos)
        {
            if (roleInfo.roleType == RoleType.Modifier && teamId != RoleType.Modifier) continue;
            else if (roleInfo.roleType == RoleType.Neutral && teamId != RoleType.Neutral) continue;
            else if (roleInfo.roleType == RoleType.Impostor && teamId != RoleType.Impostor) continue;
            else if (roleInfo.roleType == RoleType.Crewmate && teamId != RoleType.Crewmate) continue;
            else if (roleInfo.roleType == RoleType.Ghost && teamId != RoleType.Ghost) continue;

            var buttonTransform = UObject.Instantiate(buttonTemplate, container.transform);
            buttonTransform.name = Cs(roleInfo.color, roleInfo.Name) + " Button";
            buttonTransform.GetComponent<BoxCollider2D>().size = new Vector2(2.5f, 0.55f);
            var label = UObject.Instantiate(textTemplate, buttonTransform);
            buttonTransform.GetComponent<SpriteRenderer>().sprite = UnityHelper.loadSpriteFromResources("TheOtherRoles.Resources.LobbyRoleInfo.RolePlate2.png", 215f);
            buttons.Add(buttonTransform);
            int row = count / 3, col = count % 3;
            buttonTransform.localPosition = new Vector3(-3.205f + (col * 3.2f), 2.9f - (row * 0.75f), -5);
            buttonTransform.localScale = new Vector3(1.125f, 1.125f, 1f);
            label.text = Cs(roleInfo.color, roleInfo.Name);
            label.alignment = TextAlignmentOptions.Center;
            label.transform.localPosition = new Vector3(0, 0, label.transform.localPosition.z);
            label.transform.localScale *= 1.5f;
            var button = buttonTransform.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            var onClick = button.OnClick = new Button.ButtonClickedEvent();
            onClick.AddListener((Action)(() =>
            {
                UObject.Destroy(container.gameObject);
                AddInfoCard(roleInfo);
            }));
            button.OnMouseOut.RemoveAllListeners();
            button.OnMouseOver.AddListener((Action)(() =>
            {
                buttonTransform.GetComponent<SpriteRenderer>().color = Color.yellow;
            }));
            button.OnMouseOut.RemoveAllListeners();
            button.OnMouseOut.AddListener((Action)(() =>
            {
                buttonTransform.GetComponent<SpriteRenderer>().color = Color.white;
            }));
            count++;
        }
    }

    public static void AddInfoCard(RoleInfo roleInfo)
    {
        var roleSettingDescription = roleInfo.FullDescription != "" ? roleInfo.FullDescription : roleInfo.ShortDescription;
        var coloredHelp = Cs(Color.white, roleSettingDescription);

        var buttonTemplate = HudManager.Instance.SettingsButton.transform;
        var roleCard = UObject.Instantiate(new GameObject("RoleCard"), HudManager.Instance.transform);
        var roleCardRend = roleCard.AddComponent<SpriteRenderer>();
        roleCard.layer = 5;
        roleCard.transform.localPosition = new Vector3(0f, 0f, -150f);
        roleCard.transform.localScale = new Vector3(0.68f, 0.68f, 1f);
        RolesSummaryUI = roleCard.gameObject;

        // 添加退出按钮
        var exitButtonTransform = UObject.Instantiate(buttonTemplate, roleCardRend.transform);
        exitButtonTransform.name = "RoleCardExit";
        exitButtonTransform.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
        exitButtonTransform.GetComponent<SpriteRenderer>().sprite = new ResourceSprite("ExitButton.png", 135f);
        exitButtonTransform.localPosition = new Vector3(5.35f, 0.5f, -5);
        exitButtonTransform.localScale = new Vector3(1f, 1f, 1f);

        var exitButton = exitButtonTransform.GetComponent<PassiveButton>();
        var exitOnClick = exitButton.OnClick = new Button.ButtonClickedEvent();
        exitOnClick.AddListener((Action)(() =>
        {
            UObject.Destroy(RolesSummaryUI);
        }));

        // 添加返回按钮
        var backButtonTransform = UObject.Instantiate(buttonTemplate, roleCardRend.transform);
        backButtonTransform.name = "RoleCardBack";
        backButtonTransform.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
        backButtonTransform.GetComponent<SpriteRenderer>().sprite = new ResourceSprite("BackButton.png", 135f);
        backButtonTransform.localPosition = new Vector3(5.35f, 1.5f, -5);
        backButtonTransform.localScale = new Vector3(1f, 1f, 1f);

        var backButton = backButtonTransform.GetComponent<PassiveButton>();
        var backOnClick = backButton.OnClick = new Button.ButtonClickedEvent();
        backOnClick.AddListener((Action)(() =>
        {
            UObject.Destroy(roleCardRend.gameObject);
            _ = new LateTask(RoleSummaryOnClick, 0);
        }));

        roleCardRend.sprite = UnityHelper.loadSpriteFromResources("TheOtherRoles.Resources.LobbyRoleInfo.SummaryScreen.png", 110f);

        infoButtonText = UObject.Instantiate(HudManager.Instance.TaskPanel.taskText, roleCard.transform);
        infoButtonText.color = Color.white;
        infoButtonText.text = coloredHelp;
        infoButtonText.enableWordWrapping = false;
        infoButtonText.transform.localScale = Vector3.one * 1.25f;
        infoButtonText.transform.localPosition = new Vector3(-2.9f, 0f, -50f);
        infoButtonText.alignment = TextAlignmentOptions.TopLeft;
        infoButtonText.fontStyle = FontStyles.Bold;

        infoTitleText = UObject.Instantiate(HudManager.Instance.TaskPanel.taskText, roleCard.transform);
        infoTitleText.color = Color.white;
        infoTitleText.text = Cs(roleInfo.color, roleInfo.Name);
        infoTitleText.enableWordWrapping = false;
        infoTitleText.transform.localScale = Vector3.one * 3f;
        infoTitleText.transform.localPosition = new Vector3(0f, 2.4f, -50f);
        infoTitleText.alignment = TextAlignmentOptions.Center;
        infoTitleText.fontStyle = FontStyles.Bold;
    }
}