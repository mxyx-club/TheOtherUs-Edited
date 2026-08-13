namespace TheOtherRoles.Roles.Crewmate;

/// <summary>
/// The Aurial learns a player's current faction after repeatedly hitting them
/// with Radiation. The hit state is deliberately keyed by player id so it
/// survives meetings, deaths and revives for the duration of the game.
/// </summary>
public static class Aurial
{
    public sealed class SessionSnapshot
    {
        internal PlayerControl Holder;
        internal readonly Dictionary<byte, int> HitCounts = new();
        internal readonly HashSet<byte> RevealedPlayers = new();
    }

    public enum Faction
    {
        Crewmate,
        Neutral,
        Impostor,
    }

    public static PlayerControl aurial;

    // Compatibility with roles which use a capitalised holder (and with the
    // Imitator session, which temporarily replaces the role holder).
    public static PlayerControl Player
    {
        get => aurial;
        set => aurial = value;
    }

    public static Color color = new Color32(179, 77, 153, byte.MaxValue);

    public static float cooldown = 20f;
    public static float radius = 2.5f;
    public static int hitsToReveal = 3;
    public static float vision = 0.35f;
    public static bool affectedByVortox;

    public static readonly Dictionary<byte, int> hitCounts = new();
    public static readonly HashSet<byte> revealedPlayers = new();

    private static readonly HashSet<byte> anonymousPlayers = new();
    private static int anonymousAppearanceSignature = int.MinValue;

    public static Sprite buttonSprite = new ResourceSprite("AurialRadiateButton.png");

    /// <summary>
    /// Computes the canonical target list for a Radiation cast. There is no
    /// line-of-sight or vent check: every living non-caster in range is hit.
    /// This method is intended to be called by the host before broadcasting
    /// the AurialRadiate RPC.
    /// </summary>
    public static List<byte> GetRadiationTargets(byte casterId)
    {
        var caster = PlayerById(casterId);
        return GetRadiationTargets(caster);
    }

    public static List<byte> GetRadiationTargets(PlayerControl caster = null)
    {
        var targets = new List<byte>();
        caster ??= aurial;

        if (caster == null || aurial == null || caster.PlayerId != aurial.PlayerId ||
            caster.Data == null || !caster.IsAlive())
            return targets;

        var casterPosition = caster.GetTruePosition();
        var maximumDistanceSquared = Mathf.Max(0f, radius);
        maximumDistanceSquared *= maximumDistanceSquared;

        foreach (var target in PlayerControl.AllPlayerControls.ToArray())
        {
            if (target == null || target.Data == null || target.PlayerId == caster.PlayerId ||
                !target.IsAlive())
                continue;

            var offset = target.GetTruePosition() - casterPosition;
            if (offset.sqrMagnitude <= maximumDistanceSquared)
                targets.Add(target.PlayerId);
        }

        // A stable order makes the RPC payload deterministic and also protects
        // against clients enumerating the IL2CPP player collection differently.
        targets.Sort();
        return targets;
    }

    /// <summary>
    /// Applies a host-confirmed Radiation result on every client. Duplicate ids
    /// in one payload are ignored, so one cast can add at most one hit per player.
    /// Returns the players which became permanently revealed on this call.
    /// </summary>
    public static List<byte> ApplyRadiation(byte casterId, IEnumerable<byte> targetIds)
    {
        var newlyRevealed = new List<byte>();
        if (aurial == null || aurial.PlayerId != casterId || targetIds == null)
            return newlyRevealed;

        var uniqueTargets = new HashSet<byte>();
        foreach (var targetId in targetIds)
        {
            if (targetId == casterId || !uniqueTargets.Add(targetId))
                continue;

            // The host has already validated range and life state at cast time.
            // Only require a known player here, since the target may die before
            // the confirmation packet reaches another client.
            if (PlayerById(targetId) == null)
                continue;

            hitCounts.TryGetValue(targetId, out var previousHits);
            var currentHits = Math.Min(hitsToReveal, previousHits + 1);
            hitCounts[targetId] = currentHits;

            if (currentHits >= hitsToReveal && revealedPlayers.Add(targetId))
                newlyRevealed.Add(targetId);
        }

        return newlyRevealed;
    }

    // Explicit RPC-facing alias to keep the network handler independent of the
    // internal state names.
    public static List<byte> ReceiveRadiation(byte casterId, IEnumerable<byte> targetIds)
        => ApplyRadiation(casterId, targetIds);

    public static int GetHitCount(byte playerId)
        => hitCounts.TryGetValue(playerId, out var count) ? count : 0;

