using TheOtherRoles.Attributes;

namespace TheOtherRoles.Objects;

public class Decoy
{
    public static List<Decoy> Decoys = new();
    public GameObject gameObject;
    public SpriteRenderer renderer;
    public PlayerControl player;
    public DateTime placedTime;
    public bool Active;
    public MonoBehaviour behaviour;
    public float elapsedTime;

    public int Id;
    private static int maxId;

    public static bool ResetPlaceAfterMeeting;
    public static float DecoyDelayedDisplay;
    public static bool DecoyPermanent;
    public static float DecoyDuration;

    public static Sprite decoySprite = new ResourceSprite("Decoy.png", 150f);

    public Decoy(PlayerControl player, Vector3 pos)
    {
        this.player = player;
        Id = ++maxId;
        gameObject = new GameObject("Trap");
        renderer = gameObject.AddComponent<SpriteRenderer>();
        behaviour = gameObject.AddComponent<CustomObjectBehaviour>();
        gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        gameObject.SetActive(false);
        gameObject.transform.position = pos;
        renderer.sprite = decoySprite;
        renderer.color = Color.white * new Vector4(1, 1, 1, 0.5f);
        placedTime = DateTime.Now;
        elapsedTime = 0f;
        Decoys.Add(this);

        _ = new LateTask(() =>
        {
            if (gameObject == null) return;
            Active = true;
            renderer.color = Color.white;
        }, DecoyDelayedDisplay);

    }

    public void Destroy()
    {
        behaviour?.Destroy();
        renderer?.Destroy();
        gameObject?.Destroy();
        gameObject = null;
        Decoys.Remove(this);
    }

    public void Update()
    {
        if (!InMeeting)
        {
            elapsedTime += Time.deltaTime;
            if (!DecoyPermanent && elapsedTime >= DecoyDuration)
            {
                RPCProcedure.DecoyDestroy(player, Id);
                return;
            }
        }

        if (!DecoyPermanent && InMeeting)
        {
            RPCProcedure.DecoyDestroy(player, Id);
            return;
        }
        else if (InMeeting)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!Active && (DateTime.Now - placedTime).TotalSeconds >= 5f)
        {
            Active = true;
        }

        var canSee = PlayerControl.LocalPlayer == Marionette.Player || CanSeeGhostInfo
                  || (Active && ((Marionette.ShowDecoy == 2 && PlayerControl.LocalPlayer.IsImpostor())
                               || Marionette.ShowDecoy == 3));

        gameObject.SetActive(canSee);
    }

    [OnGameStart, OnGameEnd]
    public static void ClearAndReload()
    {
        ResetPlaceAfterMeeting = CustomOptionHolder.marionetteResetPlaceAfterMeeting.GetBool();
        DecoyDuration = CustomOptionHolder.marionetteDecoyDuration.GetFloat();
        DecoyPermanent = CustomOptionHolder.marionetteDecoyPermanent.GetBool();
        DecoyDelayedDisplay = CustomOptionHolder.marionetteDecoyDelayedDisplay.GetFloat();

        maxId = 0;
        var list = Decoys;
        foreach (var x in list)
        {
            x?.Destroy();
        }
        Decoys = new();
    }

    public static void UpdateAll()
    {
        var list = Decoys;
        foreach (var x in list)
        {
            x?.Update();
        }
    }
}
