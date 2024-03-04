using System;
using System.Collections.Generic;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static TheOtherRoles.TheOtherRoles;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Objects;

public class CustomButton
{
    public static List<CustomButton> buttons = new();
    private static readonly int Desat = Shader.PropertyToID("_Desat");
    private readonly string buttonText;
    private readonly Action InitialOnClick;
    private readonly Action OnEffectEnds;
    private readonly Action OnMeetingEnds;
    public ActionButton actionButton;
    public GameObject actionButtonGameObject;
    public TextMeshPro actionButtonLabelText;
    public Material actionButtonMat;
    public SpriteRenderer actionButtonRenderer;
    public Func<bool> CouldUse;
    public float DeputyTimer;
    public float EffectDuration;
    public Func<bool> HasButton;
    public bool HasEffect;
    public KeyCode? hotkey;
    public HudManager hudManager;
    public bool isEffectActive;
    public bool isHandcuffed = false;
    public float MaxTimer = float.MaxValue;
    public bool mirror;
    private Action OnClick;
    public Vector3 PositionOffset;
    public bool showButtonText;
    public Sprite Sprite;
    public float Timer;
    public string buttonTextstring = "";

    public CustomButton(Action OnClick, Func<bool> HasButton, Func<bool> CouldUse, Action OnMeetingEnds, Sprite Sprite,
        Vector3 PositionOffset, HudManager hudManager, KeyCode? hotkey, bool HasEffect, float EffectDuration,
        Action OnEffectEnds, bool mirror = false, string buttonText = "")
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
        this.OnEffectEnds = OnEffectEnds;
        this.Sprite = Sprite;
        this.mirror = mirror;
        this.hotkey = hotkey;
        this.buttonText = buttonText;
        buttons.Add(this);
        actionButton = Object.Instantiate(hudManager.KillButton, hudManager.KillButton.transform.parent);
        actionButtonGameObject = actionButton.gameObject;
        actionButtonRenderer = actionButton.graphic;
        actionButtonMat = actionButtonRenderer.material;
        actionButtonLabelText = actionButton.buttonLabelText;
        var button = actionButton.GetComponent<PassiveButton>();
        showButtonText = actionButtonRenderer.sprite == Sprite || buttonText != "";
        button.OnClick = new Button.ButtonClickedEvent();
        button.OnClick.AddListener((UnityAction)onClickEvent);

        //开局按钮cd
        Timer = ResetButtonCooldown.killCooldown + 8.3f;

