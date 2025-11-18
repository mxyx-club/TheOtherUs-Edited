#nullable enable
using AmongUs.GameOptions;
using System.Runtime.CompilerServices;
using TheOtherRoles.Attributes;
using TheOtherRoles.Mode;
using TheOtherRoles.Objects;
using TheOtherRoles.Objects.Map;
using TheOtherRoles.Patches;
using static TheOtherRoles.Buttons.HudManagerStartPatch;
using static TheOtherRoles.Options.ModOption;

namespace TheOtherRoles;

public enum CustomRPC : byte
{
    // Main Controls
    ShareOptions = 80,
    WorkaroundSetRoles,
    SetRole,
    SetModifier,
    SetGhostRole,
    VersionHandshake,
    UseUncheckedVent,
    DynamicMapOption,
    SetGameStarting,
    StopStart,
    DraftModePickOrder,
    DraftModePick,
    ShareGameMode,
    ShareFriendCode,
    ShareGhostInfo,
    ShareDeathReasonAndKiller,

    CustomMurderPlayer,
    UncheckedExilePlayer,
    RevivePlayer,
    HostControl,
    NoCheckStartMeeting,
    GuesserMessage,
    // Gamemode
    SetGuesserGm,

    NetworkTransform = 250,
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class CustomRpcHolderAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Method)]
public class CustomRpcAttribute : Attribute
{

}


public class RPCInvoker
{
    private Action<MessageWriter> sender;
    private Action localBodyProcess;
    private int hash;
    public bool IsDummy { get; private set; }

    public RPCInvoker(int hash, Action<MessageWriter> sender, Action localBodyProcess)
    {
        this.hash = hash;
        this.sender = sender;
        this.localBodyProcess = localBodyProcess;
        this.IsDummy = false;
    }

    public RPCInvoker(Action localAction)
    {
        this.hash = 0;
        this.sender = null!;
        this.localBodyProcess = localAction;
        this.IsDummy = true;
    }

    public void Invoke(MessageWriter writer)
    {
        writer.Write(hash);
        sender.Invoke(writer);
        localBodyProcess.Invoke();
    }

    public void InvokeSingle()
    {
        if (IsDummy)
            localBodyProcess.Invoke();
        else
            RPCRouter.SendRpc("Invoker", hash, (writer) => sender.Invoke(writer), () => localBodyProcess.Invoke());
    }

    public void InvokeLocal()
    {
        localBodyProcess.Invoke();
    }
}

public static class RPCRouter
{
    public class RPCSection : IDisposable
    {
        public string Name;
        public void Dispose()
        {
            if (currentSection != this) return;

            currentSection = null;
            Debug($"End Evacuating Rpcs ({Name}, Size = {evacuateds.Count})");

            CombinedRemoteProcess.CombinedRPC.Invoke([.. evacuateds]);
            evacuateds.Clear();
        }

        public RPCSection(string? name = null)
        {
            Name = name ?? "Untitled";
            if (currentSection == null)
            {
                currentSection = this;
                Debug($"Start Evacuating Rpcs ({Name})");
            }
        }
    }

    public static RPCSection CreateSection(string label = null) => new(label);

    private static RPCSection currentSection = null;
    private static List<RPCInvoker> evacuateds = [];
    public static void SendRpc(string name, int hash, Action<MessageWriter> sender, Action localBodyProcess)
    {
        if (currentSection == null)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, 128, SendOption.Reliable, -1);
            writer.Write(hash);
            sender.Invoke(writer);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            try
            {
                localBodyProcess.Invoke();
            }
            catch (Exception ex)
            {
                Error($"Error in RPC(Invoke: {name})" + ex.Message + ex.StackTrace);
            }
        }
        else
        {
            evacuateds.Add(new(hash, sender, localBodyProcess));
        }
    }
}

public class RemoteProcessBase
{
    public static Dictionary<int, RemoteProcessBase> AllTORProcess = new();


    public int Hash { get; private set; } = -1;
    public string Name { get; private set; }


    public RemoteProcessBase(string name)
    {
        Hash = name.ComputeConstantHash();
        Name = name;

        if (AllTORProcess.ContainsKey(Hash)) Warn(name + " is duplicated. (" + Hash + ")");

        AllTORProcess[Hash] = this;
    }

