using System;
using System.Linq;
using Hazel;
using Il2CppSystem.Collections.Generic;
using TheOtherRoles.Buttons;
using TheOtherRoles.Objects.Map;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
internal class IntroCutsceneOnDestroyPatch
{
    public static PoolablePlayer playerPrefab;
    public static Vector3 bottomLeft;

    public static void Prefix(IntroCutscene __instance)
    {
        Message("游戏开始");
        // Generate and initialize player icons
        var playerCounter = 0;
        if (PlayerControl.LocalPlayer != null && FastDestroyableSingleton<HudManager>.Instance != null)
        {
            var aspect = Camera.main.aspect;
            var safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
            var xpos = 1.75f - (safeOrthographicSize * aspect * 1.70f);
            var ypos = 0.15f - (safeOrthographicSize * 1.7f);
            bottomLeft = new Vector3(xpos / 2, ypos / 2, -61f);

            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                var data = p.Data;
                var player = Object.Instantiate(__instance.PlayerPrefab,
                    FastDestroyableSingleton<HudManager>.Instance.transform);
                playerPrefab = __instance.PlayerPrefab;
                p.SetPlayerMaterialColors(player.cosmetics.currentBodySprite.BodySprite);
                player.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
                player.cosmetics.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
                player.cosmetics.nameText.text = data.PlayerName;
                player.SetFlipX(true);
                ModOption.playerIcons[p.PlayerId] = player;
                player.gameObject.SetActive(false);

                //游戏开始时重置cd
                CustomButton.ResetAllCooldowns(ModOption.ButtonCooldown);

                if (PlayerControl.LocalPlayer == Arsonist.arsonist && p != Arsonist.arsonist)
                {
                    player.transform.localPosition = bottomLeft + new Vector3(-0.25f, -0.25f, 0) +
                                                     (Vector3.right * playerCounter++ * 0.35f);
                    player.transform.localScale = Vector3.one * 0.2f;
                    player.setSemiTransparent(true);
                    player.gameObject.SetActive(true);
                }
                else
                {
                    //  This can be done for all players not just for the bounty hunter as it was before. Allows the thief to have the correct position and scaling
                    player.transform.localPosition = bottomLeft;
                    player.transform.localScale = Vector3.one * 0.4f;
                    player.gameObject.SetActive(false);
                }
            }
        }

        // 管道追加
        AdditionalVents.AddAdditionalVents();

        // Add Electrical
        FungleAdditionalElectrical.CreateElectrical();

        // AntiTeleport set position
        AntiTeleport.setPosition();

        if (CustomOptionHolder.randomGameStartPosition.GetBool()) MapData.RandomSpawnPlayers();

        if (AmongUsClient.Instance.AmHost)
        {
            var mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
            var writerS = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.DynamicMapOption, SendOption.Reliable);
            writerS.Write(mapId);
            AmongUsClient.Instance.FinishRpcImmediately(writerS);

