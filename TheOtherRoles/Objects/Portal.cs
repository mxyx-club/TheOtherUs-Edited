using TheOtherRoles.Attributes;

namespace TheOtherRoles.Objects;

public class Portal
{
    public static Portal firstPortal;
    public static Portal secondPortal;
    public static bool bothPlacedAndEnabled;
    public static ResourceSpriteArray portalFgAnimationSprites;
    public static Sprite portalSprite;
    public static bool isTeleporting;
    public static float teleportDuration = 3.4166666667f;

    public static List<tpLogEntry> teleportedPlayers;
    private readonly SpriteRenderer animationFgRenderer;
    private readonly SpriteRenderer portalRenderer;

    public GameObject portalFgAnimationGameObject;
    public GameObject portalGameObject;
    public string room;

    public Portal(Vector2 p)
    {
        portalGameObject = new GameObject("Portal") { layer = 11 };
        //Vector3 position = new Vector3(p.x, p.y, PlayerControl.LocalPlayer.transform.position.z + 1f);
        var position = new Vector3(p.x, p.y, (p.y / 1000f) + 0.01f);

        // Create the portal            
        portalGameObject.transform.position = position;
        portalGameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        portalRenderer = portalGameObject.AddComponent<SpriteRenderer>();
        portalRenderer.sprite = portalSprite;

        var fgPosition = new Vector3(0, 0, -1f);
        portalFgAnimationGameObject = new GameObject("PortalAnimationFG");
        portalFgAnimationGameObject.transform.SetParent(portalGameObject.transform);
        portalFgAnimationGameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        portalFgAnimationGameObject.transform.localPosition = fgPosition;
        animationFgRenderer = portalFgAnimationGameObject.AddComponent<SpriteRenderer>();
        animationFgRenderer.material = FastDestroyableSingleton<HatManager>.Instance.PlayerMaterial;

        // Only render the inactive portals for the Portalmaker
        var playerIsPortalmaker = PlayerControl.LocalPlayer == Portalmaker.portalmaker;
        portalGameObject.SetActive(playerIsPortalmaker);
        portalFgAnimationGameObject.SetActive(true);

        if (firstPortal == null) firstPortal = this;
        else if (secondPortal == null) secondPortal = this;
        var lastRoom = FastDestroyableSingleton<HudManager>.Instance?.roomTracker?.LastRoom?.RoomId ?? null;
        room = lastRoom != null ? FastDestroyableSingleton<TranslationController>.Instance.GetString((SystemTypes)lastRoom) : "室外";
    }

    public static Sprite getFgAnimationSprite(int index)
    {
        if (portalFgAnimationSprites == null) return null;
        index = Mathf.Clamp(index, 0, portalFgAnimationSprites.Sprites.Length - 1);
        return portalFgAnimationSprites.GetSprite(index);
    }

