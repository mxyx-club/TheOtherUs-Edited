namespace TheOtherRoles.Roles.Crewmate;

public static class Alchemyst
{
    public static PlayerControl Player;
    public static DeadPlayer target;
    public static DeadPlayer soulTarget;
    public static Color color = new Color32(98, 120, 115, byte.MaxValue);
    public static List<Tuple<DeadPlayer, Vector3>> deadBodies = new();
    public static List<Tuple<DeadPlayer, Vector3>> futureDeadBodies = new();
    public static List<SpriteRenderer> souls = new();
    public static DateTime meetingStartTime = DateTime.UtcNow;

    public static float cooldown = 30f;
    public static float duration = 3f;
    public static bool oneTimeUse;
    public static float chanceAdditionalInfo;

    public static int skillUseCount;
    public static bool canUseKill;
    public static int requiredUses = 3;
    public static float KillCooldown = 25f;
    public static PlayerControl CurrentTarget;

    public static Sprite soulSprite = new ResourceSprite("Soul.png", 500f);

    public static Sprite question = new ResourceSprite("MediumButton.png");

    public static void clearAndReload()
    {
        Player = null;
        target = null;
        soulTarget = null;
        deadBodies.Clear();
        futureDeadBodies.Clear();
        souls.Clear();
        meetingStartTime = DateTime.UtcNow;
        cooldown = CustomOptionHolder.alchemystCooldown.GetFloat();
        duration = CustomOptionHolder.alchemystDuration.GetFloat();
        oneTimeUse = CustomOptionHolder.alchemystOneTimeUse.GetBool();
        chanceAdditionalInfo = CustomOptionHolder.alchemystChanceAdditionalInfo.GetSelection() / 10f;

        skillUseCount = 0;
        canUseKill = false;
        requiredUses = (int)CustomOptionHolder.alchemystRequiredUses.GetFloat();
        KillCooldown = CustomOptionHolder.alchemystKillCooldown.GetFloat();
    }


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


    public static string getInfo(PlayerControl target, PlayerControl killer)
    {
        var msg = "";

        var infos = new List<SpecialInfo>();
        // collect fitting death info types.
        // suicides:
        if (killer == target)
        {
            if (Sheriff.Player.Any(x => x == target)) infos.Add(SpecialInfo.SheriffSuicide);
            if (target == Lovers.lover1 || target == Lovers.lover2) infos.Add(SpecialInfo.PassiveLoverSuicide);
            if (target == Thief.thief) infos.Add(SpecialInfo.ThiefSuicide);
            if (target == Warlock.warlock) infos.Add(SpecialInfo.WarlockSuicide);
        }
        else
        {
            if (target == Lovers.lover1 || target == Lovers.lover2) infos.Add(SpecialInfo.ActiveLoverDies);
            if (target.Data.Role.IsImpostor && killer.Data.Role.IsImpostor && Thief.formerThief != killer)
                infos.Add(SpecialInfo.ImpostorTeamkill);
        }

        if (target == Jackal.Sidekick && Jackal.jackal.Any(x => x.PlayerId == killer.PlayerId))
            infos.Add(SpecialInfo.JackalKillsSidekick);
        if (target == Lawyer.lawyer && killer == Lawyer.target) infos.Add(SpecialInfo.LawyerKilledByClient);
        if (Alchemyst.target.wasCleaned) infos.Add(SpecialInfo.BodyCleaned);

        if (infos.Count > 0)
        {
            var selectedInfo = infos[rnd.Next(infos.Count)];
            switch (selectedInfo)
            {
                case SpecialInfo.SheriffSuicide:
                    msg = GetString("MediumInfo.SheriffSuicide");
                    break;
                case SpecialInfo.WarlockSuicide:
                    msg = GetString("MediumInfo.WarlockSuicide");
                    break;
                case SpecialInfo.ThiefSuicide:
                    msg = GetString("MediumInfo.ThiefSuicide");
                    break;
                case SpecialInfo.ActiveLoverDies:
                    msg = GetString("MediumInfo.ActiveLoverDies");
                    break;
                case SpecialInfo.PassiveLoverSuicide:
                    msg = GetString("MediumInfo.PassiveLoverSuicide");
                    break;
                case SpecialInfo.LawyerKilledByClient:
                    msg = GetString("MediumInfo.LawyerKilledByClient");
                    break;
                case SpecialInfo.JackalKillsSidekick:
                    msg = GetString("MediumInfo.JackalKillsSidekick");
                    break;
                case SpecialInfo.ImpostorTeamkill:
                    msg = GetString("MediumInfo.ImpostorTeamkill");
                    break;
                case SpecialInfo.BodyCleaned:
                    msg = GetString("MediumInfo.BodyCleaned");
                    break;
            }
        }
        else
        {
            var randomNumber = rnd.Next(4);
            var typeOfColor = IsLightColor(Alchemyst.target.KilledBy) ? GetString("Color.Light") : GetString("Color.Dark");
            var timeSinceDeath = (float)(meetingStartTime - Alchemyst.target.TimeOfDeath).TotalMilliseconds;
            var roleString = RoleInfo.GetRolesString(Alchemyst.target.Player, false, false, false, false);
            var seconds = Math.Round((meetingStartTime - Alchemyst.target.TimeOfDeath).TotalSeconds);

            switch (randomNumber)
            {
                case 0:
                    msg = string.Format(GetString("MediumInfo.PlayerRole"), RoleInfo.GetRolesString(Alchemyst.target.Player, false, false, false));
                    break;

                case 1:
                    msg = string.Format(GetString("MediumInfo.KillerColor"), typeOfColor);
                    break;

                case 2:
                    msg = string.Format(GetString("MediumInfo.DeathTime"), seconds);
                    break;

                default:
                    msg = string.Format(GetString("MediumInfo.KillerRole"), RoleInfo.GetRolesString(Alchemyst.target.KilledBy, false, false, false));
                    break;
            }
        }

        if (rnd.NextDouble() < chanceAdditionalInfo)
        {
            var count = 0;
            var condition = "";
            var alivePlayersList = PlayerControl.AllPlayerControls.ToArray().Where(pc => !pc.Data.IsDead);
            switch (rnd.Next(3))
            {
                case 0:
                    count = alivePlayersList.Count(pc =>
                        pc.Data.Role.IsImpostor || pc.IsKillerNeutral() ||
                        new List<RoleInfo> { RoleInfo.sheriff, RoleInfo.veteran, RoleInfo.thief }
                            .Contains(RoleInfo.getRoleInfoForPlayer(pc, false).FirstOrDefault()));
                    condition = "个杀手" + (count == 1 ? "" : "");
                    break;
                case 1:
                    count = alivePlayersList.Count(x => x.RoleCanUseVents());
                    condition = "个可以使用管道的玩家" + (count == 1 ? "" : "");
                    break;
                case 2:
                    count = alivePlayersList.Count(pc => pc.IsNeutral() && !pc.IsKillerNeutral());
                    condition = $"名玩家{(count == 1 ? "" : "")}{(count == 1 ? "是" : "是")}非击杀型中立";
                    break;
            }

            msg += $"\n你问我的时候,有{count} " + condition + (count == 1 ? "" : "") + " 还活着";
        }

        return Alchemyst.target.Player.Data.PlayerName + " 的灵魂说:\n" + msg;
    }

    private enum SpecialInfo
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
        BodyCleaned
    }
}
