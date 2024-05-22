using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Hazel;
using InnerNet;
using UnityEngine;

namespace TheOtherRoles.Helper;

#nullable enable
internal class FastRpcWriter
{
    private readonly RPCSendMode _rpcSendMode;
    private byte CallId;

    private int msgCount;

    private SendOption Option;

    private int SendTargetId;

    private uint targetObjectId;

    private MessageWriter? writer;

    private FastRpcWriter(SendOption option, RPCSendMode mode = RPCSendMode.SendToAll, int TargetId = -1,
        uint ObjectId = 255)
    {
        Option = option;
        _rpcSendMode = mode;
        SetTargetId(TargetId);
        SetTargetObjectId(ObjectId);
    }

    private static FastRpcWriter StartNew(SendOption option = SendOption.Reliable,
        RPCSendMode mode = RPCSendMode.SendToAll, int TargetId = -1, uint targetObjectId = 255)
    {
        var writer = new FastRpcWriter(option, mode, TargetId, targetObjectId);
        writer.CreateWriter();
        return writer;
    }

    internal static FastRpcWriter StartNewRpcWriter(CustomRPC rpc, SendOption option = SendOption.Reliable,
        RPCSendMode mode = RPCSendMode.SendToAll, int TargetId = -1, uint targetObjectId = 255)
    {
        var writer = StartNew(option, mode, TargetId, targetObjectId);
        writer.SetRpcCallId(rpc);

        if (mode == RPCSendMode.SendToAll)
            writer.StartDataAllMessage();

        if (mode == RPCSendMode.SendToPlayer)
            writer.StartDataToPlayerMessage();

        writer.StartRPCMessage();
        return writer;
    }
    
    internal static FastRpcWriter StartNewRpcWriter(CustomRPC rpc, InnerNetObject obNetObject, SendOption option = SendOption.Reliable,
        RPCSendMode mode = RPCSendMode.SendToAll, int TargetId = -1)
    {
        var writer = StartNewRpcWriter(rpc, option, mode, TargetId, obNetObject.NetId);
        return writer;
    }

    public FastRpcWriter CreateWriter()
    {
        Clear();
        writer = MessageWriter.Get(Option);
        return this;
    }

    public FastRpcWriter StartSendAllRPCWriter()
    {
        CreateWriter();
        StartDataAllMessage();
        StartRPCMessage();
        return this;
    }

    public FastRpcWriter StartSendToPlayerRPCWriter()
    {
        CreateWriter();
        StartDataToPlayerMessage();
        StartRPCMessage();
        return this;
    }

    public FastRpcWriter SetSendOption(SendOption option)
    {
        Option = option;
        return this;
    }

    public FastRpcWriter SetTargetObjectId(uint id)
    {
        if (id == 255)
        {
            targetObjectId = PlayerControl.LocalPlayer.NetId;
            return this;
        }

        targetObjectId = id;
        return this;
    }

    public FastRpcWriter SetRpcCallId(CustomRPC id)
    {
        CallId = (byte)id;
        return this;
    }

    public FastRpcWriter SetRpcCallId(byte id)
    {
        CallId = id;
        return this;
    }

    public FastRpcWriter SetTargetId(int id)
    {
        if (id == -1)
            return this;

        SendTargetId = id;
        return this;
    }

    public void Clear()
    {
        if (writer == null) return;
        Recycle();
        writer = null;
    }

    public FastRpcWriter Write(bool value)
    {
        writer?.Write(value);
        return this;
    }

    public FastRpcWriter Write(int value)
    {
        writer?.Write(value);
        return this;
    }

    public FastRpcWriter Write(float value)
    {
        writer?.Write(value);
        return this;
    }

    public FastRpcWriter Write(string value)
    {
        writer?.Write(value);
        return this;
    }

    public FastRpcWriter Write(byte value)
    {
        writer?.Write(value);
        return this;
    }

    public FastRpcWriter Write(byte[] value)
    {
        writer?.Write(value);
        return this;
    }

    public FastRpcWriter Write(Vector2 value)
    {
        writer?.Write(value.x);
        writer?.Write(value.y);
        return this;
    }

    public FastRpcWriter Write(Vector3 value)
    {
        writer?.Write(value.x);
        writer?.Write(value.y);
        writer?.Write(value.z);
        return this;
    }

    public FastRpcWriter Write(Rect value)
    {
        writer?.Write(value.x);
        writer?.Write(value.y);
        writer?.Write(value.width);
        writer?.Write(value.height);
        return this;
    }

    public FastRpcWriter Write(params object[]? objects)
    {
        if (objects == null) return this;

        foreach (var obj in objects)
            switch (obj)
            {
                case byte _byte:
                    Write(_byte);
                    break;
                case string _string:
                    Write(_string);
                    break;
                case float _float:
                    Write(_float);
                    break;
                case int _int:
                    Write(_int);
                    break;
                case bool _bool:
                    Write(_bool);
                    break;
                case byte[] _bytes:
                    Write(_bytes);
                    break;
            }

        return this;
    }

