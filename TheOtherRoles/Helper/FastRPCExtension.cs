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

#nullable enable
    public static void WriteExtra(this MessageWriter writer, object?[]? extra)
    {
        if (extra == null || extra.Length == 0)
        {
            writer.Write((byte)0);
            return;
        }

        writer.Write((byte)extra.Length);
        foreach (var item in extra)
        {
            switch (item)
            {
                case null:
                    writer.Write((byte)0);
                    break;
                case string s:
                    writer.Write((byte)1);
                    writer.Write(s);
                    break;
                case int i:
                    writer.Write((byte)2);
                    writer.Write(i);
                    break;
                case float f:
                    writer.Write((byte)3);
                    writer.Write(f);
                    break;
                case bool b:
                    writer.Write((byte)4);
                    writer.Write(b);
                    break;
                case byte bv:
                    writer.Write((byte)5);
                    writer.Write(bv);
                    break;
                default:
                    writer.Write((byte)1);
                    writer.Write(item.ToString() ?? "");
                    break;
            }
        }
    }

    public static object?[]? ReadExtra(this MessageReader reader)
    {
        var count = reader.ReadByte();
        if (count == 0) return null;

        var result = new object?[count];
        for (int i = 0; i < count; i++)
        {
            var type = reader.ReadByte();
            result[i] = type switch
            {
                0 => null,
                1 => reader.ReadString(),
                2 => reader.ReadInt32(),
                3 => reader.ReadSingle(),
                4 => reader.ReadBoolean(),
                5 => reader.ReadByte(),
                _ => null,
            };
        }
        return result;
    }
}