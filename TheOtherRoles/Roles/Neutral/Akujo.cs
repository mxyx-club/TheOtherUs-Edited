namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Akujo : RoleBase, INeutral
{
    public static Color color = new Color32(142, 69, 147, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Akujo),
        (p) => new Akujo(p),
        RoleId.Akujo,
        RoleType.Neutral,
        "Akujo",
        color,
        203600,
        AddOptions
    );

    public Akujo(PlayerControl player) : base(player, roleinfo) { Players.Add(this); }

    public NeutralType NeutralType => NeutralType.Evil;

    public static List<Akujo> Players = new();
    public PlayerControl honmei;
    public List<PlayerControl> Keeps = new();
    public PlayerControl currentTarget;
    public int keepsLeft;
    public int timeLeft;
    public DateTime startTime;

    public static float timeLimit = 1300f;
    public static bool knowsRoles = true;
    public static bool honmeiCannotFollowWin;
    public static bool honmeiOptimizeWin;
    public static bool forceKeeps;
    public static int numKeeps;
    public static CustomOption akujoTimeLimit;
    public static CustomOption akujoForceKeeps;
    public static CustomOption akujoNumKeeps;
    public static CustomOption akujoKnowsRoles;
    public static CustomOption akujoHonmeiCannotFollowWin;
    public static CustomOption akujoHonmeiOptimizeWin;

    public CustomButton akujoHonmeiButton;
    public CustomButton akujoBackupButton;
    public static Sprite honmeiSprite = new ResourceSprite("AkujoHonmeiButton.png");
    public static Sprite keepSprite = new ResourceSprite("AkujoKeepButton.png");
    public static RemoteProcess<(PlayerControl player, PlayerControl target)> AkujoSetHonmei = new("AkujoSetHonmei", (data, _) =>
    {
        if (data.player.TryGetRole<Akujo>(out var akujo) && akujo.honmei == null)
        {
            akujo.honmei = data.target;
            Akujo.breakLovers(data.target);
        }
    });
    public static RemoteProcess<(PlayerControl player, PlayerControl target)> AkujoSetKeep = new("AkujoSetKeep", (data, _) =>
    {
        if (data.player.TryGetRole<Akujo>(out var akujo) && akujo.keepsLeft > 0)
        {
            akujo.Keeps.Add(data.target);
            Akujo.breakLovers(data.target);
            akujo.keepsLeft--;
        }
    });
    public static RemoteProcess<PlayerControl> AkujoSuicide = new("AkujoSuicide", (akujo, _) =>
    {
        if (akujo != null)
        {
            var partner = akujo.GetPartner();
            akujo.Exiled();
            PlayerData.SetDeathReason(akujo, CustomDeathReason.Loneliness);

            if (InMeeting && Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(akujo.KillSfx, false, 0.8f);
            if (PlayerControl.LocalPlayer == akujo)
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(akujo.Data, akujo.Data);

            if (MeetingHud.Instance)
            {
                ExtendMeetingTime(CustomOptionHolder.guessExtendmeetingTime.GetFloat());

                foreach (var pva in MeetingHud.Instance.playerStates)
                {
                    bool shouldClearVote = CustomOptionHolder.guessReVote.GetBool()
                        || pva.VotedFor == akujo.PlayerId || pva.VotedFor == partner!.PlayerId;

                    if (shouldClearVote)
                    {
                        pva.UnsetVote();
                        var voteAreaPlayer = PlayerById(pva.TargetPlayerId);
                        if (voteAreaPlayer?.AmOwner == false) continue;
                        MeetingHud.Instance.ClearVote();
                    }
                }
                if (AmongUsClient.Instance.AmHost) MeetingHud.Instance.CheckForEndVoting();
            }
        }

    });


    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        akujoTimeLimit = CustomOption.Create(configId++, CustomOptionType.Neutral, "akujoTimeLimit", 450f, 120f, 1200f, 30f, roleinfo.RoleOption);
        akujoForceKeeps = CustomOption.Create(configId++, CustomOptionType.Neutral, "akujoForceKeeps", false, roleinfo.RoleOption);
        akujoNumKeeps = CustomOption.Create(configId++, CustomOptionType.Neutral, "akujoNumKeeps", 1f, 0f, 5f, 1f, roleinfo.RoleOption);
        akujoKnowsRoles = CustomOption.Create(configId++, CustomOptionType.Neutral, "akujoKnowsRoles", true, roleinfo.RoleOption);
        akujoHonmeiCannotFollowWin = CustomOption.Create(configId++, CustomOptionType.Neutral, "akujoHonmeiCannotFollowWin", true, roleinfo.RoleOption);
        akujoHonmeiOptimizeWin = CustomOption.Create(configId++, CustomOptionType.Neutral, "akujoHonmeiOptimizeWin", true, roleinfo.RoleOption);
    }

    public bool IsKillerLover()
    {
        return honmei.IsAlive() && honmei.IsKiller();
    }

    public bool isAkujoTeam(PlayerControl player)
    {
        return player != null && (player == Player || player == honmei);
    }

    public PlayerControl OtherLover(PlayerControl player)
    {
        if (Player == null || honmei == null) return null;
        if (player == Player) return honmei;
        if (player == honmei) return Player;
        return null;
    }

    public static void breakLovers(PlayerControl player)
    {
        if (player.TryGetModifier<Lovers>(out var lovers))
        {
            var otherLover = lovers.OtherLover(player);
            if (otherLover != null)
            {
                otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                PlayerData.SetDeathReason(otherLover, CustomDeathReason.LoveStolen);
                lovers.Destroy();
            }
        }
    }

    public override bool SeeRoleName(PlayerControl seen, PlayerControl seer)
    {
        if (Player == seen && (Keeps.Contains(seer) || seer == honmei)) return true;
        return false;
    }

    public override bool SeeRoleTag(PlayerControl seen, PlayerControl seer, out string tag)
    {
        if ((isAkujoTeam(seen) && seer == OtherLover(seen)) || (Keeps.Contains(seen) && seer == Player))
        {
            tag = Cs(color, " ♥");
            return true;
        }
        if (Player == seen && Keeps.Contains(seer))
        {
            tag = Cs(Color.gray, " ♥");
            return true;
        }
        tag = string.Empty;
        return false;
    }

    public override void OnDestroy()
    {
        Players.Remove(this);
        base.OnDestroy();
    }

    public override void Initialize()
    {
        honmei = null;
        Keeps.Clear();
        currentTarget = null;
        startTime = DateTime.UtcNow;
        timeLimit = akujoTimeLimit.GetFloat();
        forceKeeps = akujoForceKeeps.GetBool();
        knowsRoles = akujoKnowsRoles.GetBool();
        honmeiCannotFollowWin = akujoHonmeiCannotFollowWin.GetBool();
        honmeiOptimizeWin = akujoHonmeiOptimizeWin.GetBool();
        timeLeft = (int)Math.Ceiling(timeLimit - (DateTime.UtcNow - startTime).TotalSeconds);
        numKeeps = Math.Min(akujoNumKeeps.GetInt(), PlayerControl.AllPlayerControls.Count - 2);
        keepsLeft = numKeeps;
    }

    public override string UpdateMeetingVoteText(MeetingHud __instance)
    {
        var meetingInfoText = string.Format(GetString("akujoTimeRemaining"), $"{TimeSpan.FromSeconds(timeLeft):mm\\:ss}");
        return meetingInfoText;
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        timeLeft = (int)Math.Ceiling(timeLimit - (DateTime.UtcNow - startTime).TotalSeconds);
        if (timeLeft > 0)
        {
            if (honmei == null)
            {
                if (akujoHonmeiButton.ButtonTitle != null)
                {
                    akujoHonmeiButton.ButtonTitle.text = TimeSpan.FromSeconds(timeLeft).ToString(@"mm\:ss");
                }
                akujoHonmeiButton.ButtonTitle.enabled = !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                                                        !MeetingHud.Instance &&
                                                        !ExileController.Instance;
            }
            else akujoHonmeiButton.ButtonTitle.enabled = false;
        }
        else if (timeLeft <= 0)
        {
            if (honmei == null || (Keeps?.Count < 1 && forceKeeps))
            {
                AkujoSuicide.Invoke(Player);
            }
        }

        var untargetables = new List<PlayerControl>();
        if (honmei != null) untargetables.Add(honmei);
        if (Keeps != null) untargetables.AddRange(Keeps);
        currentTarget = SetTarget(untargetables);
        SetPlayerOutline(currentTarget, color);
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        if ((Player != null && Info.Target == Player) || (honmei != null && Info.Target == honmei))
        {
            PlayerControl akujoPartner = Info.Target == Player ? honmei : Player;
            if (akujoPartner != null && !akujoPartner.Data.IsDead)
            {
                akujoPartner.MurderPlayer(akujoPartner, MurderResultFlags.Succeeded);
                PlayerData.SetDeathReason(akujoPartner, CustomDeathReason.LoverSuicide);
            }
        }
    }
    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        if ((Player != null && Player.PlayerId == exiled.PlayerId) || (honmei != null && honmei.PlayerId == exiled.PlayerId))
        {
            PlayerControl akujoPartner = exiled.PlayerId == Player.PlayerId ? honmei : Player;
            if (akujoPartner != null && !akujoPartner.Data.IsDead)
            {
                akujoPartner.Exiled();
                PlayerData.SetDeathReason(akujoPartner, CustomDeathReason.LoverSuicide);
            }

            if (MeetingHud.Instance && akujoPartner != null)
            {
                foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                {
                    if (pva.VotedFor != akujoPartner.PlayerId) continue;
                    pva.UnsetVote();
                    var voteAreaPlayer = PlayerById(pva.TargetPlayerId);
                    if (!voteAreaPlayer.AmOwner) continue;
                    MeetingHud.Instance.ClearVote();
                }

                if (AmongUsClient.Instance.AmHost)
                    MeetingHud.Instance.CheckForEndVoting();
            }
        }
    }
    public override void OnPlayerDisconnect(PlayerControl player)
    {
        if (player == honmei) honmei = null;
    }

    public override void CleanUp(HudManager __instance)
    {
        akujoHonmeiButton?.Destroy();
        akujoBackupButton?.Destroy();
        akujoHonmeiButton = null;
        akujoBackupButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Akujo Honmei
        akujoHonmeiButton?.Destroy();
        akujoHonmeiButton = new CustomButton(
            () =>
            {
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;

                AkujoSetHonmei.Invoke((PlayerControl.LocalPlayer, currentTarget));
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer
                       && Player.IsAlive()
                       && honmei == null
                       && timeLeft > 0;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer
                       && Player.IsAlive()
                       && currentTarget != null
                       && honmei == null
                       && timeLeft > 0;
            },
            () => { akujoHonmeiButton.Timer = akujoHonmeiButton.MaxTimer; },
            honmeiSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("AkujoHonmeiText")
        )
        {
            MaxTimer = 0f,
        };

        // Akujo Keep
        akujoBackupButton?.Destroy();
        akujoBackupButton = new CustomButton(
            () =>
            {
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;

                AkujoSetKeep.Invoke((PlayerControl.LocalPlayer, currentTarget));
            },
            () => { return Player == PlayerControl.LocalPlayer && Player.IsAlive() && keepsLeft > 0; },
            () =>
            {
                if (akujoBackupButton.ButtonTitle != null)
                {
                    if (keepsLeft > 0)
                        akujoBackupButton.ButtonTitle.text = keepsLeft.ToString();
                    else
                        akujoBackupButton.ButtonTitle.text = "";
                }
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() && currentTarget != null && keepsLeft > 0 && timeLeft > 0;
            },
            () => { akujoBackupButton.Timer = akujoBackupButton.MaxTimer; },
            keepSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.secondaryAbilityInput.keyCode,
            buttonText: GetString("AkujoBackupText")
        )
        {
            MaxTimer = 0f,
        };
        // akujoBackupLeftText -> akujoBackupButton
        // akujoTimeRemainingText -> akujoHonmeiButton
    }
}