    public FastRpcWriter WritePacked(int value)
    {
        writer?.WritePacked(value);
        return this;
    }

    public FastRpcWriter WritePacked(uint value)
    {
        writer?.WritePacked(value);
        return this;
    }

    private void StartDataAllMessage()
    {
        StartMessage((byte)_rpcSendMode);
        Write(AmongUsClient.Instance.GameId);
    }

    private void StartDataToPlayerMessage()
    {
        StartMessage((byte)_rpcSendMode);
        Write(AmongUsClient.Instance.GameId);
        WritePacked(SendTargetId);
    }

    private void StartRPCMessage()
    {
        StartMessage(2);
        WritePacked(targetObjectId);
        Write(CallId);
    }

    public FastRpcWriter StartMessage(byte flag)
    {
        writer?.StartMessage(flag);
        msgCount++;
        return this;
    }

    public FastRpcWriter EndMessage()
    {
        writer?.EndMessage();
        msgCount--;
        return this;
    }

    public void EndAllMessage()
    {
        while (msgCount > 0)
            EndMessage();
    }

    public void Recycle()
    {
        writer?.Recycle();
    }

    public void RPCSend()
    {
        EndAllMessage();
        AmongUsClient.Instance.SendOrDisconnect(writer);
        Recycle();
    }
}

public static class FastRPCExtension
{
    public static Vector2 ReadVector2(this MessageReader reader)
    {
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        return new Vector2(x, y);
    }

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
}

internal enum RPCSendMode
{
    SendToAll = 5,
    SendToPlayer = 6
}

[Harmony]
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
internal class RPCListener : Attribute
{
    private static readonly List<RPCListener> _allListeners = [];
    public Action<MessageReader> OnRPC = null!;
    private readonly CustomRPC RPCId;

    public RPCListener(CustomRPC rpc)
    {
        RPCId = rpc;
        OnRPC += n => Info($"{RPCId} {n.Tag} {n.Length}");
    }

    public static void Register(Assembly assembly)
    {
        var types = assembly.GetTypes().SelectMany(n => n.GetMethods())
            .Where(n => n.IsDefined(typeof(RPCListener)));
        types.Do(n =>
        {
            var listener = n.GetCustomAttribute<RPCListener>();
            if (listener == null) return;
            listener.OnRPC += reader => n.Invoke(null, [reader]);
            Info($"add listener {n.Name} {n.GetType().Name}");
            _allListeners.Add(listener);
        });
    }

    [HarmonyPatch(typeof(InnerNetClient._HandleGameDataInner_d__39), nameof(InnerNetClient._HandleGameDataInner_d__39.MoveNext))]
    [HarmonyPrefix]
    private static void InnerNet_ReaderPath(InnerNetClient._HandleGameDataInner_d__39 __instance)
    {
        if (_allListeners.Count <= 0) return;
        var innerNetClient = __instance.__4__this;
        var reader = __instance.reader;

        if (__instance.__1__state != 0) return;
        
        var HandleReader = MessageReader.Get(reader);
        HandleReader.Position = 0;
        var tag = reader.Tag;
        if (tag != 2)
            goto recycle;
        var objetId = HandleReader.ReadPackedUInt32();
        var callId = HandleReader.ReadByte();
        if (_allListeners.All(n => n.RPCId != (CustomRPC)callId)) 
            goto recycle;
        try
        {
            _allListeners.Where(n => n.RPCId == (CustomRPC)callId).Do(n => n.OnRPC.Invoke(HandleReader));
            Info("Listener");
        }
        catch (Exception e)
        {
            Exception(e);
        }

        finally
        {
            HandleReader.Recycle();
        }
        
        return;
        recycle:
          HandleReader.Recycle();
          return;
    }
}

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
internal class RPCMethod(CustomRPC rpc) : Attribute
{
    public static readonly List<RPCMethod> _AllRPCMethod =
        [];

    private readonly CustomRPC RPC = rpc;

    private Type[] _types = [];

    public Action<object[]>? Start;

    private int count => _types.Length;

    public static void Register(Assembly assembly)
    {
        var types = assembly.GetTypes().SelectMany(n => n.GetMethods(BindingFlags.Static))
            .Where(n => n.IsDefined(typeof(RPCMethod)));
        types.Do(n =>
        {
            var method = n.GetCustomAttribute<RPCMethod>();
            if (method == null) return;
            method.Start = objs => n.Invoke(null, objs);
            method._types = n.GetGenericArguments();
            _AllRPCMethod.Add(method);
        });
    }

    public bool Match(object[] objects)
    {
        if (objects.Length != count) return false;
        for (var i = 0; i < count; i++)
            if (objects[i].GetType() != _types[i])
                return false;
        return true;
    }

    public static void StartRPCMethod(CustomRPC rpc, params object[] objects)
    {
        _AllRPCMethod.Where(n => n.RPC == rpc && n.Match(objects)).Do(n => n.Start?.Invoke(objects));
    }
}