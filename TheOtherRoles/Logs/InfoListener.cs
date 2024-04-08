using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TheOtherRoles.Logs;

[Harmony]
public static class InfoListener
{
    [HarmonyPatch]
    internal static class HandleRpcPatch
    {
        private static IEnumerable<Type> InnerNetObjectTypes { get; } =
            typeof(InnerNetObject).Assembly.GetTypes()
                .Where(x => x.IsSubclassOf(typeof(InnerNetObject)) && x != typeof(LobbyBehaviour) &&
                            x != typeof(PlayerControl)).ToList();

        public static IEnumerable<MethodBase> TargetMethods()
        {
            return InnerNetObjectTypes
                .Select(x => x.GetMethod(nameof(InnerNetObject.HandleRpc), AccessTools.allDeclared))
                .Where(m => m != null)!;
        }

        public static void Postfix(InnerNetObject __instance, [HarmonyArgument(0)] byte callId,
            [HarmonyArgument(1)] MessageReader reader)
        {
            Info($"object: {__instance.name} obejctNetId:{__instance.NetId} " +
                 $"Rpc {callId} received, rpc length => {reader.Length}");
        }
    }
}