namespace TheOtherRoles.Helper;

public static class FastRPCExtension
{
    public static Vector3 ReadVector3(this MessageReader reader)
    {
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        var z = reader.ReadSingle();
        return new Vector3(x, y, z);
    }

    public static Rect ReadRect(this MessageReader reader)
    {
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        var width = reader.ReadSingle();
        var height = reader.ReadSingle();
        return new Rect(x, y, width, height);
    }

    public static PlayerControl ReadPlayer(this MessageReader reader)
    {
        var id = reader.ReadByte();
        return PlayerControl.AllPlayerControls.FirstOrDefault(n => n.PlayerId == id);
    }

    public static Il2CppStructArray<byte> ReadBytesFormLength(this MessageReader reader)
    {
        var length = reader.ReadPackedInt32();
        return reader.ReadBytes(length);
    }

    public static MessageReader ReadReader(this MessageReader reader)
    {
        return MessageReader.Get(reader.ReadBytesFormLength());
    }

    public static Version ReadVersion(this MessageReader reader)
    {
        var major = reader.ReadInt32();
        var minor = reader.ReadInt32();
        var build = reader.ReadInt32();
        var revision = reader.ReadInt32();
        return revision == -1 ? new Version(major, minor, build) : new Version(major, minor, build, revision);
    }

    public static MessageWriter StartRPC(RpcCalls RPCId, PlayerControl SendTarget = null)
    {
        return StartRPC(PlayerControl.LocalPlayer.NetId, (byte)RPCId, SendTarget);
    }

    public static MessageWriter StartRPC(uint NetId, RpcCalls RPCId, PlayerControl SendTarget = null)
    {
        return StartRPC(NetId, (byte)RPCId, SendTarget);
    }

    public static MessageWriter StartRPC(CustomRPC RPCId, PlayerControl SendTarget = null)
    {
        return StartRPC(PlayerControl.LocalPlayer.NetId, (byte)RPCId, SendTarget);
    }

    public static MessageWriter StartRPC(uint NetId, CustomRPC RPCId, PlayerControl SendTarget = null)
    {
        return StartRPC(NetId, (byte)RPCId, SendTarget);
    }

    public static MessageWriter StartRPC(PlayerControl player, CustomRPC RPCId, PlayerControl SendTarget = null)
    {
        return StartRPC(player.NetId, (byte)RPCId, SendTarget);
    }

    public static MessageWriter StartRPC(byte RPCId, PlayerControl SendTarget = null)
    {
        return StartRPC(PlayerControl.LocalPlayer.NetId, RPCId, SendTarget);
    }

    public static MessageWriter StartRPC(uint NetId, byte RPCId, PlayerControl SendTarget = null)
    {
        var target = SendTarget != null ? SendTarget.GetClientId() : -1;
        return AmongUsClient.Instance.StartRpcImmediately(NetId, RPCId, SendOption.Reliable, target);
    }

    public static void EndRPC(this MessageWriter Writer)
    {
        AmongUsClient.Instance.FinishRpcImmediately(Writer);
    }

    public static ClientData GetClient(this PlayerControl player)
    {
        if (AmongUsClient.Instance?.allClients == null)
            return null;
        return AmongUsClient.Instance.allClients.FirstOrDefault(cd => cd.Character != null && cd.Character.PlayerId == player.PlayerId);
    }

    public static int GetClientId(this PlayerControl player)
    {
        return player.GetClient() == null ? -1 : player.GetClient().Id;
    }
}