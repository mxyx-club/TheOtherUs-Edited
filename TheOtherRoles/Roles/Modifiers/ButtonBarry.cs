namespace TheOtherRoles.Roles.Modifier;

public class ButtonBarry : ModifierBase
{
    public static Color color = Color.yellow;

    public static readonly RoleInfo roleinfo = new(
        typeof(ButtonBarry),
        (p) => new ButtonBarry(p),
        RoleId.ButtonBarry,
        "ButtonBarry",
        color,
        402800,
        AddOptions
    );

    public ButtonBarry(PlayerControl p) : base(p, roleinfo) { }

    public override RoleId[] RemoveRole { get; set; } = [RoleId.Mayor];

    public int remoteMeetingsLeft = 1;
    public static bool SabotageRemoteMeetings;

    public static CustomOption modifierButtonSabotageRemoteMeetings;

    public CustomButton buttonBarryButton;
    public static ResourceSprite buttonSprite = new("EmergencyButton.png", 550);

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierButtonSabotageRemoteMeetings = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierButtonSabotageRemoteMeetings", true, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        remoteMeetingsLeft = 1;
        SabotageRemoteMeetings = modifierButtonSabotageRemoteMeetings.GetBool();
    }

    public override void CleanUp(HudManager __instance)
    {
        buttonBarryButton?.Destroy();
        buttonBarryButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {

        // ButtonBarry Meetings
        buttonBarryButton?.Destroy();
        buttonBarryButton = new CustomButton(
            () =>
            {
                //PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement 
                remoteMeetingsLeft--;

                var writer = StartRPC(CustomRPC.NoCheckStartMeeting);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(byte.MaxValue);
                writer.Write(true);
                writer.EndRPC();
                PlayerControl.LocalPlayer.NoCheckStartMeeting(null, true);

                buttonBarryButton.Timer = 1f;

            },
            () =>
            {
                return Player.IsAlive() && PlayerControl.LocalPlayer == Player && remoteMeetingsLeft > 0;
            },
            () =>
            {
                var sabotageActive = false;
                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                {
                    if ((task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor ||
                    task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms || task.TaskType == TaskTypes.StopCharles ||
                        (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)) && !SabotageRemoteMeetings)
                        sabotageActive = true;
                }

                return !sabotageActive && PlayerControl.LocalPlayer.CanMove;
            },
            () => { buttonBarryButton.Timer = buttonBarryButton.MaxTimer; },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.modifierAbilityInput.keyCode,
            true,
            buttonText: "buttonBarryText".Translate()
        )
        { MaxTimer = 0f };

    }
}