    public static bool HasRevealed(byte playerId) => revealedPlayers.Contains(playerId);

    /// <summary>
    /// Saves the holder and all game-long information before Imitator borrows
    /// this role. The borrowed session then starts with no prior knowledge.
    /// </summary>
    public static SessionSnapshot BeginImitation(PlayerControl imitator)
    {
        RestoreAnonymousWorldView();
        var snapshot = new SessionSnapshot { Holder = aurial };
        foreach (var pair in hitCounts)
            snapshot.HitCounts[pair.Key] = pair.Value;
        foreach (var playerId in revealedPlayers)
            snapshot.RevealedPlayers.Add(playerId);

        aurial = imitator;
        hitCounts.Clear();
        revealedPlayers.Clear();
        return snapshot;
    }

    /// <summary>
    /// Discards only the borrowed Aurial knowledge and restores the original
    /// role holder's exact state.
    /// </summary>
    public static void EndImitation(SessionSnapshot snapshot, bool restoreHolder = true)
    {
        RestoreAnonymousWorldView();
        aurial = restoreHolder ? snapshot?.Holder : null;
        hitCounts.Clear();
        revealedPlayers.Clear();

        // If the original holder was erased, disconnected or changed role while
        // the imitation was active, their private knowledge must be discarded as
        // well. Restoring it with no holder would let a later Aurial assignment
        // inherit information collected by an unrelated player.
        if (snapshot == null || !restoreHolder)
            return;

        foreach (var pair in snapshot.HitCounts)
            hitCounts[pair.Key] = pair.Value;
        foreach (var playerId in snapshot.RevealedPlayers)
            revealedPlayers.Add(playerId);
    }

    /// <summary>
    /// Re-evaluates the target's current faction. Results are not snapshotted,
    /// so conversions immediately change the displayed colour. The Spy remains
    /// crew-aligned because its RoleInfo has RoleType.Crewmate.
    /// </summary>
    public static Faction GetCurrentFaction(PlayerControl target)
    {
        if (target == null)
            return Faction.Crewmate;

        if (target == SchrodingersCat.Player)
        {
            return SchrodingersCat.State switch
            {
                SchrodingersCat.CatState.Crewmate => Faction.Crewmate,
                SchrodingersCat.CatState.Impostor => Faction.Impostor,
                _ => Faction.Neutral,
            };
        }

        // PlayerData is the canonical main-role snapshot and changes immediately
        // when a conversion RPC is applied. Prefer it over static holder scans,
        // which can briefly contain both the old and new role during transitions.
        var mainRole = PlayerData.GetPlayerData(target)?.MainRole ?? RoleId.DefaultRole;
        if (mainRole != RoleId.DefaultRole &&
            RoleInfo.RoleInfoById.TryGetValue(mainRole, out var currentRole))
        {
            if (currentRole.roleType == RoleType.Impostor) return Faction.Impostor;
            if (currentRole.roleType == RoleType.Neutral) return Faction.Neutral;
            if (currentRole.roleType == RoleType.Crewmate) return Faction.Crewmate;
        }

        var primaryRole = RoleInfo.getRoleInfoForPlayer(target, false, false)
            .FirstOrDefault(role => role.roleType is RoleType.Crewmate or RoleType.Neutral or RoleType.Impostor);

        if (primaryRole != null)
        {
            return primaryRole.roleType switch
            {
                RoleType.Impostor => Faction.Impostor,
                RoleType.Neutral => Faction.Neutral,
                _ => Faction.Crewmate,
            };
        }

        // Defensive fallback for the short interval before custom role holders
        // have been populated on a client.
        if (target.IsNeutral()) return Faction.Neutral;
        return target.Data?.Role?.IsImpostor == true ? Faction.Impostor : Faction.Crewmate;
    }

    public static Color GetFactionColor(PlayerControl target)
    {
        var faction = GetCurrentFaction(target);
        if (affectedByVortox && Vortox.Reversal)
        {
            if (faction == Faction.Crewmate) faction = Faction.Impostor;
            else if (faction == Faction.Impostor) faction = Faction.Crewmate;
        }

        return faction switch
        {
            Faction.Impostor => Palette.ImpostorRed,
            Faction.Neutral => Color.gray,
            _ => Color.green,
        };
    }

    /// <summary>
    /// The anonymous view is entirely local. It never changes the player's
    /// network outfit or GameData snapshot, and is disabled in meetings.
    /// </summary>
    public static bool HasAnonymousWorldView
    {
        get
        {
            var local = PlayerControl.LocalPlayer;
            return InGame && !InMeeting && ExileController.Instance == null &&
                   local != null && aurial != null && local.PlayerId == aurial.PlayerId && aurial.IsAlive();
        }
    }

