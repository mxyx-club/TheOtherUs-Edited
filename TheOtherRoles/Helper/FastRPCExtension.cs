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

    private static PlayerData<ClientData> PlayerClients;
    private static PlayerData<int> PlayerClientIds;

    public static void InitClientCache()
    {
        PlayerClients = new();
        PlayerClientIds = new(defaultvalue: -1);
        foreach (var clientData in AmongUsClient.Instance.allClients)
        {
            PlayerClients[clientData.Character] = clientData;
            PlayerClientIds[clientData.Character] = clientData.Id;
        }
    }

    public static void DestoryClientCache()
    {
        PlayerClients = null;
        PlayerClientIds = null;
    }

    public static ClientData GetClient(this PlayerControl player)
    {
        if (AmongUsClient.Instance?.allClients == null)
            return null;
        if (PlayerClients == null)
            return AmongUsClient.Instance.allClients.FirstOrDefault(cd => cd.Character != null && cd.Character.PlayerId == player.PlayerId);
        else
            return PlayerClients[player];
    }

    public static int GetClientId(this PlayerControl player)
    {
        if (PlayerClientIds == null)
        {
            var client = player.GetClient();
            return client == null ? -1 : client.Id;
        }
        else
        {
            return PlayerClientIds[player];
        }
    }
}