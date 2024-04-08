using Hazel;
using System;
using System.Collections.Generic;
using System.Reflection;
using TheOtherRoles.Patches;
using TheOtherRoles.Utilities;

namespace TheOtherRoles.Helper;

public static class HandshakeHelper
{
    public enum ShareMode
    {
        Guid = 0,
        Again = 1
    }

    public static readonly Dictionary<int, AgainInfo> PlayerAgainInfo = new();

    public static void shareGameVersion()
    {
        versionHandshake(Main.Version.Major, Main.Version.Minor,
            Main.Version.Build, Main.Version.Revision, AmongUsClient.Instance.ClientId);

        var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
            (byte)CustomRPC.VersionHandshake, SendOption.Reliable);
        writer.WritePacked(AmongUsClient.Instance.ClientId);
        writer.Write(Main.Version.Major);
        writer.Write(Main.Version.Minor);
        writer.Write(Main.Version.Build);
        writer.Write(AmongUsClient.Instance.AmHost ? GameStartManagerPatch.timer : -1f);
        writer.Write((byte)(Main.Version.Revision < 0 ? 0xFF : Main.Version.Revision));
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }

    public static void versionHandshake(int major, int minor, int build, int revision, int clientId)
    {
        var ver = revision < 0 ? new Version(major, minor, build) : new Version(major, minor, build, revision);
        GameStartManagerPatch.playerVersions[clientId] = new PlayerVersion(ver)
        {
            PlayerId = clientId
        };
    }

    public static void VersionHandshakeEx(MessageReader reader)
    {
        var clientId = reader.ReadPackedInt32();
        switch ((ShareMode)reader.ReadByte())
        {
            case ShareMode.Guid:
                ShareGuid();
                break;

            case ShareMode.Again:
                Again();
                break;
        }

        return;

        void ShareGuid()
        {
            var length = reader.ReadInt32();
            var bytes = reader.ReadBytes(length);
            GameStartManagerPatch.playerVersions[clientId].guid = new Guid(bytes);
        }

        void Again()
        {
            var mode = (ShareMode)reader.ReadByte();

            switch (mode)
            {
                case ShareMode.Again:
                    shareGameVersion();
                    break;
                case ShareMode.Guid:
                    shareGameGUID();
                    break;
            }
        }
    }

    public static void shareGameGUID()
    {
        var clientId = AmongUsClient.Instance.ClientId;
        var bytes = Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray();
        GameStartManagerPatch.playerVersions[clientId].guid = new Guid(bytes);

        var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
            (byte)CustomRPC.VersionHandshakeEx, SendOption.Reliable);
        writer.WritePacked(AmongUsClient.Instance.ClientId);
        writer.Write((byte)ShareMode.Guid);
        writer.Write(bytes.Length);
        writer.Write(bytes);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }

    public static void againSend(int playerId, ShareMode mode)
    {
        if (PlayerAgainInfo.TryGetValue(playerId, out var againInfo))
        {
            againInfo.Update(mode);
        }
        else
        {
            var info = PlayerAgainInfo[playerId] = new AgainInfo { playerId = playerId };
            info.Start(mode);
        }
    }

    public class PlayerVersion(Version version)
    {
        public readonly Version version = version;
        public int PlayerId { get; set; }
        public Guid? guid { get; set; } = null;

        public bool GuidMatches()
        {
            return Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.Equals(guid);
        }
    }

    public class AgainInfo
    {
        public int playerId = -1;
        public int MaxCount { get; set; } = 5;
        public int Count { get; set; }
        public int Time { get; set; }

        public int MaxTime { get; set; } = 2;

        public void Start(ShareMode mode)
        {
            Send(mode);
            Time = MaxTime;
        }

        public void Update(ShareMode mode)
        {
            if (Count == MaxCount) return;
            if (Time < 0)
            {
                Send(mode);
                Time = MaxTime;
                Count++;
            }
            else
            {
                Time--;
            }
        }

        public void Send(ShareMode mode)
        {

            Info($"again send mode:{mode} id:{playerId}");

            if (AmongUsClient.Instance == null || CachedPlayer.LocalPlayer.PlayerControl == null) return;


            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.VersionHandshakeEx, SendOption.Reliable, playerId);
            writer.WritePacked(AmongUsClient.Instance.ClientId);
            writer.Write((byte)ShareMode.Again);
            writer.Write((byte)mode);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }
}