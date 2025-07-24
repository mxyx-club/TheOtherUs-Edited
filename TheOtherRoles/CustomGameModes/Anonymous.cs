using TheOtherRoles.Attributes;

namespace TheOtherRoles.CustomGameModes;

internal class Anonymous
{
    public static bool IsEnabled => true;




    [OnGameStart]
    public static void Init()
    {

    }
}
