using Assets.InnerNet;
using static UnityEngine.UI.Button;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
public class MainMenuPatch
{
    private static AnnouncementPopUp popUp;
    private static void Prefix(MainMenuManager __instance)
    {
        // Force Reload of SoundEffectHolder
        SoundEffectsManager.Load();

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

        var buttonGitHub = UObject.Instantiate(template, template.transform.parent);
        buttonGitHub.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
        buttonGitHub.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.542f, 0.5f);
        var textGitHub = buttonGitHub.transform.GetComponentInChildren<TMP_Text>();
        __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>(p => { textGitHub.SetText("GitHub"); })));
        var passiveButtonGitHub = buttonGitHub.GetComponent<PassiveButton>();
        passiveButtonGitHub.OnClick = new ButtonClickedEvent();
        passiveButtonGitHub.OnClick.AddListener((Action)(() => Application.OpenURL("https://github.com/mxyx-club/TheOtherUs/")));

        if (IsCN())
        {
            var buttonDiscord = UObject.Instantiate(template, null);
            UObject.Destroy(buttonDiscord.GetComponent<AspectPosition>());
            buttonDiscord.transform.localPosition = new(-0.459f, -1.5f, 0);

            var textDiscord = buttonDiscord.GetComponentInChildren<TextMeshPro>();
            textDiscord.transform.localPosition = new(0, 0.035f, -2);
            textDiscord.alignment = TextAlignmentOptions.Right;
            _ = __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) =>
            {
                textDiscord?.SetText("模组QQ群");
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
        var creditsButton = UObject.Instantiate(template, template.transform.parent);

        creditsButton.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
        creditsButton.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.462f, 0.5f);

        var textCreditsButton = creditsButton.transform.GetComponentInChildren<TMP_Text>();
        __instance.StartCoroutine(Effects.Lerp(0.5f,
            new Action<float>(p => { textCreditsButton.SetText(GetString("Credits")); })));
        var passiveCreditsButton = creditsButton.GetComponent<PassiveButton>();

        passiveCreditsButton.OnClick = new ButtonClickedEvent();

        passiveCreditsButton.OnClick.AddListener((Action)delegate
        {
            // do stuff
            if (popUp != null) UObject.Destroy(popUp);
            var popUpTemplate = UObject.FindObjectOfType<AnnouncementPopUp>(true);
            if (popUpTemplate == null)
            {
                Error("couldnt show credits, popUp is null");
                return;
            }

            popUp = UObject.Instantiate(popUpTemplate);

            popUp.gameObject.SetActive(true);
            var creditsString = GetString("creditsString1");

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
TheEpicRoles - Idea for the first kill shield (partly) and the (old) tabbed option menu (fully + some code), by LaicosVK DasMonschta Nova
ugackMiner53 - Idea and core code for the Prop Hunt game mode

License: TheOtherRoles is licensed under the [https://github.com/TheOtherRolesAU/TheOtherRoles?tab=GPL-3.0-1-ov-file#readme]GPLv3[]
</size>";
            creditsString += "</align>";

            Announcement creditsAnnouncement = new()
            {
                Id = "ModCredits",
                Language = 13,
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
                    DataManager.Player.Announcements.allAnnouncements = new ISystem.List<Announcement>();
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
}

[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
public static class VersionShower_Start
{
    public static void Postfix(VersionShower __instance)
    {
        __instance.text.text = $"Among Us v{Application.version} | <color=#ff351f>The Other Us Edited</color> <color=#FCCE03FF>v{Main.Version}{Main.VersionSuffix}</color>";
    }
}