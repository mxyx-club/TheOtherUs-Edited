using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace TheOtherRoles.Modules;

[Harmony]
public static class CrowdedPlayer
{
    public static int MaxPlayer = 120;

    public static int MaxImpostor = 15;

    public static bool Enable = true;

    public static void Start()
    {
        if (!Enable) return;
        NormalGameOptionsV08.RecommendedImpostors = NormalGameOptionsV08.MaxImpostors = Enumerable.Repeat(MaxPlayer, MaxPlayer).ToArray();
        NormalGameOptionsV08.MinPlayers = Enumerable.Repeat(4, MaxPlayer).ToArray();
    }

    [HarmonyPatch(typeof(SecurityLogger), nameof(SecurityLogger.Awake))]
    [HarmonyPostfix]
    public static void SecurityLoggerPatch_Postfix(ref SecurityLogger __instance)
    {
        if (!Enable) return;
        __instance.Timers = new float[MaxPlayer];
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Initialize))]
    [HarmonyPostfix]
    public static void GameOptionsMenu_Initialize_Postfix(GameOptionsMenu __instance)
    {
        if (!Enable) return;
        var numberOptions = __instance.GetComponentsInChildren<NumberOption>();

        var impostorsOption = numberOptions.FirstOrDefault(o => o.Title == StringNames.GameNumImpostors);
        if (impostorsOption != null)
        {
            impostorsOption.ValidRange = new FloatRange(0, MaxImpostor);
        }
    }

    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.AreInvalid))]
    [HarmonyPrefix]
    public static bool InvalidOptionsPatches_Prefix
        (GameOptionsData __instance, [HarmonyArgument(0)] int maxExpectedPlayers)
    {
        if (!Enable) return true;
        return __instance.MaxPlayers > maxExpectedPlayers ||
               __instance.NumImpostors < 1 ||
               __instance.NumImpostors + 1 > maxExpectedPlayers / 4 ||
               __instance.KillDistance is < 0 or > 2 ||
               __instance.PlayerSpeedMod is <= 0f or > 3f;
    }

    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.Awake))]
    [HarmonyPostfix]
    public static void CreateOptionsPicker_Awake_Postfix(CreateOptionsPicker __instance)
    {
        if (!Enable) return;
        if (__instance.mode != SettingsMode.Host) return;

        var firstButtonRenderer = __instance.MaxPlayerButtons.Get(0);
        firstButtonRenderer.GetComponentInChildren<TextMeshPro>().text = "-";
        firstButtonRenderer.enabled = false;


        var firstButtonButton = firstButtonRenderer.GetComponent<PassiveButton>();
        firstButtonButton.OnClick.RemoveAllListeners();
        firstButtonButton.OnClick.AddListener((Action)(() =>
        {
            for (var i = 1; i < 11; i++)
            {
                var playerButton = __instance.MaxPlayerButtons.Get(i);

                var tmp = playerButton.GetComponentInChildren<TextMeshPro>();
                var newValue = Mathf.Max(byte.Parse(tmp.text) - 10, byte.Parse(playerButton.name) - 2);
                tmp.text = newValue.ToString();
            }

            __instance.UpdateMaxPlayersButtons(__instance.GetTargetOptions());
        }));

        firstButtonRenderer.Destroy();

        var lastButtonRenderer = __instance.MaxPlayerButtons.Get(__instance.MaxPlayerButtons.Count - 1);
        lastButtonRenderer.GetComponentInChildren<TextMeshPro>().text = "+";
        lastButtonRenderer.enabled = false;

        var lastButtonButton = lastButtonRenderer.GetComponent<PassiveButton>();
        lastButtonButton.OnClick.RemoveAllListeners();
        lastButtonButton.OnClick.AddListener((Action)(() =>
        {
            for (var i = 1; i < 11; i++)
            {
                var playerButton = __instance.MaxPlayerButtons.Get(i);

                var tmp = playerButton.GetComponentInChildren<TextMeshPro>();
                var newValue = Mathf.Min(byte.Parse(tmp.text) + 10,
                    MaxPlayer - 14 + byte.Parse(playerButton.name));
                tmp.text = newValue.ToString();
            }

            __instance.UpdateMaxPlayersButtons(__instance.GetTargetOptions());
        }));
        lastButtonRenderer.Destroy();

        for (var i = 1; i < 11; i++)
        {
            var playerButton = __instance.MaxPlayerButtons.Get(i).GetComponent<PassiveButton>();
            var text = playerButton.GetComponentInChildren<TextMeshPro>();

            playerButton.OnClick.RemoveAllListeners();
            playerButton.OnClick.AddListener((Action)(() =>
            {
                var maxPlayers = byte.Parse(text.text);
                var maxImp = Mathf.Min(__instance.GetTargetOptions().NumImpostors, maxPlayers / 4);
                __instance.GetTargetOptions().SetInt(Int32OptionNames.NumImpostors, maxImp);
                __instance.ImpostorButtons[1].TextMesh.text = maxImp.ToString();
                __instance.SetMaxPlayersButtons(maxPlayers);
            }));
        }

        foreach (var button in __instance.MaxPlayerButtons)
            button.enabled = button.GetComponentInChildren<TextMeshPro>().text ==
                             __instance.GetTargetOptions().MaxPlayers.ToString();

        var secondButton = __instance.ImpostorButtons[1];
        secondButton.SpriteRenderer.enabled = false;
        secondButton.transform.FindChild("ConsoleHighlight").gameObject.Destroy();
        secondButton.PassiveButton.Destroy();
        secondButton.BoxCollider.Destroy();

        var secondButtonText = secondButton.TextMesh;
        secondButtonText.text = __instance.GetTargetOptions().NumImpostors.ToString();

        var firstButton = __instance.ImpostorButtons[0];
        firstButton.SpriteRenderer.enabled = false;
        firstButton.TextMesh.text = "-";

        var firstPassiveButton = firstButton.PassiveButton;
        firstPassiveButton.OnClick.RemoveAllListeners();
        firstPassiveButton.OnClick.AddListener((Action)(() =>
        {
            var newVal = Mathf.Clamp(
                byte.Parse(secondButtonText.text) - 1,
                1,
                __instance.GetTargetOptions().MaxPlayers / 4
            );

            __instance.SetImpostorButtons(newVal);
            secondButtonText.text = newVal.ToString();
        }));

        var thirdButton = __instance.ImpostorButtons[2];
        thirdButton.SpriteRenderer.enabled = false;
        thirdButton.TextMesh.text = "+";

        var thirdPassiveButton = thirdButton.PassiveButton;
        thirdPassiveButton.OnClick.RemoveAllListeners();
        thirdPassiveButton.OnClick.AddListener((Action)(() =>
        {
            var newVal = Mathf.Clamp(
                byte.Parse(secondButtonText.text) + 1,
                1,
                __instance.GetTargetOptions().MaxPlayers / 4
            );
            __instance.SetImpostorButtons(newVal);
            secondButtonText.text = newVal.ToString();
        }));
        Start();
    }

    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.UpdateMaxPlayersButtons))]
    [HarmonyPrefix]
    public static bool CreateOptionsPicker_UpdateMaxPlayersButtons_Prefix(CreateOptionsPicker __instance,
        [HarmonyArgument(0)] IGameOptions opts)
    {
        if (!Enable) return true;
        if (__instance.CrewArea) __instance.CrewArea.SetCrewSize(opts.MaxPlayers, opts.NumImpostors);

        var selectedAsString = opts.MaxPlayers.ToString();
        for (var i = 1; i < __instance.MaxPlayerButtons.Count - 1; i++)
            __instance.MaxPlayerButtons.Get(i).enabled =
                __instance.MaxPlayerButtons.Get(i).GetComponentInChildren<TextMeshPro>().text == selectedAsString;

        return false;
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    [HarmonyPostfix]
    public static void MeetingHudStartPatch_Postfix(MeetingHud __instance)
    {
        if (!Enable) return;
        __instance.gameObject.AddComponent<MeetingHudPagingBehaviour>().meetingHud = __instance;
    }

    [HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
    [HarmonyPostfix]
    public static void ShapeshifterMinigameBeginPatch_Postfix(ShapeshifterMinigame __instance)
    {
        if (!Enable) return;
        __instance.gameObject.AddComponent<ShapeShifterPagingBehaviour>().shapeshifterMinigame = __instance;
    }

    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
    [HarmonyPostfix]
    public static void VitalsMinigameBeginPatch_Postfix(VitalsMinigame __instance)
    {
        if (!Enable) return;
        __instance.gameObject.AddComponent<VitalsPagingBehaviour>().vitalsMinigame = __instance;
    }

    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.UpdateImpostorsButtons))]
    [HarmonyPrefix]
    public static bool CreateOptionsPicker_UpdateImpostorsButtons_Prefix()
    {
        return !Enable;
    }

    private class AbstractPagingBehaviour : MonoBehaviour
    {
        protected const string PAGE_INDEX_GAME_OBJECT_NAME = "PageIndex";

        private int _page;

        public virtual int MaxPerPage => 15;
        // public virtual IEnumerable<T> Targets { get; }

        public virtual int PageIndex
        {
            get => _page;
            set
            {
                _page = value;
                OnPageChanged();
            }
        }

        public virtual int MaxPageIndex => throw new NotImplementedException();

        public virtual void Start()
        {
            OnPageChanged();
        }

        public virtual void Update()
        {
            if (Guesser.guesserUI != null || Balancer.currentAbilityUser != null) return;

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.mouseScrollDelta.y > 0f)
                Cycle(false);
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.mouseScrollDelta.y < 0f)
                Cycle(true);

        }

        public virtual void OnPageChanged()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Loops around if you go over the limits.<br />
        ///     Attempting to go up a page while on the first page will take you to the last page and vice versa.
        /// </summary>
        public virtual void Cycle(bool increment)
        {
            var change = increment ? 1 : -1;
            PageIndex = Mathf.Clamp(PageIndex + change, 0, MaxPageIndex);
        }
    }

    [RegisterInIl2Cpp]
    private class MeetingHudPagingBehaviour : AbstractPagingBehaviour
    {
        internal MeetingHud meetingHud = null!;

        [HideFromIl2Cpp] public IEnumerable<PlayerVoteArea> Targets => meetingHud.playerStates.OrderBy(p => p.AmDead);

        public override int MaxPageIndex => (Targets.Count() - 1) / MaxPerPage;

        public override void Start()
        {
            OnPageChanged();
        }

        public override void Update()
        {
            base.Update();

            if (meetingHud.state is MeetingHud.VoteStates.Animating or MeetingHud.VoteStates.Proceeding ||
                meetingHud.TimerText.text.Contains($" ({PageIndex + 1}/{MaxPageIndex + 1})"))
                return;
            // TimerText does not update there Sometimes the timer text is spammed with the page counter for some weird reason so this is just a band-aid fix for it

            meetingHud.TimerText.text += $" ({PageIndex + 1}/{MaxPageIndex + 1})";
        }

        public override void OnPageChanged()
        {
            var i = 0;

            foreach (var button in Targets)
            {
                if (i >= PageIndex * MaxPerPage && i < (PageIndex + 1) * MaxPerPage)
                {
                    button.gameObject.SetActive(true);

                    var relativeIndex = i % MaxPerPage;
                    var row = relativeIndex / 3;
                    var col = relativeIndex % 3;
                    var buttonTransform = button.transform;
                    buttonTransform.localPosition = meetingHud.VoteOrigin +
                                                    new Vector3(
                                                        meetingHud.VoteButtonOffsets.x * col,
                                                        meetingHud.VoteButtonOffsets.y * row,
                                                        buttonTransform.localPosition.z
                                                    );
                }
                else
                {
                    button.gameObject.SetActive(false);
                }

                i++;
            }
        }
    }

    [RegisterInIl2Cpp]
    private class ShapeShifterPagingBehaviour : AbstractPagingBehaviour
    {
        public ShapeshifterMinigame shapeshifterMinigame = null!;
        private TextMeshPro PageText = null!;

        [HideFromIl2Cpp]
        public IEnumerable<ShapeshifterPanel> Targets => shapeshifterMinigame.potentialVictims.ToArray();

        public override int MaxPageIndex => (Targets.Count() - 1) / MaxPerPage;

        public override void Start()
        {
            PageText = Instantiate(HudManager.Instance.KillButton.cooldownTimerText, shapeshifterMinigame.transform);
            PageText.name = PAGE_INDEX_GAME_OBJECT_NAME;
            PageText.enableWordWrapping = false;
            PageText.gameObject.SetActive(true);
            PageText.transform.localPosition = new Vector3(4.1f, -2.36f, -1f);
            PageText.transform.localScale *= 0.5f;
            OnPageChanged();
        }

        public override void OnPageChanged()
        {
            PageText.text = $"({PageIndex + 1}/{MaxPageIndex + 1})";
            var i = 0;

            foreach (var panel in Targets)
            {
                if (i >= PageIndex * MaxPerPage && i < (PageIndex + 1) * MaxPerPage)
                {
                    panel.gameObject.SetActive(true);

                    var relativeIndex = i % MaxPerPage;
                    var row = relativeIndex / 3;
                    var col = relativeIndex % 3;
                    var buttonTransform = panel.transform;
                    buttonTransform.localPosition = new Vector3(
                        shapeshifterMinigame.XStart + (shapeshifterMinigame.XOffset * col),
                        shapeshifterMinigame.YStart + (shapeshifterMinigame.YOffset * row),
                        buttonTransform.localPosition.z
                    );
                }
                else
                {
                    panel.gameObject.SetActive(false);
                }

                i++;
            }
        }
    }

    [RegisterInIl2Cpp]
    private class VitalsPagingBehaviour : AbstractPagingBehaviour
    {
        public VitalsMinigame vitalsMinigame = null!;
        private TextMeshPro PageText = null!;

        [HideFromIl2Cpp] public IEnumerable<VitalsPanel> Targets => vitalsMinigame.vitals.ToArray();
        public override int MaxPageIndex => (Targets.Count() - 1) / MaxPerPage;

        public override void Start()
        {
            PageText = Instantiate(HudManager.Instance.KillButton.cooldownTimerText, vitalsMinigame.transform);
            PageText.name = PAGE_INDEX_GAME_OBJECT_NAME;
            PageText.enableWordWrapping = false;
            PageText.gameObject.SetActive(true);
            PageText.transform.localPosition = new Vector3(2.7f, -2f, -1f);
            PageText.transform.localScale *= 0.5f;
            OnPageChanged();
        }

        public override void OnPageChanged()
        {
            if (PlayerTask.PlayerHasTaskOfType<HudOverrideTask>(PlayerControl.LocalPlayer))
                return;

            PageText.text = $"({PageIndex + 1}/{MaxPageIndex + 1})";
            var i = 0;

            foreach (var panel in Targets)
            {
                if (i >= PageIndex * MaxPerPage && i < (PageIndex + 1) * MaxPerPage)
                {
                    panel.gameObject.SetActive(true);
                    var relativeIndex = i % MaxPerPage;
                    var row = relativeIndex / 3;
                    var col = relativeIndex % 3;
                    var panelTransform = panel.transform;
                    panelTransform.localPosition = new Vector3(
                        vitalsMinigame.XStart + (vitalsMinigame.XOffset * col),
                        vitalsMinigame.YStart + (vitalsMinigame.YOffset * row),
                        panelTransform.localPosition.z
                    );
                }
                else
                {
                    panel.gameObject.SetActive(false);
                }

                i++;
            }
        }
    }
}