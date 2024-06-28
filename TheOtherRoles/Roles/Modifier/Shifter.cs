using UnityEngine;

namespace TheOtherRoles.Roles.Modifier;
public static class Shifter
{
    public static PlayerControl shifter;

    public static PlayerControl futureShift;
    public static PlayerControl currentTarget;
    public static PlayerControl InvertDuration;

    public static bool shiftNeutral;
    public static bool shiftALLNeutra;

    private static Sprite buttonSprite;

    public static Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = loadSpriteFromResources("TheOtherRoles.Resources.ShiftButton.png", 115f);
        return buttonSprite;
    }

    public static void shiftRole(PlayerControl player1, PlayerControl player2, bool repeat = true)
    {
        if (Guesser.niceGuesser != null && Guesser.niceGuesser == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Guesser.niceGuesser = player1;
        }
        else if (Mayor.mayor != null && Mayor.mayor == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Mayor.mayor = player1;
        }
        else if (Portalmaker.portalmaker != null && Portalmaker.portalmaker == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Portalmaker.portalmaker = player1;
        }
        else if (Engineer.engineer != null && Engineer.engineer == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Engineer.engineer = player1;
        }
        else if (PrivateInvestigator.privateInvestigator != null && PrivateInvestigator.privateInvestigator == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            PrivateInvestigator.privateInvestigator = player1;
        }
        else if (Sheriff.sheriff != null && Sheriff.sheriff == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            if (Sheriff.formerDeputy != null && Sheriff.formerDeputy == Sheriff.sheriff)
                Sheriff.formerDeputy = player1; // Shifter also shifts info on promoted deputy (to get handcuffs)
            Sheriff.sheriff = player1;
        }
        else if (Deputy.deputy != null && Deputy.deputy == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Deputy.deputy = player1;
        }
        else if (Sheriff.formerSheriff != null && Sheriff.formerSheriff == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Sheriff.formerSheriff = null;
            Sheriff.formerDeputy = null;
            Deputy.deputy = player1;
        }
        else if (BodyGuard.bodyguard != null && BodyGuard.bodyguard == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            BodyGuard.bodyguard = player1;
        }
        else if (Jumper.jumper != null && Jumper.jumper == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Jumper.jumper = player1;
        }
        else if (Detective.detective != null && Detective.detective == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Detective.detective = player1;
        }
        else if (TimeMaster.timeMaster != null && TimeMaster.timeMaster == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            TimeMaster.timeMaster = player1;
        }
        else if (Veteren.veteren != null && Veteren.veteren == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Veteren.veteren = player1;
        }
        else if (Medic.medic != null && Medic.medic == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Medic.medic = player1;
        }
        else if (Swapper.swapper != null && Swapper.swapper == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Swapper.swapper = player1;
        }
        else if (Seer.seer != null && Seer.seer == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Seer.seer = player1;
        }
        else if (Hacker.hacker != null && Hacker.hacker == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Hacker.hacker = player1;
        }
        else if (Tracker.tracker != null && Tracker.tracker == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Tracker.tracker = player1;
        }
        else if (Snitch.snitch != null && Snitch.snitch == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Snitch.snitch = player1;
        }
        else if (Spy.spy != null && Spy.spy == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Spy.spy = player1;
        }
        else if (SecurityGuard.securityGuard != null && SecurityGuard.securityGuard == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            SecurityGuard.securityGuard = player1;
        }
        else if (Medium.medium != null && Medium.medium == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Medium.medium = player1;
        }
        else if (Trapper.trapper != null && Trapper.trapper == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Trapper.trapper = player1;
        }
        else if (Prophet.prophet != null && Prophet.prophet == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Prophet.prophet = player1;
        }
        else if (Prosecutor.prosecutor != null && Prosecutor.prosecutor == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Prosecutor.prosecutor = player1;
        }
        else if (Amnisiac.amnisiac != null && Amnisiac.amnisiac == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Amnisiac.amnisiac = player1;
        }
        else if (Jester.jester != null && Jester.jester == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Jester.jester = player1;
        }
        else if (Vulture.vulture != null && Vulture.vulture == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Vulture.vulture = player1;
        }
        else if (Lawyer.lawyer != null && Lawyer.lawyer == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Lawyer.lawyer = player1;
        }
        else if (Executioner.executioner != null && Executioner.executioner == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Executioner.executioner = player1;
        }
        else if (Pursuer.pursuer != null && Pursuer.pursuer == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Pursuer.pursuer = player1;
        }
        else if (Arsonist.arsonist != null && Arsonist.arsonist == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Arsonist.arsonist = player1;
        }
        else if (Thief.thief != null && Thief.thief == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Thief.thief = player1;
        }
        else if (Doomsayer.doomsayer != null && Doomsayer.doomsayer == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Doomsayer.doomsayer = player1;
        }
        else if (Werewolf.werewolf != null && Werewolf.werewolf == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Werewolf.werewolf = player1;
        }
        else if (Swooper.swooper != null && Swooper.swooper == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Swooper.swooper = player1;
        }
        else if (Juggernaut.juggernaut != null && Juggernaut.juggernaut == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Juggernaut.juggernaut = player1;
        }
        else if (Akujo.akujo != null && Akujo.akujo == player2)
        {
            if (repeat) shiftRole(player2, player1, false);
            Akujo.akujo = player1;
        }
    }

    public static void clearAndReload()
    {
        shifter = null;
        currentTarget = null;
        futureShift = null;
        shiftNeutral = CustomOptionHolder.modifierShiftNeutral.getBool();
        shiftALLNeutra = CustomOptionHolder.modifierShiftALLNeutral.getBool();
    }
}