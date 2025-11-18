using AmongUs.GameOptions;

namespace TheOtherRoles.Roles;

internal static class RoleAssignmentPatch
{
    [HarmonyPatch(typeof(RoleOptionsCollectionV07), nameof(RoleOptionsCollectionV07.GetNumPerGame))]
    internal class RoleOptionsDataGetNumPerGamePatch
    {
        public static void Postfix(ref int __result)
        {
            // Deactivate Vanilla Roles if the mod roles are active
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal) __result = 0;
        }
    }

    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.GetAdjustedNumImpostors))]
    internal class GameOptionsDataGetAdjustedNumImpostorsPatch
    {
        public static void Postfix(ref int __result)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal)
            {
                // Ignore Vanilla impostor limits in TOR Games.
                __result = Mathf.Clamp(GameOptionsManager.Instance.CurrentGameOptions.NumImpostors, 0, 15);
            }
        }
    }

    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Validate))]
    internal class GameOptionsDataValidatePatch
    {
        public static void Postfix(GameOptionsData __instance)
        {
            __instance.NumImpostors = GameOptionsManager.Instance.CurrentGameOptions.NumImpostors;
        }
    }

    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
    internal class RoleManagerSelectRolesPatch
    {
        public static void Postfix()
        {
            // Don't assign Roles in Hide N Seek
            if (IsHideNSeek || RoleDraft.isEnabled) return;
            SetAllPlayerRole();
        }

        public static bool isGuesserGamemode => ModOption.GameMode == CustomGameModes.Guesser;

        public static int blockedAssignments;
        public static int maxBlocks = 10;
        private static List<Tuple<byte, byte>> playerRoleMap = new();

        public static void SetAllPlayerRole()
        {
            var data = SelectorRoles();
            AssignSpecialRoles(data);
            AssignEnsuredRoles(data);
            AssignChanceRoles(data);
            //assignRoleTargets(data); // Assign targets for Lawyer & Prosecutor
            //if (isGuesserGamemode) assignGuesserGamemode();
            //assignModifiers(); // Assign modifier
            //setRolesAgain(); //brb
        }

        public static RoleSelectorStorage SelectorRoles()
        {
            var crewmates = PlayerControl.AllPlayerControls.ToList().OrderBy(x => Guid.NewGuid()).ToList();
            crewmates.RemoveAll(x => x.Data.Role.IsImpostor);
            var impostors = PlayerControl.AllPlayerControls.ToList().OrderBy(x => Guid.NewGuid()).ToList();
            impostors.RemoveAll(x => !x.Data.Role.IsImpostor);

            var neutralMin = CustomOptionHolder.neutralRolesCountMin.GetSelection();
            var neutralMax = CustomOptionHolder.neutralRolesCountMax.GetSelection();
            var killerNeutralMin = CustomOptionHolder.killerNeutralRolesCountMin.GetSelection();
            var killerNeutralMax = CustomOptionHolder.killerNeutralRolesCountMax.GetSelection();
            var impostorNum = ModOption.NumImpostors;

            if (RoleDraft.isEnabled)
            {
                neutralMin = neutralMax;
                killerNeutralMin = killerNeutralMax;
            }

            // Make sure min is less or equal to max
            neutralMin = Math.Min(neutralMin, neutralMax);
            killerNeutralMin = Math.Min(killerNeutralMin, killerNeutralMax);

            // Get the maximum allowed count of each role type based on the minimum and maximum option
            var neutralCountSettings = rnd.Next(neutralMin, neutralMax + 1);
            var killerNeutralCount = rnd.Next(killerNeutralMin, killerNeutralMax + 1);
            var crewCountSettings = PlayerControl.AllPlayerControls.Count - neutralCountSettings - impostorNum;

            // 杀手中立不能超过中立阵营总数
            killerNeutralCount = Math.Min(killerNeutralCount, neutralCountSettings);

            // 职业总数设定
            var maxCrewmateRoles = Mathf.Min(crewmates.Count, crewCountSettings);
            var maxNeutralRoles = Mathf.Min(crewmates.Count, neutralCountSettings);
            var maxKillerNeutralRoles = Mathf.Min(crewmates.Count, killerNeutralCount);
            var maxImpostorRoles = Mathf.Min(impostors.Count, impostorNum);

            // 职业设置
            var impSettings = new Dictionary<byte, (int rate, int count)>();
            var neutralSettings = new Dictionary<byte, (int rate, int count)>();
            var killerNeutralSettings = new Dictionary<byte, (int rate, int count)>();
            var crewSettings = new Dictionary<byte, (int rate, int count)>();

            foreach (var info in RoleInfo.AllRoleInfo.Where(x => x.RoleType == RoleType.Crewmate && x.CanAssign))
            {
                if (info.AssignSelection <= 0) continue;
                if (info.AssignSelection > 0) crewSettings.Add((byte)info.RoleId, (info.AssignSelection, info.PlayerCount));
            }

            foreach (var info in RoleInfo.AllRoleInfo.Where(x => x.RoleType == RoleType.Impostor && x.CanAssign))
            {
                if (info.AssignSelection <= 0) continue;
                if (info.RoleId == RoleId.Poucher && PoucherA.spawnModifier) continue;
                if (info.AssignSelection > 0) impSettings.Add((byte)info.RoleId, (info.AssignSelection, info.PlayerCount));
            }

            foreach (var info in RoleInfo.AllRoleInfo.Where(x => x.RoleType == RoleType.Neutral && x.CanAssign))
            {
                if (info.AssignSelection <= 0) continue;
                if (info.AssignSelection > 0 && !IsKillerNeutral(info.RoleId)) neutralSettings.Add((byte)info.RoleId, (info.AssignSelection, info.PlayerCount));
                if (info.AssignSelection > 0 && IsKillerNeutral(info.RoleId)) killerNeutralSettings.Add((byte)info.RoleId, (info.AssignSelection, info.PlayerCount));
                if (killerNeutralMin + killerNeutralMax == 0)
                {
                    neutralSettings.AddRange(killerNeutralSettings);
                    maxKillerNeutralRoles = 0;
                    killerNeutralSettings.Clear();
                }
                else
                {

                    maxNeutralRoles -= maxKillerNeutralRoles;
                }
            }

            Message("----------------------------------------------");
            Message($"impostors {impostors.Count}");
            Message($"crewmates {crewmates.Count}");
            Message("----------------------------------------------");
            Message($"impSettings {impSettings.Count}");
            Message($"crewSettings {crewSettings.Count}");
            Message($"neutralSettings {neutralSettings.Count}");
            Message($"killerNeutralSettings {killerNeutralSettings.Count}");
            Message("----------------------------------------------");
            Message($"maxImpostorRoles {maxImpostorRoles}");
            Message($"maxCrewmateRoles {maxCrewmateRoles}");
            Message($"maxNeutralRoles {maxNeutralRoles}");
            Message($"maxKillerNeutralRoles {maxKillerNeutralRoles}");
            Message("----------------------------------------------");

            return new RoleSelectorStorage
            {
                crewmates = crewmates,
                impostors = impostors,
                crewSettings = crewSettings,
                neutralSettings = neutralSettings,
                killerNeutralSettings = killerNeutralSettings,
                impSettings = impSettings,
                maxCrewmateRoles = Mathf.Max(0, maxCrewmateRoles),
                maxNeutralRoles = Mathf.Max(0, maxNeutralRoles),
                maxKillerNeutralRoles = Mathf.Max(0, maxKillerNeutralRoles),
                maxImpostorRoles = Mathf.Max(0, maxImpostorRoles)
            };
        }

        private static void AssignEnsuredRoles(RoleSelectorStorage data)
        {
            blockedAssignments = 0;

            static List<byte> GetEnsuredRoles(Dictionary<byte, (int rate, int count)> settings) => settings
                .Where(x => x.Value.rate == 10)
                .Select(x => Enumerable.Repeat(x.Key, x.Value.count))
                .SelectMany(x => x)
                .ToList();

            // Get all roles where the chance to occur is set to 100%
            var ensuredCrewmateRoles = GetEnsuredRoles(data.crewSettings);
            var ensuredNeutralRoles = GetEnsuredRoles(data.neutralSettings);
            var ensuredKillerNeutralRoles = GetEnsuredRoles(data.killerNeutralSettings);
            var ensuredImpostorRoles = GetEnsuredRoles(data.impSettings);

            // Assign roles until we run out of either players we can assign roles to or run out of roles we can assign to players
            while ((data.impostors.Count > 0 && data.maxImpostorRoles > 0 && ensuredImpostorRoles.Count > 0) ||
                (data.crewmates.Count > 0 && (
                    (data.maxCrewmateRoles > 0 && ensuredCrewmateRoles.Count > 0) ||
                    (data.maxNeutralRoles > 0 && ensuredNeutralRoles.Count > 0) ||
                    (data.maxKillerNeutralRoles > 0 && ensuredKillerNeutralRoles.Count > 0)
                )))
            {
                var rolesToAssign = new Dictionary<AssignType, List<byte>>();
                if (data.crewmates.Count > 0 && data.maxCrewmateRoles > 0 && ensuredCrewmateRoles.Count > 0)
                    rolesToAssign.Add(AssignType.Crewmate, ensuredCrewmateRoles);
                if (data.crewmates.Count > 0 && data.maxNeutralRoles > 0 && ensuredNeutralRoles.Count > 0)
                    rolesToAssign.Add(AssignType.Neutral, ensuredNeutralRoles);
                if (data.crewmates.Count > 0 && data.maxKillerNeutralRoles > 0 && ensuredKillerNeutralRoles.Count > 0)
                    rolesToAssign.Add(AssignType.KillerNeutral, ensuredKillerNeutralRoles);
                if (data.impostors.Count > 0 && data.maxImpostorRoles > 0 && ensuredImpostorRoles.Count > 0)
                    rolesToAssign.Add(AssignType.Impostor, ensuredImpostorRoles);

                // Randomly select a pool of roles to assign a role from next (Crewmate role, Neutral role or Impostor role)
                // then select one of the roles from the selected pool to a player
                // and remove the role (and any potentially blocked role pairings) from the pool(s)
                var roleType = rolesToAssign.Keys.ElementAt(rnd.Next(0, rolesToAssign.Keys.Count()));
                var players = roleType is AssignType.Impostor ? data.impostors : data.crewmates;
                var index = rnd.Next(0, rolesToAssign[roleType].Count);
                var roleId = rolesToAssign[roleType][index];
                var player = setRoleToRandomPlayer(rolesToAssign[roleType][index], players);
                if (player == byte.MaxValue && blockedAssignments < maxBlocks)
                {
                    blockedAssignments++;
                    continue;
                }
                blockedAssignments = 0;

                rolesToAssign[roleType].RemoveAt(index);

                if (blockedRolePairings.Any(pair => pair.Contains((RoleId)roleId)))
                {
                    foreach (var blockedRoleId in blockedRolePairings
                        .Where(pair => pair.Contains((RoleId)roleId))
                        .SelectMany(x => x)
                        .Where(x => x != (RoleId)roleId)
                        .Select(x => (byte)x))
                    {
                        // Set chance for the blocked roles to 0 for chances less than 100%
                        if (data.impSettings.ContainsKey(blockedRoleId)) data.impSettings[blockedRoleId] = (0, 0);
                        if (data.neutralSettings.ContainsKey(blockedRoleId)) data.neutralSettings[blockedRoleId] = (0, 0);
                        if (data.crewSettings.ContainsKey(blockedRoleId)) data.crewSettings[blockedRoleId] = (0, 0);

                        // Remove blocked roles even if the chance was 100%
                        foreach (var ensuredRolesList in rolesToAssign.Values)
                        {
                            ensuredRolesList.RemoveAll(x => x == blockedRoleId);
                        }
                    }
                }

                // Adjust the role limit
                switch (roleType)
                {
                    case AssignType.Crewmate:
                        data.maxCrewmateRoles--;
                        break;
                    case AssignType.Neutral:
                        data.maxNeutralRoles--;
                        break;
                    case AssignType.KillerNeutral:
                        data.maxKillerNeutralRoles--;
                        break;
                    case AssignType.Impostor:
                        data.maxImpostorRoles--;
                        break;
                }
            }
        }

        private static void AssignChanceRoles(RoleSelectorStorage data)
        {
            blockedAssignments = 0;

            static List<byte> GetEnsuredRoles(Dictionary<byte, (int rate, int count)> settings) => settings
                .Where(x => x.Value.rate is > 0 and < 10)
                .Select(x => Enumerable.Repeat(x.Key, x.Value.count))
                .SelectMany(x => x)
                .ToList();

            // Get all roles where the chance to occur is set grater than 0% but not 100% and build a ticket pool based on their weight
            var crewmateTickets = GetEnsuredRoles(data.crewSettings);
            var neutralTickets = GetEnsuredRoles(data.neutralSettings);
            var killerNeutralTickets = GetEnsuredRoles(data.killerNeutralSettings);
            var impostorTickets = GetEnsuredRoles(data.impSettings);

            // Assign roles until we run out of either players we can assign roles to or run out of roles we can assign to players
            while ((data.impostors.Count > 0 && data.maxImpostorRoles > 0 && impostorTickets.Count > 0) ||
                (data.crewmates.Count > 0 && (
                    (data.maxCrewmateRoles > 0 && crewmateTickets.Count > 0) ||
                    (data.maxNeutralRoles > 0 && neutralTickets.Count > 0) ||
                    (data.maxKillerNeutralRoles > 0 && killerNeutralTickets.Count > 0))))
            {

                var rolesToAssign = new Dictionary<AssignType, List<byte>>();
                if (data.crewmates.Count > 0 && data.maxCrewmateRoles > 0 && crewmateTickets.Count > 0)
                    rolesToAssign.Add(AssignType.Crewmate, crewmateTickets);
                if (data.crewmates.Count > 0 && data.maxNeutralRoles > 0 && neutralTickets.Count > 0)
                    rolesToAssign.Add(AssignType.Neutral, neutralTickets);
                if (data.crewmates.Count > 0 && data.maxKillerNeutralRoles > 0 && killerNeutralTickets.Count > 0)
                    rolesToAssign.Add(AssignType.KillerNeutral, killerNeutralTickets);
                if (data.impostors.Count > 0 && data.maxImpostorRoles > 0 && impostorTickets.Count > 0)
                    rolesToAssign.Add(AssignType.Impostor, impostorTickets);

                // Randomly select a pool of role tickets to assign a role from next (Crewmate role, Neutral role or Impostor role)
                // then select one of the roles from the selected pool to a player
                // and remove all tickets of this role (and any potentially blocked role pairings) from the pool(s)
                var roleType = rolesToAssign.Keys.ElementAt(rnd.Next(0, rolesToAssign.Keys.Count));
                var players = roleType is AssignType.Impostor ? data.impostors : data.crewmates;
                var index = rnd.Next(0, rolesToAssign[roleType].Count);
                var roleId = rolesToAssign[roleType][index];
                var player = setRoleToRandomPlayer(rolesToAssign[roleType][index], players);
                if (player == byte.MaxValue && blockedAssignments < maxBlocks)
                {
                    blockedAssignments++;
                    continue;
                }
                blockedAssignments = 0;

                rolesToAssign[roleType].RemoveAll(x => x == roleId);

                if (blockedRolePairings.Any(pair => pair.Contains((RoleId)roleId)))
                {
                    foreach (var blockedRoleId in blockedRolePairings.Where(pair => pair.Contains((RoleId)roleId)).SelectMany(pair => pair))
                    {
                        // Remove tickets of blocked roles from all pools 
                        crewmateTickets.RemoveAll(x => (RoleId)x == blockedRoleId);
                        neutralTickets.RemoveAll(x => (RoleId)x == blockedRoleId);
                        impostorTickets.RemoveAll(x => (RoleId)x == blockedRoleId);
                    }
                }

                // Adjust the role limit
                switch (roleType)
                {
                    case AssignType.Crewmate:
                        data.maxCrewmateRoles--;
                        break;
                    case AssignType.Neutral:
                        data.maxNeutralRoles--;
                        break;
                    case AssignType.KillerNeutral:
                        data.maxKillerNeutralRoles--;
                        break;
                    case AssignType.Impostor:
                        data.maxImpostorRoles--;
                        break;
                }
            }
        }

        private static void AssignSpecialRoles(RoleSelectorStorage data)
        {
            // Assign GM
            if (CustomOptionHolder.gmEnabled.GetBool())
            {
                byte gmID = 0;
                if (CustomOptionHolder.gmIsHost.GetBool())
                {
                    var host = AmongUsClient.Instance?.GetHost().Character;
                    gmID = setRoleToHost((byte)RoleId.GM, host);

                    // First, remove the GM from role selection.
                    data.crewmates.RemoveAll(x => x.PlayerId == host.PlayerId);
                    data.impostors.RemoveAll(x => x.PlayerId == host.PlayerId);
                }
                else
                {
                    gmID = setRoleToRandomPlayer((byte)RoleId.GM, data.crewmates);
                }

                var p = PlayerById(gmID);
                if (p != null && CustomOptionHolder.gmDiesAtStart.GetBool())
                    p.Exiled();
            }
        }









        private static byte setRoleToHost(byte roleId, PlayerControl host)
        {
            var playerId = host.PlayerId;

            var writer = StartRPC(CustomRPC.SetRole);
            writer.Write(playerId);
            writer.Write(roleId);
            writer.EndRPC();
            RPCProcedure.setRole(playerId, roleId);
            return playerId;
        }

        private static byte setRoleToRandomPlayer(byte roleId, List<PlayerControl> playerList, bool removePlayer = true)
        {
            var index = rnd.Next(0, playerList.Count);
            var playerId = playerList[index].PlayerId;
            if (removePlayer) playerList.RemoveAt(index);

            playerRoleMap.Add(new Tuple<byte, byte>(playerId, roleId));

            var writer = StartRPC(CustomRPC.SetRole);
            writer.Write(playerId);
            writer.Write(roleId);
            writer.EndRPC();
            RPCProcedure.setRole(playerId, roleId);
            return playerId;
        }

        public class RoleSelectorStorage
        {
            public Dictionary<byte, (int rate, int count)> crewSettings = new();
            public Dictionary<byte, (int rate, int count)> impSettings = new();
            public Dictionary<byte, (int rate, int count)> neutralSettings = new();
            public Dictionary<byte, (int rate, int count)> killerNeutralSettings = new();
            public List<PlayerControl> crewmates { get; set; }
            public List<PlayerControl> impostors { get; set; }
            public int maxCrewmateRoles { get; set; }
            public int maxNeutralRoles { get; set; }
            public int maxKillerNeutralRoles { get; set; }
            public int maxImpostorRoles { get; set; }
        }

        public enum AssignType
        {
            Crewmate,
            Neutral,
            KillerNeutral,
            Impostor
        }
    }
}