            // First kill
            if (ModOption.shieldFirstKill && ModOption.firstKillName != "")
            {
                var target = PlayerControl.AllPlayerControls.ToList().FirstOrDefault(x => x.Data.PlayerName.Equals(ModOption.firstKillName));
                if (target != null)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetFirstKill, SendOption.Reliable);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.setFirstKill(target.PlayerId);
                }
            }

        }

        // Force Bounty Hunter to load a new Bounty when the Intro is over
        if (BountyHunter.bounty != null && PlayerControl.LocalPlayer == BountyHunter.bountyHunter)
        {
            BountyHunter.bountyUpdateTimer = 0f;
            if (FastDestroyableSingleton<HudManager>.Instance != null)
            {
                BountyHunter.cooldownText =
                    Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText,
                        FastDestroyableSingleton<HudManager>.Instance.transform);
                BountyHunter.cooldownText.alignment = TextAlignmentOptions.Center;
                BountyHunter.cooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -0.35f, -62f);
                BountyHunter.cooldownText.transform.localScale = Vector3.one * 0.4f;
                BountyHunter.cooldownText.gameObject.SetActive(true);
            }
        }

        ModOption.firstKillName = "";
    }

    public static void Postfix(IntroCutscene __instance)
    {

        // 显示按键提示
        Rewired.KeyboardMap keyboardMap = Rewired.ReInput.mapping.GetKeyboardMapInstance(0, 0);
        Il2CppReferenceArray<Rewired.ActionElementMap> actionArray;
        Rewired.ActionElementMap actionMap;

        // 地图
        actionArray = keyboardMap.GetButtonMapsWithAction(4);
        if (actionArray.Count > 0)
        {
            actionMap = actionArray[0];
            _ = CustomButton.SetKeyGuideOnSmallButton(HudManager.Instance.MapButton.gameObject, actionMap.keyCode);
            //_ = CustomButton.SetKeyGuide(HudManager.Instance.SabotageButton.gameObject, actionMap.keyCode);
        }

        // 使用
        actionArray = keyboardMap.GetButtonMapsWithAction(6);
        if (actionArray.Count > 0)
        {
            actionMap = actionArray[0];
            _ = CustomButton.SetKeyGuide(HudManager.Instance.UseButton.gameObject, actionMap.keyCode);
            _ = CustomButton.SetKeyGuide(HudManager.Instance.PetButton.gameObject, actionMap.keyCode);
        }

        // 报告
        actionArray = keyboardMap.GetButtonMapsWithAction(7);
        if (actionArray.Count > 0)
        {
            actionMap = actionArray[0];
            _ = CustomButton.SetKeyGuide(HudManager.Instance.ReportButton.gameObject, actionMap.keyCode);
        }

        // 击杀
        actionArray = keyboardMap.GetButtonMapsWithAction(8);
        if (actionArray.Count > 0)
        {
            actionMap = actionArray[0];
            _ = CustomButton.SetKeyGuide(HudManager.Instance.KillButton.gameObject, actionMap.keyCode);
        }

        // 管道
        actionArray = keyboardMap.GetButtonMapsWithAction(50);
        if (actionArray.Count > 0)
        {
            actionMap = actionArray[0];
            _ = CustomButton.SetKeyGuide(HudManager.Instance.ImpostorVentButton.gameObject, actionMap.keyCode);
        }
    }
}

[HarmonyPatch]
internal class IntroPatch
{
    public static void setupIntroTeamIcons(IntroCutscene __instance, ref List<PlayerControl> yourTeam)
    {
        // Intro solo teams
        if (PlayerControl.LocalPlayer.isNeutral())
        {
            var soloTeam = new List<PlayerControl>();
            soloTeam.Add(PlayerControl.LocalPlayer);
            yourTeam = soloTeam;
        }

        // Add the Spy to the Impostor team (for the Impostors)
        if (Spy.spy != null && PlayerControl.LocalPlayer.Data.Role.IsImpostor)
        {
            var players = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
            var fakeImpostorTeam =
                new List<PlayerControl>(); // The local player always has to be the first one in the list (to be displayed in the center)
            fakeImpostorTeam.Add(PlayerControl.LocalPlayer);
            foreach (var p in players)
                if (PlayerControl.LocalPlayer != p && (p == Spy.spy || p.Data.Role.IsImpostor))
                    fakeImpostorTeam.Add(p);
            yourTeam = fakeImpostorTeam;
        }

        // Role draft: If spy is enabled, don't show the team
        if (CustomOptionHolder.spySpawnRate.GetSelection() > 0 && PlayerControl.AllPlayerControls.ToArray().ToList().Where(x => x.Data.Role.IsImpostor).Count() > 1)
        {
            // The local player always has to be the first one in the list (to be displayed in the center)
            var fakeImpostorTeam = new List<PlayerControl>();
            fakeImpostorTeam.Add(PlayerControl.LocalPlayer);
            yourTeam = fakeImpostorTeam;
        }
    }

    public static void setupIntroTeam(IntroCutscene __instance, ref List<PlayerControl> yourTeam)
    {
        var infos = RoleInfo.getRoleInfoForPlayer(PlayerControl.LocalPlayer);
        var roleInfo = infos.FirstOrDefault(info => info.roleType != RoleType.Modifier);
        if (roleInfo == null) return;
        if (roleInfo.roleType == RoleType.Neutral)
        {
            var neutralColor = new Color32(76, 84, 78, 255);
            __instance.BackgroundBar.material.color = roleInfo.color;
            __instance.TeamTitle.text = "NeutralTeam".Translate();
            __instance.TeamTitle.color = neutralColor;
        }
        else
        {
            var isCrew = true;
            if (roleInfo.color == Palette.ImpostorRed) isCrew = false;
            if (isCrew)
            {
                __instance.BackgroundBar.material.color = roleInfo.color;
                __instance.TeamTitle.text = "CrewmateTeam".Translate();
                __instance.TeamTitle.color = Color.cyan;
            }
            else
            {
                __instance.BackgroundBar.material.color = roleInfo.color;
                __instance.TeamTitle.text = "ImpostorTeam".Translate();
                __instance.TeamTitle.color = Palette.ImpostorRed;
            }
        }
    }

