using Rewired;
using UnityEngine.Events;
using UnityEngine.UI;
using static TheOtherRoles.Buttons.HudManagerStartPatch;

namespace TheOtherRoles.Buttons;

public class CustomButton
{
    public static List<CustomButton> buttons = new();
    private static readonly int Desat = Shader.PropertyToID("_Desat");

    private Action OnClick;
    private readonly Action InitialOnClick;
    public Func<bool> HasButton;
    public Func<bool> CouldUse;
    private readonly Action OnMeetingEnds;
    public Func<bool> OnEffectCouldUse;
    private readonly Action OnEffectClick;
    private readonly Action OnEffectEnd;

    public Sprite Sprite;
    public HudManager hudManager;
    public ActionButton actionButton;
    public ActionButton textTemplate;
    public GameObject actionButtonGameObject;
    public TextMeshPro actionButtonLabelText;
    public Material actionButtonMat;
    public SpriteRenderer actionButtonRenderer;

    public float EffectDuration;
    public bool HasEffect;
    public bool isEffectActive;
    public bool isHandcuffed;
    public Vector3 PositionOffset;

    public KeyCode? hotkey;
    public KeyCode? originalHotkey;
    public TMP_Text ButtonTitle;
    public string buttonText;
    public bool mirror;
    public bool showButtonText;
    public float DeputyTimer;
    public float Timer;

    public float _MaxTimer;
    public float MaxTimer
    {
        get
        {
            if (!IsKillButton) return _MaxTimer;
            var time = _MaxTimer;
            var local = PlayerControl.LocalPlayer;
            if (IsKillButton)
            {
                time *= Mini.Multiplier;
                if (local.IsImpostor() && local == LastImpostor.lastImpostor)
                    time -= LastImpostor.deduce;
            }
            return time;
        }
        set => _MaxTimer = value;
    }

    private int _lastUsesCount = int.MinValue;
    public int UsesCount = -1;
    public bool IsKillButton;
    public bool isCoolingDown => Timer <= 0 && !isEffectActive;

    public CustomButton(
        Action OnClick,
        Func<bool> HasButton,
        Func<bool> CouldUse,
        Action OnMeetingEnds,
        Sprite Sprite,
        Vector3 PositionOffset,
        HudManager hudManager,
        ActionButton textTemplate,
        KeyCode? hotkey,
        bool HasEffect,
        float EffectDuration,
        Func<bool> onEffectCouldUs,
        Action onEffectClick,
        Action OnEffectEnd,
        bool mirror = false,
        string buttonText = "")
    {
        this.hudManager = hudManager;
        this.OnClick = OnClick;
        InitialOnClick = OnClick;
        this.HasButton = HasButton;
        this.CouldUse = CouldUse;
        this.PositionOffset = PositionOffset;
        this.OnMeetingEnds = OnMeetingEnds;
        this.HasEffect = HasEffect;
        this.EffectDuration = EffectDuration;
        this.OnEffectEnd = OnEffectEnd;
        this.Sprite = Sprite;
        this.mirror = mirror;
        this.hotkey = hotkey;
        this.buttonText = buttonText;
        this.textTemplate = textTemplate;

        IsKillButton = textTemplate is KillButton;
        actionButton = UObject.Instantiate(textTemplate, textTemplate.transform.parent);
        actionButtonGameObject = actionButton.gameObject;
        actionButtonRenderer = actionButton.graphic;
        actionButtonMat = actionButtonRenderer.material;
        actionButtonLabelText = actionButton.buttonLabelText;
        var button = actionButton.GetComponent<PassiveButton>();
        showButtonText = actionButtonRenderer.sprite == Sprite || buttonText != "";
        button.OnClick = new Button.ButtonClickedEvent();
        button.OnClick.AddListener((UnityAction)onClickEvent);
        originalHotkey = GetHotKeys(hotkey);
        Timer = 15f;
        SetHotKeyGuide();
        setActive(false);
        OnEffectClick = onEffectClick;
        OnEffectCouldUse = onEffectCouldUs;

        ButtonTitle = UObject.Instantiate(actionButton.cooldownTimerText, actionButton.cooldownTimerText.transform.parent);
        ButtonTitle.text = "";
        ButtonTitle.enableWordWrapping = false;
        ButtonTitle.transform.localScale = Vector3.one * 0.5f;
        ButtonTitle.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        buttons.Add(this);
    }