    private static void SwapMethodPointer(MethodInfo method0, MethodInfo method1)
    {
        unsafe
        {
            var functionPointer0 = method0.MethodHandle.Value.ToPointer();
            var functionPointer1 = method1.MethodHandle.Value.ToPointer();
            var functionShiftedPointer0 = *((int*)new IntPtr(((int*)functionPointer0 + 1)).ToPointer());
            var functionShiftedPointer1 = *((int*)new IntPtr(((int*)functionPointer1 + 1)).ToPointer());
            *((int*)new IntPtr(((int*)functionPointer0 + 1)).ToPointer()) = functionShiftedPointer1;
            *((int*)new IntPtr(((int*)functionPointer1 + 1)).ToPointer()) = functionShiftedPointer0;
        }
    }

    private static Dictionary<string, RemoteProcess<object[]>> harmonyRpcMap = new();
    private static void WrapRpcMethod(Harmony harmony, MethodInfo method)
    {
        //元の静的メソッドをコピーしておく
        var copiedOriginal = harmony.Patch(method);

        //メソッド呼び出しのパラメータを取得
        var parameters = method.GetParameters();

        //RPCを定義し、登録

        List<(Action<MessageWriter, object> writer, Func<MessageReader, object> reader)> processList = new();
        if (!method.IsStatic) processList.Add(RemoteProcessAsset.GetProcess(method.DeclaringType!));
        processList.AddRange(parameters.Select(p => RemoteProcessAsset.GetProcess(p.ParameterType)));

        RemoteProcess<object[]> rpc = new(method.Name,
            (writer, args) =>
            {
                for (int i = 0; i < processList.Count; i++) processList[i].writer(writer, args[i]);
            },
            (reader) =>
            {
                return [.. processList.Select(p => p.reader(reader))];
            },
            (args, _) =>
            {
                copiedOriginal.Invoke(null, args);
            }
            );
        harmonyRpcMap[method.DeclaringType!.FullName + "." + method.Name] = rpc;

        //静的メソッドをRPC呼び出しに変更
        static bool RpcPrefix(object __instance, object[] __args, MethodBase __originalMethod)
        {
            var name = __originalMethod.DeclaringType!.FullName + "." + __originalMethod.Name;
            if (!__originalMethod.IsStatic) __args = __args.Prepend(__instance!).ToArray();
            harmonyRpcMap[name].Invoke(__args);
            return false;
        }

        var prefixInfo = RpcPrefix;

        var newMethod = harmony.Patch(method, new HarmonyMethod(prefixInfo.Method));
    }

    public static void Load()
    {
        var types = Assembly.GetAssembly(typeof(RemoteProcessBase))?.GetTypes().Where((type) => type.IsDefined(typeof(CustomRpcHolderAttribute)));
        if (types == null) return;

        foreach (var type in types)
        {
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            var methods = type.GetMethods().Where(m => m.IsDefined(typeof(CustomRpcAttribute))).ToList();
            foreach (var method in methods) WrapRpcMethod(Main.Instance.Harmony, method);
        }
    }

    public virtual void Receive(MessageReader reader) { }
}

public static class RemoteProcessAsset
{
    private static Dictionary<Type, (Action<MessageWriter, object>, Func<MessageReader, object>)> defaultProcessDic = new();

    static RemoteProcessAsset()
    {
        defaultProcessDic[typeof(byte)] = ((writer, obj) => writer.Write((byte)obj), (reader) => reader.ReadByte());
        defaultProcessDic[typeof(short)] = ((writer, obj) => writer.Write((short)obj), (reader) => reader.ReadInt16());
        defaultProcessDic[typeof(int)] = ((writer, obj) => writer.Write((int)obj), (reader) => reader.ReadInt32());
        defaultProcessDic[typeof(ulong)] = ((writer, obj) => writer.Write((ulong)obj), (reader) => reader.ReadUInt64());
        defaultProcessDic[typeof(float)] = ((writer, obj) => writer.Write((float)obj), (reader) => reader.ReadSingle());
        defaultProcessDic[typeof(bool)] = ((writer, obj) => writer.Write((bool)obj), (reader) => reader.ReadBoolean());
        defaultProcessDic[typeof(string)] = ((writer, obj) => writer.Write((string)obj), (reader) => reader.ReadString());
        defaultProcessDic[typeof(Vector2)] = ((writer, obj) => writer.Write((Vector2)obj), (reader) => reader.ReadVector2());
        defaultProcessDic[typeof(Vector3)] = ((writer, obj) => writer.Write((Vector3)obj), (reader) => reader.ReadVector3());
        defaultProcessDic[typeof(PlayerControl)] = ((writer, obj) => { var player = (PlayerControl)obj; writer.Write(player?.PlayerId ?? byte.MaxValue); }, (reader) => reader.ReadPlayer());

        defaultProcessDic[typeof(byte[])] = ((writer, obj) => writer.WriteBytesAndSize((byte[])obj), (reader) => reader.ReadBytesAndSize().ToArray());
        defaultProcessDic[typeof(int[])] = ((writer, obj) => { var ary = (int[])obj; writer.Write(ary.Length); for (int i = 0; i < ary.Length; i++) writer.Write(ary[i]); }, (reader) => { var ary = new int[reader.ReadInt32()]; for (int i = 0; i < ary.Length; i++) ary[i] = reader.ReadInt32(); return ary; });
        defaultProcessDic[typeof(float[])] = ((writer, obj) => { var ary = (float[])obj; writer.Write(ary.Length); for (int i = 0; i < ary.Length; i++) writer.Write(ary[i]); }, (reader) => { var ary = new float[reader.ReadInt32()]; for (int i = 0; i < ary.Length; i++) ary[i] = reader.ReadSingle(); return ary; });
        defaultProcessDic[typeof(string[])] = ((writer, obj) => { var ary = (string[])obj; writer.Write(ary.Length); for (int i = 0; i < ary.Length; i++) writer.Write(ary[i]); }, (reader) => { var ary = new string[reader.ReadInt32()]; for (int i = 0; i < ary.Length; i++) ary[i] = reader.ReadString(); return ary; });
    }

