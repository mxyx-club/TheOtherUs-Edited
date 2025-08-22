namespace TheOtherRoles.Roles.Impostor;

public class Gunsmith
{
    public static PlayerControl Player;
    public static Color color = Palette.ImpostorRed;

    public static float KillCooldown = 25f;
    public static float setKillCooldown;
    public static int maxChangeCount;

    public static int remainingChange;

    public static Sprite AddButton = new ResourceSprite("GunsmithAddButton.png");
    public static Sprite GetButton = new ResourceSprite("GunsmithGetButton.png");


    public static void ClearAndReload()
    {
        Player = null;
        remainingChange = 0;
        KillCooldown = CustomOptionHolder.gunsmithKillCooldown.GetFloat();
        setKillCooldown = CustomOptionHolder.gunsmithSetKillCooldown.GetFloat();
        maxChangeCount = CustomOptionHolder.gunsmithMaxChangeCount.GetInt();
    }
}
