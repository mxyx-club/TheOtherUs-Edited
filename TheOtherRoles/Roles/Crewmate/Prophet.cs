using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class Prophet : RoleBase
{
    public static Color32 color = new(255, 204, 127, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Prophet),
        (p) => new Prophet(p),
        RoleId.Prophet,
        RoleType.Crewmate,
        "Prophet",
        color,
        303600,
        AddOptions
    );

    public Prophet(PlayerControl p) : base(p, roleinfo) { }

    public static Dictionary<byte, Arrow> arrows = new();

    public static float cooldown = 25f;
    public static bool killCrewAsRed;
    public static bool benignNeutralAsRed;
    public static bool evilNeutralAsRed;
    public static bool killNeutralAsRed;
    public static bool canCallEmergency;
    public static int examineNum = 3;
    public static int examinesToBeRevealed = 1;
    public static bool revealProphet = true;

    public Dictionary<PlayerControl, bool> examined = new();
    public bool isRevealed;
    public int examinesLeft;
    public PlayerControl currentTarget;

    public static CustomOption prophetCooldown;
    public static CustomOption prophetNumExamines;
    public static CustomOption prophetCanCallEmergency;
    public static CustomOption prophetIsRevealed;
    public static CustomOption prophetExaminesToBeRevealed;
    public static CustomOption prophetKillCrewAsRed;
    public static CustomOption prophetBenignNeutralAsRed;
    public static CustomOption prophetEvilNeutralAsRed;
    public static CustomOption prophetKillNeutralAsRed;

    public CustomButton prophetButton;
    public static Sprite buttonSprite = new ResourceSprite("SeerButton.png");

    public static RemoteProcess<(PlayerControl player, PlayerControl target)> ProphetExamine = new("ProphetExamine", (data, _) =>
    {
        if (data.target == null) return;
        if (data.player != null && data.player.TryGetRole<Prophet>(out var prophet))
        {
            if (prophet.examined.ContainsKey(data.target)) prophet.examined.Remove(data.target);
            prophet.examined.Add(data.target, IsRed(data.target));
            prophet.examinesLeft--;
            if ((examineNum - prophet.examinesLeft >= examinesToBeRevealed) && revealProphet) prophet.isRevealed = true;
        }
    });



    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        prophetCooldown = CustomOption.Create(configId++, CustomOptionType.Crewmate, "prophetCooldown", 20f, 5f, 60f, 2.5f, roleinfo.RoleOption);
        prophetNumExamines = CustomOption.Create(configId++, CustomOptionType.Crewmate, "prophetNumExamines", 4, 1, 10, 1, roleinfo.RoleOption);
        prophetCanCallEmergency = CustomOption.Create(configId++, CustomOptionType.Crewmate, "canCallEmergency", true, roleinfo.RoleOption);
        prophetIsRevealed = CustomOption.Create(configId++, CustomOptionType.Crewmate, "prophetIsRevealed", false, roleinfo.RoleOption);
        prophetExaminesToBeRevealed = CustomOption.Create(configId++, CustomOptionType.Crewmate, "prophetExaminesToBeRevealed", 3, 1, 10, 1, prophetIsRevealed);
        prophetKillCrewAsRed = CustomOption.Create(configId++, CustomOptionType.Crewmate, "prophetKillCrewAsRed", false, roleinfo.RoleOption);
        prophetBenignNeutralAsRed = CustomOption.Create(configId++, CustomOptionType.Crewmate, "prophetBenignNeutralAsRed", false, roleinfo.RoleOption);
        prophetEvilNeutralAsRed = CustomOption.Create(configId++, CustomOptionType.Crewmate, "prophetEvilNeutralAsRed", true, roleinfo.RoleOption);
        prophetKillNeutralAsRed = CustomOption.Create(configId++, CustomOptionType.Crewmate, "prophetKillNeutralAsRed", true, roleinfo.RoleOption);
    }

    public static bool IsRed(PlayerControl p)
    {
        if (p.IsImpostor(AndCat: true) || p.IsKillerNeutral()) return true;

        if (killCrewAsRed && (p.Is(RoleId.Sheriff) || p.Is(RoleId.Veteran) || p.Is(RoleId.Vigilante))) return true;

        if (benignNeutralAsRed && p.IsBenignNeutral()) return true;

        return evilNeutralAsRed && p.IsEvilNeutral();
    }

    public override void Initialize()
    {
        currentTarget = null;
        isRevealed = false;
        examined.Clear();
        revealProphet = prophetIsRevealed.GetBool();
        cooldown = prophetCooldown.GetFloat();
        examineNum = prophetNumExamines.GetInt();
        killCrewAsRed = prophetKillCrewAsRed.GetBool();
        benignNeutralAsRed = prophetBenignNeutralAsRed.GetBool();
        evilNeutralAsRed = prophetEvilNeutralAsRed.GetBool();
        killNeutralAsRed = prophetKillNeutralAsRed.GetBool();
        canCallEmergency = prophetCanCallEmergency.GetBool();
        examinesToBeRevealed = Math.Min(examineNum, prophetExaminesToBeRevealed.GetInt());
        examinesLeft = examineNum;
        if (arrows.TryGetValue(Player.PlayerId, out var arrow)) UObject.Destroy(arrow?.arrow);
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        if (arrows == null) return;

        foreach (var arrow in arrows.Values) arrow?.arrow?.SetActive(false);

        if (Player.IsDead()) return;

        var local = PlayerControl.LocalPlayer;

        if (isRevealed && (local.IsImpostor() || local.IsEvilNeutral() || local.IsKillerNeutral()))
        {

            arrows[Player.PlayerId] ??= new Arrow(color);
            arrows[Player.PlayerId].arrow.SetActive(true);
            arrows[Player.PlayerId].Update(Player.transform.position);
        }
        else if (arrows[Player.PlayerId]?.arrow != null)
        {
            arrows[Player.PlayerId]?.arrow.Destroy();
            arrows.Remove(Player.PlayerId);
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        prophetButton?.Destroy();
        prophetButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        prophetButton?.Destroy();
        prophetButton = new CustomButton(
                () =>
                {
                    if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;
                    if (currentTarget != null)
                    {
                        ProphetExamine.Invoke((PlayerControl.LocalPlayer, currentTarget));
                        prophetButton.Timer = prophetButton.MaxTimer;
                    }
                },
                () =>
                {
                    return Player == PlayerControl.LocalPlayer && Player.IsAlive() && examinesLeft > 0;
                },
                () =>
                {
                    currentTarget = SetTarget();
                    SetPlayerOutline(currentTarget, color);

                    if (prophetButton.ButtonTitle != null)
                    {
                        if (examinesLeft > 0)
                            prophetButton.ButtonTitle.text = $"{examinesLeft}";
                        else
                            prophetButton.ButtonTitle.text = "";
                    }
                    return currentTarget != null && PlayerControl.LocalPlayer.CanMove;
                },
                () => { prophetButton.Timer = prophetButton.MaxTimer; },
                buttonSprite,
                __instance,
                __instance.AbilityButton,
                ModInputManager.abilityInput.keyCode,
                buttonText: GetString("ProphetText")
            );
    }
}