    public CustomButton(Action OnClick,
        Func<bool> HasButton,
        Func<bool> CouldUse,
        Action OnMeetingEnds,
        Sprite Sprite,
        Vector3 PositionOffset,
        HudManager hudManager,
        ActionButton textTemplate,
        KeyCode? hotkey,
        bool mirror = false,
        string buttonText = "")
        : this(OnClick, HasButton, CouldUse, OnMeetingEnds, Sprite, PositionOffset, hudManager, textTemplate, hotkey, false, 0f,
            null, null, null, mirror, buttonText)
    { }

    public CustomButton(Action OnClick,
        Func<bool> HasButton,
        Func<bool> CouldUse,
        Action OnMeetingEnds,
        Sprite Sprite,
        Vector3 PositionOffset,
        HudManager hudManager,
        ActionButton textTemplate,
        KeyCode? hotkey,
        bool HasEffect,
        float EffectDuration,
        Action OnEffectEnds,
        bool mirror = false,
        string buttonText = "")
        : this(OnClick, HasButton, CouldUse, OnMeetingEnds, Sprite, PositionOffset, hudManager, textTemplate, hotkey, HasEffect, EffectDuration,
            () => true, null, OnEffectEnds, mirror, buttonText)
    { }

    public void Destroy()
    {
        if (actionButton)
        {
            if (HudManager.InstanceExists)
            {
                _ = GridArrange.currentChildren.Remove(actionButton.transform);
            }
            UObject.Destroy(actionButton.gameObject);
        }
        actionButton = null;
        buttons.Remove(this);
    }

    public void SetButtonText(string text)
    {
        actionButton.OverrideText(text);
        showButtonText = true;
    }

