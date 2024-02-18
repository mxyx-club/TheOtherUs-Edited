using System;
using System.Collections.Generic;
using System.Linq;
using PowerTools;
using TheOtherRoles.Helper;
using TheOtherRoles.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Objects;

public class JackInTheBox
{
    public static List<JackInTheBox> AllJackInTheBoxes = new();
    public static int JackInTheBoxLimit = 3;
    public static bool boxesConvertedToVents;
    public static Sprite[] boxAnimationSprites = new Sprite[18];
    private readonly SpriteRenderer boxRenderer;

    private readonly GameObject gameObject;
    private readonly SpriteRenderer ventRenderer;
    public Vent vent;

    public JackInTheBox(Vector2 p)
    {
        gameObject = new GameObject("JackInTheBox") { layer = 11 };
        gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        var position = new Vector3(p.x, p.y, (p.y / 1000f) + 0.01f);
        position += (Vector3)CachedPlayer.LocalPlayer.PlayerControl.Collider
            .offset; // Add collider offset that DoMove moves the player up at a valid position
        // Create the marker
        gameObject.transform.position = position;
        boxRenderer = gameObject.AddComponent<SpriteRenderer>();
        boxRenderer.sprite = getBoxAnimationSprite(0);
        boxRenderer.color = boxRenderer.color.SetAlpha(0.5f);

        // Create the vent
        var referenceVent = Object.FindObjectOfType<Vent>();
        vent = Object.Instantiate(referenceVent);
        vent.gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        vent.transform.position = gameObject.transform.position;
        vent.Left = null;
        vent.Right = null;
        vent.Center = null;
        vent.EnterVentAnim = null;
        vent.ExitVentAnim = null;
        vent.Offset = new Vector3(0f, 0.25f, 0f);
        vent.GetComponent<SpriteAnim>()?.Stop();
        vent.Id = MapUtilities.CachedShipStatus.AllVents.Select(x => x.Id).Max() + 1; // Make sure we have a unique id
        ventRenderer = vent.GetComponent<SpriteRenderer>();
        if (Helpers.isFungle())
        {
            ventRenderer = vent.transform.GetChild(3).GetComponent<SpriteRenderer>();
            var animator = vent.transform.GetChild(3).GetComponent<SpriteAnim>();
            animator?.Stop();
        }

        //ventRenderer.Destroy();
        ventRenderer.sprite = null; // Use the box.boxRenderer instead
        vent.myRend = ventRenderer;
        var allVentsList = MapUtilities.CachedShipStatus.AllVents.ToList();
        allVentsList.Add(vent);
        MapUtilities.CachedShipStatus.AllVents = allVentsList.ToArray();
        vent.gameObject.SetActive(false);
        vent.name = "JackInTheBoxVent_" + vent.Id;

        // Only render the box for the Trickster and for Ghosts
        var showBoxToLocalPlayer = CachedPlayer.LocalPlayer.PlayerControl == Trickster.trickster ||
                                   PlayerControl.LocalPlayer.Data.IsDead;
        gameObject.SetActive(showBoxToLocalPlayer);

        AllJackInTheBoxes.Add(this);
    }

    public static Sprite getBoxAnimationSprite(int index)
    {
        if (boxAnimationSprites == null || boxAnimationSprites.Length == 0) return null;
        index = Mathf.Clamp(index, 0, boxAnimationSprites.Length - 1);
        if (boxAnimationSprites[index] == null)
            boxAnimationSprites[index] =
                Helpers.loadSpriteFromResources(
                    $"TheOtherRoles.Resources.TricksterAnimation.trickster_box_00{index + 1:00}.png", 175f);
        return boxAnimationSprites[index];
    }

    public static void startAnimation(int ventId)
    {
        var box = AllJackInTheBoxes.FirstOrDefault(x => x?.vent != null && x.vent.Id == ventId);
        if (box == null) return;

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(0.6f, new Action<float>(p =>
        {
            if (box.boxRenderer != null)
            {
                box.boxRenderer.sprite = getBoxAnimationSprite((int)(p * boxAnimationSprites.Length));
                if (p == 1f) box.boxRenderer.sprite = getBoxAnimationSprite(0);
            }
        })));
    }

    public static void UpdateStates()
    {
        if (boxesConvertedToVents) return;
        foreach (var box in AllJackInTheBoxes)
        {
            var showBoxToLocalPlayer = CachedPlayer.LocalPlayer.PlayerControl == Trickster.trickster ||
                                       PlayerControl.LocalPlayer.Data.IsDead;
            box.gameObject.SetActive(showBoxToLocalPlayer);
        }
    }

    public void convertToVent()
    {
        gameObject.SetActive(true);
        vent.gameObject.SetActive(true);
        boxRenderer.color = boxRenderer.color.SetAlpha(1f);
        ventRenderer.sprite = null;
    }

    public static void convertToVents()
    {
        foreach (var box in AllJackInTheBoxes) box.convertToVent();
        connectVents();
        boxesConvertedToVents = true;
    }

    public static bool hasJackInTheBoxLimitReached()
    {
        return AllJackInTheBoxes.Count >= JackInTheBoxLimit;
    }

    private static void connectVents()
    {
        for (var i = 0; i < AllJackInTheBoxes.Count - 1; i++)
        {
            var a = AllJackInTheBoxes[i];
            var b = AllJackInTheBoxes[i + 1];
            a.vent.Right = b.vent;
            b.vent.Left = a.vent;
        }

        // Connect first with last
        AllJackInTheBoxes.First().vent.Left = AllJackInTheBoxes.Last().vent;
        AllJackInTheBoxes.Last().vent.Right = AllJackInTheBoxes.First().vent;
    }

    public static void clearJackInTheBoxes()
    {
        boxesConvertedToVents = false;
        AllJackInTheBoxes = new List<JackInTheBox>();
    }
}