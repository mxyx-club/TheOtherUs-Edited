using UnityEngine;

namespace TheOtherRoles.CustomCosmetics;

public class CustomVisor : ICustomCosmetic
{
    public CustomCosmeticConfig config { get; set; }
    public Sprite Resource { get; set; }
    public CosmeticData data { get; set; }
    public CosmeticsManagerConfig ManagerConfig { get; set; }
    public string Id { get; set; }

    public VisorViewData View
    {
        get;
        set;
    }
}