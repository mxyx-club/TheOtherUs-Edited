using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Morphling : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static readonly RoleInfo roleInfo = new(
        typeof(Morphling),
        (p) => new Morphling(p),
        RoleId.Morphling,
        RoleType.Impostor,
        "Morphling",
        color,
        101100,
        AddOptions
    );

    public Morphling(PlayerControl p) : base(p, roleInfo) { }

    public static float cooldown = 30f;
    public static float duration = 10f;

    public PlayerControl currentTarget;
    public PlayerControl sampledTarget;
    public PlayerControl morphTarget;
    public float morphTimer;

    public static CustomOption morphlingCooldown;
    public static CustomOption morphlingDuration;

    public CustomButton morphlingButton;
    public PoolablePlayer targetDisplay;
    public static ResourceSprite sampleSprite = new("SampleButton.png");
    public static ResourceSprite morphSprite = new("MorphButton.png");
    public static RemoteProcess<(PlayerControl player, PlayerControl target)> MorphlingMorph = new("MorphlingMorph", (data, _) =>
    {
        if (data.player == null || data.target == null) return;
        if (data.player.TryGetRole<Morphling>(out var morphling))
        {
            morphling.morphTimer = Morphling.duration;
            morphling.morphTarget = data.target;
            if (!isActiveCamoComms)
                morphling.Player.setLook(data.target.Data.PlayerName, data.target.Data.DefaultOutfit.ColorId,
                    data.target.Data.DefaultOutfit.HatId, data.target.Data.DefaultOutfit.VisorId, data.target.Data.DefaultOutfit.SkinId,
                    data.target.Data.DefaultOutfit.PetId);
        }
    });

    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        morphlingCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "morphlingCooldown", 15f, 10f, 60f, 2.5f, roleInfo.RoleOption);
        morphlingDuration = CustomOption.Create(configId++, CustomOptionType.Impostor, "morphlingDuration", 15f, 1f, 20f, 0.5f, roleInfo.RoleOption);
    }

    public void resetMorph()
    {
        morphTarget = null;
        morphTimer = 0f;
        Player.setDefaultLook();
    }


    public override void Initialize()
    {
        resetMorph();
        currentTarget = null;
        sampledTarget = null;
        morphTarget = null;
        morphTimer = 0f;
        cooldown = morphlingCooldown.GetFloat();
        duration = morphlingDuration.GetFloat();
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        var oldMorphTimer = morphTimer;
        morphTimer = Mathf.Max(0f, morphTimer - Time.fixedDeltaTime);

        if (MushroomSabotageActive) return;

        // Camouflage reset and set Morphling look if necessary
        if (morphTimer > 0f && morphTarget != null)
        {
            var target = morphTarget;
            Player.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId,
                target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId,
                target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
        }

        // If the MushRoomSabotage ends while Morph is still active set the Morphlings look to the target's look
        if (MushroomSabotageActive)
        {
            if (morphTimer > 0f && morphTarget != null)
            {
                var target = morphTarget;
                Player.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId,
                    target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId,
                    target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
            }
        }

        // Morphling reset (only if camouflage is inactive)
        if (Camouflager.CamoTimer <= 0f && oldMorphTimer > 0f && morphTimer <= 0f)
            resetMorph();
    }

    private static readonly Dictionary<byte, (string name, Color color)> TagColorDict = new();
    public override void OnHudUpdate(HudManager hudManager)
    {

        var localPlayer = PlayerControl.LocalPlayer;
        var myData = PlayerControl.LocalPlayer.Data;
        var amImpostor = myData.Role.IsImpostor;
        var morphTimerNotUp = morphTimer > 0f;
        var morphTargetNotNull = morphTarget != null;

        var dict = TagColorDict;
        dict.Clear();

        foreach (var data in GameData.Instance.AllPlayers.GetFastEnumerator())
        {
            var player = data.Object;
            var text = data.PlayerName;
            Color color;
            if (player)
            {
                var playerName = text;
                var nameText = player.cosmetics.nameText;
                if (morphTimerNotUp && morphTargetNotNull && Player == player)
                    playerName = morphTarget.Data.PlayerName;

                nameText.text = hidePlayerName(localPlayer, player) ? "" : playerName;
                if (DataManager.Settings.Accessibility.ColorBlindMode)
                {
                    player.cosmetics.colorBlindText.gameObject.SetActive(!hidePlayerName(localPlayer, player));
                }

                player.cosmetics.colorBlindText.gameObject.transform.SetLocalZ(0.0001f);
                nameText.color = color = amImpostor && data.Role.IsImpostor ? Palette.ImpostorRed : Color.white;
                nameText.color = nameText.color.SetAlpha(Chameleon.visibility(player.PlayerId));
            }
            else
            {
                color = Color.white;
            }

            dict.Add(data.PlayerId, (text, color));
        }

        if (MeetingHud.Instance != null)
            foreach (var playerVoteArea in MeetingHud.Instance.playerStates)
            {
                var (name, color) = dict[playerVoteArea.TargetPlayerId];
                var text = playerVoteArea.NameText;
                text.text = name;
                text.color = color;
            }
    }

    private void setButtonTargetDisplay(PlayerControl target, CustomButton button = null, Vector3? offset = null)
    {
        if (target == null || button == null)
        {
            if (targetDisplay != null)
            {
                // Reset the poolable player
                targetDisplay.gameObject.SetActive(false);
                UObject.Destroy(targetDisplay.gameObject);
                targetDisplay = null;
            }

            return;
        }

        // Add poolable player to the button so that the target outfit is shown
        button.actionButton.cooldownTimerText.transform.localPosition =
            new Vector3(0, 0, -1f); // Before the poolable player
        targetDisplay = UObject.Instantiate(IntroCutsceneOnDestroyPatch.playerPrefab, button.actionButton.transform);
        var data = target.Data;
        target.SetPlayerMaterialColors(targetDisplay.cosmetics.currentBodySprite.BodySprite);
        targetDisplay.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
        targetDisplay.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
        targetDisplay.cosmetics.nameText.text = ""; // Hide the name!
        targetDisplay.transform.localPosition = new Vector3(0f, 0.22f, -0.01f);
        if (offset != null) targetDisplay.transform.localPosition += (Vector3)offset;
        targetDisplay.transform.localScale = Vector3.one * 0.33f;
        targetDisplay.setSemiTransparent(false);
        targetDisplay.gameObject.SetActive(true);
    }

    public override void CleanUp(HudManager __instance)
    {
        morphlingButton?.Destroy();
        morphlingButton = null;

        // Reset the poolable player
        if (targetDisplay != null)
        {
            targetDisplay.gameObject.SetActive(false);
            UObject.Destroy(targetDisplay.gameObject);
            targetDisplay = null;
        }
    }

    public override void CreateButton(HudManager __instance)
    {
        // Morphling morphs
        morphlingButton?.Destroy();
        morphlingButton = new CustomButton(
            () =>
            {
                if (sampledTarget != null)
                {
                    if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;
                    MorphlingMorph.Invoke((PlayerControl.LocalPlayer, sampledTarget));
                    sampledTarget = null;
                    morphlingButton.EffectDuration = duration;
                    SoundEffectsManager.play("morphlingMorph");
                }
                else if (currentTarget != null)
                {
                    sampledTarget = currentTarget;
                    morphlingButton.Sprite = morphSprite;
                    morphlingButton.EffectDuration = 1f;
                    SoundEffectsManager.play("morphlingSample");

                    // Add poolable player to the button so that the target outfit is shown
                    setButtonTargetDisplay(sampledTarget, morphlingButton);
                }
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                currentTarget = SetTarget();
                SetPlayerOutline(currentTarget, color);

                if (sampledTarget == null) morphlingButton.showTargetNameOnButton(currentTarget, GetString("SampleText"));
                return (currentTarget || sampledTarget) && !isActiveCamoComms &&
                       PlayerControl.LocalPlayer.CanMove && !MushroomSabotageActive;
            },
            () =>
            {
                morphlingButton.Timer = morphlingButton.MaxTimer;
                morphlingButton.Sprite = sampleSprite;
                morphlingButton.isEffectActive = false;
                morphlingButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                sampledTarget = null;
                setButtonTargetDisplay(null);
            },
            sampleSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            true,
            duration,
            () =>
            {
                if (sampledTarget == null)
                {
                    morphlingButton.Timer = morphlingButton.MaxTimer;
                    morphlingButton.Sprite = sampleSprite;
                    SoundEffectsManager.play("morphlingMorph");

                    // Reset the poolable player
                    setButtonTargetDisplay(null);
                }
            },
            buttonText: GetString("SampleText")
        )
        {
            MaxTimer = cooldown,
            EffectDuration = duration,
        };
    }
}