    public static (Action<MessageWriter, object>, Func<MessageReader, object>) GetProcess(Type type)
    {
        if (type.IsAssignableTo(typeof(Enum)))
            return defaultProcessDic[Enum.GetUnderlyingType(type)];
        return defaultProcessDic[type];
    }

    public static void GetMessageTreater<Parameter>(out Action<MessageWriter, Parameter> sender, out Func<MessageReader, Parameter> receiver)
    {
        Type paramType = typeof(Parameter);

        if (!typeof(Parameter).IsAssignableTo(typeof(ITuple))) throw new Exception("Can not generate sender and receiver for Non-tuple object.");

        int count = 0;


        List<(Action<MessageWriter, object>, Func<MessageReader, object>)> processList = new();
        while (true)
        {
            var field = paramType.GetField("Item" + (count + 1).ToString());
            if (field == null) break;

            processList.Add(GetProcess(field.FieldType));
            count++;
        }

        var processAry = processList.ToArray();
        var constructor = paramType.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == processAry.Length);

        if (constructor == null) throw new Exception("Can not Tuple Constructor");

        sender = (writer, param) =>
        {
            ITuple tuple = (param as ITuple)!;
            for (int i = 0; i < processAry.Length; i++) processAry[i].Item1.Invoke(writer, tuple[i]!);
        };
        receiver = (reader) =>
        {
            return (Parameter)constructor.Invoke(processAry.Select(p => p.Item2.Invoke(reader)).ToArray());
        };
    }

}
public class RemoteProcess<Parameter> : RemoteProcessBase
{
    public delegate void Process(Parameter parameter, bool isCalledByMe);

    private Action<MessageWriter, Parameter> Sender { get; set; }
    private Func<MessageReader, Parameter> Receiver { get; set; }
    private Process Body { get; set; }

    public RemoteProcess(string name, Action<MessageWriter, Parameter> sender, Func<MessageReader, Parameter> receiver, Process process)
    : base(name)
    {
        Sender = sender;
        Receiver = receiver;
        Body = process;
    }

    public RemoteProcess(string name, Process process) : base(name)
    {
        Body = process;
        RemoteProcessAsset.GetMessageTreater<Parameter>(out var sender, out var receiver);
        Sender = sender;
        Receiver = receiver;
    }


    public void Invoke(Parameter parameter)
    {
        RPCRouter.SendRpc(Name, Hash, (writer) => Sender(writer, parameter), () => Body.Invoke(parameter, true));
    }

    public RPCInvoker GetInvoker(Parameter parameter)
    {
        return new RPCInvoker(Hash, (writer) => Sender(writer, parameter), () => Body.Invoke(parameter, true));
    }

    public void LocalInvoke(Parameter parameter)
    {
        Body.Invoke(parameter, true);
    }

    public override void Receive(MessageReader reader)
    {
        try
        {
            Body.Invoke(Receiver.Invoke(reader), false);
        }
        catch (Exception ex)
        {
            Error($"Error in RPC(Received: {Name})" + ex.Message);
        }
    }
}

