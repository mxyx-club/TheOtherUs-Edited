namespace TheOtherRoles.Modules;

public class ModInputManager
{
    public static List<ModInput> allInputs = new();
    public static Dictionary<KeyCode, KeyCodeData> allKeyCodes = new();

    public static ModInput abilityInput;
    public static ModInput secondaryAbilityInput;
    public static ModInput modifierAbilityInput;
    public static ModInput changeAbilityInput;
    public static ModInput modKillInput;
    public static ModInput showOptionPageInput;
    public static ModInput helpInput;
    public static ModInput toggleChat;
    public static ModInput metaControlInput;
    public static ModInput endGameInput;
    public static ModInput meetingInput;
    public static ModInput screenResolution;
    public static ModInput nextChatChannel;

    public class ModInput
    {
        public string identifier { get; private set; }
        private readonly ConfigOption<KeyCode> config;
        public KeyCode keyCode => config.Value;
        private readonly KeyCode defaultKeyCode;

        public ModInput(string identifier, KeyCode defaultKeyCode)
        {
            this.identifier = identifier;
            this.defaultKeyCode = defaultKeyCode;
            config = ModConfig.Settings.CreateOption($"keybinding.{identifier}", defaultKeyCode);

            allInputs.Add(this);
        }

        public void changeKeyCode(KeyCode keyCode)
        {
            if (this.keyCode == keyCode && config.Value == keyCode)
            {
                return;
            }

            if (config.Value == keyCode) return;
            config.Update(keyCode);
        }

        public void resetToDefault() => config.Update(defaultKeyCode);
    }

#nullable enable
    public class KeyInputTexture(string address)
    {
        private readonly string address = address;
        private Texture2D? texture;
        public Texture2D GetTexture()
        {
            if (texture == null || !texture)
            {
                texture = UnityHelper.loadTextureFromResources(address);
            }

            return texture;
        }
    }

    public class KeyCodeData
    {
        public KeyCode keyCode { get; private set; }
        public KeyInputTexture texture { get; private set; }
        public int textureNum { get; private set; }
        public string displayKey { get; private set; }
        private Sprite? sprite;
        public KeyCodeData(KeyCode keyCode, string displayKey, KeyInputTexture texture, int num)
        {
            this.keyCode = keyCode;
            this.displayKey = displayKey;
            this.texture = texture;
            textureNum = num;

            allKeyCodes.Add(keyCode, this);
        }

        public Sprite GetSprite()
        {
            if (sprite == null || !sprite)
            {
                sprite = UnityHelper.loadSpriteFromResources(texture.GetTexture(), 100f, new Rect(0f, -19f * textureNum, 18f, -19f));
            }

            return sprite;
        }
    }

    public static void Load()
    {
        KeyInputTexture kit;
        kit = new KeyInputTexture("TheOtherRoles.Resources.KeyBind.Characters0.png");
        _ = new KeyCodeData(KeyCode.Tab, "Tab", kit, 0);
        _ = new KeyCodeData(KeyCode.Space, "Space", kit, 1);
        _ = new KeyCodeData(KeyCode.Comma, "<", kit, 2);
        _ = new KeyCodeData(KeyCode.Period, ">", kit, 3);
        kit = new KeyInputTexture("TheOtherRoles.Resources.KeyBind.Characters1.png");
        for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++) _ = new KeyCodeData(key, ((char)('A' + key - KeyCode.A)).ToString(), kit, key - KeyCode.A);

        kit = new KeyInputTexture("TheOtherRoles.Resources.KeyBind.Characters2.png");
        for (int i = 0; i < 15; i++) _ = new KeyCodeData(KeyCode.F1 + i, "F" + (i + 1), kit, i);

        kit = new KeyInputTexture("TheOtherRoles.Resources.KeyBind.Characters3.png");
        _ = new KeyCodeData(KeyCode.RightShift, "RShift", kit, 0);
        _ = new KeyCodeData(KeyCode.LeftShift, "LShift", kit, 1);
        _ = new KeyCodeData(KeyCode.RightControl, "RControl", kit, 2);
        _ = new KeyCodeData(KeyCode.LeftControl, "LControl", kit, 3);
        _ = new KeyCodeData(KeyCode.RightAlt, "RAlt", kit, 4);
        _ = new KeyCodeData(KeyCode.LeftAlt, "LAlt", kit, 5);
        kit = new KeyInputTexture("TheOtherRoles.Resources.KeyBind.Characters4.png");
        for (int i = 0; i < 6; i++) _ = new KeyCodeData(KeyCode.Mouse1 + i, "Mouse " + (i == 0 ? "Right" : i == 1 ? "Middle" : (i + 1).ToString()), kit, i);

        kit = new KeyInputTexture("TheOtherRoles.Resources.KeyBind.Characters5.png");
        for (int i = 0; i < 10; i++) _ = new KeyCodeData(KeyCode.Alpha0 + i, "0" + (i + 1), kit, i);

        modKillInput = new ModInput("kill", KeyCode.Q);
        abilityInput = new ModInput("ability", KeyCode.F);
        secondaryAbilityInput = new ModInput("secondaryAbility", KeyCode.G);
        modifierAbilityInput = new ModInput("modifierAbility", KeyCode.Z);
        changeAbilityInput = new ModInput("changeAbility", KeyCode.LeftShift);
        showOptionPageInput = new ModInput("showOptionPage", KeyCode.F1);
        helpInput = new ModInput("help", KeyCode.H);
        toggleChat = new ModInput("toggleChat", KeyCode.F2);
        screenResolution = new ModInput("screenResolution", KeyCode.F11);
        nextChatChannel = new ModInput("nextChatChannel", KeyCode.Tab);
        metaControlInput = new ModInput("metaControl", KeyCode.LeftControl);
        endGameInput = new ModInput("endGame", KeyCode.F5);
        meetingInput = new ModInput("meeting", KeyCode.M);
    }
}
