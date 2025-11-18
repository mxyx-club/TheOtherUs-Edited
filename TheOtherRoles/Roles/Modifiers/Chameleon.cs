namespace TheOtherRoles.Roles.Modifier;

public class Chameleon : ModifierBase
{
    public static Color color = Color.yellow;

    public static readonly RoleInfo roleinfo = new(
        typeof(Chameleon),
        (p) => new Chameleon(p),
        RoleId.Chameleon,
        "Chameleon",
        color,
        403300,
        AddOptions
    );

    public Chameleon(PlayerControl p) : base(p, roleinfo) { CustomRoleManager.OnFixedUpdateOthers.Add(OnFixedUpdate); }

    public static float minVisibility = 0.2f;
    public static float holdDuration = 1f;
    public static float fadeDuration = 0.5f;

    public static Dictionary<byte, float> lastMoved = new();

    public static CustomOption modifierChameleonHoldDuration;
    public static CustomOption modifierChameleonFadeDuration;
    public static CustomOption modifierChameleonMinVisibility;

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierChameleonHoldDuration = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierChameleonHoldDuration", 3f, 1f, 10f, 0.5f, roleinfo.RoleOption);
        modifierChameleonFadeDuration = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierChameleonFadeDuration", 1f, 0.25f, 10f, 0.25f, roleinfo.RoleOption);
        modifierChameleonMinVisibility = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierChameleonMinVisibility", ["0%", "10%", "20%", "30%", "40%", "50%"], roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        lastMoved = new();
        holdDuration = modifierChameleonHoldDuration.GetFloat();
        fadeDuration = modifierChameleonFadeDuration.GetFloat();
        minVisibility = modifierChameleonMinVisibility.GetSelection() / 10f;
    }

    public static float visibility(byte playerId)
    {
        var visibility = 1f;
        if (lastMoved != null && lastMoved.ContainsKey(playerId))
        {
            var tStill = Time.time - lastMoved[playerId];
            if (tStill > holdDuration)
            {
                if (tStill - holdDuration > fadeDuration) visibility = minVisibility;
                else
                    visibility = ((1 - ((tStill - holdDuration) / fadeDuration)) * (1 - minVisibility)) + minVisibility;
            }
        }

        // Ghosts can always see!
        if (PlayerControl.LocalPlayer.Data.IsDead && visibility < 0.1f) visibility = 0.1f;
        return visibility;
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        lastMoved.Clear();
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        // Dont make Ninja visible...
        if ((Player.TryGetRole<Ninja>(out var ninja) && ninja.isInvisable) ||
            (Player.TryGetRole<Swooper>(out var swooper) && swooper.IsInvisable) ||
            (Player.TryGetRole<Jackal>(out var jackal) && jackal.IsInvisable)) return;

        // check movement by animation
        var playerPhysics = Player.MyPhysics;
        var currentPhysicsAnim = playerPhysics.Animations.Animator.GetCurrentAnimation();
        if (currentPhysicsAnim != playerPhysics.Animations.group.IdleAnim)
            lastMoved[Player.PlayerId] = Time.time;

        // calculate and set visibility
        var visibility = Chameleon.visibility(Player.PlayerId);
        var petVisibility = visibility;

        if (Player.Data.IsDead)
        {
            visibility = 0.5f;
            petVisibility = 1f;
        }

        try
        {
            // Sometimes renderers are missing for weird reasons. Try catch to avoid exceptions
            Player.cosmetics.currentBodySprite.BodySprite.color = Player.cosmetics.currentBodySprite.BodySprite.color.SetAlpha(visibility);
            Player.SetHatAndVisorAlpha(visibility);
            Player.cosmetics.skin.layer.color = Player.cosmetics.skin.layer.color.SetAlpha(visibility);
            Player.cosmetics.nameText.color = Player.cosmetics.nameText.color.SetAlpha(visibility);
            if (DataManager.Settings.Accessibility.ColorBlindMode)
                Player.cosmetics.colorBlindText.color = Player.cosmetics.colorBlindText.color.SetAlpha(visibility);
            foreach (var rend in Player.cosmetics.currentPet.renderers)
                rend.color = rend.color.SetAlpha(petVisibility);
            foreach (var shadowRend in Player.cosmetics.currentPet.shadows)
                shadowRend.color = shadowRend.color.SetAlpha(petVisibility);
        }
        catch { }
    }
}