public static class RemotePrimitiveProcess
{
    public static RemoteProcess<int> OfInteger(string name, RemoteProcess<int>.Process process) => new(name, (writer, message) => writer.Write(message), (reader) => reader.ReadInt32(), process);
    public static RemoteProcess<float> OfFloat(string name, RemoteProcess<float>.Process process) => new(name, (writer, message) => writer.Write(message), (reader) => reader.ReadSingle(), process);
    public static RemoteProcess<string> OfString(string name, RemoteProcess<string>.Process process) => new(name, (writer, message) => writer.Write(message), (reader) => reader.ReadString(), process);
    public static RemoteProcess<byte> OfByte(string name, RemoteProcess<byte>.Process process) => new(name, (writer, message) => writer.Write(message), (reader) => reader.ReadByte(), process);
    public static RemoteProcess<Vector2> OfVector2(string name, RemoteProcess<Vector2>.Process process) => new(name, (writer, message) => { writer.Write(message.x); writer.Write(message.y); }, (reader) => new(reader.ReadSingle(), reader.ReadSingle()), process);
    public static RemoteProcess<Vector3> OfVector3(string name, RemoteProcess<Vector3>.Process process) => new(name, (writer, message) => { writer.Write(message.x); writer.Write(message.y); writer.Write(message.z); }, (reader) => new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()), process);
    public static RemoteProcess<bool> OfBoolean(string name, RemoteProcess<bool>.Process process) => new(name, (writer, message) => writer.Write(message), (reader) => reader.ReadBoolean(), process);
}


[CustomRpcHolder]
public class CombinedRemoteProcess : RemoteProcessBase
{
    public static CombinedRemoteProcess CombinedRPC = new();
    private CombinedRemoteProcess() : base("CombinedRPC") { }

    public override void Receive(MessageReader reader)
    {
        int num = reader.ReadInt32();
        for (int i = 0; i < num; i++) AllTORProcess[reader.ReadInt32()].Receive(reader);
    }

    public void Invoke(params RPCInvoker[] invokers)
    {
        RPCRouter.SendRpc(Name, Hash, (writer) =>
        {
            writer.Write(invokers.Count(i => !i.IsDummy));
            foreach (var invoker in invokers)
            {
                if (!invoker.IsDummy)
                    invoker.Invoke(writer);
                else
                    invoker.InvokeLocal();
            }
        },
        () => { });
    }
}

public class RemoteProcess : RemoteProcessBase
{
    public delegate void Process(bool isCalledByMe);
    private Process Body { get; set; }
    public RemoteProcess(string name, Process process)
    : base(name)
    {
        Body = process;
    }

    public void Invoke()
    {
        RPCRouter.SendRpc(Name, Hash, (writer) => { }, () => Body.Invoke(true));
    }

    public RPCInvoker GetInvoker()
    {
        return new RPCInvoker(Hash, (writer) => { }, () => Body.Invoke(true));
    }

    public override void Receive(MessageReader reader)
    {
        try
        {
            Body.Invoke(false);
        }
        catch (Exception ex)
        {
            Error($"Error in RPC(Received: {Name})" + ex.Message);
        }
    }
}

public class DivisibleRemoteProcess<Parameter, DividedParameter> : RemoteProcessBase
{
    public delegate void Process(DividedParameter parameter, bool isCalledByMe);

    private Func<Parameter, IEnumerator<DividedParameter>> Divider;
    private Action<MessageWriter, DividedParameter> DividedSender { get; set; }
    private Func<MessageReader, DividedParameter> Receiver { get; set; }
    private Process Body { get; set; }

    public DivisibleRemoteProcess(string name, Func<Parameter, IEnumerator<DividedParameter>> divider, Action<MessageWriter, DividedParameter> dividedSender, Func<MessageReader, DividedParameter> receiver, DivisibleRemoteProcess<Parameter, DividedParameter>.Process process)
    : base(name)
    {
        Divider = divider;
        DividedSender = dividedSender;
        Receiver = receiver;
        Body = process;
    }

    public DivisibleRemoteProcess(string name, Func<Parameter, IEnumerator<DividedParameter>> divider, DivisibleRemoteProcess<Parameter, DividedParameter>.Process process)
    : base(name)
    {
        Divider = divider;
        RemoteProcessAsset.GetMessageTreater<DividedParameter>(out var sender, out var receiver);
        DividedSender = sender;
        Receiver = receiver;
        Body = process;
    }

    public void Invoke(Parameter parameter)
    {
        void dividedSend(DividedParameter param)
        {
            RPCRouter.SendRpc(Name, Hash, (writer) => DividedSender(writer, param), () => Body.Invoke(param, true));
        }
        var enumerator = Divider.Invoke(parameter);
        while (enumerator.MoveNext()) dividedSend(enumerator.Current);
    }

