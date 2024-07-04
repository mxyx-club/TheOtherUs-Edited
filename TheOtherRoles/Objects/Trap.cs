using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TheOtherRoles.Patches;
using TheOtherRoles.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Objects;

public class Trap
{
    public static List<Trap> traps = new();
    public static Dictionary<byte, Trap> trapPlayerIdMap = new();

    private static int instanceCounter;

    private static Sprite trapSprite;
    private readonly Arrow arrow = new(Color.blue);
    private readonly int neededCount;
    public readonly int instanceId;
    public bool revealed;
    public readonly GameObject trap;
    public List<PlayerControl> trappedPlayer = new();
    public bool triggerable;
    private int usedCount;

    public Trap(Vector2 p)
    {
        trap = new GameObject("Trap") { layer = 11 };
        trap.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        var position = new Vector3(p.x, p.y, (p.y / 1000) + 0.001f); // just behind player
        trap.transform.position = position;
        neededCount = Trapper.trapCountToReveal;

        var trapRenderer = trap.AddComponent<SpriteRenderer>();
        trapRenderer.sprite = getTrapSprite();
        trap.SetActive(false);
        if (CachedPlayer.LocalPlayer.PlayerId == Trapper.trapper.PlayerId) trap.SetActive(true);
        trapRenderer.color = Color.white * new Vector4(1, 1, 1, 0.5f);
        instanceId = ++instanceCounter;
        traps.Add(this);
        arrow.Update(position);
        arrow.arrow.SetActive(false);
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(5, new Action<float>(x =>
        {
            if ((int)x == 1) triggerable = true;
            trapRenderer.color = Color.white;
        })));
    }

    public static Sprite getTrapSprite()
    {
        if (trapSprite) return trapSprite;
        trapSprite = loadSpriteFromResources("TheOtherRoles.Resources.Trapper_Trap_Ingame.png", 300f);
        return trapSprite;
    }

    public static void clearTraps()
    {
        foreach (var t in traps)
        {
            Object.Destroy(t.arrow.arrow);
            Object.Destroy(t.trap);
        }

        traps = [];
        trapPlayerIdMap = new Dictionary<byte, Trap>();
        instanceCounter = 0;
    }

    public static void clearRevealedTraps()
    {
        var trapsToClear = traps.FindAll(x => x.revealed);

        foreach (var t in trapsToClear)
        {
            traps.Remove(t);
            Object.Destroy(t.trap);
        }
    }

    public static void triggerTrap(byte playerId, byte trapId)
    {
        var t = traps.FirstOrDefault(x => x.instanceId == trapId);
        var player = playerById(playerId);
        if (Trapper.trapper == null || t == null || player == null) return;
        var localIsTrapper = CachedPlayer.LocalPlayer.PlayerId == Trapper.trapper.PlayerId;
        trapPlayerIdMap.TryAdd(playerId, t);
        t.usedCount++;
        t.triggerable = false;
        if (playerId == CachedPlayer.LocalPlayer.PlayerId || playerId == Trapper.trapper.PlayerId)
        {
            t.trap.SetActive(true);
            SoundEffectsManager.play("trapperTrap");
        }

        player.moveable = false;
        player.NetTransform.Halt();
        Trapper.playersOnMap.Add(player);
        if (localIsTrapper) t.arrow.arrow.SetActive(true);

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Trapper.trapDuration,
            new Action<float>(p =>
            {
                if (p == 1f)
                {
                    player.moveable = true;
                    Trapper.playersOnMap.RemoveAll(x => x == player);
                    if (trapPlayerIdMap.ContainsKey(playerId)) trapPlayerIdMap.Remove(playerId);
                    t.arrow.arrow.SetActive(false);
                }
            })));

        if (t.usedCount == t.neededCount) t.revealed = true;

        t.trappedPlayer.Add(player);
        t.triggerable = true;
    }

    public static void Update()
    {
        if (Trapper.trapper == null) return;
        var player = CachedPlayer.LocalPlayer;
        var vent = MapUtilities.CachedShipStatus.AllVents[0];
        var closestDistance = float.MaxValue;

        if (vent == null || player == null) return;
        var ud = vent.UsableDistance / 2;
        Trap target = null;
        foreach (var trap in traps)
        {
            if (trap.arrow.arrow.active) trap.arrow.Update();
            if (trap.revealed || !trap.triggerable || trap.trappedPlayer.Contains(player.PlayerControl)) continue;
            if (player.PlayerControl.inVent || !player.PlayerControl.CanMove) continue;
            var distance = Vector2.Distance(trap.trap.transform.position, player.PlayerControl.GetTruePosition());
            if (!(distance <= ud) || !(distance < closestDistance)) continue;
            closestDistance = distance;
            target = trap;
        }

        if (target != null && player.PlayerId != Trapper.trapper.PlayerId && !player.Data.IsDead)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.TriggerTrap, SendOption.Reliable);
            writer.Write(player.PlayerId);
            writer.Write(target.instanceId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.triggerTrap(player.PlayerId, (byte)target.instanceId);
        }


        if (!player.Data.IsDead || player.PlayerId == Trapper.trapper.PlayerId) return;
        foreach (var trap in traps.Where(trap => !trap.trap.active))
            trap.trap.SetActive(true);
    }
}


