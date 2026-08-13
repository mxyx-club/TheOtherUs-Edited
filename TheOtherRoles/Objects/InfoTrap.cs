namespace TheOtherRoles.Objects;

/// <summary>
/// Non-hostile information trap used by TrapperPlus. It never freezes, outlines or
/// notifies its targets. Only the owner and normal ghost-info observers can see it.
/// </summary>
public sealed class InfoTrap : CustomObjectBase<InfoTrap>
{
    private static Sprite trapSprite = new ResourceSprite("Trapper_Trap_Ingame.png", 300f);
    private const float MarkerDiameter = 0.35f;
    private const float MarkerAlpha = 0.55f;
    private const float RingAlpha = 0.35f;
    private const float RingWidth = 0.035f;
    private const int RingSegments = 96;

    private readonly Dictionary<byte, float> dwellSeconds = new();
    private readonly HashSet<byte> completedTargets = new();
    private readonly LineRenderer rangeRenderer;
    private Material rangeMaterial;

    public PlayerControl Owner { get; }
    public byte OwnerId { get; }
    public int NetworkId { get; }
    public float Radius { get; }

    public InfoTrap(PlayerControl owner, Vector3 position, int networkId)
    {
        Owner = owner;
        OwnerId = owner.PlayerId;
        NetworkId = networkId;
        Radius = Math.Max(0.01f, TrapperPlus.trapSize * (ShipStatus.Instance?.MaxLightRadius ?? 1f));

        GameObject.layer = 11;
        GameObject.name = $"InfoTrap {NetworkId}";
        GameObject.transform.position = new Vector3(position.x, position.y, position.y + 0.001f);

        Renderer.sprite = trapSprite;
        Renderer.color = Color.white.SetAlpha(MarkerAlpha);
        ResizeMarker();
        rangeRenderer = CreateRangeRenderer();
        RefreshVisibility();
    }

    public static InfoTrap Find(int networkId)
    {
        return AllObjects.FirstOrDefault(trap => trap.NetworkId == networkId);
    }

    public static void ClearAll(PlayerControl owner = null)
    {
        var traps = owner == null
            ? AllObjects.ToArray()
            : AllObjects.Where(trap => trap.OwnerId == owner.PlayerId).ToArray();

        foreach (var trap in traps)
            trap?.Destroy();
    }

    public void MarkTargetCompleted(byte targetId)
    {
        completedTargets.Add(targetId);
        dwellSeconds.Remove(targetId);
    }

    public void ResetForNextRound()
    {
        dwellSeconds.Clear();
        completedTargets.Clear();
    }

    public override void OnMeetingStart()
    {
        ResetForNextRound();
    }

    public override void OnDestroy()
    {
        if (rangeMaterial != null)
        {
            UObject.Destroy(rangeMaterial);
            rangeMaterial = null;
        }

        base.OnDestroy();
    }

    public override void Update()
    {
        RefreshVisibility();

        if (!AmongUsClient.Instance.AmHost || !InGame || InMeeting || ExileController.Instance || !TrapperPlus.IsActiveOwner(OwnerId))
        {
            dwellSeconds.Clear();
            return;
        }

        var playersInRange = new HashSet<byte>();
        foreach (var player in PlayerControl.AllPlayerControls.ToArray())
        {
            if (player == null || player.Data == null || player.Data.Disconnected || player.Data.IsDead || player.PlayerId == OwnerId)
                continue;

            if (completedTargets.Contains(player.PlayerId))
                continue;

            var distance = Vector2.Distance(GameObject.transform.position, player.GetTruePosition());
            if (distance > Radius + 0.01f)
                continue;

            playersInRange.Add(player.PlayerId);
            dwellSeconds.TryGetValue(player.PlayerId, out var elapsed);
            elapsed += Time.deltaTime;
            dwellSeconds[player.PlayerId] = elapsed;

            if (elapsed < TrapperPlus.minDwellTime)
                continue;

            // Mark first so a missing RPC subscriber cannot emit repeatedly every frame.
            completedTargets.Add(player.PlayerId);
            dwellSeconds.Remove(player.PlayerId);
            TrapperPlus.ConfirmHostDwell(this, player);
        }

        foreach (var playerId in dwellSeconds.Keys.Where(id => !playersInRange.Contains(id)).ToArray())
            dwellSeconds.Remove(playerId);
    }

    private void RefreshVisibility()
    {
        if (GameObject == null)
        {
            IsActive = false;
            return;
        }

        var localPlayer = PlayerControl.LocalPlayer;
        var canSee = localPlayer != null && (localPlayer.PlayerId == OwnerId || CanSeeGhostInfo);

        // Keep the object and its Submerged ElevatorMover active on every client.
        // Hiding the whole GameObject would stop Unity component updates for a
        // living non-owner host, leaving the host's authoritative trap position
        // on the wrong floor after an elevator moves.
        if (!GameObject.activeSelf)
            GameObject.SetActive(true);
        if (Renderer != null)
            Renderer.enabled = canSee;
        if (rangeRenderer != null)
            rangeRenderer.enabled = canSee;
        IsActive = canSee;
    }

    private void ResizeMarker()
    {
        var sprite = Renderer.sprite;
        if (sprite == null)
            return;

        var bounds = sprite.bounds.size;
        var x = bounds.x > 0f ? MarkerDiameter / bounds.x : 1f;
        var y = bounds.y > 0f ? MarkerDiameter / bounds.y : x;
        GameObject.transform.localScale = new Vector3(x, y, 1f);
    }

    private LineRenderer CreateRangeRenderer()
    {
        var ringObject = new GameObject("InfoTrap Range") { layer = 11 };
        ringObject.transform.SetParent(GameObject.transform, false);
        ringObject.transform.localPosition = new Vector3(0f, 0f, -0.01f);
        // The marker root is scaled to a fixed visual size; cancel that scale so
        // ring coordinates remain exact world units and match the host radius.
        var rootScale = GameObject.transform.localScale;
        ringObject.transform.localScale = new Vector3(
            rootScale.x > 0f ? 1f / rootScale.x : 1f,
            rootScale.y > 0f ? 1f / rootScale.y : 1f,
            1f);

        var line = ringObject.AddComponent<LineRenderer>();
        var spriteShader = Shader.Find("Sprites/Default");
        if (spriteShader != null)
        {
            rangeMaterial = new Material(spriteShader);
            line.sharedMaterial = rangeMaterial;
        }
        line.useWorldSpace = false;
        line.loop = true;
        line.positionCount = RingSegments;
        line.startWidth = RingWidth;
        line.endWidth = RingWidth;
        line.numCornerVertices = 2;
        line.numCapVertices = 2;
        line.startColor = Color.yellow.SetAlpha(RingAlpha);
        line.endColor = Color.yellow.SetAlpha(RingAlpha);

        for (var index = 0; index < RingSegments; index++)
        {
            var angle = index * Mathf.PI * 2f / RingSegments;
            line.SetPosition(index, new Vector3(Mathf.Cos(angle) * Radius, Mathf.Sin(angle) * Radius, 0f));
        }

        return line;
    }
}
