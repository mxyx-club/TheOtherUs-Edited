namespace TheOtherRoles.Roles.Crewmate;

public class Medium : RoleBase
{
    public static Color color = new Color32(98, 120, 115, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Medium),
        (p) => new Medium(p),
        RoleId.Medium,
        RoleType.Crewmate,
        "Medium",
        color,
        303100,
        AddOptions
    );

    public Medium(PlayerControl p) : base(p, roleinfo) { }

    public DeadPlayer Target;
    public DeadPlayer soulTarget;
    public List<Tuple<DeadPlayer, Vector3>> deadBodies = new();
    public List<Tuple<DeadPlayer, Vector3>> futureDeadBodies = new();
    public List<SpriteRenderer> souls = new();
    public DateTime meetingStartTime = DateTime.UtcNow;

    public static float cooldown = 30f;
    public static float duration = 3f;
    public static bool oneTimeUse;
    public static float chanceAdditionalInfo;

    public CustomButton mediumButton;
    public static Sprite soulSprite = new ResourceSprite("Soul.png", 500f);
    public static Sprite question = new ResourceSprite("MediumButton.png");

    public static CustomOption mediumCooldown;
    public static CustomOption mediumDuration;
    public static CustomOption mediumOneTimeUse;
    public static CustomOption mediumChanceAdditionalInfo;

    public class DeadPlayer
    {
        public CustomDeathReason DeathReason { get; set; }
        public PlayerControl KilledBy { get; set; }
        public PlayerControl Player { get; }
        public DateTime TimeOfDeath { get; }
        public bool wasCleaned { get; set; }
        public Vector3 DeadPos { get; set; }

        public DeadPlayer(PlayerControl Player, DateTime TimeOfDeath, CustomDeathReason DeathReason, PlayerControl KilledBy, Vector3 deadPos)
        {
            this.Player = Player;
            this.TimeOfDeath = TimeOfDeath;
            this.DeathReason = DeathReason;
            this.KilledBy = KilledBy;
            wasCleaned = false;
            DeadPos = deadPos;
        }
    }

    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        mediumCooldown = CustomOption.Create(configId++, CustomOptionType.Crewmate, "mediumCooldown", 7.5f, 2.5f, 120f, 2.5f, roleinfo.RoleOption);
        mediumDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "mediumDuration", 0.5f, 0f, 15f, 0.5f, roleinfo.RoleOption);
        mediumOneTimeUse = CustomOption.Create(configId++, CustomOptionType.Crewmate, "mediumOneTimeUse", false, roleinfo.RoleOption);
        mediumChanceAdditionalInfo = CustomOption.Create(configId++, CustomOptionType.Crewmate, "mediumChanceAdditionalInfo", CustomOptionHolder.rates, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        Target = null;
        soulTarget = null;
        deadBodies.Clear();
        futureDeadBodies.Clear();
        souls.Clear();
        meetingStartTime = DateTime.UtcNow;
        cooldown = mediumCooldown.GetFloat();
        duration = mediumDuration.GetFloat();
        oneTimeUse = mediumOneTimeUse.GetBool();
        chanceAdditionalInfo = mediumChanceAdditionalInfo.GetSelection() / 10f;
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        if (souls != null)
        {
            foreach (var sr in souls) UObject.Destroy(sr.gameObject);
            souls = new List<SpriteRenderer>();
        }

        if (futureDeadBodies != null)
        {
            foreach (var (_, ps) in futureDeadBodies)
            {
                var s = new GameObject();
                //s.transform.position = ps;
                s.transform.position = new Vector3(ps.x, ps.y, (ps.y / 1000) - 1f);
                s.layer = 5;
                var rend = s.AddComponent<SpriteRenderer>();
                s.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                rend.sprite = soulSprite;
                souls.Add(rend);
            }

            deadBodies = futureDeadBodies;
            futureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
        }
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        var deadPlayer = new DeadPlayer(Info.Target, DateTime.UtcNow, Info.DeathReason, Info.Killer, Info.Target.transform.position);
        // Medium add body
        if (deadBodies != null)
        {
            futureDeadBodies.Add(new Tuple<DeadPlayer, Vector3>(deadPlayer, Info.Target.transform.position));
        }

    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        // Medium meeting start time
        meetingStartTime = DateTime.UtcNow;
    }

    public string getInfo(PlayerControl target, PlayerControl killer)
    {
        var msg = "";

        var infos = new List<SpecialMediumInfo>();
        // collect fitting death info types.
        // suicides:
        if (killer == target)
        {
            if (target.Is(RoleId.Sheriff)) infos.Add(SpecialMediumInfo.SheriffSuicide);
            if (target.IsLover()) infos.Add(SpecialMediumInfo.PassiveLoverSuicide);
            if (target.Is(RoleId.Thief)) infos.Add(SpecialMediumInfo.ThiefSuicide);
            if (target.Is(RoleId.Warlock)) infos.Add(SpecialMediumInfo.WarlockSuicide);
        }
        else
        {
            if (target.IsLover()) infos.Add(SpecialMediumInfo.ActiveLoverDies);
            if (target.IsImpostor() && killer.IsImpostor())
                infos.Add(SpecialMediumInfo.ImpostorTeamkill);
        }

        if (target.Is(RoleId.Sidekick) && killer.Is(RoleId.Jackal)) infos.Add(SpecialMediumInfo.JackalKillsSidekick);
        if (target.TryGetRole<Lawyer>(out var lawyer) && killer == lawyer.Target) infos.Add(SpecialMediumInfo.LawyerKilledByClient);
        if (Target.wasCleaned) infos.Add(SpecialMediumInfo.BodyCleaned);
        if (target?.Data?.Disconnected == true) infos.Add(SpecialMediumInfo.Disconnected);

        if (infos.Count > 0)
        {
            var selectedInfo = infos[rnd.Next(infos.Count)];
            switch (selectedInfo)
            {
                case SpecialMediumInfo.SheriffSuicide:
                    msg = "哎呀，枪走火了！[警长自杀].";
                    break;
                case SpecialMediumInfo.WarlockSuicide:
                    msg = "啊哦，我好像把自己咒死了耶。[术士死于自杀].";
                    break;
                case SpecialMediumInfo.ThiefSuicide:
                    msg = "我试图从他们口袋里偷枪，却把自己害死了。[窃贼自杀].";
                    break;
                case SpecialMediumInfo.ActiveLoverDies:
                    msg = "无论如何，我都想摆脱这种有毒的关系。[带着恋人死去].";
                    break;
                case SpecialMediumInfo.PassiveLoverSuicide:
                    msg = "在天愿作比翼鸟,在地愿为连理枝，所爱以逝，吾亦寻之。[被恋人带死].";
                    break;
                case SpecialMediumInfo.LawyerKilledByClient:
                    msg = "我的客户杀了我。我还能得到报酬吗？[律师被客户杀害]";
                    break;
                case SpecialMediumInfo.JackalKillsSidekick:
                    msg = "既已纳我为伍，何必取我性命，算了，至少不用做任务了。[跟班被豺狼杀害]";
                    break;
                case SpecialMediumInfo.ImpostorTeamkill:
                    msg = "他们肯定是把我当成卧底才杀了我，有没有？[内鬼死于队友]";
                    break;
                case SpecialMediumInfo.BodyCleaned:
                    msg = "我的尸体现在是某种艺术还是。。。啊，它不见了。[尸体被清理或吃了]";
                    break;
                case SpecialMediumInfo.Disconnected:
                    msg = "目标玩家已掉线！";
                    break;
            }
        }
        else
        {
            var randomNumber = rnd.Next(4);
            var typeOfColor = IsLightColor(Target.KilledBy) ? "浅" : "深";
            var timeSinceDeath = (float)(meetingStartTime - Target.TimeOfDeath).TotalMilliseconds;
            var roleString = RoleInfo.GetRolesString(Target.Player, false, false, false, false);
            if (randomNumber == 0)
            {
                msg = "我的职业是 " + roleString + " .";
            }
            else if (randomNumber == 1)
            {
                msg = "我不确定，但我想应该是 " + typeOfColor + " 色的凶手杀了我.";
            }
            else if (randomNumber == 2)
            {
                msg = "如果我数对了，我就在会议前 " + Math.Round(timeSinceDeath / 1000) + " 秒死了.";
            }
            else
            {
                msg = "我好像是被 " + target.GetRoleInfo().Name + " 无情的杀害了.";
            }
        }

        if (rnd.NextDouble() < chanceAdditionalInfo)
        {
            var count = 0;
            var condition = "";
            var alivePlayersList = PlayerControl.AllPlayerControls.ToArray().Where(pc => pc.IsAlive());
            var role = target.GetRole();
            switch (rnd.Next(3))
            {
                case 0:
                    count = alivePlayersList.Count(pc =>
                        pc.IsImpostor() || pc.IsKillerNeutral() || (pc.GetRole() is RoleId.Sheriff or RoleId.Veteran or RoleId.Thief));
                    condition = "个杀手" + (count == 1 ? "" : "");
                    break;
                case 1:
                    count = alivePlayersList.Count(RoleHelpers.CanUseVents);
                    condition = "个可以使用管道的玩家" + (count == 1 ? "" : "");
                    break;
                case 2:
                    count = alivePlayersList.Count(pc => pc.IsNeutral() && !pc.IsKiller());
                    condition = $"名玩家是非击杀型中立";
                    break;
            }

            msg += $"\n你问我的时候,有{count} " + condition + (count == 1 ? "" : "") + " 还活着";
        }

        return Target.Player.Data.PlayerName + " 的灵魂说:\n" + msg;
    }

    private enum SpecialMediumInfo
    {
        SheriffSuicide,
        ThiefSuicide,
        ActiveLoverDies,
        PassiveLoverSuicide,
        LawyerKilledByClient,
        JackalKillsSidekick,
        ImpostorTeamkill,
        SubmergedO2,
        WarlockSuicide,
        BodyCleaned,
        Disconnected
    }

    public override void CleanUp(HudManager __instance)
    {
        mediumButton?.Destroy();
        mediumButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Medium button
        mediumButton?.Destroy();
        mediumButton = new CustomButton(
            () =>
            {
                if (Target != null)
                {
                    soulTarget = Target;
                    mediumButton.HasEffect = true;
                    SoundEffectsManager.play("mediumAsk");
                }
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {

                DeadPlayer target = null;
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                var closestDistance = float.MaxValue;
                var usableDistance = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault().UsableDistance;
                foreach (var (dp, ps) in deadBodies)
                {
                    var distance = Vector2.Distance(ps, truePosition);
                    if (distance <= usableDistance && distance < closestDistance)
                    {
                        closestDistance = distance;
                        target = dp;
                    }
                }
                Target = target;

                if (mediumButton.isEffectActive && Target != soulTarget)
                {
                    soulTarget = null;
                    mediumButton.Timer = 0f;
                    mediumButton.isEffectActive = false;
                }

                return Target != null && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                mediumButton.Timer = mediumButton.MaxTimer;
                mediumButton.isEffectActive = false;
                soulTarget = null;
            },
            question,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            true,
            duration,
            () =>
            {
                mediumButton.Timer = mediumButton.MaxTimer;
                if (Target == null || Target.Player == null) return;
                var msg = getInfo(Target.Player, Target.KilledBy);
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, msg);

                // Ghost Info
                var writer = StartRPC(CustomRPC.ShareGhostInfo);
                writer.Write(Target.Player.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.GhostChat);
                writer.Write(msg);
                writer.EndRPC();

                // Remove soul
                if (oneTimeUse)
                {
                    var closestDistance = float.MaxValue;
                    SpriteRenderer target = null;

                    foreach (var (db, ps) in deadBodies)
                        if (db == Target)
                        {
                            var deadBody = Tuple.Create(db, ps);
                            deadBodies.Remove(deadBody);
                            break;
                        }

                    foreach (var rend in souls)
                    {
                        var distance = Vector2.Distance(rend.transform.position,
                            PlayerControl.LocalPlayer.GetTruePosition());
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            target = rend;
                        }
                    }

                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(5f, new Action<float>(p =>
                    {
                        if (target != null)
                        {
                            var tmp = target.color;
                            tmp.a = Mathf.Clamp01(1 - p);
                            target.color = tmp;
                        }

                        if (p == 1f && target != null && target.gameObject != null) UObject.Destroy(target.gameObject);
                    })));

                    souls.Remove(target);
                }

                SoundEffectsManager.stop("mediumAsk");
            },
            buttonText: GetString("MediumText")
        )
        {
            MaxTimer = cooldown,
            EffectDuration = duration
        };
    }
}
