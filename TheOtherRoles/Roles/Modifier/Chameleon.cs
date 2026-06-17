namespace TheOtherRoles.Roles.Modifier;

public static class Chameleon
{
    public static List<PlayerControl> chameleon = new();
    public static float minVisibility = 0.2f;
    public static float holdDuration = 1f;
    public static float fadeDuration = 0.5f;
    public static Dictionary<byte, float> lastMoved = new();

    public static void clearAndReload()
    {
        chameleon = new();
        lastMoved = new();
        holdDuration = CustomOptionHolder.modifierChameleonHoldDuration.GetFloat();
        fadeDuration = CustomOptionHolder.modifierChameleonFadeDuration.GetFloat();
        minVisibility = CustomOptionHolder.modifierChameleonMinVisibility.GetSelection() / 10f;
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

    public static void update()
    {
        foreach (var player in chameleon)
        {
            if (player?.Data == null) continue;
            if ((player == Ninja.ninja && Ninja.isInvisable) ||
                (player == Phantom.Player && Phantom.isInvisable) ||
                (Jackal.jackal.Any(x => x == player) && Jackal.isInvisable))
                continue; // Dont make Ninja visible...
            // check movement by animation
            var playerPhysics = player.MyPhysics;
            if (playerPhysics == null) continue;
            var currentPhysicsAnim = playerPhysics.Animations.Animator.GetCurrentAnimation();
            if (currentPhysicsAnim != playerPhysics.Animations.group.IdleAnim) lastMoved[player.PlayerId] = Time.time;
            // calculate and set visibility
            var visibility = Chameleon.visibility(player.PlayerId);
            var petVisibility = visibility;
            if (player.Data.IsDead)
            {
                visibility = 0.5f;
                petVisibility = 1f;
            }

            try
            {
                // Sometimes renderers are missing for weird reasons. Try catch to avoid exceptions
                player.cosmetics.currentBodySprite.BodySprite.color =
                    player.cosmetics.currentBodySprite.BodySprite.color.SetAlpha(visibility);
                if (DataManager.Settings.Accessibility.ColorBlindMode)
                    player.cosmetics.colorBlindText.color =
                        player.cosmetics.colorBlindText.color.SetAlpha(visibility);
                player.SetHatAndVisorAlpha(visibility);
                player.cosmetics.skin.layer.color =
                    player.cosmetics.skin.layer.color.SetAlpha(visibility);
                player.cosmetics.nameText.color =
                    player.cosmetics.nameText.color.SetAlpha(visibility);
                foreach (var rend in player.cosmetics.currentPet.renderers)
                    rend.color = rend.color.SetAlpha(petVisibility);
                foreach (var shadowRend in player.cosmetics.currentPet.shadows)
                    shadowRend.color = shadowRend.color.SetAlpha(petVisibility);
            }
            catch { }
        }
    }
}
