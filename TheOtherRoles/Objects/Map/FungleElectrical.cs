using BepInEx.Unity.IL2CPP.Utils.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TheOtherRoles.Objects.Map;

public static class FungleAdditionalElectrical
{
    public static void CreateElectrical()
    {
        if (!isFungle || !CustomOptionHolder.fungleElectrical.GetBool() || !CustomOptionHolder.enableFungleModify.GetBool())
            return;
        try
        {
            FungleShipStatus fungleShipStatus = ShipStatus.Instance.CastFast<FungleShipStatus>();
            SwitchSystem system = new();
            fungleShipStatus.Systems[SystemTypes.Electrical] = system.TryCast<ISystemType>();
            MapUtilities.RefreshSystems(fungleShipStatus);
            fungleShipStatus.Systems[SystemTypes.Sabotage].TryCast<SabotageSystemType>().specials.Add(system.TryCast<IActivatable>());
            List<PlayerTask> Tasks = ShipStatus.Instance.SpecialTasks.ToList();
            PlayerTask fixLightsTask = MapLoader.Airship?.SpecialTasks?.FirstOrDefault(x => x.TaskType == TaskTypes.FixLights);
            if (fixLightsTask != null && !Tasks.Any(x => x.TaskType == TaskTypes.FixLights))
                Tasks.Add(fixLightsTask);
            ShipStatus.Instance.SpecialTasks = new(Tasks.ToArray());

            Console console1 = UObject.Instantiate(VanillaAsset.MapAsset[3].transform.FindChild("Storage/task_lightssabotage (cargo)"), fungleShipStatus.transform).GetComponent<Console>();
            console1.transform.localPosition = new(-16.2f, 7.67f, 0);
            console1.ConsoleId = 0;

            Console console2 = UObject.Instantiate(VanillaAsset.MapAsset[0].transform.FindChild("Electrical/Ground/electric_frontset/SwitchConsole"), fungleShipStatus.transform).GetComponent<Console>();
            console2.transform.localPosition = new(-5.7f, -7.7f, -1.008f);
            console2.ConsoleId = 1;

            Console console3 = UObject.Instantiate(console1, fungleShipStatus.transform);
            console3.transform.localPosition = new(21.48f, 4.27f, 0f);
            console3.ConsoleId = 2;
            List<Console> Consoles = ShipStatus.Instance.AllConsoles.ToList();
            Consoles.Add(console1);
            Consoles.Add(console2);
            Consoles.Add(console3);
            ShipStatus.Instance.AllConsoles = Consoles.ToArray();
        }
        catch (Exception e)
        {
            Error(e.Message);
        }
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Awake))]
    private class MapBehaviourAwakePatch
    {
        public static void Postfix(MapBehaviour __instance)
        {
            if (!isFungle || !CustomOptionHolder.fungleElectrical.GetBool())
                return;

            MapRoom mapRoom = UObject.Instantiate(VanillaAsset.MapAsset[3].MapPrefab.infectedOverlay.rooms.FirstOrDefault(x => x.room == SystemTypes.Electrical), __instance.infectedOverlay.transform);
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
    public static ShipStatus Polus;
    public static GameObject PolusObject => Polus.gameObject;
    public static ShipStatus Skeld { get; private set; }
    public static ShipStatus Airship { get; private set; }

    public static IEnumerator LoadMaps()
    {
        while (AmongUsClient.Instance == null)
            yield return null;

        int mapAssetIndex = 0;
        for (int i = 0; i < AmongUsClient.Instance.ShipPrefabs.Count; i++)
        {
            if (i == 3) continue;
            if (mapAssetIndex >= VanillaAsset.MapAsset.Length) break;

            var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(AmongUsClient.Instance.ShipPrefabs[i].RuntimeKey);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
            {
                VanillaAsset.MapAsset[mapAssetIndex] = handle.Result.GetComponent<ShipStatus>();

                var mapName = VanillaAsset.MapAsset[mapAssetIndex].name;
                if (mapName.Contains("Skeld") || mapAssetIndex == 0)
                    Skeld = VanillaAsset.MapAsset[mapAssetIndex];
                else if (mapName.Contains("Airship") || mapAssetIndex == 3)
                    Airship = VanillaAsset.MapAsset[mapAssetIndex];
                else if (mapName.Contains("Polus") || mapAssetIndex == 2)
                    Polus = VanillaAsset.MapAsset[mapAssetIndex];

                mapAssetIndex++;
            }
            else
            {
                Error($"Failed to load map at ShipPrefabs[{i}]");
            }
        }
    }
}

[HarmonyPatch(typeof(AmongUsClient), "Awake")]
public static class AmongUsClientAwakePatch
{
    private static bool Loaded;

    public static void Prefix(AmongUsClient __instance)
    {
        if (Loaded) return;
        Loaded = true;

        VanillaAsset.LoadAssetsOnTitle();
        __instance.StartCoroutine(MapLoader.LoadMaps().WrapToIl2Cpp());
    }
}