    public void onClickEvent()
    {
        if (!HasButton()) return;

        actionButtonRenderer.color = new Color(1f, 1f, 1f, 0.3f);

        if (!isEffectActive && Timer < 0 && CouldUse())
        {
            OnClick();
            if (HasEffect && !isEffectActive)
            {
                DeputyTimer = EffectDuration;
                Timer = EffectDuration;
                actionButton.cooldownTimerText.color = new Color32(0, 204, 0, 255);
                isEffectActive = true;
            }
        }
        else if (!isEffectActive && Timer >= 0)
        {
            return;
        }
        else if (isEffectActive && Timer >= 0 && OnEffectCouldUse?.Invoke() == true)
        {
            OnEffectClick?.Invoke();
        }

        // Deputy skip onClickEvent if handcuffed
        if (Sheriff.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && Sheriff.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] > 0f)
            return;

    }

    public static void HudUpdate()
    {
        buttons.RemoveAll(item => item.actionButton == null);
        foreach (var t in buttons.ToArray())
        {
            try
            {
                t.Update();
            }
            catch (Exception e)
            {
                Warn($"NullReferenceException from HudUpdate().HasButton(), if theres only one warning its fine\n{e}");
            }
        }
    }

    public static void MeetingEndedUpdate()
    {
        buttons.RemoveAll(item => item.actionButton == null);
        buttons.Do(t =>
        {
            try
            {
                t.OnMeetingEnds();
                t.isEffectActive = false;
                t.Update();
            }
            catch (NullReferenceException)
            {
                Warn("NullReferenceException from MeetingEndedUpdate().HasButton(), if theres only one warning its fine");
            }
        });
    }

    public static void ResetAllCooldowns(float Time = -1)
    {
        var time = Time == -1 ? ModOption.KillCooldown : Time;

        buttons.Where(x => x.HasButton() && x.actionButton != null).Do(t =>
        {
            var maxTime = Time == -1 ? t.MaxTimer : Time;
            try
            {
                t.Timer = t.MaxTimer == 0 ? 0 : maxTime;
                t.DeputyTimer = maxTime;
                t.isEffectActive = false;
                t.Update();
            }
            catch (Exception e)
            {
                Error($"NullReferenceException from ResetAllCooldowns(), if theres only one warning its fine\n{e}", "CustomButton");
            }
        });

        PlayerControl.LocalPlayer.SetKillTimer(time);
    }

    public static void SetKillTimer(float time = -1f)
    {
        foreach (var t in buttons.Where(x => x.IsKillButton))
        {
            var newTimer = time == -1f ? t.MaxTimer : time;
            if (!t.isEffectActive)
            {
                t.Timer = newTimer;
                t.Update();
            }
        }

        var killtime = time == -1f ? ModOption.KillCooldown : time;
        PlayerControl.LocalPlayer.SetKillTimer(killtime);
        Message($"SetKillTimer {killtime}", "CustomButton");
        _ = new LateTask(() =>
        {
            if (PlayerControl.LocalPlayer.killTimer > killtime) PlayerControl.LocalPlayer.killTimer = killtime - 0.25f;
        }, 0.25f);
    }

    public void setActive(bool isActive)
    {
        if (isActive)
        {
            actionButtonGameObject.SetActive(true);
            actionButtonRenderer.enabled = true;
        }
        else
        {
            actionButtonGameObject.SetActive(false);
            actionButtonRenderer.enabled = false;
        }
    }

    public CustomButton SetTimer(float timer)
    {
        Timer = timer;
        return this;
    }

    public void Update()
    {
        var localPlayer = PlayerControl.LocalPlayer;

        if (localPlayer.Data == null || MeetingHud.Instance || ExileController.Instance || !HasButton())
        {
            setActive(false);
            return;
        }

        setActive(hudManager.UseButton.isActiveAndEnabled || hudManager.PetButton.isActiveAndEnabled);

        if (!IsKillButton && _lastUsesCount != UsesCount)
        {
            var usesRemainingText = actionButton?.usesRemainingText;
            var usesRemainingSprite = actionButton?.usesRemainingSprite;

            if (UsesCount == -1)
            {
                usesRemainingText?.gameObject?.SetActive(false);
                usesRemainingSprite?.gameObject?.SetActive(false);
            }
            else if (UsesCount >= 0)
            {
                usesRemainingText?.gameObject?.SetActive(true);
                usesRemainingSprite?.gameObject?.SetActive(true);
                usesRemainingText.text = UsesCount.ToString();
            }
            _lastUsesCount = UsesCount;
        }

        if (DeputyTimer >= 0)
        {
            // This had to be reordered, so that the handcuffs do not stop the underlying timers from running
            if (HasEffect && isEffectActive) DeputyTimer -= Time.deltaTime;
            else if (!localPlayer.inVent) DeputyTimer -= Time.deltaTime;
        }

        if (DeputyTimer <= 0 && HasEffect && isEffectActive)
        {
            isEffectActive = false;
            actionButton.cooldownTimerText.color = new Color(1, 1, 1); // Palette.EnabledColor
            OnEffectEnd?.Invoke();
        }

        if (isHandcuffed)
        {
            setActive(false);
            return;
        }

        actionButtonRenderer.sprite = Sprite;
        if (showButtonText && buttonText != "") actionButton.OverrideText(buttonText);
        actionButtonLabelText.enabled = showButtonText; // Only show the text if it's a kill button

        if (hudManager.UseButton != null)
        {
            var pos = hudManager.UseButton.transform.localPosition;
            if (mirror)
            {
                var aspect = Camera.main.aspect;
                var safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
                var xpos = 0.05f - (safeOrthographicSize * aspect * 1.70f);
                pos = new Vector3(xpos, pos.y, pos.z);
            }
            actionButton.transform.localPosition = pos + PositionOffset;
        }

        if (CouldUse() || (isEffectActive && OnEffectCouldUse?.Invoke() == true))
        {
            actionButtonRenderer.color = actionButtonLabelText.color = Palette.EnabledColor;
            actionButtonMat.SetFloat(Desat, 0f);
        }
        else
        {
            actionButtonRenderer.color = actionButtonLabelText.color = Palette.DisabledClear;
            actionButtonMat.SetFloat(Desat, 1f);
        }

        if (Timer >= 0 && ((HasEffect && isEffectActive) || !localPlayer.inVent))
            Timer -= Time.deltaTime;

        if (Timer <= 0 && HasEffect && isEffectActive)
        {
            isEffectActive = false;
            actionButton.cooldownTimerText.color = new Color(1, 1, 1, 0.3f); // Palette.DisabledClear
            OnEffectEnd();
        }

        actionButton.SetCoolDown(Timer, HasEffect && isEffectActive ? EffectDuration : MaxTimer);

        // Trigger OnClickEvent if the hotkey is being pressed down
        if ((hotkey.HasValue && Input.GetKeyDown(hotkey.Value)) || (originalHotkey.HasValue && Input.GetKeyDown(originalHotkey.Value)))
            onClickEvent();

        // Deputy disable the button and display Handcuffs instead...
        if (Sheriff.handcuffedPlayers.Contains(localPlayer.PlayerId))
            OnClick = () => Sheriff.setHandcuffedKnows();
        else
            OnClick = InitialOnClick;
    }

    /// <summary>
    /// Disables / Enables all Buttons (except the ones disabled in the Deputy class), and replaces them with new buttons.
    /// </summary>
    /// <param name="handcuffed"></param>
    /// <param name="reset"></param>
    public static void setAllButtonsHandcuffedStatus(bool handcuffed, bool reset = false)
    {
        if (reset)
        {
            deputyHandcuffedButtons = [];
            return;
        }

        if (handcuffed && !deputyHandcuffedButtons.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
        {
            var maxI = buttons.Count;
            for (var i = 0; i < maxI; i++)
            {
                try
                {
                    if (buttons[i].HasButton()) // For each custombutton the player has
                        addReplacementHandcuffedButton(buttons[i]);
                    // The new buttons are the only non-handcuffed buttons now!
                    buttons[i].isHandcuffed = true;
                }
                catch (Exception e)
                {
                    // Note: idk what this is good for, but i copied it from above /gendelo
                    Warn($"NullReferenceException from MeetingEndedUpdate().HasButton(), if theres only one warning its fine\n{e.Message}");
                }
            }

            // Non Custom (Vanilla) Buttons. The Originals are disabled / hidden in UpdatePatch.cs already, just need to replace them. Can use any button, as we replace onclick etc anyways.
            // Kill Button if enabled for the Role
            if (FastDestroyableSingleton<HudManager>.Instance.KillButton.isActiveAndEnabled)
                addReplacementHandcuffedButton(arsonistButton, ButtonPositions.upperRowRight,
                    () => { return FastDestroyableSingleton<HudManager>.Instance.KillButton.currentTarget != null; });
            // Vent Button if enabled
            if (PlayerControl.LocalPlayer.RoleCanUseVents())
                addReplacementHandcuffedButton(arsonistButton, ButtonPositions.upperRowCenter,
                    () =>
                    {
                        return FastDestroyableSingleton<HudManager>.Instance.ImpostorVentButton.currentTarget != null;
                    });
            // Report Button
            addReplacementHandcuffedButton(arsonistButton,
                !PlayerControl.LocalPlayer.Data.Role.IsImpostor
                    ? new Vector3(-1f, -0.06f, 0)
                    : ButtonPositions.lowerRowRight,
                () =>
                {
                    return FastDestroyableSingleton<HudManager>.Instance.ReportButton.graphic.color ==
                           Palette.EnabledColor;
                });
        }
        // Reset to original. Disables the replacements, enables the original buttons.
        else if (!handcuffed && deputyHandcuffedButtons.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
        {
            foreach (var replacementButton in deputyHandcuffedButtons[PlayerControl.LocalPlayer.PlayerId])
            {
                replacementButton.HasButton = () => { return false; };
                replacementButton.Update(); // To make it disappear properly.
                buttons.Remove(replacementButton);
            }

            deputyHandcuffedButtons.Remove(PlayerControl.LocalPlayer.PlayerId);

            foreach (var button in buttons) button.isHandcuffed = false;
        }

        static void addReplacementHandcuffedButton(CustomButton button, Vector3? positionOffset = null, Func<bool> couldUse = null)
        {
            // For non custom buttons, we can set these manually.
            var positionOffsetValue = positionOffset ?? button.PositionOffset;
            positionOffsetValue.z = -0.1f;
            couldUse ??= button.CouldUse;
            var replacementHandcuffedButton = new CustomButton(() => { }, () => { return true; }, couldUse, () => { },
                Sheriff.handcuffedSprite, positionOffsetValue, button.hudManager, button.textTemplate, null,
                true, Sheriff.handcuffDuration, null, null, null, button.mirror);
            replacementHandcuffedButton.Timer = replacementHandcuffedButton.EffectDuration;
            replacementHandcuffedButton.actionButton.cooldownTimerText.color = new Color32(0, 204, 0, 255);
            replacementHandcuffedButton.isEffectActive = true;
            if (deputyHandcuffedButtons.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
                deputyHandcuffedButtons[PlayerControl.LocalPlayer.PlayerId].Add(replacementHandcuffedButton);
            else
                deputyHandcuffedButtons.Add(PlayerControl.LocalPlayer.PlayerId, [replacementHandcuffedButton]);
        }
    }

    #region 按钮键位图标
    public static KeyCode? GetHotKeys(KeyCode? origin)
    {
        Player player = ReInput.players.GetPlayer(0);
        KeyCode? newKey = null;
        if (origin == ModInputManager.modKillInput.keyCode)
        {
            string keycode = player.controllers.maps.GetFirstButtonMapWithAction(8, true).elementIdentifierName;
            newKey = (KeyCode)Enum.Parse(typeof(KeyCode), keycode);
        }
        if (origin == ModInputManager.abilityInput.keyCode)
        {
            string keycode = player.controllers.maps.GetFirstButtonMapWithAction(49, true).elementIdentifierName;
            newKey = (KeyCode)Enum.Parse(typeof(KeyCode), keycode);
        }
        return newKey ?? origin;
    }

    public static GameObject SetKeyGuide(GameObject button, KeyCode key, Vector2 pos)
    {
        Sprite numSprite = null;
        if (ModInputManager.allKeyCodes.ContainsKey(key))
            numSprite = ModInputManager.allKeyCodes[key].GetSprite();

        if (numSprite == null)
            return null;

        GameObject obj = new()
        {
            name = "HotKeyGuide"
        };
        obj.transform.SetParent(button.transform);
        obj.layer = button.layer;
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.transform.localPosition = (Vector3)pos + new Vector3(0f, 0f, -10f);
        renderer.sprite = new ResourceSprite("KeyBind.Background.png", 100f);

        GameObject numObj = new()
        {
            name = "HotKeyText"
        };
        numObj.transform.SetParent(obj.transform);
        numObj.layer = button.layer;
        renderer = numObj.AddComponent<SpriteRenderer>();
        renderer.transform.localPosition = new Vector3(0, 0, -1f);
        renderer.sprite = numSprite;

        return obj;
    }

    public static GameObject SetKeyGuide(GameObject button, KeyCode key)
    {
        return ModOption.showKeyReminder ? SetKeyGuide(button, key, new Vector2(0.48f, 0.48f)) : null;
    }

    private void SetHotKeyGuide()
    {
        if (!ModOption.showKeyReminder) return;
        SetKeyGuide(hotkey, new Vector2(0.48f, 0.48f), false);
    }

    public static GameObject SetKeyGuideOnSmallButton(GameObject button, KeyCode key)
    {
        return ModOption.showKeyReminder ? SetKeyGuide(button, key, new Vector2(0.28f, 0.28f)) : null;
    }

    public void SetKeyGuide(KeyCode? key, Vector2 pos, bool requireChangeOption)
    {
        if (!ModOption.showKeyReminder || !key.HasValue) return;

        var guideObj = SetKeyGuide(actionButton.gameObject, key.Value, pos);

        if (guideObj == null)
            return;

        if (requireChangeOption)
        {
            SpriteRenderer renderer;

            GameObject obj = new("HotKeyOption");
            obj.transform.SetParent(guideObj.transform);
            obj.layer = actionButton.gameObject.layer;
            renderer = obj.AddComponent<SpriteRenderer>();
            renderer.transform.localPosition = new Vector3(0.12f, 0.07f, -2f);
            renderer.sprite = new ResourceSprite("KeyBind.Option.png", 100f);
        }
    }
    #endregion

    public static class ButtonPositions
    {
        public static readonly Vector3 lowerRowRight = new(-2f, -0.06f, 0); // Not usable for imps beacuse of new button positions!
        public static readonly Vector3 lowerRowCenter = new(-3f, -0.06f, 0);
        public static readonly Vector3 lowerRowLeft = new(-4f, -0.06f, 0);
        public static readonly Vector3 lowerRowFarLeft = new(-3f, -0.06f, 0f);
        public static readonly Vector3 upperRowRight = new(0f, 1f, 0f); // Not usable for imps beacuse of new button positions!
        public static readonly Vector3 upperRowCenter = new(-1f, 1f, 0f); // Not usable for imps beacuse of new button positions!
        public static readonly Vector3 upperRowLeft = new(-2f, 1f, 0f);
        public static readonly Vector3 upperRowFarLeft = new(-3f, 1f, 0f);
        public static readonly Vector3 highRowRight = new(0f, 2.06f, 0f);

        public static readonly Vector3 LeftOffset = new(1f, 0f, 0f);
        public static readonly Vector3 UpOffset = new(0f, 1.06f, 0f);
    }
}

[HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
public class AbilityUpdate
{
    public static void Postfix(AbilityButton __instance)
    {
        if (PlayerControl.LocalPlayer.IsAlive() && __instance.commsDown.active)
        {
            __instance.commsDown.SetActive(false);
        }
    }
}