    public static void startTeleport(byte playerId, byte exit)
    {
        if (firstPortal == null || secondPortal == null) return;

        var entryPortal = firstPortal;
        var exitPortal = secondPortal;

        // Generate log info
        var playerControl = PlayerById(playerId);
        if (playerControl == null) return;
        var instantTeleport = isInstantTeleport(playerControl);
        var flip = playerControl.cosmetics.currentBodySprite.BodySprite
            .flipX; // use the original player control here, not the morph target.
        if (Glitch.Player != null && Glitch.morphTimer > 0)
            playerControl = Glitch.morphTarget; // Will output info of morph-target instead
        var playerNameDisplay = Portalmaker.logOnlyHasColors
            ? "一名玩家 (" + (IsLightColor(playerControl) ? "浅" : "深") + ")"
            : playerControl.Data.PlayerName;

        if (Camouflager.camouflageTimer > 0 || MushroomSabotageActive)
        {
            playerNameDisplay = "A camouflaged player";
        }

        if (!playerControl.Data.IsDead)
            teleportedPlayers.Add(new tpLogEntry(playerId, playerNameDisplay, DateTime.UtcNow));

        // Portalmaker teleports have no animation and must not occupy the global portal lock.
        if (instantTeleport) return;

        isTeleporting = true;
        entryPortal.animationFgRenderer.flipX = flip;
        exitPortal.animationFgRenderer.flipX = flip;

        var cancelled = false;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(teleportDuration,
            new Action<float>(p =>
            {
                if (cancelled) return;
                var portalsReplaced = firstPortal != entryPortal || secondPortal != exitPortal;
                if (portalsReplaced || entryPortal.animationFgRenderer == null || exitPortal.animationFgRenderer == null)
                {
                    cancelled = true;
                    if (!portalsReplaced) isTeleporting = false;
                    return;
                }
                if (exit is 0 or 1)
                    entryPortal.animationFgRenderer.sprite = getFgAnimationSprite((int)(p * portalFgAnimationSprites.Sprites.Length));
                if (exit is 0 or 2)
                    exitPortal.animationFgRenderer.sprite = getFgAnimationSprite((int)(p * portalFgAnimationSprites.Sprites.Length));
                playerControl.SetPlayerMaterialColors(entryPortal.animationFgRenderer);
                playerControl.SetPlayerMaterialColors(exitPortal.animationFgRenderer);
                if ((int)p != 1) return;
                entryPortal.animationFgRenderer.sprite = null;
                exitPortal.animationFgRenderer.sprite = null;
                isTeleporting = false;
                cancelled = true;
            })));
    }

    public static bool isInstantTeleport(PlayerControl player)
    {
        return player != null && player == Portalmaker.portalmaker;
    }

    public static void teleportLocalPlayer(Vector3 exit, Vector3? entry = null)
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (isInstantTeleport(localPlayer))
        {
            if (SubmergedCompatibility.IsSubmerged) SubmergedCompatibility.ChangeFloor(exit.y > -7);
            localPlayer.NetTransform.RpcSnapTo(exit);
            return;
        }

        if (entry.HasValue) localPlayer.NetTransform.RpcSnapTo(entry.Value);

        var entryPortal = firstPortal;
        var exitPortal = secondPortal;
        var didTeleport = false;
        var cancelled = false;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(teleportDuration,
            new Action<float>(p =>
            {
                if (cancelled) return;
                if (localPlayer == null || PlayerControl.LocalPlayer != localPlayer || MeetingHud.Instance != null ||
                    firstPortal != entryPortal || secondPortal != exitPortal ||
                    entryPortal?.portalGameObject == null || exitPortal?.portalGameObject == null)
                {
                    cancelled = true;
                    if (localPlayer != null) localPlayer.moveable = true;
                    return;
                }

                // Preserve the original animated and movement-locking path for every non-Portalmaker user.
                localPlayer.moveable = false;
                localPlayer.NetTransform.Halt();
                if (p >= 0.5f && p <= 0.53f && !didTeleport && !MeetingHud.Instance)
                {
                    if (SubmergedCompatibility.IsSubmerged) SubmergedCompatibility.ChangeFloor(exit.y > -7);
                    localPlayer.NetTransform.RpcSnapTo(exit);
                    didTeleport = true;
                }

                if (p == 1f)
                {
                    localPlayer.moveable = true;
                    cancelled = true;
                }
            })));
    }

    public static bool locationNearEntry(Vector2 p)
    {
        if (!bothPlacedAndEnabled) return false;
        const float maxDist = 0.25f;

        var dist1 = Vector2.Distance(p, firstPortal.portalGameObject.transform.position);
        var dist2 = Vector2.Distance(p, secondPortal.portalGameObject.transform.position);
        return !(dist1 > maxDist) || !(dist2 > maxDist);
    }

    public static Vector2 findExit(Vector2 p)
    {
        var dist1 = Vector2.Distance(p, firstPortal.portalGameObject.transform.position);
        var dist2 = Vector2.Distance(p, secondPortal.portalGameObject.transform.position);
        return dist1 < dist2
            ? secondPortal.portalGameObject.transform.position
            : firstPortal.portalGameObject.transform.position;
    }

    public static Vector2 findEntry(Vector2 p)
    {
        var dist1 = Vector2.Distance(p, firstPortal.portalGameObject.transform.position);
        var dist2 = Vector2.Distance(p, secondPortal.portalGameObject.transform.position);
        return dist1 > dist2
            ? secondPortal.portalGameObject.transform.position
            : firstPortal.portalGameObject.transform.position;
    }

    public static void meetingEndsUpdate()
    {
        // checkAndEnable
        if (secondPortal != null)
        {
            firstPortal.portalGameObject.SetActive(true);
            secondPortal.portalGameObject.SetActive(true);
            bothPlacedAndEnabled = true;
            HudManagerStartPatch.portalmakerMoveToPortalButton?.ButtonTitle?.text = "2. " + secondPortal.room;
        }

        // reset teleported players
        teleportedPlayers = new List<tpLogEntry>();
    }

    public static void EnablePlacedPortals()
    {
        if (firstPortal == null || secondPortal == null)
            return;
        firstPortal.portalGameObject?.SetActive(true);
        secondPortal.portalGameObject?.SetActive(true);
        bothPlacedAndEnabled = true;
        if (HudManagerStartPatch.portalmakerMoveToPortalButton?.ButtonTitle != null)
            HudManagerStartPatch.portalmakerMoveToPortalButton.ButtonTitle.text = "2. " + secondPortal.room;
    }
    private static void preloadSprites()
    {
        var sprites = new (string, float)[205];
        for (int i = 0; i < sprites.Length; i++) sprites[i] = ($"PortalAnimation.portal_{i:000}.png", 115f);

        portalFgAnimationSprites = new ResourceSpriteArray(sprites, true);
        portalSprite = UnityHelper.loadSpriteFromResources("TheOtherRoles.Resources.PortalAnimation.plattform.png", 115f);
    }

    [OnGameStart, OnGameEnd]
    public static void clearPortals()
    {
        preloadSprites(); // Force preload of sprites to avoid lag
        bothPlacedAndEnabled = false;
        firstPortal = null;
        secondPortal = null;
        isTeleporting = false;
        teleportedPlayers = new List<tpLogEntry>();
    }

    public struct tpLogEntry(byte playerId, string name, DateTime time)
    {
        public byte playerId = playerId;
        public string name = name;
        public DateTime time = time;
    }
}
