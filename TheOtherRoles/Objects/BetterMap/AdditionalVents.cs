using System.Collections.Generic;
using System.Linq;
using InnerNet;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Objects.BetterMap;

public class AdditionalVents
{
    private static List<AdditionalVents> AllVents = new();
    private static bool flag;
    private readonly Vent vent;

    private AdditionalVents(Vector3 p)
    {
        // Create the vent
        var referenceVent = Object.FindObjectOfType<Vent>();
        vent = Object.Instantiate(referenceVent);
        vent.transform.position = p;
        vent.Left = null;
        vent.Right = null;
        vent.Center = null;
        var tmp = MapUtilities.CachedShipStatus.AllVents[0];
        vent.EnterVentAnim = tmp.EnterVentAnim;
        vent.ExitVentAnim = tmp.ExitVentAnim;
        vent.Offset = new Vector3(0f, 0.25f, 0f);
        vent.Id = MapUtilities.CachedShipStatus.AllVents.Select(x => x.Id).Max() + 1; // Make sure we have a unique id
        var allVentsList = MapUtilities.CachedShipStatus.AllVents.ToList();
        allVentsList.Add(vent);
        MapUtilities.CachedShipStatus.AllVents = allVentsList.ToArray();
        vent.gameObject.SetActive(true);
        vent.name = "AdditionalVent_" + vent.Id;
        AllVents.Add(this);
    }

    public static void AddAdditionalVents()
    {
        if (flag) return;
        flag = true;
        if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started) return;
        System.Console.WriteLine("AddAdditionalVents");

        switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
        {
            // Polus管道追加
            case 2 when
                CustomOptionHolder.enableBetterPolus.getBool() && CustomOptionHolder.addPolusVents.getBool():
                {
                    var position = PlayerControl.LocalPlayer.transform.position;
                    AdditionalVents vents1 =
                        new(new Vector3(36.54f, -21.77f, position.z + 1f)); // 样本室
                    AdditionalVents vents2 =
                        new(new Vector3(16.64f, -2.46f, position.z + 1f)); // 运输船
                    AdditionalVents vents3 =
                        new(new Vector3(26.67f, -17.54f, position.z + 1f)); // 办公室
                    vents1.vent.Left = vents3.vent; // 样本室 - 办公室
                    vents1.vent.Right = vents2.vent; // 样本室 - 运输船
                    vents2.vent.Center = vents3.vent; // 运输船 - 办公室
                    vents2.vent.Left = vents1.vent; // 运输船 - 样本室
                    vents3.vent.Right = vents1.vent; // 办公室 - 样本室
                    vents3.vent.Left = vents2.vent; // 办公室 - 运输船
                    break;
                }
            // AirShip管道追加
            case 4 when
                CustomOptionHolder.enableAirShipModify.getBool() && CustomOptionHolder.addAirShipVents.getBool():
                {
                    var transform = CachedPlayer.LocalPlayer.PlayerControl.transform;
                    var position = transform.position;
                    var vents1 = new AdditionalVents(new Vector3(17.086f, 15.24f,
                        position.z + 1f)); // 会议室
                    var vents2 = new AdditionalVents(new Vector3(19.137f, -11.32f,
                        position.z + 1f)); // 电力
                    vents1.vent.Right = vents2.vent;
                    vents2.vent.Left = vents1.vent;
                    break;
                }
        }
    }

    public static void clearAndReload()
    {
        System.Console.WriteLine("additionalVentsClearAndReload");
        flag = false;
        AllVents = [];
    }
}