        setActive(false);
    }

    public CustomButton(Action OnClick, Func<bool> HasButton, Func<bool> CouldUse, Action OnMeetingEnds, Sprite Sprite,
        Vector3 PositionOffset,
        HudManager hudManager, KeyCode? hotkey, bool mirror = false, string buttonText = "")
        : this(OnClick, HasButton, CouldUse, OnMeetingEnds, Sprite, PositionOffset, hudManager, hotkey, false, 0f,
            () => { }, mirror, buttonText)
    {
    }

    public void onClickEvent()
    {
        if (!(Timer < 0f) || !HasButton() || !CouldUse()) return;
        actionButtonRenderer.color = new Color(1f, 1f, 1f, 0.3f);
        OnClick();

        // Deputy skip onClickEvent if handcuffed
        if (Deputy.handcuffedKnows.ContainsKey(CachedPlayer.LocalPlayer.PlayerId) &&
            Deputy.handcuffedKnows[CachedPlayer.LocalPlayer.PlayerId] > 0f) return;

        if (!HasEffect || isEffectActive) return;
        DeputyTimer = EffectDuration;
        Timer = EffectDuration;
        actionButton.cooldownTimerText.color = new Color(0F, 0.8F, 0F);
        isEffectActive = true;
    }

    public static void HudUpdate()
    {
        buttons.RemoveAll(item => item.actionButton == null);

        foreach (var t in buttons)
            try
            {
                t.Update();
            }
            catch (NullReferenceException)
            {
                Warn("NullReferenceException from HudUpdate().HasButton(), if theres only one warning its fine");
            }
    }

    public static void MeetingEndedUpdate()
    {
        buttons.RemoveAll(item => item.actionButton == null);
        foreach (var t in buttons)
            try
            {
                t.OnMeetingEnds();
                t.Update();
            }
            catch (NullReferenceException)
            {
                Warn("NullReferenceException from MeetingEndedUpdate().HasButton(), if theres only one warning its fine");
            }
    }

    public static void ResetAllCooldowns()
    {
        foreach (var t in buttons)
            try
            {
                t.Timer = t.MaxTimer;
                t.DeputyTimer = t.MaxTimer;
                t.Update();
            }
            catch (NullReferenceException)
            {
                System.Console.WriteLine(
                    "[WARNING] NullReferenceException from MeetingEndedUpdate().HasButton(), if theres only one warning its fine");
            }
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

    public void Update()
    {
        var localPlayer = CachedPlayer.LocalPlayer;
        var moveable = localPlayer.PlayerControl.moveable;

        if (localPlayer.Data == null || MeetingHud.Instance || ExileController.Instance || !HasButton())
        {
            setActive(false);
            return;
        }

        setActive(hudManager.UseButton.isActiveAndEnabled || hudManager.PetButton.isActiveAndEnabled);

        if (DeputyTimer >= 0)
        {
            // This had to be reordered, so that the handcuffs do not stop the underlying timers from running
            if (HasEffect && isEffectActive)
                DeputyTimer -= Time.deltaTime;
            else if (!localPlayer.PlayerControl.inVent && moveable)
                DeputyTimer -= Time.deltaTime;
        }

        if (DeputyTimer <= 0 && HasEffect && isEffectActive)
        {
            isEffectActive = false;
            actionButton.cooldownTimerText.color = Palette.EnabledColor;
            OnEffectEnds();
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

        if (CouldUse())
        {
            actionButtonRenderer.color = actionButtonLabelText.color = Palette.EnabledColor;
            actionButtonMat.SetFloat(Desat, 0f);
        }
        else
        {
            actionButtonRenderer.color = actionButtonLabelText.color = Palette.DisabledClear;
            actionButtonMat.SetFloat(Desat, 1f);
        }

        if (Timer >= 0)
        {
            if (HasEffect && isEffectActive)
                Timer -= Time.deltaTime;
            else if (!localPlayer.PlayerControl.inVent && moveable)
                Timer -= Time.deltaTime;
        }

        if (Timer <= 0 && HasEffect && isEffectActive)
        {
            isEffectActive = false;
            actionButton.cooldownTimerText.color = Palette.EnabledColor;
            OnEffectEnds();
        }

        actionButton.SetCoolDown(Timer, HasEffect && isEffectActive ? EffectDuration : MaxTimer);

        // Trigger OnClickEvent if the hotkey is being pressed down
        if (hotkey.HasValue && Input.GetKeyDown(hotkey.Value)) onClickEvent();

        // Deputy disable the button and display Handcuffs instead...
        if (Deputy.handcuffedPlayers.Contains(localPlayer.PlayerId))
            OnClick = () => { Deputy.setHandcuffedKnows(); };
        else // Reset.
            OnClick = InitialOnClick;
    }

    public static class ButtonPositions
    {
        public static readonly Vector3
            lowerRowRight = new(-2f, -0.06f, 0); // Not usable for imps beacuse of new button positions!

        public static readonly Vector3 lowerRowCenter = new(-3f, -0.06f, 0);
        public static readonly Vector3 lowerRowLeft = new(-4f, -0.06f, 0);
        public static readonly Vector3 lowerRowFarLeft = new(-3f, -0.06f, 0f);

        public static readonly Vector3
            upperRowRight = new(0f, 1f, 0f); // Not usable for imps beacuse of new button positions!

        public static readonly Vector3
            upperRowCenter = new(-1f, 1f, 0f); // Not usable for imps beacuse of new button positions!

        public static readonly Vector3 upperRowLeft = new(-2f, 1f, 0f);
        public static readonly Vector3 upperRowFarLeft = new(-3f, 1f, 0f);
    }
}