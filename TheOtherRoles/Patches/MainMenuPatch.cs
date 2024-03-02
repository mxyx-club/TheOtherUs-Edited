using System;
using AmongUs.Data;
using Assets.InnerNet;
using Il2CppSystem.Collections.Generic;
using TheOtherRoles.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Modules;

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
public class MainMenuPatch
{
    private static bool horseButtonState = TORMapOptions.enableHorseMode;

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


        var buttonDiscord = Object.Instantiate(template, template.transform.parent);
        buttonDiscord.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
        buttonDiscord.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.542f, 0.5f);

        var textDiscord = buttonDiscord.transform.GetComponentInChildren<TMP_Text>();
        __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>(p => { textDiscord.SetText("GitHub"); })));
        var passiveButtonDiscord = buttonDiscord.GetComponent<PassiveButton>();

        passiveButtonDiscord.OnClick = new Button.ButtonClickedEvent();
        passiveButtonDiscord.OnClick.AddListener((Action)(() => Application.OpenURL("https://github.com/mxyx-club/TheOtherUs/")));


        // TOR credits button
        if (template == null) return;
        var creditsButton = Object.Instantiate(template, template.transform.parent);

        creditsButton.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
        creditsButton.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.462f, 0.5f);

        var textCreditsButton = creditsButton.transform.GetComponentInChildren<TMP_Text>();
        __instance.StartCoroutine(Effects.Lerp(0.5f,
            new Action<float>(p => { textCreditsButton.SetText(ModTranslation.getString("Credits")); })));
        var passiveCreditsButton = creditsButton.GetComponent<PassiveButton>();

        passiveCreditsButton.OnClick = new Button.ButtonClickedEvent();

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
            var creditsString = @"<align=""center""><b>TOR Team:</b>
Mallöris    K3ndo    Bavari    Gendelo

<b>Former TOR Team Members:</b>
Eisbison (GOAT)    Thunderstorm584    EndOfFile

<b>Additional Devs:</b>
EnoPM    twix    NesTT

<b>Github Contributors:</b>
Alex2911    amsyarasyiq    MaximeGillot
Psynomit    probablyadnf    JustASysAdmin (Scoom)
Mr-Fluuff    Xer

<b>[https://discord.gg/77RkMJHWsM]Discord[] Moderators (TOR):</b>
Draco Cordraconis    Streamblox (formerly)
Thanks to all our discord helpers!

Thanks to The Other Roles, The Other Roles CE, Town of Us, Stellar Roles!

Thanks to miniduikboot & GD for hosting modded servers (and so much more)

";
            creditsString += @"<size=60%> <b>Other Credits & Resources:</b>
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
                Id = "torCredits",
                Language = 0,
                Number = 500,
                Title = "The Other Us\nCredits & Resources",
                ShortTitle = "TOU Credits",
                SubTitle = "",
                PinState = false,
                Date = "01.07.2021",
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
            TORMapOptions.gameMode = CustomGamemodes.Classic;
            // Add buttons For Guesser Mode, Hide N Seek in this scene.
            // find "HostLocalGameButton"
            var template = Object.FindObjectOfType<HostLocalGameButton>();
            var gameButton = template.transform.FindChild("CreateGameButton");
            var gameButtonPassiveButton = gameButton.GetComponentInChildren<PassiveButton>();

            var guesserButton = Object.Instantiate(gameButton, gameButton.parent);
            guesserButton.transform.localPosition += new Vector3(0f, -0.5f);
            var guesserButtonText = guesserButton.GetComponentInChildren<TextMeshPro>();
            var guesserButtonPassiveButton = guesserButton.GetComponentInChildren<PassiveButton>();

            guesserButtonPassiveButton.OnClick = new Button.ButtonClickedEvent();
            guesserButtonPassiveButton.OnClick.AddListener((Action)(() =>
            {
                TORMapOptions.gameMode = CustomGamemodes.Guesser;
                template.OnClick();
            }));

            var HideNSeekButton = Object.Instantiate(gameButton, gameButton.parent);
            HideNSeekButton.transform.localPosition += new Vector3(1.7f, -0.5f);
            var HideNSeekButtonText = HideNSeekButton.GetComponentInChildren<TextMeshPro>();
            var HideNSeekButtonPassiveButton = HideNSeekButton.GetComponentInChildren<PassiveButton>();

            HideNSeekButtonPassiveButton.OnClick = new Button.ButtonClickedEvent();
            HideNSeekButtonPassiveButton.OnClick.AddListener((Action)(() =>
            {
                TORMapOptions.gameMode = CustomGamemodes.HideNSeek;
                template.OnClick();
            }));

            var PropHuntButton = Object.Instantiate(gameButton, gameButton.parent);
            PropHuntButton.transform.localPosition += new Vector3(3.4f, -0.5f);
            var PropHuntButtonText = PropHuntButton.GetComponentInChildren<TextMeshPro>();
            var PropHuntButtonPassiveButton = PropHuntButton.GetComponentInChildren<PassiveButton>();

            PropHuntButtonPassiveButton.OnClick = new Button.ButtonClickedEvent();
            PropHuntButtonPassiveButton.OnClick.AddListener((Action)(() =>
            {
                TORMapOptions.gameMode = CustomGamemodes.PropHunt;
                template.OnClick();
            }));

            template.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(p =>
            {
                guesserButtonText.SetText(ModTranslation.getString("isGuesserGm"));
                HideNSeekButtonText.SetText(ModTranslation.getString("isHideNSeekGM"));
                PropHuntButtonText.SetText(ModTranslation.getString("isPropHuntGM"));
            })));
        }));
    }
}