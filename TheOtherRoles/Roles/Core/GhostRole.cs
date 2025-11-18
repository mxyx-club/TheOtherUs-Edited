namespace TheOtherRoles.Roles;

public class GhostRole
{
    public static Dictionary<AssignType, List<Assignment>> GhostRoles = new();
    public static List<PlayerControl> GhostPlayer = new();

    public static void ClearAndReload()
    {
        GhostRoles.Clear();
        GhostPlayer.Clear();

        GhostEngineer.ClearAndReload();
        Specter.ClearAndReload();
        Poltergeist.ClearAndReload();

        GhostRoles[AssignType.Crewmate] = new List<Assignment>
        {
            new(RoleId.GhostEngineer, CustomOptionHolder.ghostEngineerSpawnRate.GetSelection()),
            new(RoleId.Poltergeist, CustomOptionHolder.poltergeistSpawnRate.GetSelection())
        };

        GhostRoles[AssignType.otherNeutral] = new List<Assignment>
        {
            new(RoleId.Specter, CustomOptionHolder.specterSpawnRate.GetSelection())
        };
    }

    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.AssignRoleOnDeath))]
    public static class AssignRoleOnDeathPatch
    {
        public static bool Prefix([HarmonyArgument(0)] PlayerControl player, [HarmonyArgument(1)] bool specialRolesAllowed)
        {
            if (player.IsAlive() || player == null || !specialRolesAllowed) return false;
            return true;
        }

        public static void Postfix([HarmonyArgument(0)] PlayerControl player)
        {
            if (GhostPlayer.Contains(player)) return;

            if (player.IsCrew()) AssignRole(player, AssignType.Crewmate);

            if (otherNeutral(player)) AssignRole(player, AssignType.otherNeutral);
        }

        public static bool otherNeutral(PlayerControl player)
        {
            var role = player.GetRole();
            return role != RoleId.Pelican
                && role != RoleId.PartTimer
                && role != RoleId.Lawyer
                && role != RoleId.Jackal
                && role != RoleId.Sidekick
                && role != RoleId.Pavlovsowner
                && role != RoleId.Pavlovsdogs
                // && role != RoleId.Infected
                && player.IsNeutral();
        }

        private static void AssignRole(PlayerControl player, AssignType assignType)
        {
            if (!GhostRoles.TryGetValue(assignType, out var roles) || roles.All(x => x.SpawnRate == 0)) return;
            roles = roles.OrderBy(x => Guid.NewGuid()).ToList();
            foreach (var role in roles.Where(x => x.SpawnRate == 10 && !x.Assigned).ToList())
            {
                AssignRoleToPlayer(player, role);
                return;
            }

            var maxCount = roles.Count;
            var count = 0;
            while (count < maxCount)
            {
                var assigned = false;
                foreach (var role in roles.Where(x => x.SpawnRate is > 0 and < 10 && !x.Assigned).ToList())
                {
                    if (rnd.Next(1, 101) <= role.SpawnRate * (10 + count))
                    {
                        AssignRoleToPlayer(player, role);
                        assigned = true;
                        break;
                    }
                }
                if (assigned) break;
                count++;
            }
        }

        private static void AssignRoleToPlayer(PlayerControl player, Assignment role)
        {
            var write = StartRPC(PlayerControl.LocalPlayer, CustomRPC.SetGhostRole);
            write.Write(player.PlayerId);
            write.Write((byte)role.RoleId);
            write.EndRPC();

            RPCProcedure.setGhostRole(player.PlayerId, (byte)role.RoleId);
            GhostPlayer.Add(player);
            role.Assigned = true;
        }
    }

    public class Assignment
    {
        public RoleId RoleId { get; set; }
        public int SpawnRate { get; set; }
        public bool Assigned { get; set; }

        public Assignment(RoleId roleId, int Rate)
        {
            RoleId = roleId;
            SpawnRate = Rate;
        }
    }

    public enum AssignType
    {
        None,
        Crewmate,
        Impostor,
        Neutral,
        otherNeutral,
        Custom
    }
}
