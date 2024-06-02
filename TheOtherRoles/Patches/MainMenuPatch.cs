using System;
using AmongUs.Data;
using Assets.InnerNet;
using Il2CppSystem.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
public class MainMenuPatch
{
    private static bool horseButtonState = MapOption.enableHorseMode;

    //private static Sprite horseModeOffSprite = null;
    //private static Sprite horseModeOnSprite = null;
    private static AnnouncementPopUp popUp;

    private static void Prefix(MainMenuManager __instance)
    {
        var template = GameObject.Find("ExitGameButton");
        var template2 = GameObject.Find("CreditsButton");
        if (template == null || template2 == null) return;
        template.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
        template.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.625f, 0.5f);
        template.transform.FindChild("FontPlacer").transform.localScale = new Vector3(1.8f, 0.9f, 0.9f);
        template.transform.FindChild("FontPlacer").transform.localPosition = new Vector3(-1.1f, 0f, 0f);

        template2.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
        template2.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.378f, 0.5f);
        template2.transform.FindChild("FontPlacer").transform.localScale = new Vector3(1.8f, 0.9f, 0.9f);
        template2.transform.FindChild("FontPlacer").transform.localPosition = new Vector3(-1.1f, 0f, 0f);

        var buttonGitHub = Object.Instantiate(template, template.transform.parent);
        buttonGitHub.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
        buttonGitHub.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.542f, 0.5f);
        var textGitHub = buttonGitHub.transform.GetComponentInChildren<TMP_Text>();
        __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>(p => { textGitHub.SetText("GitHub"); })));
        var passiveButtonGitHub = buttonGitHub.GetComponent<PassiveButton>();
        passiveButtonGitHub.OnClick = new ButtonClickedEvent();
        passiveButtonGitHub.OnClick.AddListener((Action)(() => Application.OpenURL("https://github.com/mxyx-club/TheOtherUs/")));

        if (IsCN())
        {
#if SUNDAY
            return;
#endif
            GameObject buttonDiscord = Object.Instantiate(template, null);
            Object.Destroy(buttonDiscord.GetComponent<AspectPosition>());
            buttonDiscord.transform.localPosition = new(-0.459f, -1.5f, 0);

            var textDiscord = buttonDiscord.GetComponentInChildren<TextMeshPro>();
            textDiscord.transform.localPosition = new(0, 0.035f, -2);
            textDiscord.alignment = TextAlignmentOptions.Right;
            _ = __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) =>
            {
                if (textDiscord != null)
                    textDiscord.SetText("模组QQ群");
            })));

            PassiveButton passiveButtonDiscord = buttonDiscord.GetComponent<PassiveButton>();
            SpriteRenderer buttonSpriteDiscord = buttonDiscord.transform.FindChild("Inactive").GetComponent<SpriteRenderer>();

            passiveButtonDiscord.OnClick = new ButtonClickedEvent();
            passiveButtonDiscord.OnClick.AddListener((Action)(() => Application.OpenURL("https://qm.qq.com/q/Xr8HijGZK8")));

            Color discordColor = Color.cyan;
            buttonSpriteDiscord.color = textDiscord.color = discordColor;
            passiveButtonDiscord.OnMouseOut.AddListener((Action)delegate
            {
                buttonSpriteDiscord.color = textDiscord.color = discordColor;
            });
        }

        // TOR credits button
        if (template == null) return;
        var creditsButton = Object.Instantiate(template, template.transform.parent);

        creditsButton.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
        creditsButton.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.462f, 0.5f);

        var textCreditsButton = creditsButton.transform.GetComponentInChildren<TMP_Text>();
        __instance.StartCoroutine(Effects.Lerp(0.5f,
            new Action<float>(p => { textCreditsButton.SetText(getString("Credits")); })));
        var passiveCreditsButton = creditsButton.GetComponent<PassiveButton>();

        passiveCreditsButton.OnClick = new ButtonClickedEvent();

        passiveCreditsButton.OnClick.AddListener((Action)delegate
        {
            // do stuff
            if (popUp != null) Object.Destroy(popUp);
            var popUpTemplate = Object.FindObjectOfType<AnnouncementPopUp>(true);
            if (popUpTemplate == null)
            {
                Error("couldnt show credits, popUp is null");
                return;
            }

            popUp = Object.Instantiate(popUpTemplate);

            popUp.gameObject.SetActive(true);
            var creditsString = getString("creditsString1");

            creditsString += @"
<size=60%> <b>Other Credits & Resources:</b>
OxygenFilter - For the versions v2.3.0 to v2.6.1, we were using the OxygenFilter for automatic deobfuscation
Reactor - The framework used for all versions before v2.0.0, and again since 4.2.0
BepInEx - Used to hook game functions
Essentials - Custom game options by DorCoMaNdO:
Before v1.6: We used the default Essentials release
v1.6-v1.8: We slightly changed the default Essentials.
v2.0.0 and later: As we were not using Reactor anymore, we are using our own implementation, inspired by the one from DorCoMaNdO
Jackal and Sidekick - Original idea for the Jackal and Sidekick came from Dhalucard
Among-Us-Love-Couple-Mod - Idea for the Lovers modifier comes from Woodi-dev
Jester - Idea for the Jester role came from Maartii
ExtraRolesAmongUs - Idea for the Engineer and Medic role came from NotHunter101. Also some code snippets from their implementation were used.
Among-Us-Sheriff-Mod - Idea for the Sheriff role came from Woodi-dev
TooManyRolesMods - Idea for the Detective and Time Master roles comes from Hardel-DW. Also some code snippets from their implementation were used.
TownOfUs - Idea for the Swapper, Shifter, Arsonist and a similar Mayor role came from Slushiegoose
Ottomated - Idea for the Morphling, Snitch and Camouflager role came from Ottomated
Crowded-Mod - Our implementation for 10+ player lobbies was inspired by the one from the Crowded Mod Team
Goose-Goose-Duck - Idea for the Vulture role came from Slushiegoose
TheEpicRoles - Idea for the first kill shield (partly) and the tabbed option menu (fully + some code), by LaicosVK DasMonschta Nova
ugackMiner53 - Idea and core code for the Prop Hunt game mode</size>";
            creditsString += "</align>";

            Announcement creditsAnnouncement = new()
            {
                Id = "Credits",
                Language = 0,
                Number = 500,
                Title = "The Other Us Edited\nCredits & Resources",
                ShortTitle = "TOU Credits",
                SubTitle = "",
                PinState = false,
                Date = "03.03.2024",
                Text = creditsString
            };
            __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(p =>
            {
                if (p == 1)
                {
                    var backup = DataManager.Player.Announcements.allAnnouncements;
                    DataManager.Player.Announcements.allAnnouncements = new List<Announcement>();
                    popUp.Init(false);
                    DataManager.Player.Announcements.SetAnnouncements(new[] { creditsAnnouncement });
                    popUp.CreateAnnouncementList();
                    popUp.UpdateAnnouncementText(creditsAnnouncement.Number);
                    popUp.visibleAnnouncements._items[0].PassiveButton.OnClick.RemoveAllListeners();
                    DataManager.Player.Announcements.allAnnouncements = backup;
                }
            })));
        });
    }

    public static void addSceneChangeCallbacks()
    {
        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, _) =>
        {
            if (!scene.name.Equals("MatchMaking", StringComparison.Ordinal)) return;
            MapOption.gameMode = CustomGamemodes.Classic;
            // Add buttons For Guesser Mode, Hide N Seek in this scene.
            // find "HostLocalGameButton"
            var template = Object.FindObjectOfType<HostLocalGameButton>();
            var gameButton = template.transform.FindChild("CreateGameButton");
            var gameButtonPassiveButton = gameButton.GetComponentInChildren<PassiveButton>();

            var guesserButton = Object.Instantiate(gameButton, gameButton.parent);
            guesserButton.transform.localPosition += new Vector3(0f, -0.5f);
            var guesserButtonText = guesserButton.GetComponentInChildren<TextMeshPro>();
            var guesserButtonPassiveButton = guesserButton.GetComponentInChildren<PassiveButton>();

            guesserButtonPassiveButton.OnClick = new ButtonClickedEvent();
            guesserButtonPassiveButton.OnClick.AddListener((Action)(() =>
            {
                MapOption.gameMode = CustomGamemodes.Guesser;
                template.OnClick();
            }));

            var HideNSeekButton = Object.Instantiate(gameButton, gameButton.parent);
            HideNSeekButton.transform.localPosition += new Vector3(1.7f, -0.5f);
            var HideNSeekButtonText = HideNSeekButton.GetComponentInChildren<TextMeshPro>();
            var HideNSeekButtonPassiveButton = HideNSeekButton.GetComponentInChildren<PassiveButton>();

            HideNSeekButtonPassiveButton.OnClick = new ButtonClickedEvent();
            HideNSeekButtonPassiveButton.OnClick.AddListener((Action)(() =>
            {
                MapOption.gameMode = CustomGamemodes.HideNSeek;
                template.OnClick();
            }));

            var PropHuntButton = Object.Instantiate(gameButton, gameButton.parent);
            PropHuntButton.transform.localPosition += new Vector3(3.4f, -0.5f);
            var PropHuntButtonText = PropHuntButton.GetComponentInChildren<TextMeshPro>();
            var PropHuntButtonPassiveButton = PropHuntButton.GetComponentInChildren<PassiveButton>();

            PropHuntButtonPassiveButton.OnClick = new ButtonClickedEvent();
            PropHuntButtonPassiveButton.OnClick.AddListener((Action)(() =>
            {
                MapOption.gameMode = CustomGamemodes.PropHunt;
                template.OnClick();
            }));

            template.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(p =>
            {
                guesserButtonText.SetText(getString("isGuesserGm"));
                HideNSeekButtonText.SetText(getString("isHideNSeekGM"));
                PropHuntButtonText.SetText(getString("isPropHuntGM"));
            })));
        }));
    }
}
[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
public static class VersionShower_Start
{
    public static void Postfix(VersionShower __instance)
    {
        __instance.text.text = $"Among Us v{Application.version} - <color=#ff351f>The Other Us Edition</color> <color=#FCCE03FF>v{Main.Version.ToString() + (Main.betaDays > 0 ? "-BETA" : "")}</color>";
    }
}
/*
[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start)), HarmonyPriority(Priority.First)]
internal class TitleLogoPatch
{
    public static GameObject Sizer;
    public static GameObject AULogo;
    public static GameObject BottomButtonBounds;
    private static void Postfix(MainMenuManager __instance)
    {
        if (!(Sizer = GameObject.Find("Sizer"))) return;
        if (!(AULogo = GameObject.Find("LOGO-AU"))) return;
        Sizer.transform.localPosition += new Vector3(0f, 0.12f, 0f);
        AULogo.transform.localScale = new Vector3(0.66f, 0.67f, 1f);
        AULogo.transform.position -= new Vector3(0f, 0.1f, 0f);
        var logoRenderer = AULogo.GetComponent<SpriteRenderer>();
        logoRenderer.sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.mxyx-Logo.png", 60f);
    }
}*/