namespace TheOtherRoles.Roles;

public class GhostRole
{
    public enum AssignType
    {
        None,
        Crewmate,
        Impostor,
        Neutral,
        otherNeutral,
        Custom
    }

    public static Dictionary<AssignType, List<Assignment>> GhostRoles = new();
    public static List<PlayerControl> GhostPlayer = new();

    public static void ClearAndReload()
    {
        GhostRoles.Clear();
        GhostPlayer.Clear();

        GhostRoles[AssignType.Crewmate] = new List<Assignment>
        {
            new(RoleId.GhostEngineer, CustomOptionHolder.ghostEngineerSpawnRate.GetSelection()),
            new(RoleId.Poltergeist, CustomOptionHolder.poltergeistSpawnRate.GetSelection())
        };

        GhostRoles[AssignType.otherNeutral] = new List<Assignment>
        {
            new(RoleId.Specter, CustomOptionHolder.specterSpawnRate.GetSelection())
        };

        GhostRoles[AssignType.Impostor] = new List<Assignment>
        {
            new(RoleId.Clog, CustomOptionHolder.clogSpawnRate.GetSelection())
        };
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
            else if (otherNeutral(player)) AssignRole(player, AssignType.otherNeutral);
            else if (player.IsImpostor()) AssignRole(player, AssignType.Impostor);
            else if (player.IsNeutral()) AssignRole(player, AssignType.Neutral);
        }

        private static bool otherNeutral(PlayerControl player)
        {
            if ((PartTimer.partTimer == player && PartTimer.target != null) ||
                (Lawyer.lawyer == player && Lawyer.target.IsAlive()) ||
                player == Jackal.Sidekick ||
                player == Pavlovsdogs.pavlovsowner ||
                (player == BandLeader.Player && BandLeader.Formed) ||
                Jackal.jackal.Any(x => x.PlayerId == player.PlayerId) ||
                Infected.Player.Any(x => x.PlayerId == player.PlayerId) ||
                Pavlovsdogs.pavlovsdogs.Any(x => x.PlayerId == player.PlayerId))
            {
                return false;
            }

            return player.IsNeutral();
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

            int maxCount = roles.Count;
            int count = 0;
            while (count < maxCount)
            {
                bool assigned = false;
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
}
