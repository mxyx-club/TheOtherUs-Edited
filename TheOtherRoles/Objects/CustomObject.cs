using TheOtherRoles.Attributes;

namespace TheOtherRoles.Objects;

#nullable enable
public interface ICustomObject
{
    GameObject? GameObject { get; set; }
}

public abstract class CustomObject : IDisposable, ICustomObject
{
    public static List<CustomObject> AllObject = new();
    public GameObject? GameObject { get; set; }
    public SpriteRenderer? Renderer { get; set; }
    public bool IsActive { get; set; }

    public DateTime placedTime;

    public int Id;
    private static int maxId;

    public virtual void Destroy()
    {
        GameObject?.Destroy();
        GameObject = null;
        Renderer?.Destroy();
        Renderer = null;
        AllObject.Remove(this);
    }

    public virtual void OnMeetingStart()
    {
    }

    public virtual void OnMeetingEnd()
    {
    }

    public virtual void Update()
    {
    }

    public void Dispose()
    {
        Destroy();
    }

    public CustomObject()
    {
        Id = maxId++;
        GameObject = new GameObject("Custom Object " + Id);
        Renderer = GameObject.AddComponent<SpriteRenderer>();
        GameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        placedTime = DateTime.Now;
        AllObject.Add(this);
    }

    [OnGameStart, OnGameEnd]
    public static void DestroyAll()
    {
        foreach (var obj in AllObject.ToArray())
        {
            obj?.Destroy();
        }
        AllObject.Clear();
        maxId = 0;
    }

    public static void EndMeeting()
    {
        foreach (var obj in AllObject.ToArray())
        {
            obj?.OnMeetingEnd();
        }
    }

    public static void StartMeeting()
    {
        foreach (var obj in AllObject.ToArray())
        {
            obj?.OnMeetingStart();
        }
    }

    public static void UpdateAll()
    {
        foreach (var obj in AllObject.ToArray())
        {
            obj?.Update();
        }
    }

    public static T? FindWithInRange<T>(List<T> list, Vector3 pos, float maxDistance = 3f, bool onlyActive = true) where T : CustomObject
    {
        T? nearestObject = null;
        float closestDistance = maxDistance;

        foreach (var obj in list)
        {
            if (obj.GameObject == null || (onlyActive && obj.IsActive)) continue;

            float distance = Vector3.Distance(pos, obj.GameObject.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestObject = obj;
            }
        }

        return nearestObject;
    }
}

public class CustomObjectBehaviour : MonoBehaviour
{
    static CustomObjectBehaviour()
    {
        ClassInjector.RegisterTypeInIl2Cpp<CustomObjectBehaviour>();
    }

    public void OnDestroy()
    {
        if (HudManager.Instance.PlayerCam.Target == this)
        {
            HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
        }
    }
}