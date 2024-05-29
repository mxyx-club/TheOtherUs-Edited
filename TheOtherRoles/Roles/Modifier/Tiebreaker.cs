namespace TheOtherRoles.Roles.Modifier;

public static class Tiebreaker
{
    public static PlayerControl tiebreaker;

    public static bool isTiebreak;

    public static void clearAndReload()
    {
        tiebreaker = null;
        isTiebreak = false;
    }
}
