namespace TheOtherRoles.Helper;


public static class LayerExpansion
{
    private static int? defaultLayer;
    private static int? shortObjectsLayer;
    private static int? objectsLayer;
    private static int? playersLayer;
    private static int? ghostLayer;
    private static int? uiLayer;
    private static int? shipLayer;
    private static int? shadowLayer;
    private static int? drawShadowsLayer;

    public static int GetDefaultLayer()
    {
        if (defaultLayer == null) defaultLayer = LayerMask.NameToLayer("Default");
        return defaultLayer.Value;
    }

    public static int GetShortObjectsLayer()
    {
        if (shortObjectsLayer == null) shortObjectsLayer = LayerMask.NameToLayer("ShortObjects");
        return shortObjectsLayer.Value;
    }

    public static int GetObjectsLayer()
    {
        if (objectsLayer == null) objectsLayer = LayerMask.NameToLayer("Objects");
        return objectsLayer.Value;
    }

    public static int GetPlayersLayer()
    {
        if (playersLayer == null) playersLayer = LayerMask.NameToLayer("Players");
        return playersLayer.Value;
    }

    public static int GetGhostLayer()
    {
        if (ghostLayer == null) ghostLayer = LayerMask.NameToLayer("Ghost");
        return ghostLayer.Value;
    }

    public static int GetUILayer()
    {
        if (uiLayer == null) uiLayer = LayerMask.NameToLayer("UI");
        return uiLayer.Value;
    }

    public static int GetShipLayer()
    {
        if (shipLayer == null) shipLayer = LayerMask.NameToLayer("Ship");
        return shipLayer.Value;
    }

    public static int GetShadowLayer()
    {
        if (shadowLayer == null) shadowLayer = LayerMask.NameToLayer("Shadow");
        return shadowLayer.Value;
    }

    public static int GetDrawShadowsLayer()
    {
        if (drawShadowsLayer == null) drawShadowsLayer = LayerMask.NameToLayer("DrawShadows");
        return drawShadowsLayer.Value;
    }

    public static int GetShadowObjectsLayer()
    {
        return 30;
    }

    public static int GetArrowLayer()
    {
        return 29;
    }

    public static int GetRaiderColliderLayer()
    {
        return 28;
    }

    public static int GetVanillaShadowLightLayer()
    {
        return 27;
    }

    public static int GetLayerMask(params int[] layer)
    {
        int result = 0;
        foreach (var l in layer) result |= 1 << l;
        return result;
    }
}

