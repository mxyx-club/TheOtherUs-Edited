namespace TheOtherRoles.Objects;

public abstract class CustomObject : IDisposable
{
    public static List<CustomObject> AllObject = new();
    public virtual GameObject GameObject { get; set; }
    public virtual SpriteRenderer Renderer { get; set; }

    public virtual void Destroy()
    {
        if (GameObject != null)
        {
            UObject.Destroy(GameObject);
            GameObject = null;
        }
        if (Renderer != null)
        {
            UObject.Destroy(Renderer);
            Renderer = null;
        }
        AllObject.Remove(this);
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
        GameObject = null;
        Renderer = null;
        AllObject.Add(this);
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