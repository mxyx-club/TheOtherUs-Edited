using TheOtherRoles.Attributes;

namespace TheOtherRoles.Objects;

#nullable enable
public interface ICustomObject
{
    GameObject? GameObject { get; set; }
    bool IsActive { get; set; }

    void OnMeetingStart();
    void OnMeetingEnd(MeetingHud __instance);
    void Update();
    void OnDestroy();
}

public abstract class CustomObject : ICustomObject
{
    public static IEnumerable<CustomObject> AllCustomObject => _AllCustomObject.AsReadOnly();
    public static List<CustomObject> _AllCustomObject = new();

    public GameObject? GameObject { get; set; }
    public SpriteRenderer? Renderer { get; set; }
    public MonoBehaviour? Behaviour { get; set; }
    public bool IsActive { get; set; }

    public int Id { get; private set; }
    protected static int maxId;
    protected DateTime placedTime;

    protected CustomObject()
    {
        placedTime = DateTime.Now;
        Id = maxId++;
        GameObject = new GameObject("Custom Object " + Id);
        Renderer = GameObject.AddComponent<SpriteRenderer>();
        Behaviour = GameObject.AddComponent<CustomObjectBehaviour>();
        GameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        GameObject.SetActive(false);
        IsActive = false;
        _AllCustomObject.Add(this);
    }

    public void Destroy()
    {
        OnDestroy();
        GameObject?.Destroy();
        GameObject = null;
        Renderer = null;
        Behaviour = null;
        _AllCustomObject.Remove(this);
    }

    public virtual void OnMeetingStart() { }
    public virtual void OnMeetingEnd(MeetingHud __instance) { }
    public virtual void Update() { }
    public virtual void OnDestroy() { }

    [OnGameStart, OnGameEnd]
    public static void DestroyAll()
    {
        var list = AllCustomObject.ToArray();
        foreach (var obj in list)
        {
            obj?.Destroy();
        }
        _AllCustomObject = new();
        maxId = 0;
    }

    public static void StartMeeting()
    {
        foreach (var obj in AllCustomObject.ToArray())
        {
            obj?.OnMeetingStart();
        }
    }

    public static void EndMeeting(MeetingHud __instance)
    {
        foreach (var obj in AllCustomObject.ToArray())
        {
            obj?.OnMeetingEnd(__instance);
        }
    }

    public static void UpdateAll()
    {
        foreach (var obj in AllCustomObject.ToArray())
        {
            obj?.Update();
        }
    }
}

public abstract class CustomObjectBase<T> : CustomObject where T : CustomObjectBase<T>
{
    public static List<T> AllObjects = new();

    protected CustomObjectBase() : base()
    {
        AllObjects.Add((T)this);
    }

    public override void OnDestroy()
    {
        AllObjects.Remove((T)this);
        base.OnDestroy();
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
        try
        {
            if (HudManager.Instance != null && HudManager.Instance?.PlayerCam?.Target == this)
            {
                HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
            }
        }
        catch (Exception e)
        {
            Error("\n" + e, "CustomObjectBehaviour");
        }
    }
}

public static class CustomObjectExtensions
{
    public static void SetActive(this CustomObject obj, bool active)
    {
        if (obj.GameObject != null)
        {
            obj.GameObject.SetActive(active);
            obj.IsActive = active;
        }
    }

    public static T? FindWithInRange<T>(this IEnumerable<T> list, Vector3 pos, float maxDistance = 3f, bool onlyActive = true) where T : CustomObject
    {
        T? @object = null;
        float closestSqr = maxDistance * maxDistance;

        foreach (var obj in list)
        {
            if (obj.GameObject == null) continue;
            if (onlyActive && !obj.IsActive) continue;

            Vector3 offset = obj.GameObject.transform.position - pos;
            float distSqr = offset.sqrMagnitude;

            if (distSqr < closestSqr)
            {
                closestSqr = distSqr;
                @object = obj;
            }
        }

        return @object;
    }
}