using System;
using System.Linq;
using Hazel;
using Il2CppSystem.Collections.Generic;
using TheOtherRoles.CustomGameModes;
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
        // Generate and initialize player icons
        var playerCounter = 0;
        var hideNSeekCounter = 0;
        if (CachedPlayer.LocalPlayer != null && FastDestroyableSingleton<HudManager>.Instance != null)
        {
            var aspect = Camera.main.aspect;
            var safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
            var xpos = 1.75f - (safeOrthographicSize * aspect * 1.70f);
            var ypos = 0.15f - (safeOrthographicSize * 1.7f);
            bottomLeft = new Vector3(xpos / 2, ypos / 2, -61f);

            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                var data = p.Data;
                var player = Object.Instantiate(__instance.PlayerPrefab,
                    FastDestroyableSingleton<HudManager>.Instance.transform);
                playerPrefab = __instance.PlayerPrefab;
                p.SetPlayerMaterialColors(player.cosmetics.currentBodySprite.BodySprite);
                player.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
                player.cosmetics.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
                //开局击杀cd
                CachedPlayer.LocalPlayer.PlayerControl.SetKillTimer(MapOption.ButtonCooldown);
                player.cosmetics.nameText.text = data.PlayerName;
                player.SetFlipX(true);
                MapOption.playerIcons[p.PlayerId] = player;
                player.gameObject.SetActive(false);

                if (CachedPlayer.LocalPlayer.PlayerControl == Arsonist.arsonist && p != Arsonist.arsonist)
                {
                    player.transform.localPosition = bottomLeft + new Vector3(-0.25f, -0.25f, 0) +
                                                     (Vector3.right * playerCounter++ * 0.35f);
                    player.transform.localScale = Vector3.one * 0.2f;
                    player.setSemiTransparent(true);
                    player.gameObject.SetActive(true);
                }
                else if (HideNSeek.isHideNSeekGM)
                {
                    if (HideNSeek.isHunted() && p.Data.Role.IsImpostor)
                    {
                        player.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0.4f, 0) +
                                                         (Vector3.right * playerCounter++ * 0.6f);
                        player.transform.localScale = Vector3.one * 0.3f;
                        player.cosmetics.nameText.text += $"{cs(Color.red, " (Hunter)")}";
                        player.gameObject.SetActive(true);
                    }
                    else if (!p.Data.Role.IsImpostor)
                    {
                        player.transform.localPosition = bottomLeft + new Vector3(-0.35f, -0.25f, 0) +
                                                         (Vector3.right * hideNSeekCounter++ * 0.35f);
                        player.transform.localScale = Vector3.one * 0.2f;
                        player.setSemiTransparent(true);
                        player.gameObject.SetActive(true);
                    }
                }
                else if (PropHunt.isPropHuntGM)
                {
                    player.transform.localPosition = bottomLeft + new Vector3(-1.25f, -0.1f, 0) +
                                                     (Vector3.right * hideNSeekCounter++ * 0.4f);
                    player.transform.localScale = Vector3.one * 0.24f;
                    player.setSemiTransparent(false);
                    player.cosmetics.nameText.transform.localPosition +=
                        Vector3.up * 0.2f * (hideNSeekCounter % 2 == 0 ? 1 : -1);
                    player.SetFlipX(false);
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

        // Force Reload of SoundEffectHolder
        SoundEffectsManager.Load();

        if (AmongUsClient.Instance.AmHost)
        {
            LastImpostor.promoteToLastImpostor();
        }

        // Force Bounty Hunter to load a new Bounty when the Intro is over
        if (BountyHunter.bounty != null && CachedPlayer.LocalPlayer.PlayerControl == BountyHunter.bountyHunter)
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

        if (CustomOptionHolder.randomGameStartPosition.getBool())
        {
            // Random spawn on game start
            if (CustomOptionHolder.randomGameStartToVents.getBool())
            {
                CachedPlayer.LocalPlayer.PlayerControl.NetTransform.RpcSnapTo
                    (MapData.FindVentSpawnPositions()[rnd.Next(MapData.FindVentSpawnPositions().Count)]);
            }
            else
            {
                var SpawnPositions =
                    GameOptionsManager.Instance.currentNormalGameOptions.MapId switch
                    {
                        0 => MapData.SkeldSpawnPosition,
                        1 => MapData.MiraSpawnPosition,
                        2 => MapData.PolusSpawnPosition,
                        3 => MapData.DleksSpawnPosition,
                        4 => MapData.AirshipSpawnPosition,
                        5 => MapData.FungleSpawnPosition,
                        _ => MapData.FindVentSpawnPositions()
                    };
                CachedPlayer.LocalPlayer.PlayerControl.NetTransform.RpcSnapTo
                    (SpawnPositions[rnd.Next(SpawnPositions.Count)]);
            }
        }

        // First kill
        if (AmongUsClient.Instance.AmHost && MapOption.shieldFirstKill && MapOption.firstKillName != "" &&
            !HideNSeek.isHideNSeekGM && !PropHunt.isPropHuntGM)
        {
            var target = PlayerControl.AllPlayerControls.ToArray().ToList()
                .FirstOrDefault(x => x.Data.PlayerName.Equals(MapOption.firstKillName));
            if (target != null)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.SetFirstKill, SendOption.Reliable);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.setFirstKill(target.PlayerId);
            }
        }

        MapOption.firstKillName = "";

        if (HideNSeek.isHideNSeekGM)
        {
            foreach (PlayerControl player in HideNSeek.getHunters())
            {
                player.moveable = false;
                player.NetTransform.Halt();
                HideNSeek.timer = HideNSeek.hunterWaitingTime;
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(HideNSeek.hunterWaitingTime,
                    new Action<float>(p =>
                    {
                        if (p == 1f)
                        {
                            player.moveable = true;
                            HideNSeek.timer = CustomOptionHolder.hideNSeekTimer.getFloat() * 60;
                            HideNSeek.isWaitingTimer = false;
                        }
                    })));
                player.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
            }

            if (HideNSeek.polusVent == null && GameOptionsManager.Instance.currentNormalGameOptions.MapId == 2)
            {
                var list = Object.FindObjectsOfType<Vent>().ToList();
                var adminVent = list.FirstOrDefault(x => x.gameObject.name == "AdminVent");
                var bathroomVent = list.FirstOrDefault(x => x.gameObject.name == "BathroomVent");
                HideNSeek.polusVent = Object.Instantiate(adminVent);
                HideNSeek.polusVent.gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                HideNSeek.polusVent.transform.position = new Vector3(36.55068f, -21.5168f, -0.0215168f);
                HideNSeek.polusVent.Left = adminVent;
                HideNSeek.polusVent.Right = bathroomVent;
                HideNSeek.polusVent.Center = null;
                HideNSeek.polusVent.Id =
                    MapUtilities.CachedShipStatus.AllVents.Select(x => x.Id).Max() + 1; // Make sure we have a unique id
                var allVentsList = MapUtilities.CachedShipStatus.AllVents.ToList();
                allVentsList.Add(HideNSeek.polusVent);
                MapUtilities.CachedShipStatus.AllVents = allVentsList.ToArray();
                HideNSeek.polusVent.gameObject.SetActive(true);
                HideNSeek.polusVent.name = "newVent_" + HideNSeek.polusVent.Id;

                adminVent.Center = HideNSeek.polusVent;
                bathroomVent.Center = HideNSeek.polusVent;
            }

            ShipStatusPatch.originalNumCrewVisionOption =
                GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
            ShipStatusPatch.originalNumImpVisionOption =
                GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
            ShipStatusPatch.originalNumKillCooldownOption =
                GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;

            GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod =
                CustomOptionHolder.hideNSeekHunterVision.getFloat();
            GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod =
                CustomOptionHolder.hideNSeekHuntedVision.getFloat();
            GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown =
                CustomOptionHolder.hideNSeekKillCooldown.getFloat();
        }
    }
}