    public static System.Collections.Generic.IEnumerator<WaitForSeconds> EndShowRole(IntroCutscene __instance)
    {
        yield return new WaitForSeconds(5f);
        __instance.YouAreText.gameObject.SetActive(false);
        __instance.RoleText.gameObject.SetActive(false);
        __instance.RoleBlurbText.gameObject.SetActive(false);
        __instance.ourCrewmate.gameObject.SetActive(false);
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CreatePlayer))]
    private class CreatePlayerPatch
    {
        public static void Postfix(IntroCutscene __instance, bool impostorPositioning, ref PoolablePlayer __result)
        {
            if (impostorPositioning) __result.SetNameColor(Palette.ImpostorRed);
        }
    }


    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.ShowRole))]
    private class SetUpRoleTextPatch
    {
        public static void SetRoleTexts(IntroCutscene __instance)
        {
            // Don't override the intro of the vanilla roles
            var infos = RoleInfo.getRoleInfoForPlayer(PlayerControl.LocalPlayer);
            var roleInfo = infos.FirstOrDefault(info => info.roleType != RoleType.Modifier);
            var modifierInfo = infos.FirstOrDefault(info => info.roleType == RoleType.Modifier && info.roleId != RoleId.Assassin);

            __instance.RoleBlurbText.text = "";
            if (roleInfo != null)
            {
                __instance.RoleText.text = roleInfo.Name;
                __instance.RoleText.color = roleInfo.color;
                __instance.RoleBlurbText.text = roleInfo.IntroDescription;
                __instance.RoleBlurbText.color = roleInfo.color;
            }

            if (Sheriff.knowsSheriff && Sheriff.Deputy != null && Sheriff.Player.First() != null)
            {
                if (infos.Any(info => info.roleId == RoleId.Sheriff))
                    __instance.RoleBlurbText.text = cs(Sheriff.color, $"\n你的捕快是 {Sheriff.Deputy?.Data?.PlayerName ?? ""}");
                else if (infos.Any(info => info.roleId == RoleId.Deputy))
                    __instance.RoleBlurbText.text = cs(Sheriff.color, $"\n你的警长是 {Sheriff.Player?.FirstOrDefault()?.Data?.PlayerName ?? ""}");
            }

            if (Executioner.executioner != null && infos.Any(info => info.roleId == RoleId.Executioner))
                __instance.RoleBlurbText.text = cs(Executioner.color, $"\n把 {Executioner.target?.Data?.PlayerName ?? ""} 投出去!");

            if (Lawyer.lawyer != null && infos.Any(info => info.roleId == RoleId.Lawyer))
                __instance.RoleBlurbText.text = cs(Lawyer.color, $"\n你的辩护目标是 {Lawyer.target?.Data?.PlayerName ?? ""}");

            if (modifierInfo != null)
            {
                if (modifierInfo.roleId != RoleId.Lover)
                {
                    __instance.RoleBlurbText.text +=
                        cs(modifierInfo.color, $"\n{modifierInfo.IntroDescription}");
                }
                else
                {
                    var otherLover = PlayerControl.LocalPlayer == Lovers.lover1
                        ? Lovers.lover2
                        : Lovers.lover1;
                    __instance.RoleBlurbText.text +=
                        cs(Lovers.color, $"\n♥ 你和 {otherLover?.Data?.PlayerName ?? ""} 坠入了爱河 ♥");
                }
            }
        }

        public static bool Prefix(IntroCutscene __instance)
        {
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(1f,
                new Action<float>(p => { SetRoleTexts(__instance); })));
            return true;
        }
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    private class BeginCrewmatePatch
    {
        public static void Prefix(IntroCutscene __instance, ref List<PlayerControl> teamToDisplay)
        {
            setupIntroTeamIcons(__instance, ref teamToDisplay);
        }

        public static void Postfix(IntroCutscene __instance, ref List<PlayerControl> teamToDisplay)
        {
            setupIntroTeam(__instance, ref teamToDisplay);
        }
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    private class BeginImpostorPatch
    {
        public static void Prefix(IntroCutscene __instance, ref List<PlayerControl> yourTeam)
        {
            setupIntroTeamIcons(__instance, ref yourTeam);
        }

        public static void Postfix(IntroCutscene __instance, ref List<PlayerControl> yourTeam)
        {
            setupIntroTeam(__instance, ref yourTeam);
        }
    }
}

