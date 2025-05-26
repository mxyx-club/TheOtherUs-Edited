namespace TheOtherRoles.Roles.Modifier;

public static class Torch
{
    public static List<PlayerControl> torch = new();
    public static float vision = 1;

    public static void clearAndReload()
    {
        torch.Clear();
        vision = CustomOptionHolder.modifierTorchVision.GetFloat();
    }
}
