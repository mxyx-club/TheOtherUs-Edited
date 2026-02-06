using TheOtherRoles.Attributes;

namespace TheOtherRoles.Patches;

[HarmonyPatch]
public class StartGamePatch
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGame))]
    public static void Postfix()
    {
        OnGameStartAttribute.Invoke();
    }
}
