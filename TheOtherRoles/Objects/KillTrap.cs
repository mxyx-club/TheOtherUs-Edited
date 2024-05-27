using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using TheOtherRoles.Patches;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Objects;
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
    private static byte maxId = 0;
    public AudioSource audioSource;
    public static SortedDictionary<byte, KillTrap> traps = new();
    public bool isActive = false;
    public PlayerControl target;
    public DateTime placedTime;

    public static void loadSprite()
    {
        if (trapSprite == null)
            trapSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Trap.png", 300f);
        if (trapActiveSprite == null)
            trapActiveSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TrapActive.png", 300f);

    }

    private static byte getAvailableId()
    {
        byte ret = maxId;
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
                    UnityEngine.Object.DestroyObject(firstTrap.killtrap);
                traps.Remove(key);
                break;
            }
        }

        // 罠を設置
        this.killtrap = new GameObject("Trap");
        var trapRenderer = killtrap.AddComponent<SpriteRenderer>();
        killtrap.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        trapRenderer.sprite = trapSprite;
        Vector3 position = new(pos.x, pos.y, pos.y / 1000 + 0.001f);
        this.killtrap.transform.position = position;
        // this.trap.transform.localPosition = pos;
        this.killtrap.SetActive(true);

        // 音を鳴らす
        this.audioSource = killtrap.gameObject.AddComponent<AudioSource>();
        this.audioSource.priority = 0;
        this.audioSource.spatialBlend = 1;
        this.audioSource.clip = place;
        this.audioSource.loop = false;
        this.audioSource.playOnAwake = false;
        this.audioSource.maxDistance = 2 * EvilTrapper.maxDistance / 3;
        this.audioSource.minDistance = EvilTrapper.minDistance;
        this.audioSource.rolloffMode = rollOffMode;
        this.audioSource.PlayOneShot(place);

        // 設置時刻を設定
        this.placedTime = DateTime.UtcNow;

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
            UnityEngine.Object.Destroy(t.killtrap);
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

        bool moveableFlag = false;
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
                { // 正常にキルが発生する場合の処理
                    target.moveable = true;
                    if (CachedPlayer.LocalPlayer.PlayerControl == EvilTrapper.evilTrapper)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.TrapperKill, SendOption.Reliable, -1);
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
                if (trap.killtrap != null)
                    trap.killtrap.SetActive(false);
                UnityEngine.Object.Destroy(trap.killtrap);
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
                UnityEngine.Object.DestroyObject(trap.killtrap);
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
                    CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead ||
                    CachedPlayer.LocalPlayer.PlayerControl == TheOtherRoles.Lighter.lighter;
                var opacity = canSee ? 1.0f : 0.0f;
                if (trap.killtrap != null)
                    trap.killtrap.GetComponent<SpriteRenderer>().material.color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
            }
        }
    }
}