namespace TheOtherRoles.Roles.Impostor;

public class Professional
{
    public static PlayerControl Player;
    public static Color color = Palette.ImpostorRed;

    public static bool spawnModifier;

    public static List<PlayerControl> killed = new();

    public static CanSeeBody CanSeeBodies = CanSeeBody.Impostors;

    public static void clearAndReload()
    {
        Player = null;
        killed.Clear();
        spawnModifier = CustomOptionHolder.modifierProfessional.GetBool();
        CanSeeBodies = spawnModifier
            ? (CanSeeBody)CustomOptionHolder.modifierProfessionalWhoCanSeeBodies.GetSelection()
            : (CanSeeBody)CustomOptionHolder.professionalWhoCanSeeBodies.GetSelection();

        Message($"spawnModifier: {spawnModifier}, CanSeeBodies: {CanSeeBodies}");
    }


    public enum CanSeeBody
    {
        Impostors,
        KillNeutral,
        EvilNeutral
    }
}