    public static bool ShouldHideWorldIdentity(PlayerControl source, PlayerControl target)
    {
        return HasAnonymousWorldView && source != null && target != null &&
               source.PlayerId == PlayerControl.LocalPlayer.PlayerId && target.PlayerId != source.PlayerId;
    }

    /// <summary>
    /// Restores locally anonymised players before the normal HUD name, shield
    /// and tag passes run. This is intentionally separate from applying the
    /// view so meetings and role loss recover their current visual state in the
    /// same frame.
    /// </summary>
    public static void RestoreAnonymousWorldViewIfInactive()
    {
        if (HasAnonymousWorldView)
            return;

        RestoreAnonymousWorldView();
    }

    /// <summary>
    /// Applies outfit-free silhouettes after all ordinary world HUD rendering.
    /// Unrevealed players are black; revealed players are recoloured as a whole
    /// using their current faction colour.
    /// </summary>
    public static void ApplyAnonymousWorldView()
    {
        if (!HasAnonymousWorldView)
            return;

        var local = PlayerControl.LocalPlayer;
        var appearanceSignature = GetAppearanceSignature();
        var refreshFullLook = appearanceSignature != anonymousAppearanceSignature;
        anonymousAppearanceSignature = appearanceSignature;

        var visibleTargets = new HashSet<byte>();
        foreach (var target in PlayerControl.AllPlayerControls.ToArray())
        {
            if (target == null || target.Data == null || target.PlayerId == local.PlayerId ||
                target.Data.Disconnected || !target.IsAlive())
                continue;

            // Never turn an actually invisible player into an opaque silhouette.
            if (IsInvisible(target))
            {
                anonymousPlayers.Remove(target.PlayerId);
                continue;
            }

            visibleTargets.Add(target.PlayerId);
            var firstFrame = anonymousPlayers.Add(target.PlayerId);
            if (firstFrame || refreshFullLook)
                target.setLook(string.Empty, 6, string.Empty, string.Empty, string.Empty, string.Empty, false);

            if (target.cosmetics?.nameText != null)
                target.cosmetics.nameText.text = string.Empty;
            if (target.cosmetics?.colorBlindText != null)
                target.cosmetics.colorBlindText.gameObject.SetActive(false);

            ApplyAnonymousMaterial(target.cosmetics?.currentBodySprite?.BodySprite, target);
        }

        foreach (var playerId in anonymousPlayers.Where(id => !visibleTargets.Contains(id)).ToArray())
        {
            RestoreAnonymousPlayer(PlayerById(playerId));
            anonymousPlayers.Remove(playerId);
        }
    }

    /// <summary>
    /// Recolours an anonymous player renderer without changing network outfit
    /// data. This is also used by temporary transportation renderers so the
    /// revealed faction colour remains consistent while riding a zipline.
    /// </summary>
    public static void ApplyAnonymousMaterial(SpriteRenderer renderer, PlayerControl target)
    {
        if (renderer == null || target == null)
            return;

        var revealed = revealedPlayers.Contains(target.PlayerId);
        Color bodyColor = revealed ? GetFactionColor(target) : Palette.PlayerColors[6];
        Color backColor = revealed
            ? new Color(bodyColor.r * 0.55f, bodyColor.g * 0.55f, bodyColor.b * 0.55f, bodyColor.a)
            : Palette.ShadowColors[6];
        Color visorColor = revealed ? bodyColor : Palette.PlayerColors[6];

        renderer.material.SetColor("_BodyColor", bodyColor);
        renderer.material.SetColor("_BackColor", backColor);
        renderer.material.SetColor("_VisorColor", visorColor);
        renderer.material.SetFloat("_Outline", 0f);
        renderer.color = renderer.color.SetAlpha(Chameleon.visibility(target.PlayerId));
    }

    public static void RestoreAnonymousWorldView()
    {
        foreach (var playerId in anonymousPlayers.ToArray())
            RestoreAnonymousPlayer(PlayerById(playerId));

        anonymousPlayers.Clear();
        anonymousAppearanceSignature = int.MinValue;
    }

    private static bool IsInvisible(PlayerControl target)
    {
        return (target == Ninja.ninja && Ninja.isInvisable) ||
               (target == Phantom.Player && Phantom.isInvisable) ||
               (Jackal.jackal.Any(player => player == target) && Jackal.isInvisable);
    }