    public void LocalInvoke(Parameter parameter)
    {
        var enumerator = Divider.Invoke(parameter);
        while (enumerator.MoveNext()) Body.Invoke(enumerator.Current, true);
    }

    public override void Receive(MessageReader reader)
    {
        try
        {
            Body.Invoke(Receiver.Invoke(reader), false);
        }
        catch (Exception ex)
        {
            Error($"Error in RPC(Received: {Name})" + ex.Message);
        }
    }
}

#region Old RPC

[CustomRpcHolder]
public static class RPCProcedure
{
    public enum GhostInfoTypes
    {
        HandcuffNoticed,
        HandcuffOver,
        ArsonistDouse,
        GhostChat,
        BlankUsed,
        VampireTimer,
        DeathReasonAndKiller
    }

    public enum HostCommand
    {
        HostSay,
        HostSetRole,
        HostClearRole,
        HostKill,
        HostExile,
        HostRevive,
        HostClearTasks,
        HostClearVotes,
    }

    // Main Controls
    [OnGameStart, OnGameEnd]
    public static void resetVariables()
    {
        clearAndReloadMapOptions();
        clearAndReloadRoles();
        CustomRoleManager.Dispose();
        MapData.Clear();
        JackInTheBox.clearJackInTheBoxes();
        AdditionalVents.clearAndReload();
        Portal.clearPortals();
        Bloodytrail.resetSprites();
        ElectricPatch.Reset();
        setCustomButtonCooldowns();
        toggleZoom(true);
        GameStartManagerPatch.GameStartManagerUpdatePatch.startingTimer = 0;
        SurveillanceMinigamePatch.nightVisionOverlays = null;
    }

    public static void HandleShareOptions(byte numberOfOptions, MessageReader reader)
    {
        try
        {
            for (var i = 0; i < numberOfOptions; i++)
            {
                var optionId = reader.ReadPackedUInt32();
                var selection = reader.ReadPackedUInt32();
                var option = CustomOption.options.First(option => option.id == (int)optionId);
                option.updateSelection((int)selection);
            }
        }
        catch (Exception e)
        {
            Error("Error while deserializing options: " + e.Message);
        }
    }

    public static void shareGameMode(byte gm)
    {
        GameMode = (CustomGameModes)gm;
    }

    public static void stopStart(byte playerId)
    {
        if (AmongUsClient.Instance.AmHost && CustomOptionHolder.anyPlayerCanStopStart.GetBool())
        {
            GameStartManager.Instance.ResetStartState();
            PlayerControl.LocalPlayer.RpcSendChat($"{PlayerById(playerId)?.Data.PlayerName ?? "NULL"} 阻止游戏开始");
        }
    }

    public static void setRole(byte playerId, byte roleId)
    {
        var player = PlayerById(playerId);
        if (player == null) return;
        Message($"SetRole Role:{(RoleId)roleId} to player {player.Data.PlayerName}", "RpcSetRole");
        CustomRoleManager.CreateRole(player, (RoleId)roleId);
    }

    public static void setModifier(byte modifierId, byte playerId, byte flag)
    {
        var player = PlayerById(playerId);
        if (player == null) return;
        Message($"SetRole Modifier:{(RoleId)modifierId} to player {player.Data.PlayerName}", "RpcSetRole");
        CustomRoleManager.CreateModifier(player, (RoleId)modifierId);
    }

    public static void setGhostRole(byte playerId, byte roleId)
    {
        var player = PlayerById(playerId);
        if (player == null) return;
        switch ((RoleId)roleId)
        {
            case RoleId.GhostEngineer:
                GhostEngineer.Player = player;
                break;
            case RoleId.Poltergeist:
                GhostEngineer.Player = player;
                break;
            case RoleId.Specter:
                Specter.Player = player;
                break;
        }
        Message($"SetRole GhostRole:{(RoleId)roleId} to player {player.Data.PlayerName}", "RpcSetRole");
    }

