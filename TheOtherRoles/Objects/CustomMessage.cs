using System;
using System.Collections.Generic;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Objects;

public class CustomMessage
{
    private static readonly List<CustomMessage> customMessages = new();

    private readonly TMP_Text text;

    public CustomMessage(string message, float duration)
    {
        var roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
        if (roomTracker != null)
        {
            var gameObject = Object.Instantiate(roomTracker.gameObject);

            gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
            Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
            text = gameObject.GetComponent<TMP_Text>();
            text.text = message;

            // Use local position to place it in the player's view instead of the world location
            gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
            customMessages.Add(this);

            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>(p =>
            {
                var even = (int)(p * duration / 0.25f) % 2 == 0; // Bool flips every 0.25 seconds
                var prefix = even ? "<color=#FCBA03FF>" : "<color=#FF0000FF>";
                text.text = prefix + message + "</color>";
                if (text != null) text.color = even ? Color.yellow : Color.red;
                if (p == 1f && text != null && text.gameObject != null)
                {
                    Object.Destroy(text.gameObject);
                    customMessages.Remove(this);
                }
            })));
        }
    }
}