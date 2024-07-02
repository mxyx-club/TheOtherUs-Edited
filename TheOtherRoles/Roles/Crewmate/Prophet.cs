using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Objects;
using UnityEngine;
using Object = UnityEngine.Object;

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
    public static List<Arrow> arrows = new List<Arrow>();

    public static Dictionary<PlayerControl, bool> examined = new Dictionary<PlayerControl, bool>();
    public static PlayerControl currentTarget;

    private static Sprite buttonSprite;
    public static Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = loadSpriteFromResources("TheOtherRoles.Resources.SeerButton.png", 115f);
        return buttonSprite;
    }
    public static bool IsKiller(PlayerControl p)
    {
        if (p.Data.Role.IsImpostor || isKiller(p))
        {
            return true;
        }
        if (killCrewAsRed)
        {
            if (p == Sheriff.sheriff || p == Deputy.deputy || p == Veteren.veteren)
            {
                return true;
            }
        }
        if (benignNeutralAsRed)
        {
            if (isNeutral(p) && (p == Amnisiac.amnisiac || Pursuer.pursuer.Any(player => player == p)))
            {
                return true;
            }
        }
        return evilNeutralAsRed && isEvil(p);
    }

    public static void clearAndReload()
    {
        prophet = null;
        currentTarget = null;
        isRevealed = false;
        examined = new Dictionary<PlayerControl, bool>();
        revealProphet = CustomOptionHolder.prophetIsRevealed.getBool();
        cooldown = CustomOptionHolder.prophetCooldown.getFloat();
        examineNum = Mathf.RoundToInt(CustomOptionHolder.prophetNumExamines.getFloat());
        killCrewAsRed = CustomOptionHolder.prophetKillCrewAsRed.getBool();
        benignNeutralAsRed = CustomOptionHolder.prophetBenignNeutralAsRed.getBool();
        evilNeutralAsRed = CustomOptionHolder.prophetEvilNeutralAsRed.getBool();
        killNeutralAsRed = CustomOptionHolder.prophetKillNeutralAsRed.getBool();
        canCallEmergency = CustomOptionHolder.prophetCanCallEmergency.getBool();
        examinesToBeRevealed = Math.Min(examineNum, Mathf.RoundToInt(CustomOptionHolder.prophetExaminesToBeRevealed.getFloat()));
        examinesLeft = examineNum;
        if (arrows != null)
        {
            foreach (Arrow arrow in arrows)
                if (arrow?.arrow != null)
                    Object.Destroy(arrow.arrow);
        }
        arrows = new List<Arrow>();
    }
}
