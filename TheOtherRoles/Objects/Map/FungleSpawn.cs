namespace TheOtherRoles.Objects.Map;

public class FungleSpawn
{
    public static FungleSpawnType SpawnType => (FungleSpawnType)CustomOptionHolder.funglSpawnType.GetSelection();

    public enum FungleSpawnType
    {
        Random,
        Select
    }




}
