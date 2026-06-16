namespace TheOtherRoles.Helper;

public static class FastRPCExtension
{
    private const float MIN = -50f;
    private const float MAX = 50f;

    private static float ReverseLerp(float t)
    {
        return Mathf.Clamp((t - MIN) / (MAX - MIN), 0f, 1f);
    }

    public static void Write(this MessageWriter writer, Vector3 value)
    {
        var x = (ushort)(ReverseLerp(value.x) * ushort.MaxValue);
        var y = (ushort)(ReverseLerp(value.y) * ushort.MaxValue);
        var z = (ushort)(ReverseLerp(value.z) * ushort.MaxValue);

        writer.Write(x);
        writer.Write(y);
        writer.Write(z);
    }

    public static void Write(this MessageWriter writer, PlayerControl player)
    {
        writer.Write(player.PlayerId);
    }

    public static Vector3 ReadVector3(this MessageReader reader)
    {
        var x = reader.ReadUInt16() / (float)ushort.MaxValue;
        var y = reader.ReadUInt16() / (float)ushort.MaxValue;
        var z = reader.ReadUInt16() / (float)ushort.MaxValue;

        return new Vector3(Mathf.Lerp(MIN, MAX, x), Mathf.Lerp(MIN, MAX, y), Mathf.Lerp(MIN, MAX, z));
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
        if (id == byte.MaxValue) return null;
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            if (player.PlayerId == id) return player;
        return null;
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