    private static int GetAppearanceSignature()
    {
        unchecked
        {
            var signature = MushroomSabotageActive ? 1 : 0;
            signature = signature * 31 + (Camouflager.camouflageTimer > 0f ? 1 : 0);
            signature = signature * 31 + (isCamoComms ? 1 : 0);
            signature = signature * 31 + (TheOtherRoles.Patches.SurveillanceMinigamePatch.nightVisionIsActive ? 1 : 0);
            signature = signature * 31 + (Glitch.morphTimer > 0f ? 1 : 0);
            signature = signature * 31 + (Glitch.morphTarget?.PlayerId ?? byte.MaxValue);
            return signature;
        }
    }

    private static void RestoreAnonymousPlayer(PlayerControl target)
    {
        if (target == null || target.Data == null || IsInvisible(target))
            return;

        if (MushroomSabotageActive)
        {
            target.setDefaultLook(false);
            return;
        }

        if (Camouflager.camouflageTimer > 0f || isCamoComms)
        {
            target.setLook(string.Empty, 6, string.Empty, string.Empty, string.Empty, string.Empty, false);
            return;
        }

        if (TheOtherRoles.Patches.SurveillanceMinigamePatch.nightVisionIsActive)
        {
            target.setLook(string.Empty, 11, string.Empty, string.Empty, string.Empty, string.Empty, false);
            return;
        }

        if (target == Glitch.Player && Glitch.morphTimer > 0f && Glitch.morphTarget?.Data != null)
        {
            var outfit = Glitch.morphTarget.Data.DefaultOutfit;
            target.setLook(Glitch.morphTarget.Data.PlayerName, outfit.ColorId, outfit.HatId,
                outfit.VisorId, outfit.SkinId, outfit.PetId, false);
            return;
        }

        target.setDefaultLook(false);
    }

    /// <summary>
    /// Backwards-compatible entry point retained for callers from older builds.
    /// Anonymous silhouettes now change their whole body colour instead of
    /// changing world-space names.
    /// </summary>
    public static void ApplyWorldNameColors()
    {
        ApplyAnonymousWorldView();
    }

    public static void ShowLocalPulse()
    {
        if (aurial?.AmOwner != true || HudManager.Instance == null) return;

        var pulse = new GameObject("Aurial Radiation Pulse") { layer = 11 };
        pulse.transform.position = aurial.transform.position + new Vector3(0f, 0f, 0.05f);
        var renderer = pulse.AddComponent<SpriteRenderer>();
        renderer.sprite = buttonSprite;
        renderer.color = color.SetAlpha(0.55f);

        var spriteRadius = Mathf.Max(renderer.sprite?.bounds.extents.magnitude ?? 1f, 0.01f);
        var targetScale = Mathf.Max(radius / spriteRadius, 0.1f);
        HudManager.Instance.StartCoroutine(Effects.Lerp(0.55f, new Action<float>(p =>
        {
            if (pulse == null) return;
            var scale = Mathf.Lerp(0.15f, targetScale, p);
            pulse.transform.localScale = Vector3.one * scale;
            renderer.color = color.SetAlpha(0.55f * (1f - p));
            if (p == 1f) pulse.Destroy();
        })));
    }

    public static void ShowLocalRadiationResult(IEnumerable<byte> targetIds, IEnumerable<byte> newlyRevealed)
    {
        if (aurial?.AmOwner != true) return;
        var unlocked = new HashSet<byte>(newlyRevealed ?? Array.Empty<byte>());

        foreach (var targetId in targetIds?.Distinct() ?? Enumerable.Empty<byte>())
        {
            var target = PlayerById(targetId);
            if (target == null) continue;
            var key = unlocked.Contains(targetId) ? "Aurial.RadiationUnlocked" : "Aurial.RadiationProgress";
            // Do not leak an anonymous target's identity through private HUD
            // feedback. The newly coloured silhouette is the spatial identifier.
            ShowNotification(string.Format(GetString(key), GetHitCount(targetId), hitsToReveal));
        }
    }

    public static void clearAndReload()
    {
        RestoreAnonymousWorldView();
        aurial = null;
        hitCounts.Clear();
        revealedPlayers.Clear();

        cooldown = CustomOptionHolder.aurialCooldown.GetFloat();
        radius = CustomOptionHolder.aurialRadius.GetFloat();
        hitsToReveal = Math.Max(1, CustomOptionHolder.aurialHitsToReveal.GetInt());
        vision = Mathf.Clamp(0.1f + CustomOptionHolder.aurialVision.GetSelection() * 0.05f, 0.1f, 1f);
        affectedByVortox = CustomOptionHolder.aurialAffectedByVortox.GetBool();
    }
}
