namespace TheOtherRoles.Objects;

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