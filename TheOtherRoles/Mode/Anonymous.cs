using TheOtherRoles.Attributes;

namespace TheOtherRoles.Mode;

internal class Anonymous
{
    public static bool IsEnabled => ModOption.GameMode == CustomGameModes.Anonymous;

    public static string[] AllHats = HatManager.Instance.allHats.Select(x => x.ProductId).Where(x => x.StartsWith("hat_")).ToArray();
    public static string[] AllSkins = HatManager.Instance.allSkins.Select(x => x.ProductId).ToArray();
    public static string[] AllVisors = HatManager.Instance.allVisors.Select(x => x.ProductId).ToArray();
    public static string[] AllNamePlates = HatManager.Instance.allNamePlates.Select(x => x.ProductId).ToArray();
    public static string[] AllPet = HatManager.Instance.allPets.Select(x => x.ProductId).ToArray();

    public const string EmptyHat = "hat_NoHat";
    public const string EmptySkin = "skin_None";
    public const string EmptyVisor = "visor_EmptyVisor";
    public const string EmptyNameplate = "nameplate_NoPlate";
    public const string EmptyPet = "pet_EmptyPet";

    [OnGameStart(Attributes.Priority.VeryLow)]
    public static void OnGameStart()
    {
        if (!IsEnabled) return;

        if (!AmongUsClient.Instance.AmHost) return;

        var colors = Enumerable.Range(1, 56).Select(i => (byte)i).ToList();
        var hats = AllHats.ToList();
        var skins = AllSkins.ToList();
        var visors = AllVisors.ToList();
        var nameplates = AllNamePlates.ToList();

        Shuffle(colors);
        Shuffle(hats);
        Shuffle(skins);
        Shuffle(visors);
        Shuffle(nameplates);

        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            var color = RandomAndRemove(colors);
            var hat = RandomOrEmpty(hats, EmptyHat);
            var skin = RandomOrEmpty(skins, EmptySkin);
            var visor = RandomOrEmpty(visors, EmptyVisor);
            var namePlate = RandomOrEmpty(nameplates, EmptyNameplate, 0.55f);

            player.RpcSetColor(color);                // 随机颜色
            player.RpcSetHat(hat);                    // 随机原版帽子
            player.RpcSetSkin(skin);                  // 随机原版衣服
            player.RpcSetVisor(visor);                // 随机原版眼镜
            player.RpcSetLevel(0);                    // 重置等级为1级
            player.RpcSetNamePlate(namePlate);        // 随机原版铭牌
            player.RpcSetName(player.Data.GetPlayerColorString()); // 显示默认颜色名作为昵称
            player.RawSetPet(EmptyPet, color);        // 不要宠物
        }
    }
}
