using UnityEngine;

namespace TheOtherRoles.CustomCosmetics;

public class CustomHat : ICustomCosmetic
{
    public CustomCosmeticConfig config { get; set; }
    public Sprite Resource { get; set; }
    #nullable enable
    public Sprite? BackSprite { get; set; }
    public Sprite? ClimbSprite { get; set; }
    public Sprite? FlipSprite { get; set; }
    public Sprite? BackFlipSprite { get; set; }
    #nullable disable
    public CosmeticData data { get; set; }
    public CosmeticsManagerConfig ManagerConfig { get; set; }
    
    public string Id { get; set; }

    public HatViewData View
    {
        get;
        set;
    }
}