    public static void HostControl(PlayerControl controller, HostCommand command, MessageReader reader)
    {
        switch (command)
        {
            case HostCommand.HostSay:
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, reader.ReadString());
                break;
            case HostCommand.HostKill:
                {
                    var target = reader.ReadPlayer();
                    if (target.IsDead()) return;

                    target.Exiled();
                    PlayerData.SetDeathReason(target, CustomDeathReason.HostKill, controller);

                    DeadBody[] array = UObject.FindObjectsOfType<DeadBody>();
                    foreach (var body in array)
                    {
                        if (body.ParentId != target.PlayerId) continue;
                        UObject.Destroy(body.gameObject);
                        break;
                    }
                }
                break;
            case HostCommand.HostRevive:
                {
                    var target = reader.ReadPlayer();
                    target?.ModRevive(true, true);
                }
                break;
            case HostCommand.HostClearTasks:
                {
                    var target = reader.ReadPlayer();
                    target.clearAllTasks();
                }
                break;
            case HostCommand.HostExile:
                ExileControllerBeginPatch.ForceExile = true;
                break;
            case HostCommand.HostClearVotes:
                if (!InMeeting) return;
                MeetingHud.Instance.playerStates.ForEach((x) =>
                {
                    x.UnsetVote();
                });
                MeetingHud.Instance.ClearVote();
                break;
            case HostCommand.HostSetRole:
                {
                    var target = reader.ReadPlayer();
                    var roleId = (RoleId)reader.ReadByte();
                    Message("SetRole Role:" + target.Data.PlayerName);
                    if (target != null && CustomRoleManager.AllRolesInfo.TryGetValue(roleId, out var info))
                    {
                        if (info.RoleType == RoleType.Impostor)
                        {
                            target.Data.Role.TeamType = RoleTeamTypes.Impostor;
                            CustomRoleManager.SetRoleType(target, RoleTypes.Impostor);
                        }
                        else
                        {
                            target.Data.Role.TeamType = RoleTeamTypes.Crewmate;
                            CustomRoleManager.SetRoleType(target, RoleTypes.Crewmate);

                        }
                        setRole((byte)roleId, target.PlayerId);
                    }
                }
                break;
            case HostCommand.HostClearRole:
                {
                    var target = reader.ReadPlayer();
                    Message("Clean Role:" + target.Data.PlayerName);
                    CustomRoleManager.RemoveRole(target);
                }
                break;
            default:
                break;
        }
    }

    public static void versionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId)
    {
        Version ver;
        if (revision < 0) ver = new Version(major, minor, build);
        else ver = new Version(major, minor, build, revision);
        GameStartManagerPatch.playerVersions[clientId] = new GameStartManagerPatch.PlayerVersion(ver, guid);
    }

    public static void useUncheckedVent(int ventId, byte playerId, byte isEnter)
    {
        var player = PlayerById(playerId);
        if (player == null) return;
        // Fill dummy MessageReader and call MyPhysics.HandleRpc as the corountines cannot be accessed
        var reader = new MessageReader();
        var bytes = BitConverter.GetBytes(ventId);
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        reader.Buffer = bytes;
        reader.Length = bytes.Length;

        JackInTheBox.startAnimation(ventId);
        player.MyPhysics.HandleRpc(isEnter != 0 ? (byte)19 : (byte)20, reader);
    }

    public static void uncheckedExilePlayer(byte targetId)
    {
        var target = PlayerById(targetId);
        target?.Exiled();
    }

    public static void dynamicMapOption(byte mapId)
    {
    }

    public static void setCrewmate(PlayerControl player)
    {
        FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
        if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId)
            PlayerControl.LocalPlayer.moveable = true;
    }


    public static void setGameStarting()
    {
        GameStartManagerPatch.GameStartManagerUpdatePatch.startingTimer = 5f;
    }

    // Role functionality

    // public static void turnToImpostor(byte targetId)
    // {
    //     var player = PlayerById(targetId);
    //     CustomRoleManager.RemoveRole(player);
    //     RoleHelpers.turnToImpostor(player);
    // }

    public static void hostKill(byte targetId)
    {
        var target = PlayerById(targetId);
        target?.Exiled();
        PlayerData.SetDeathReason(target, CustomDeathReason.HostKill, GameData.Instance.GetHost()?.Object);

        DeadBody[] array = UObject.FindObjectsOfType<DeadBody>();
        foreach (var body in array)
        {
            if (body.ParentId != targetId) continue;
            UObject.Destroy(body.gameObject);
            break;
        }
    }

    public static void hostSay(string message)
    {
        if (PlayerControl.LocalPlayer.AmOwner)
        {
            Message($"Host Say: {message}");
            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
        }
    }

    public static void RevivePlayer(byte targetId, bool clear, bool setPos)
    {
        var target = PlayerById(targetId);
        target?.ModRevive(clear, setPos);
    }

    public static void placeGarlic(Vector3 pos)
    {
        new Garlic(pos);
    }

    public static void clearGhostRoles(byte playerId)
    {
        var player = PlayerById(playerId);

        if (player == GhostEngineer.Player) GhostEngineer.ClearAndReload();
        if (player == Poltergeist.Player) Poltergeist.ClearAndReload();
        if (player == Specter.Player) Specter.ClearAndReload();
    }

    public static void setInvisibleGen(byte playerId, byte flag)
    {
        var target = PlayerById(playerId);
        if (target == null) return;
        if (flag == byte.MaxValue)
        {
            target.cosmetics.currentBodySprite.BodySprite.color = Color.white;
            target.cosmetics.colorBlindText.gameObject.SetActive(DataManager.Settings.Accessibility.ColorBlindMode);
            target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(1f);
            if (Camouflager.CamoTimer <= 0 && !MushroomSabotageActive)
                target.setDefaultLook(); // testing
            return;
        }

        target.setLook("", 6, "", "", "", "");
        var color = Color.clear;
        if (PlayerControl.LocalPlayer.Data.IsDead) color.a = 0.1f;
        target.cosmetics.currentBodySprite.BodySprite.color = color;
        target.cosmetics.colorBlindText.gameObject.SetActive(false);
        //target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(canSee ? 0.1f : 0f);
    }
    // 反复确认后不确定是否保留的就注释在这了先
    // public static void placePortal(byte[] buff)
    // {
    //     Vector3 position = Vector2.zero;
    //     position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
    //     position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
    //     _ = new Portal(position);
    // }

    // public static void usePortal(byte playerId, byte exit)
    // {
    //     Portal.startTeleport(playerId, exit);
    // }
    // public static void lawyerSetTarget(byte playerId, byte targetId)
    // {
    //     var role = PlayerById(playerId)?.GetRole<Lawyer>();
    //     var target = PlayerById(targetId);
    //     if (role != null)
    //     {
    //         role.Target = target;
    //         Lawyer.RemovePlayer.Add(target);
    //     }
    // }
    public static void useCameraTime(float time)
    {
        restrictCamerasTime -= time;
    }

    public static void useVitalsTime(float time)
    {
        restrictVitalsTime -= time;
    }

    public static void bloody(byte killerPlayerId, byte bloodyPlayerId)
    {
        if (Bloody.active.ContainsKey(killerPlayerId)) return;
        Bloody.active.TryAdd(killerPlayerId, Bloody.duration);
        Bloody.bloodyKillerMap.TryAdd(killerPlayerId, bloodyPlayerId);
    }

    public static void setFirstKill(byte playerId)
    {
        var target = PlayerById(playerId);
        if (target == null) return;
        firstKillPlayer = target;
    }

    public static void setGuesserGm(byte playerId)
    {
        var target = PlayerById(playerId);
        if (target == null) return;
        _ = new GuesserGM(target);
    }

    public static void receiveGhostInfo(byte senderId, MessageReader reader)
    {
        var sender = PlayerById(senderId);

        var infoType = (GhostInfoTypes)reader.ReadByte();
        switch (infoType)
        {
            case GhostInfoTypes.HandcuffNoticed:
                CustomRoleManager.setHandcuffedKnows(true, senderId);
                break;
            case GhostInfoTypes.HandcuffOver:
                CustomRoleManager.handcuffedKnows.Remove(senderId);
                break;
            case GhostInfoTypes.ArsonistDouse:
                if (reader.ReadPlayer().TryGetRole<Arsonist>(out var arsonist))
                {
                    arsonist.dousedPlayers.Add(PlayerById(reader.ReadByte()));
                }
                break;
            case GhostInfoTypes.GhostChat:
                string chat = reader.ReadString();
                if (CanSeeGhostInfo) FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, chat);
                break;
            case GhostInfoTypes.BlankUsed:
                CustomRoleManager.blankedList.Remove(senderId);
                break;
            case GhostInfoTypes.DeathReasonAndKiller:
                PlayerData.SetDeathReason(PlayerById(reader.ReadByte()), (CustomDeathReason)reader.ReadByte(), PlayerById(reader.ReadByte()));
                break;
        }
    }

    public static void defuseBomb(int id)
    {
        var bomb = Bomb.AllObjects.FirstOrDefault(x => x.Id == id);
        if (bomb?.GameObject == null) return;
        try
        {
            SoundEffectsManager.playAtPosition("bombDefused", bomb.GameObject.transform.position, range: Terrorist.hearRange);
        }
        catch
        {
        }

        bomb.Destroy();
        /*terroristButton.Timer = terroristButton.MaxTimer;
        terroristButton.isEffectActive = false;
        terroristButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;*/
    }
}
#endregion

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
internal class RPCHandlerPatch
{
    private static string RpcName(byte callId) => callId < 80 ? ((RpcCalls)callId).ToString() : ((CustomRPC)callId).ToString();

    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.StartRpcImmediately)), HarmonyPostfix]
    private static void LogSentRpc([HarmonyArgument(1)] byte callId)
    {
        if (callId < 250 && CustomOptionHolder.logRpcSend.GetBool())
        {
            string type = callId < 80 ? "Vanilla" : "Custom";
            Info($"RpcId: {callId} Type: {type} Name: {RpcName(callId)}", "SEND");
        }
    }

    public static void ReceiveMessage(MessageReader reader)
    {
        int id = reader.ReadInt32();
        if (RemoteProcessBase.AllTORProcess.TryGetValue(id, out var rpc))
        {
            rpc.Receive(reader);
            if (CustomOptionHolder.logRpcSend.GetBool()) Info($"RpcId: {id} Type: TORProcess Name: {rpc.Name} Size: {reader.Length}", "RECV");
        }
        else
        {
            Error("RPC NotFound Error. id: " + id);
            throw new Exception("RPC Error Occurred. (Not found: " + id + ")");
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc)), HarmonyPrefix]
    private static bool HandleRpcPatch([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
    {
        if (callId == 128)
        {
            ReceiveMessage(reader);
            return false;
        }

        var packetId = (CustomRPC)callId;
        if (callId < 240 && CustomOptionHolder.logRpcSend.GetBool())
        {
            string type = callId < 80 ? "Vanilla" : "Custom";
            Info($"RpcId: {callId} Type: {type} Name: {RpcName(callId)} Size: {reader.Length}", "RECV");
        }

        if (callId < 80) return true;

        switch (packetId)
        {
            // Main Controls
            case CustomRPC.ShareOptions:
                RPCProcedure.HandleShareOptions(reader.ReadByte(), reader);
                break;
            case CustomRPC.SetRole:
                RPCProcedure.setRole(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.SetModifier:
                RPCProcedure.setModifier(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.SetGhostRole:
                RPCProcedure.setGhostRole(reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.DraftModePickOrder:
                RoleDraft.receivePickOrder(reader.ReadByte(), reader);
                break;

            case CustomRPC.DraftModePick:
                RoleDraft.receivePick(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.VersionHandshake:
                byte major = reader.ReadByte();
                byte minor = reader.ReadByte();
                byte patch = reader.ReadByte();
                float timer = reader.ReadSingle();
                if (!AmongUsClient.Instance.AmHost && timer >= 0f) GameStartManagerPatch.timer = timer;
                int versionOwnerId = reader.ReadPackedInt32();
                byte revision = 0xFF;
                Guid guid;
                if (reader.Length - reader.Position >= 17)
                { // enough bytes left to read
                    revision = reader.ReadByte();
                    // GUID
                    byte[] gbytes = reader.ReadBytes(16);
                    guid = new Guid(gbytes);
                }
                else
                {
                    guid = new Guid(new byte[16]);
                }
                RPCProcedure.versionHandshake(major, minor, patch, revision == 0xFF ? -1 : revision, guid, versionOwnerId);
                break;

            case CustomRPC.UseUncheckedVent:
                RPCProcedure.useUncheckedVent(reader.ReadPackedInt32(), reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.UncheckedExilePlayer:
                RPCProcedure.uncheckedExilePlayer(reader.ReadByte());
                break;

            case CustomRPC.DynamicMapOption:

                GameOptionsManager.Instance.currentNormalGameOptions.MapId = reader.ReadByte();
                break;

            case CustomRPC.SetGameStarting:
                RPCProcedure.setGameStarting();
                break;

            case CustomRPC.ShareGameMode:
                RPCProcedure.shareGameMode(reader.ReadByte());
                break;
            case CustomRPC.StopStart:
                RPCProcedure.stopStart(reader.ReadByte());
                break;

            // Game mode
            case CustomRPC.SetGuesserGm:
                RPCProcedure.setGuesserGm(reader.ReadByte());
                break;
            case CustomRPC.ShareGhostInfo:
                RPCProcedure.receiveGhostInfo(reader.ReadByte(), reader);
                break;
            case CustomRPC.ShareDeathReasonAndKiller:
                PlayerData.SetDeathReason(reader.ReadPlayer(), (CustomDeathReason)reader.ReadByte(), reader.ReadPlayer());
                break;
            case CustomRPC.NoCheckStartMeeting:
                Helpers.NoCheckStartMeeting(reader.ReadPlayer(), reader.ReadPlayer()?.Data, reader.ReadBoolean());
                break;
            case CustomRPC.NetworkTransform:
                ModdedNetworkTransform.ReceivedNetworkTransform(reader);
                break;
        }

        return false;
    }
}