public class KillTrap
{
    public GameObject killtrap;
    public static Sprite trapSprite;
    public static Sprite trapActiveSprite;
    public static AudioClip place;
    public static AudioClip activate;
    public static AudioClip disable;
    public static AudioClip countdown;
    public static AudioClip kill;
    public static AudioRolloffMode rollOffMode = AudioRolloffMode.Linear;
    private static byte maxId;
    public AudioSource audioSource;
    public static SortedDictionary<byte, KillTrap> traps = new();
    public bool isActive;
    public PlayerControl target;
    public DateTime placedTime;

    public static void loadSprite()
    {
        if (trapSprite == null)
            trapSprite = loadSpriteFromResources("TheOtherRoles.Resources.Trap.png", 300f);
        if (trapActiveSprite == null)
            trapActiveSprite = loadSpriteFromResources("TheOtherRoles.Resources.TrapActive.png", 300f);

    }

    private static byte getAvailableId()
    {
        var ret = maxId;
        maxId++;
        return ret;
    }

    public KillTrap(Vector3 pos)
    {
        // 最初の罠を消す
        if (traps.Count == EvilTrapper.numTrap)
        {

            foreach (var key in traps.Keys)
            {
                var firstTrap = traps[key];
                if (firstTrap.killtrap != null)
                    Object.DestroyObject(firstTrap.killtrap);
                traps.Remove(key);
                break;
            }
        }

        // 罠を設置
        killtrap = new GameObject("Trap");
        var trapRenderer = killtrap.AddComponent<SpriteRenderer>();
        killtrap.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        trapRenderer.sprite = trapSprite;
        Vector3 position = new(pos.x, pos.y, (pos.y / 1000) + 0.001f);
        killtrap.transform.position = position;
        // this.trap.transform.localPosition = pos;
        killtrap.SetActive(true);

        // 音を鳴らす
        audioSource = killtrap.gameObject.AddComponent<AudioSource>();
        audioSource.priority = 0;
        audioSource.spatialBlend = 1;
        audioSource.clip = place;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.maxDistance = 2 * EvilTrapper.maxDistance / 3;
        audioSource.minDistance = EvilTrapper.minDistance;
        audioSource.rolloffMode = rollOffMode;
        audioSource.PlayOneShot(place);

        // 設置時刻を設定
        placedTime = DateTime.UtcNow;

        traps.Add(getAvailableId(), this);

    }

