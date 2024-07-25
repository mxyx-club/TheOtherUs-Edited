using System.Collections.Generic;
using Hazel;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Roles.Modifier;

public static class LastImpostor
{
    public static PlayerControl lastImpostor;
    public static Color color = Palette.ImpostorRed;

    public static float deduce = 2.5f;
    public static bool isEnable;

    public static void promoteToLastImpostor()
    {
        if (!isEnable || MapOption.NumImpostors == 1) return;
        var impList = new List<PlayerControl>();
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.Data.Role.IsImpostor && !p.Data.IsDead) impList.Add(p);
        }
        if (impList.Count == 1)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ImpostorPromotesToLastImpostor, SendOption.Reliable, -1);
            writer.Write(impList[0].PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.impostorPromotesToLastImpostor(impList[0].PlayerId);
        }
    }

    public static void clearAndReload()
    {
        lastImpostor = null;
        deduce = CustomOptionHolder.modifierLastImpostorDeduce.getFloat();
        isEnable = CustomOptionHolder.modifierLastImpostor.getBool();
    }
}
