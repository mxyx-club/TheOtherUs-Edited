using System;
using System.Collections.Generic;
using System.Linq;
using InnerNet;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Utilities;

public static class EventUtility
{
    public enum EventTypes
    {
        Communication,
        Animation,
        Invert,
        KnockKnock
    }

    public static readonly float[] eventFrequencies = { 15f, 60f, 60f, 300f };
    public static readonly float[] eventDurations = { 0f, 1f, 5f, 0f };
    public static double[] eventProbabilities;
    private static bool knocked;
    public static bool disableHorses;

    private static List<EventTypes> eventQueue;
    public static bool eventInvert;

    private static readonly DateTime enabled = DateTime.FromBinary(-8585213068854775808);


    private static string defaultHat = "default";
    public static bool isEventDate => DateTime.Today.Date == enabled;

    public static bool canBeEnabled =>
        DateTime.Today.Date > enabled && DateTime.Today.Date <= enabled.AddDays(7); // One Week after the EVENT

    public static bool isEnabled => isEventDate || (canBeEnabled &&
                                                    CustomOptionHolder.enableCodenameHorsemode != null &&
                                                    CustomOptionHolder.enableCodenameHorsemode.getBool());

    public static void Load()
    {
        if (!isEnabled) return;
        eventProbabilities = new double[6];
        foreach (EventTypes curEvent in Enum.GetValues(typeof(EventTypes)))
        {
            var desired_trials = 60 * eventFrequencies[(int)curEvent];
            eventProbabilities[(int)curEvent] = desired_trials != 0 ? 1f / desired_trials : 1f;
        }
    }

    public static void clearAndReload()
    {
        eventQueue = new List<EventTypes>();
        eventInvert = false;
        if (canBeEnabled && CustomOptionHolder.enableCodenameDisableHorses != null)
            disableHorses = CustomOptionHolder.enableCodenameDisableHorses.getBool();
    }

    public static void Update()
    {
        if (!isEnabled || eventQueue == null || AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started ||
            rnd == null || IntroCutscene.Instance) return;
        foreach (var curEvent in eventQueue.ToArray())
            if (rnd.NextSingle() < eventProbabilities[(int)curEvent])
            {
                eventQueue.Remove(curEvent);
                StartEvent(curEvent);
            }

        AddToQueue(EventTypes.Animation);
        AddToQueue(EventTypes.Invert);
        if (!knocked) AddToQueue(EventTypes.KnockKnock);
    }

    public static void AddToQueue(EventTypes newEvent)
    {
        if (!isEnabled || eventQueue == null || eventQueue.Contains(newEvent)) return;
        eventQueue.Add(newEvent);
    }

    public static void meetingEndsUpdate()
    {
        if (!isEnabled) return;
        // TODO - Implement Horse hats
        // PlayerControl.LocalPlayer.RpcSetHat(CustomHatLoader.horseHatProductIds[rnd.Next(CustomHatLoader.horseHatProductIds.Count)]);
    }


    public static void meetingStartsUpdate()
    {
        if (!isEnabled) return;
        if (rnd.NextDouble() <= 0.3f) AddToQueue(EventTypes.Communication);
        PlayerControl.LocalPlayer.RpcSetHat(defaultHat);
        HudManager.Instance.StartCoroutine(Effects.Lerp(1f, new Action<float>(p =>
        {
            if (MeetingHud.Instance && MeetingHud.Instance.playerStates != null)
                foreach (var pva in MeetingHud.Instance.playerStates)
                {
                    var pInfo = GameData.Instance.AllPlayers.ToArray().First(x => x.PlayerId == pva.TargetPlayerId);
                    pva.SetCosmetics(pInfo); // Needed cause cosmetics are set async'd.
                }
        })));
    }

    public static void gameStartsUpdate()
    {
        if (!isEnabled) return;
        defaultHat = PlayerControl.LocalPlayer.Data.DefaultOutfit.HatId;
        meetingEndsUpdate();
        var relevantPlayers = CachedPlayer.AllPlayers.Where(x => !x.Data.IsDead && x != CachedPlayer.LocalPlayer)
            .ToList();
        foreach (var pc in relevantPlayers)
            pc.PlayerControl.MyPhysics.SetBodyType(disableHorses ? PlayerBodyTypes.Normal : PlayerBodyTypes.Horse);
    }

    public static void gameEndsUpdate()
    {
        if (!isEnabled) return;
        if (defaultHat != "default")
            PlayerControl.LocalPlayer.RpcSetHat(defaultHat);
    }

    public static void StartEvent(EventTypes eventToStart)
    {
        var relevantPlayers = CachedPlayer.AllPlayers.Where(x => !x.Data.IsDead && x != CachedPlayer.LocalPlayer)
            .ToList();
        switch (eventToStart)
        {
            case EventTypes.Animation:
                var animationPlayer = relevantPlayers[rnd.Next(relevantPlayers.Count)];
                animationPlayer.PlayerPhysics.SetBodyType(rnd.Next(2) > 0
                    ? disableHorses ? PlayerBodyTypes.Horse : PlayerBodyTypes.Normal
                    : PlayerBodyTypes.Seeker);
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(
                    eventDurations[(int)EventTypes.Animation], new Action<float>(p =>
                    {
                        if (p == 1)
                            animationPlayer.PlayerControl.MyPhysics.SetBodyType(disableHorses
                                ? PlayerBodyTypes.Normal
                                : PlayerBodyTypes.Horse);
                    })));
                break;
            case EventTypes.Communication:
                var index = rnd.Next(relevantPlayers.Count);
                var firstPlayer = relevantPlayers[index];
                relevantPlayers.RemoveAt(index);
                var msg = firstPlayer.Data.PlayerName + " ";
                foreach (var pc in relevantPlayers.ToArray())
                    if (rnd.NextSingle() < 1f / relevantPlayers.Count)
                    {
                        relevantPlayers.Remove(pc);
                        msg += pc.Data.PlayerName + " ";
                    }

                var tracker = FastDestroyableSingleton<HudManager>.Instance.roomTracker;
                var lastRoom = "";
                try
                {
                    lastRoom =
                        FastDestroyableSingleton<TranslationController>.Instance.GetString(tracker.LastRoom.RoomId);
                }
                catch
                {
                }

                msg += lastRoom != "" ? lastRoom : rnd.Next(2) > 0 ? "sus" : "safe";

                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    relevantPlayers[rnd.Next(relevantPlayers.Count)], $"{msg}");
                break;
            case EventTypes.Invert:
                eventInvert = true;
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(
                    eventDurations[(int)EventTypes.Invert], new Action<float>(p =>
                    {
                        if (p == 1)
                            eventInvert = false;
                    })));
                break;
            case EventTypes.KnockKnock:
                SoundEffectsManager.play("knockKnock");
                knocked = true;
                break;
        }
    }
}