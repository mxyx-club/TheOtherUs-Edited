using UnityEngine;

namespace TheOtherRoles.Objects.Map;

public class AddVitals
{
    public static void AddVital()
    {
        if (isMira && CustomOptionHolder.miraVitals.GetBool() && CustomOptionHolder.enableMiraModify.GetBool())
        {
            Transform Vital = Object.Instantiate(PolusObject.transform.FindChild("Office").FindChild("panel_vitals"), GameObject.Find("MiraShip(Clone)").transform);
            Vital.transform.position = new Vector3(8.5969f, 14.6337f, 0.0142f);
        }
        else if (isSkeld && CustomOptionHolder.skeldVitals.GetBool() && CustomOptionHolder.enableSkeldModify.GetBool())
        {
            Transform Vital = Object.Instantiate(PolusObject.transform.FindChild("Office").FindChild("panel_vitals"), GameObject.Find("SkeldShip(Clone)").transform);
            Vital.transform.position = new Vector3(-7.8125f, -0.614f, 1.25f);
        }
    }
    public static GameObject PolusObject => MapLoader.PolusObject;
    public static ShipStatus Polus => MapLoader.Polus;
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
internal static class ShipStatus_AwakePatch
{
    private static void Postfix(ShipStatus __instance)
    {
        AddVitals.AddVital();
    }
}
