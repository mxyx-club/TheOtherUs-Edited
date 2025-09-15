namespace TheOtherRoles.Objects;

public class Decoy : CustomObject
{
    public static List<Decoy> Decoys = new();
    public PlayerControl Player;
    public MonoBehaviour behaviour;
    public float elapsedTime;

    public static bool ResetPlaceAfterMeeting;
    public static float DecoyDelayedDisplay;
    public static bool DecoyPermanent;
    public static float DecoyDuration;

    public static Sprite decoySprite = new ResourceSprite("Decoy.png", 150f);

    public Decoy(PlayerControl player, Vector3 pos)
    {
        ResetPlaceAfterMeeting = CustomOptionHolder.marionetteResetPlaceAfterMeeting.GetBool();
        DecoyDuration = CustomOptionHolder.marionetteDecoyDuration.GetFloat();
        DecoyPermanent = CustomOptionHolder.marionetteDecoyPermanent.GetBool();
        DecoyDelayedDisplay = CustomOptionHolder.marionetteDecoyDelayedDisplay.GetFloat();

        Player = player;
        GameObject.name = "Decoy " + Id;
        behaviour = GameObject.AddComponent<CustomObjectBehaviour>();
        GameObject.SetActive(false);
        GameObject.transform.position = pos;
        Renderer.sprite = decoySprite;
        Renderer.color = Color.white * new Vector4(1, 1, 1, 0.66f);
        elapsedTime = 0f;
        Decoys.Add(this);

        _ = new LateTask(() =>
        {
            if (GameObject == null) return;
            IsActive = true;
            Renderer.color = Color.white;
        }, DecoyDelayedDisplay);

    }

    public override void Destroy()
    {
        if (Player.AmOwner && !Marionette.MonitoringCanMove)
        {
            if (HudManager.Instance.PlayerCam == behaviour)
            {
                Player.moveable = true;
            }
        }
        behaviour?.Destroy();
        Renderer?.Destroy();
        GameObject?.Destroy();
        GameObject = null;
        Decoys.Remove(this);
        base.Destroy();
    }

    public override void Update()
    {
        if (!InMeeting)
        {
            elapsedTime += Time.deltaTime;
            if (!DecoyPermanent && elapsedTime >= DecoyDuration)
            {
                RPCProcedure.DecoyDestroy(Player, Id);
                return;
            }
        }

        if (!DecoyPermanent && InMeeting)
        {
            RPCProcedure.DecoyDestroy(Player, Id);
            return;
        }
        else if (InMeeting)
        {
            GameObject.SetActive(false);
            return;
        }

        var canSee = PlayerControl.LocalPlayer == Marionette.Player || CanSeeGhostInfo
                  || (IsActive && ((Marionette.ShowDecoy == 2 && PlayerControl.LocalPlayer.IsImpostor())
                               || Marionette.ShowDecoy == 3));

        GameObject.SetActive(canSee);
    }
}
