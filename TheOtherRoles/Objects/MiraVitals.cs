using TheOtherRoles.Helper;
using UnityEngine;

namespace TheOtherRoles.Objects
{
    public class AddVitals
    {
        public static void AddVital()
        {
            if (Helpers.isMira() && CustomOptionHolder.miraVitals.getBool())
            {
                Transform Vital = GameObject.Instantiate(PolusObject.transform.FindChild("Office").FindChild("panel_vitals"), GameObject.Find("MiraShip(Clone)").transform);
                Vital.transform.position = new Vector3(8.5969f, 14.6337f, 0.0142f);
            }
        }
        public static GameObject PolusObject => MapLoader.PolusObject;
        public static ShipStatus Polus => MapLoader.Polus;
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
    static class ShipStatus_AwakePatch
    {
        static void Postfix(ShipStatus __instance)
        {
            AddVitals.AddVital();
        }
    }
}