[HarmonyPatch]
internal class IntroPatch
{
    public static void setupIntroTeamIcons(IntroCutscene __instance, ref List<PlayerControl> yourTeam)
    {
        // Intro solo teams
        if (isNeutral(CachedPlayer.LocalPlayer.PlayerControl))
        {
            var soloTeam = new List<PlayerControl>();
            soloTeam.Add(CachedPlayer.LocalPlayer.PlayerControl);
            yourTeam = soloTeam;
        }

        // Add the Spy to the Impostor team (for the Impostors)
        if (Spy.spy != null && CachedPlayer.LocalPlayer.Data.Role.IsImpostor)
        {
            var players = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
            var fakeImpostorTeam =
                new List<PlayerControl>(); // The local player always has to be the first one in the list (to be displayed in the center)
            fakeImpostorTeam.Add(CachedPlayer.LocalPlayer.PlayerControl);
            foreach (var p in players)
                if (CachedPlayer.LocalPlayer.PlayerControl != p && (p == Spy.spy || p.Data.Role.IsImpostor))
                    fakeImpostorTeam.Add(p);
            yourTeam = fakeImpostorTeam;
        }
    }

    public static void setupIntroTeam(IntroCutscene __instance, ref List<PlayerControl> yourTeam)
    {
        var infos = RoleInfo.getRoleInfoForPlayer(CachedPlayer.LocalPlayer.PlayerControl);
        var roleInfo = infos.Where(info => !info.isModifier).FirstOrDefault();
        if (roleInfo == null) return;
        if (roleInfo.isNeutral)
        {
            var neutralColor = new Color32(76, 84, 78, 255);
            __instance.BackgroundBar.material.color = roleInfo.color;
            __instance.TeamTitle.text = "中立阵营";
            __instance.TeamTitle.color = neutralColor;
        }
        else
        {
            var isCrew = true;
            if (roleInfo.color == Palette.ImpostorRed) isCrew = false;
            if (isCrew)
            {
                __instance.BackgroundBar.material.color = roleInfo.color;
                __instance.TeamTitle.text = "船员阵营";
                __instance.TeamTitle.color = Color.cyan;
            }
            else
            {
                __instance.BackgroundBar.material.color = roleInfo.color;
                __instance.TeamTitle.text = "伪装者阵营";
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
        private static int seed;

        public static void SetRoleTexts(IntroCutscene __instance)
        {
            // Don't override the intro of the vanilla roles
            var infos = RoleInfo.getRoleInfoForPlayer(CachedPlayer.LocalPlayer.PlayerControl);
            var roleInfo = infos.Where(info => !info.isModifier).FirstOrDefault();
            var modifierInfo = infos.Where(info => info.isModifier).FirstOrDefault();

            __instance.RoleBlurbText.text = "";
            if (roleInfo != null)
            {
                __instance.RoleText.text = roleInfo.name;
                __instance.RoleText.color = roleInfo.color;
                __instance.RoleBlurbText.text = roleInfo.introDescription;
                __instance.RoleBlurbText.color = roleInfo.color;
            }

            if (modifierInfo != null)
            {
                if (modifierInfo.roleId != RoleId.Lover)
                {
                    __instance.RoleBlurbText.text +=
                        cs(modifierInfo.color, $"\n{modifierInfo.introDescription}");
                }
                else
                {
                    var otherLover = CachedPlayer.LocalPlayer.PlayerControl == Lovers.lover1
                        ? Lovers.lover2
                        : Lovers.lover1;
                    __instance.RoleBlurbText.text += cs(Lovers.color,
                        $"\n♥ 你和 {otherLover?.Data?.PlayerName ?? ""} 坠入了爱河♥");
                }
            }

            if (Deputy.knowsSheriff && Deputy.deputy != null && Sheriff.sheriff != null)
            {
                if (infos.Any(info => info.roleId == RoleId.Sheriff))
                    __instance.RoleBlurbText.text +=
                        cs(Sheriff.color, $"\n你的捕快是 {Deputy.deputy?.Data?.PlayerName ?? ""}");
                else if (infos.Any(info => info.roleId == RoleId.Deputy))
                    __instance.RoleBlurbText.text += cs(Sheriff.color,
                        $"\n你的警长是 {Sheriff.sheriff?.Data?.PlayerName ?? ""}");
            }
        }

        public static bool Prefix(IntroCutscene __instance)
        {
            seed = rnd.Next(5000);
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

