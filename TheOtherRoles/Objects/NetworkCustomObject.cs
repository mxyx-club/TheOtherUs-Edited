using System.Runtime.CompilerServices;
using TheOtherRoles.Attributes;

namespace TheOtherRoles.Objects;

public abstract class NetworkCustomObject : CustomObjectBase<NetworkCustomObject>
{
    static NetworkCustomObject()
    {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => !t.IsAbstract && typeof(NetworkCustomObject).IsAssignableFrom(t)))
        {
            try
            {
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
            catch (Exception e)
            {
                Error($"NetworkCustomObject 初始化失败: {type.Name}\n{e}", "NetworkCustomObject");
            }
        }
    }

    public ushort NetworkId { get; internal set; }

    public delegate NetworkCustomObject Factory(Vector3 position, byte[] payload);

    private static readonly Dictionary<string, Factory> factories = new();
    private static ushort nextNetworkId = 1;

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (InGame && NetworkId != 0) NetworkDespawn(this);
    }

    public static NetworkCustomObject Find(ushort networkId)
        => networkId == 0 ? null : AllObjects.FirstOrDefault(obj => obj.NetworkId == networkId);

    public static T Find<T>(ushort networkId) where T : NetworkCustomObject
        => networkId == 0 ? null : AllObjects.OfType<T>().FirstOrDefault(obj => obj.NetworkId == networkId);

    protected static void Register<T>(Factory factory) where T : NetworkCustomObject
    {
        var typeName = typeof(T).Name;
        if (factories.ContainsKey(typeName))
            throw new InvalidOperationException($"NetworkCustomObject 重复注册: {typeName}");
        factories[typeName] = factory;
    }

    // 仅 host 可调用
    public static void NetworkSpawn(NetworkCustomObject obj, byte[] payload = null)
    {
        if (!AmongUsClient.Instance.AmHost || obj == null) return;
        obj.NetworkId = AllocateNetworkId();
        var typeName = obj.GetType().Name;
        var position = obj.GameObject != null ? obj.GameObject.transform.position : Vector3.zero;
        payload ??= [];
        var writer = StartRPC(CustomRPC.NetworkObjectSpawn);
        writer.Write(typeName);
        writer.Write(obj.NetworkId);
        writer.Write(position);
        writer.WriteBytesAndSize(payload);
        writer.EndRPC();
        HandleSpawn(typeName, obj.NetworkId, position, payload);
    }

    // 仅 host 可调用
    public static void NetworkDespawn(NetworkCustomObject obj)
    {
        if (!AmongUsClient.Instance.AmHost || obj == null) return;
        var writer = StartRPC(CustomRPC.NetworkObjectDespawn);
        writer.Write(obj.NetworkId);
        writer.EndRPC();
        HandleDespawn(obj.NetworkId);
    }

    public static void HandleSpawn(string typeName, ushort networkId, Vector3 position, byte[] payload)
    {
        if (Find(networkId) != null || !factories.TryGetValue(typeName, out var factory)) return;
        var obj = factory(position, payload);
        obj.NetworkId = networkId;
    }

    public static void HandleDespawn(ushort networkId)
    {
        Find(networkId)?.Destroy();
    }

    private static ushort AllocateNetworkId()
    {
        ushort id;
        do
        {
            id = nextNetworkId++;
        } while (AllObjects.Any(obj => obj.NetworkId == id));
        return id;
    }

    [OnGameStart, OnGameEnd]
    public static void ResetNetworkIds() => nextNetworkId = 1;
}
