namespace TheOtherRoles.Objects;

public class Decoy : CustomObjectBase<Decoy>
{
    public PlayerControl Player;
    public float elapsedTime;

    public static bool ResetPlaceAfterMeeting;
    public static float DecoyDelayedDisplay;
    public static bool DecoyPermanent;
    public static float DecoyDuration;
    public static bool MonitoringCanMove;
    public static int ShowDecoy; // 1: self, 2: impostor, 3: all

    public static Sprite decoySprite = new ResourceSprite("Decoy.png", 150f);

    public Decoy(PlayerControl player, Vector3 pos)
    {
        ResetPlaceAfterMeeting = Marionette.marionetteResetPlaceAfterMeeting.GetBool();
        DecoyDuration = Marionette.marionetteDecoyDuration.GetFloat();
        DecoyPermanent = Marionette.marionetteDecoyPermanent.GetBool();
        DecoyDelayedDisplay = Marionette.marionetteDecoyDelayedDisplay.GetFloat();
        MonitoringCanMove = Marionette.marionetteMonitoringCanMove.GetBool();
        ShowDecoy = Marionette.marionetteShowDecoy.GetQuantity();

        Player = player;
        GameObject.name = "Decoy " + Id;
        GameObject.SetActive(false);
        GameObject.transform.position = pos;
        Renderer.sprite = decoySprite;
        Renderer.color = Color.white * new Vector4(1, 1, 1, 0.66f);
        elapsedTime = 0f;

        _ = new LateTask(() =>
        {
            if (GameObject == null) return;
            IsActive = true;
            Renderer.color = Color.white;
        }, DecoyDelayedDisplay);

    }

    public override void OnDestroy()
    {
        if (Player.AmOwner && !MonitoringCanMove)
        {
            if (HudManager.Instance.PlayerCam == Behaviour)
            {
                Player.moveable = true;
            }
        }
        Behaviour?.Destroy();
        Renderer?.Destroy();
        GameObject?.Destroy();
        GameObject = null;
        base.OnDestroy();
    }

    public override void Update()
    {
        if (!InMeeting)
        {
            elapsedTime += Time.deltaTime;
            if (!DecoyPermanent && elapsedTime >= DecoyDuration)
            {
                Marionette.DecoyDestroy.LocalInvoke((Player, Id));
                return;
            }
        }

        if (!DecoyPermanent && InMeeting)
        {
            Marionette.DecoyDestroy.LocalInvoke((Player, Id));
            return;
        }
        else if (InMeeting)
        {
            GameObject.SetActive(false);
            return;
        }

        var canSee = PlayerControl.LocalPlayer == Player || CanSeeGhostInfo
                  || (IsActive && ((ShowDecoy == 2 && PlayerControl.LocalPlayer.IsImpostor()) || ShowDecoy == 3));

        GameObject.SetActive(canSee);
    }
}