    public static void activateTrap(byte trapId, PlayerControl trapper, PlayerControl target)
    {
        var trap = traps[trapId];

        // 有効にする
        trap.isActive = true;
        trap.target = target;
        var spriteRenderer = trap.killtrap.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = trapActiveSprite;

        // 他のトラップを全て無効化する
        var newTraps = new SortedDictionary<byte, KillTrap>
            {
                { trapId, trap }
            };
        foreach (var t in traps.Values)
        {
            if (t.killtrap == null || t == trap) continue;
            t.killtrap.SetActive(false);
            Object.Destroy(t.killtrap);
        }
        traps = newTraps;


        // 音を鳴らす
        trap.audioSource.Stop();
        trap.audioSource.loop = true;
        trap.audioSource.priority = 0;
        trap.audioSource.spatialBlend = 1;
        trap.audioSource.maxDistance = EvilTrapper.maxDistance;
        trap.audioSource.clip = countdown;
        trap.audioSource.Play();

        // ターゲットを動けなくする
        target.NetTransform.Halt();

        var moveableFlag = false;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(EvilTrapper.killTimer, new Action<float>((p) =>
        {
            try
            {
                if (EvilTrapper.meetingFlag) return;
                if (trap == null || trap.killtrap == null || !trap.isActive) //　解除された場合の処理
                {
                    if (!moveableFlag)
                    {
                        target.moveable = true;
                        moveableFlag = true;
                    }
                    return;
                }
                else if ((p == 1f) && !target.Data.IsDead)
                {
                    // 正常にキルが発生する場合の処理
                    target.moveable = true;
                    if (CachedPlayer.LocalPlayer.PlayerControl == EvilTrapper.evilTrapper)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.TrapperKill, SendOption.Reliable, -1);
                        writer.Write(trapId);
                        writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.trapperKill(trapId, CachedPlayer.LocalPlayer.PlayerControl.PlayerId, target.PlayerId);
                    }
                }
                else
                { // カウントダウン中の処理
                    target.moveable = false;
                    target.NetTransform.Halt();
                    target.transform.position = trap.killtrap.transform.position + new Vector3(0, 0.3f, 0);
                }
            }
            catch (Exception e)
            {
                Error("An error occured during the countdown");
                Error(e.Message);
            }
        })));
    }

    public static void disableTrap(byte trapId)
    {
        var trap = traps[trapId];
        trap.isActive = false;
        trap.audioSource.Stop();
        trap.audioSource.PlayOneShot(disable);
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(disable.length, new Action<float>((p) =>
        {
            if (p == 1f)
            {
                trap.killtrap?.SetActive(false);
                Object.Destroy(trap.killtrap);
                traps.Remove(trapId);
            }
        })));

        if (CachedPlayer.LocalPlayer.PlayerControl == EvilTrapper.evilTrapper)
        {
            CachedPlayer.LocalPlayer.PlayerControl.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + EvilTrapper.penaltyTime;
            HudManagerStartPatch.evilTrapperSetTrapButton.Timer = EvilTrapper.cooldown + EvilTrapper.penaltyTime;
        }
    }

    public static void onMeeting()
    {
        EvilTrapper.meetingFlag = true;
        foreach (var trap in traps)
        {
            trap.Value.audioSource.Stop();
            if (trap.Value.target != null)
            {
                if (CachedPlayer.LocalPlayer.PlayerControl == EvilTrapper.evilTrapper)
                {
                    if (!trap.Value.target.Data.IsDead)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.TrapperKill, SendOption.Reliable, -1);
                        writer.Write(trap.Key);
                        writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                        writer.Write(trap.Value.target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.trapperKill(trap.Key, CachedPlayer.LocalPlayer.PlayerControl.PlayerId, trap.Value.target.PlayerId);
                    }
                }

            }
        }
    }

    public static bool hasTrappedPlayer()
    {
        foreach (var trap in traps.Values)
        {
            if (trap.target != null) return true;
        }
        return false;
    }

    public static KillTrap getActiveTrap()
    {
        foreach (var trap in traps.Values)
        {
            if (trap.target != null) return trap;
        }
        return null;
    }

    public static bool isTrapped(PlayerControl p)
    {
        foreach (var trap in traps.Values)
        {
            if (trap.target == p) return true;
        }
        return false;
    }

    public static void trapKill(byte trapId, PlayerControl trapper, PlayerControl target)
    {
        var trap = traps[trapId];
        var audioSource = trap.audioSource;

        audioSource.Stop();
        audioSource.maxDistance = EvilTrapper.maxDistance;
        audioSource.PlayOneShot(kill);
        if (target == Medic.currentTarget || target == Veteran.veteran && Veteran.alertActive || target == BodyGuard.currentTarget
         || MapOption.shieldFirstKill && MapOption.firstKillPlayer == target || target == Mini.mini)
        {
            clearAllTraps();
            checkMuderAttempt(EvilTrapper.evilTrapper, target);
        }
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(kill.length, new Action<float>((p) =>
        {
            if (p == 1f)
            {
                clearAllTraps();
            }
        })));
        EvilTrapper.isTrapKill = true;
        KillAnimationCoPerformKillPatch.hideNextAnimation = true;
        trapper.MurderPlayer(target, MurderResultFlags.Succeeded);
    }

    public static void clearAllTraps()
    {
        loadSprite();
        foreach (var trap in traps.Values)
        {
            if (trap.killtrap != null)
                Object.DestroyObject(trap.killtrap);
        }
        traps = new SortedDictionary<byte, KillTrap>();
        maxId = 0;
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class PlayerPhysicsTrapPatch
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            foreach (var trap in traps.Values)
            {
                bool canSee =
                    trap.isActive ||
                    CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor ||
                    CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead;
                var opacity = canSee ? 1.0f : 0.0f;
                if (trap.killtrap != null)
                    trap.killtrap.GetComponent<SpriteRenderer>().material.color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
            }
        }
    }
}