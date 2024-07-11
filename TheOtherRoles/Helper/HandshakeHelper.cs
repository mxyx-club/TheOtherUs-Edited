using System;
using System.Collections.Generic;
using System.Reflection;
using Hazel;
using TheOtherRoles.Patches;
using TheOtherRoles.Utilities;

namespace TheOtherRoles.Helper;

public static class HandshakeHelper
{

    public static void shareGameVersion()
    {
        var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.VersionHandshake, SendOption.Reliable, -1);
        writer.Write((byte)Main.Version.Major);
        writer.Write((byte)Main.Version.Minor);
        writer.Write((byte)Main.Version.Build);
        writer.Write(AmongUsClient.Instance.AmHost ? GameStartManagerPatch.timer : -1f);
        writer.WritePacked(AmongUsClient.Instance.ClientId);
        writer.Write((byte)(Main.Version.Revision < 0 ? 0xFF : Main.Version.Revision));
        writer.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        versionHandshake(Main.Version.Major, Main.Version.Minor, Main.Version.Build, Main.Version.Revision, Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
    }


    public static void versionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId)
    {
        Version ver;
        if (revision < 0)
            ver = new Version(major, minor, build);
        else
            ver = new Version(major, minor, build, revision);
        GameStartManagerPatch.playerVersions[clientId] = new PlayerVersion(ver, guid);
    }


    public class PlayerVersion(Version version, Guid guid)
    {
        public readonly Version version = version;
        public readonly Guid guid = guid;

        public bool GuidMatches()
        {
            return Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.Equals(guid);
        }
    }

}