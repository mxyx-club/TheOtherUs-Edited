using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Helper;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TheOtherRoles.Objects
{
    public static class FungleAdditionalElectrical
    {
        public static void CreateElectrical()
        {
            if (!Helpers.isFungle() || !CustomOptionHolder.fungleElectrical.getBool())
                return;

            FungleShipStatus fungleShipStatus = ShipStatus.Instance.CastFast<FungleShipStatus>();
            SwitchSystem system = new();
            fungleShipStatus.Systems[SystemTypes.Electrical] = system.TryCast<ISystemType>();
            fungleShipStatus.Systems[SystemTypes.Sabotage].TryCast<SabotageSystemType>().specials.Add(system.TryCast<IActivatable>());
            List<PlayerTask> Tasks = ShipStatus.Instance.SpecialTasks.ToList();
            Tasks.Add(MapLoader.Airship.SpecialTasks.FirstOrDefault(x => x.TaskType == TaskTypes.FixLights));
            ShipStatus.Instance.SpecialTasks = new(Tasks.ToArray());

            Console console1 = GameObject.Instantiate(MapLoader.Airship.transform.FindChild("Storage/task_lightssabotage (cargo)"), fungleShipStatus.transform).GetComponent<Console>();
            console1.transform.localPosition = new(-16.2f, 7.67f, 0);
            console1.ConsoleId = 0;

            Console console2 = GameObject.Instantiate(MapLoader.Skeld.transform.FindChild("Electrical/Ground/electric_frontset/SwitchConsole"), fungleShipStatus.transform).GetComponent<Console>();
            console2.transform.localPosition = new(-5.7f, -7.7f, -1.008f);
            console2.ConsoleId = 1;

            Console console3 = GameObject.Instantiate(console1, fungleShipStatus.transform);
            console3.transform.localPosition = new(21.48f, 4.27f, 0f);
            console3.ConsoleId = 2;
            //Agartha.MapLoader.Airship.transform.FindChild("Storage/task_lightssabotage (cargo)"), fungleShipStatus.transform).GetComponent<Console>();
            List<Console> Consoles = ShipStatus.Instance.AllConsoles.ToList();
            Consoles.Add(console1);
            Consoles.Add(console2);
            Consoles.Add(console3);
            ShipStatus.Instance.AllConsoles = Consoles.ToArray();
        }

        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Awake))]
        class MapBehaviourAwakePatch
        {
            public static void Postfix(MapBehaviour __instance)
            {
                if (!Helpers.isFungle() || !CustomOptionHolder.fungleElectrical.getBool())
                    return;

                MapRoom mapRoom = GameObject.Instantiate(MapLoader.Airship.MapPrefab.infectedOverlay.rooms.FirstOrDefault(x => x.room == SystemTypes.Electrical), __instance.infectedOverlay.transform);
                mapRoom.Parent = __instance.infectedOverlay;
                mapRoom.transform.localPosition = new(-0.83f, -1.8f, -1f);
                var buttons = __instance.infectedOverlay.allButtons.ToList();
                buttons.Add(mapRoom.GetComponentInChildren<ButtonBehavior>());
                __instance.infectedOverlay.allButtons = buttons.ToArray();
                var buttons2 = __instance.infectedOverlay.rooms.ToList();
                buttons2.Add(mapRoom);
                __instance.infectedOverlay.rooms = buttons2.ToArray();
            }
        }
    }

    public static class MapLoader
    {
        private static ShipStatus skeld;
        private static ShipStatus airship;
        public static ShipStatus Polus;
        public static GameObject PolusObject => Polus.gameObject;
        public static ShipStatus Skeld => skeld;
        public static ShipStatus Airship => airship;

        public static IEnumerator LoadMaps()
        {
            while (AmongUsClient.Instance == null)
                yield return null;
            AsyncOperationHandle<GameObject> skeldAsset = AmongUsClient.Instance.ShipPrefabs.ToArray()[0].LoadAsset<GameObject>();
            while (!skeldAsset.IsDone)
                yield return null;
            skeld = skeldAsset.Result.GetComponent<ShipStatus>();
            AsyncOperationHandle<GameObject> polusAsset = AmongUsClient.Instance.ShipPrefabs.ToArray()[2].LoadAsset<GameObject>();
            while (!polusAsset.IsDone)
                yield return null;
            Polus = polusAsset.Result.GetComponent<ShipStatus>();
            AsyncOperationHandle<GameObject> airshipAsset = AmongUsClient.Instance.ShipPrefabs.ToArray()[4].LoadAsset<GameObject>();
            while (!airshipAsset.IsDone)
                yield return null;
            airship = airshipAsset.Result.GetComponent<ShipStatus>();
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), "Awake")]
    public static class AmongUsClientAwakePatch
    {
        private static bool Loaded;

        public static void Prefix(AmongUsClient __instance)
        {
            if (Loaded)
                return;
            Loaded = true;
            __instance.StartCoroutine(BepInEx.Unity.IL2CPP.Utils.Collections.CollectionExtensions.WrapToIl2Cpp(MapLoader.LoadMaps()));
        }
    }
}