using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Crewmate;

public static class Prophet
{
    public static PlayerControl prophet;
    public static Color32 color = new(255, 204, 127, byte.MaxValue);

    public static float cooldown = 25f;
    public static bool killCrewAsRed;
    public static bool benignNeutralAsRed;
    public static bool evilNeutralAsRed;
    public static bool killNeutralAsRed;
    public static bool canCallEmergency;
    public static int examineNum = 3;
    public static int examinesToBeRevealed = 1;
    public static int examinesLeft;
    public static bool revealProphet = true;
    public static bool isRevealed;
    public static List<Arrow> arrows = new();

    public static Dictionary<PlayerControl, bool> examined = new();
    public static PlayerControl currentTarget;

    public static Sprite buttonSprite = new ResourceSprite("SeerButton.png");
    public static bool IsRed(PlayerControl p)
    {
        if (p.Data.Role.IsImpostor || p.IsKillerNeutral()) return true;

        if (killCrewAsRed && (Sheriff.Player.Any(x => x == p) || p == Sheriff.Deputy || p == Veteran.veteran)) return true;

        if (benignNeutralAsRed && p.IsNeutral() && (Amnisiac.Player.Contains(p) || Pursuer.Player.Contains(p) || Survivor.Player.Contains(p))) return true;

        return evilNeutralAsRed && p.IsEvilNeutral();
    }

    public static void clearAndReload()
    {
        prophet = null;
        currentTarget = null;
        isRevealed = false;
        examined.Clear();
        revealProphet = CustomOptionHolder.prophetIsRevealed.GetBool();
        cooldown = CustomOptionHolder.prophetCooldown.GetFloat();
        examineNum = CustomOptionHolder.prophetNumExamines.GetInt();
        killCrewAsRed = CustomOptionHolder.prophetKillCrewAsRed.GetBool();
        benignNeutralAsRed = CustomOptionHolder.prophetBenignNeutralAsRed.GetBool();
        evilNeutralAsRed = CustomOptionHolder.prophetEvilNeutralAsRed.GetBool();
        killNeutralAsRed = CustomOptionHolder.prophetKillNeutralAsRed.GetBool();
        canCallEmergency = CustomOptionHolder.prophetCanCallEmergency.GetBool();
        examinesToBeRevealed = Math.Min(examineNum, CustomOptionHolder.prophetExaminesToBeRevealed.GetInt());
        examinesLeft = examineNum;
        if (arrows != null)
        {
            foreach (Arrow arrow in arrows)
                if (arrow?.arrow != null)
                    UObject.Destroy(arrow.arrow);
        }
        arrows.Clear();
    }
}
