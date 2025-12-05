namespace TheOtherRoles.Roles.Impostor;

public static class Mimic
{
    public static PlayerControl mimic;
    public static bool hasMimic;
    public static Color color = Palette.ImpostorRed;
    public static List<PlayerControl> killed = new();


    public static void clearAndReload(bool clearList = true)
    {
        mimic = null;
        if (clearList) hasMimic = false;
    }


    public static void MimicRole(byte targetId)
    {
        var target = PlayerById(targetId);
        if (target == null || mimic == null) return;
        var targetInfo = RoleInfo.getRoleInfoForPlayer(target);
        var roleInfo = targetInfo.FirstOrDefault(info => info.roleType != RoleType.Modifier);
        switch (roleInfo!.roleId)
        {
            case RoleId.BodyGuard:
                if (Amnisiac.resetRole) BodyGuard.clearAndReload();
                BodyGuard.bodyguard = mimic;
                hasMimic = true;
                break;

            case RoleId.Mayor:
                if (Amnisiac.resetRole) Mayor.clearAndReload();
                Mayor.mayor = mimic;

                hasMimic = true;
                break;

            case RoleId.Prosecutor:
                if (Amnisiac.resetRole) Prosecutor.clearAndReload();
                Prosecutor.prosecutor = mimic;
                Prosecutor.diesOnIncorrectPros = false;
                hasMimic = true;
                break;

            case RoleId.InfoSleuth:
                InfoSleuth.infoSleuth = mimic;
                hasMimic = true;
                break;

            case RoleId.Trapper:
                if (Amnisiac.resetRole) Trapper.clearAndReload();
                Trapper.trapper = mimic;
                hasMimic = true;
                break;

            case RoleId.Portalmaker:
                if (Amnisiac.resetRole) Portalmaker.clearAndReload();
                Portalmaker.portalmaker = mimic;
                hasMimic = true;
                break;

            case RoleId.Engineer:
                if (Amnisiac.resetRole) Engineer.clearAndReload();
                Engineer.engineer = mimic;
                hasMimic = true;
                break;

            case RoleId.Jumper:
                if (Amnisiac.resetRole) Jumper.clearAndReload();
                Jumper.jumper = mimic;
                hasMimic = true;
                break;

            case RoleId.Detective:
                if (Amnisiac.resetRole) Detective.clearAndReload();
                Detective.detective = mimic;
                hasMimic = true;
                break;

            case RoleId.Veteran:
                if (Amnisiac.resetRole) Veteran.clearAndReload();
                Veteran.veteran = mimic;
                hasMimic = true;
                break;

            case RoleId.Medic:
                if (Amnisiac.resetRole) Medic.clearAndReload();
                Medic.medic = mimic;
                hasMimic = true;
                break;

            case RoleId.Swapper:
                if (Amnisiac.resetRole) Swapper.clearAndReload();
                Swapper.swapper = mimic;
                hasMimic = true;
                break;

            case RoleId.Seer:
                if (Amnisiac.resetRole) Seer.clearAndReload();
                Seer.seer = mimic;
                hasMimic = true;
                break;

            case RoleId.Hacker:
                if (Amnisiac.resetRole) Hacker.clearAndReload();
                Hacker.hacker = mimic;
                hasMimic = true;
                break;

            case RoleId.Tracker:
                if (Amnisiac.resetRole) Tracker.clearAndReload();
                Tracker.tracker = mimic;
                hasMimic = true;
                break;

            case RoleId.SecurityGuard:
                if (Amnisiac.resetRole) SecurityGuard.clearAndReload();
                SecurityGuard.securityGuard = mimic;
                hasMimic = true;
                break;

            case RoleId.Medium:
                if (Amnisiac.resetRole) Medium.clearAndReload();
                Medium.medium = mimic;
                hasMimic = true;
                break;

            case RoleId.Balancer:
                if (Amnisiac.resetRole) Balancer.clearAndReload();
                Balancer.balancer = mimic;
                hasMimic = true;
                break;

            case RoleId.Prophet:
                if (Amnisiac.resetRole) Prophet.clearAndReload();
                Prophet.prophet = mimic;
                hasMimic = true;
                break;

            case RoleId.Oracle:
                if (Amnisiac.resetRole) Oracle.ClearAndReload();
                Oracle.Player = mimic;
                hasMimic = true;
                break;

            case RoleId.Jailor:
                if (Amnisiac.resetRole) Jailor.ClearAndReload();
                Jailor.Player = mimic;
                Jailor.usesCount = 1;
                hasMimic = true;
                break;

            case RoleId.Redemptor:
                if (Amnisiac.resetRole) Redemptor.ClearAndReload();
                Redemptor.Player = mimic;
                hasMimic = true;
                break;
        }
    }

}
