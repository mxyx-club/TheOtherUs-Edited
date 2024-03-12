using System;
using System.Linq;
using Hazel;
using Il2CppSystem.Collections.Generic;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Helper;
using TheOtherRoles.Objects;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using Object = UnityEngine.Object;
using Random = System.Random;

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
                CachedPlayer.LocalPlayer.PlayerControl.SetKillTimer(ResetButtonCooldown.killCooldown);
                //PlayerControl.SetPetImage(data.DefaultOutfit.PetId, data.DefaultOutfit.ColorId, player.PetSlot);
                player.cosmetics.nameText.text = data.PlayerName;
                player.SetFlipX(true);
                TORMapOptions.playerIcons[p.PlayerId] = player;
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
                        player.cosmetics.nameText.text += $"{Helpers.cs(Color.red, " (Hunter)")}";
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

        // Polus管道追加
        AdditionalVents.AddAdditionalVents();

        // Add Electrical
        FungleAdditionalElectrical.CreateElectrical();

        // Force Reload of SoundEffectHolder
        SoundEffectsManager.Load();


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
            //Random spawn on game start

            var skeldSpawn = new System.Collections.Generic.List<Vector3>
            {
                new(-2.2f, 2.2f, 0.0f), //cafeteria. botton. top left.
                new(0.7f, 2.2f, 0.0f), //caffeteria. button. top right.
                new(-2.2f, -0.2f, 0.0f), //caffeteria. button. bottom left.
                new(0.7f, -0.2f, 0.0f), //caffeteria. button. bottom right.
                new(10.0f, 3.0f, 0.0f), //weapons top
                new(9.0f, 1.0f, 0.0f), //weapons bottom
                new(6.5f, -3.5f, 0.0f), //O2
                new(11.5f, -3.5f, 0.0f), //O2-nav hall
                new(17.0f, -3.5f, 0.0f), //navigation top
                new(18.2f, -5.7f, 0.0f), //navigation bottom
                new(11.5f, -6.5f, 0.0f), //nav-shields top
                new(9.5f, -8.5f, 0.0f), //nav-shields bottom
                new(9.2f, -12.2f, 0.0f), //shields top
                new(8.0f, -14.3f, 0.0f), //shields bottom
                new(2.5f, -16f, 0.0f), //coms left
                new(4.2f, -16.4f, 0.0f), //coms middle
                new(5.5f, -16f, 0.0f), //coms right
                new(-1.5f, -10.0f, 0.0f), //storage top
                new(-1.5f, -15.5f, 0.0f), //storage bottom
                new(-4.5f, -12.5f, 0.0f), //storrage left
                new(0.3f, -12.5f, 0.0f), //storrage right
                new(4.5f, -7.5f, 0.0f), //admin top
                new(4.5f, -9.5f, 0.0f), //admin bottom
                new(-9.0f, -8.0f, 0.0f), //elec top left
                new(-6.0f, -8.0f, 0.0f), //elec top right
                new(-8.0f, -11.0f, 0.0f), //elec bottom
                new(-12.0f, -13.0f, 0.0f), //elec-lower hall
                new(-17f, -10f, 0.0f), //lower engine top
                new(-17.0f, -13.0f, 0.0f), //lower engine bottom
                new(-21.5f, -3.0f, 0.0f), //reactor top
                new(-21.5f, -8.0f, 0.0f), //reactor bottom
                new(-13.0f, -3.0f, 0.0f), //security top
                new(-12.6f, -5.6f, 0.0f), // security bottom
                new(-17.0f, 2.5f, 0.0f), //upper engibe top
                new(-17.0f, -1.0f, 0.0f), //upper engine bottom
                new(-10.5f, 1.0f, 0.0f), //upper-mad hall
                new(-10.5f, -2.0f, 0.0f), //medbay top
                new(-6.5f, -4.5f, 0.0f) //medbay bottom
            };

            var miraSpawn = new System.Collections.Generic.List<Vector3>
            {
                new(-4.5f, 3.5f, 0.0f), //launchpad top
                new(-4.5f, -1.4f, 0.0f), //launchpad bottom
                new(8.5f, -1f, 0.0f), //launchpad- med hall
                new(14f, -1.5f, 0.0f), //medbay
                new(16.5f, 3f, 0.0f), // comms
                new(10f, 5f, 0.0f), //lockers
                new(6f, 1.5f, 0.0f), //locker room
                new(2.5f, 13.6f, 0.0f), //reactor
                new(6f, 12f, 0.0f), //reactor middle
                new(9.5f, 13f, 0.0f), //lab
                new(15f, 9f, 0.0f), //bottom left cross
                new(17.9f, 11.5f, 0.0f), //middle cross
                new(14f, 17.3f, 0.0f), //office
                new(19.5f, 21f, 0.0f), //admin
                new(14f, 24f, 0.0f), //greenhouse left
                new(22f, 24f, 0.0f), //greenhouse right
                new(21f, 8.5f, 0.0f), //bottom right cross
                new(28f, 3f, 0.0f), //caf right
                new(22f, 3f, 0.0f), //caf left
                new(19f, 4f, 0.0f), //storage
                new(22f, -2f, 0.0f) //balcony
            };

            var polusSpawn = new System.Collections.Generic.List<Vector3>
            {
                new(16.6f, -1f, 0.0f), //dropship top
                new(16.6f, -5f, 0.0f), //dropship bottom
                new(20f, -9f, 0.0f), //above storrage
                new(22f, -7f, 0.0f), //right fuel
                new(25.5f, -6.9f, 0.0f), //drill
                new(29f, -9.5f, 0.0f), //lab lockers
                new(29.5f, -8f, 0.0f), //lab weather notes
                new(35f, -7.6f, 0.0f), //lab table
                new(40.4f, -8f, 0.0f), //lab scan
                new(33f, -10f, 0.0f), //lab toilet
                new(39f, -15f, 0.0f), //specimen hall top
                new(36.5f, -19.5f, 0.0f), //specimen top
                new(36.5f, -21f, 0.0f), //specimen bottom
                new(28f, -21f, 0.0f), //specimen hall bottom
                new(24f, -20.5f, 0.0f), //admin tv
                new(22f, -25f, 0.0f), //admin books
                new(16.6f, -17.5f, 0.0f), //office coffe
                new(22.5f, -16.5f, 0.0f), //office projector
                new(24f, -17f, 0.0f), //office figure
                new(27f, -16.5f, 0.0f), //office lifelines
                new(32.7f, -15.7f, 0.0f), //lavapool
                new(31.5f, -12f, 0.0f), //snowmad below lab
                new(10f, -14f, 0.0f), //below storrage
                new(21.5f, -12.5f, 0.0f), //storrage vent
                new(19f, -11f, 0.0f), //storrage toolrack
                new(12f, -7.2f, 0.0f), //left fuel
                new(5f, -7.5f, 0.0f), //above elec
                new(10f, -12f, 0.0f), //elec fence
                new(9f, -9f, 0.0f), //elec lockers
                new(5f, -9f, 0.0f), //elec window
                new(4f, -11.2f, 0.0f), //elec tapes
                new(5.5f, -16f, 0.0f), //elec-O2 hall
                new(1f, -17.5f, 0.0f), //O2 tree hayball
                new(3f, -21f, 0.0f), //O2 middle
                new(2f, -19f, 0.0f), //O2 gas
                new(1f, -24f, 0.0f), //O2 water
                new(7f, -24f, 0.0f), //under O2
                new(9f, -20f, 0.0f), //right outside of O2
                new(7f, -15.8f, 0.0f), //snowman under elec
                new(11f, -17f, 0.0f), //comms table
                new(12.7f, -15.5f, 0.0f), //coms antenna pult
                new(13f, -24.5f, 0.0f), //weapons window
                new(15f, -17f, 0.0f), //between coms-office
                new(17.5f, -25.7f, 0.0f) //snowman under office
            };

            var dleksSpawn = new System.Collections.Generic.List<Vector3>
            {
                new(2.2f, 2.2f, 0.0f), //cafeteria. botton. top left.
                new(-0.7f, 2.2f, 0.0f), //caffeteria. button. top right.
                new(2.2f, -0.2f, 0.0f), //caffeteria. button. bottom left.
                new(-0.7f, -0.2f, 0.0f), //caffeteria. button. bottom right.
                new(-10.0f, 3.0f, 0.0f), //weapons top
                new(-9.0f, 1.0f, 0.0f), //weapons bottom
                new(-6.5f, -3.5f, 0.0f), //O2
                new(-11.5f, -3.5f, 0.0f), //O2-nav hall
                new(-17.0f, -3.5f, 0.0f), //navigation top
                new(-18.2f, -5.7f, 0.0f), //navigation bottom
                new(-11.5f, -6.5f, 0.0f), //nav-shields top
                new(-9.5f, -8.5f, 0.0f), //nav-shields bottom
                new(-9.2f, -12.2f, 0.0f), //shields top
                new(-8.0f, -14.3f, 0.0f), //shields bottom
                new(-2.5f, -16f, 0.0f), //coms left
                new(-4.2f, -16.4f, 0.0f), //coms middle
                new(-5.5f, -16f, 0.0f), //coms right
                new(1.5f, -10.0f, 0.0f), //storage top
                new(1.5f, -15.5f, 0.0f), //storage bottom
                new(4.5f, -12.5f, 0.0f), //storrage left
                new(-0.3f, -12.5f, 0.0f), //storrage right
                new(-4.5f, -7.5f, 0.0f), //admin top
                new(-4.5f, -9.5f, 0.0f), //admin bottom
                new(9.0f, -8.0f, 0.0f), //elec top left
                new(6.0f, -8.0f, 0.0f), //elec top right
                new(8.0f, -11.0f, 0.0f), //elec bottom
                new(12.0f, -13.0f, 0.0f), //elec-lower hall
                new(17f, -10f, 0.0f), //lower engine top
                new(17.0f, -13.0f, 0.0f), //lower engine bottom
                new(21.5f, -3.0f, 0.0f), //reactor top
                new(21.5f, -8.0f, 0.0f), //reactor bottom
                new(13.0f, -3.0f, 0.0f), //security top
                new(12.6f, -5.6f, 0.0f), // security bottom
                new(17.0f, 2.5f, 0.0f), //upper engibe top
                new(17.0f, -1.0f, 0.0f), //upper engine bottom
                new(10.5f, 1.0f, 0.0f), //upper-mad hall
                new(10.5f, -2.0f, 0.0f), //medbay top
                new(6.5f, -4.5f, 0.0f) //medbay bottom
            };

            var fungleSpawn = new System.Collections.Generic.List<Vector3>
            {
                new(-10.0842f, 13.0026f, 0.013f),
                new(0.9815f, 6.7968f, 0.0068f),
                new(22.5621f, 3.2779f, 0.0033f),
                new(-1.8699f, -1.3406f, -0.0013f),
                new(12.0036f, 2.6763f, 0.0027f),
                new(21.705f, -7.8691f, -0.0079f),
                new(1.4485f, -1.6105f, -0.0016f),
                new(-4.0766f, -8.7178f, -0.0087f),
                new(2.9486f, 1.1347f, 0.0011f),
                new(-4.2181f, -8.6795f, -0.0087f),
                new(19.5553f, -12.5014f, -0.0125f),
                new(15.2497f, -16.5009f, -0.0165f),
                new(-22.7174f, -7.0523f, 0.0071f),
                new(-16.5819f, -2.1575f, 0.0022f),
                new(9.399f, -9.7127f, -0.0097f),
                new(7.3723f, 1.7373f, 0.0017f),
                new(22.0777f, -7.9315f, -0.0079f),
                new(-15.3916f, -9.3659f, -0.0094f),
                new(-16.1207f, -0.1746f, -0.0002f),
                new(-23.1353f, -7.2472f, -0.0072f),
                new(-20.0692f, -2.6245f, -0.0026f),
                new(-4.2181f, -8.6795f, -0.0087f),
                new(-9.9285f, 12.9848f, 0.013f),
                new(-8.3475f, 1.6215f, 0.0016f),
                new(-17.7614f, 6.9115f, 0.0069f),
                new(-0.5743f, -4.7235f, -0.0047f),
                new(-20.8897f, 2.7606f, 0.002f)
            };

            var airshipSpawn =
                new System.Collections.Generic.List<Vector3>(); //no spawns since it already has random spawns

            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 0)
                CachedPlayer.LocalPlayer.PlayerControl.transform.position = skeldSpawn[rnd.Next(skeldSpawn.Count)];
            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 1)
                CachedPlayer.LocalPlayer.PlayerControl.transform.position = miraSpawn[rnd.Next(miraSpawn.Count)];
            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 2)
                CachedPlayer.LocalPlayer.PlayerControl.transform.position = polusSpawn[rnd.Next(polusSpawn.Count)];
            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 3)
                CachedPlayer.LocalPlayer.PlayerControl.transform.position = dleksSpawn[rnd.Next(dleksSpawn.Count)];
            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 4)
                CachedPlayer.LocalPlayer.PlayerControl.transform.position = airshipSpawn[rnd.Next(airshipSpawn.Count)];
            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 5)
                CachedPlayer.LocalPlayer.PlayerControl.transform.position = fungleSpawn[rnd.Next(fungleSpawn.Count)];
        }

        // First kill
        if (AmongUsClient.Instance.AmHost && TORMapOptions.shieldFirstKill && TORMapOptions.firstKillName != "" &&
            !HideNSeek.isHideNSeekGM && !PropHunt.isPropHuntGM)
        {
            var target = PlayerControl.AllPlayerControls.ToArray().ToList()
                .FirstOrDefault(x => x.Data.PlayerName.Equals(TORMapOptions.firstKillName));
            if (target != null)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.SetFirstKill, SendOption.Reliable);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.setFirstKill(target.PlayerId);
            }
        }

        TORMapOptions.firstKillName = "";

        EventUtility.gameStartsUpdate();

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
        if (Helpers.isNeutral(CachedPlayer.LocalPlayer.PlayerControl))
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

            if (EventUtility.isEnabled)
            {
                var roleInfos = RoleInfo.allRoleInfos.Where(x => !x.isModifier).ToList();
                if (roleInfo.isNeutral) roleInfos.RemoveAll(x => !x.isNeutral);
                if (roleInfo.color == Palette.ImpostorRed) roleInfos.RemoveAll(x => x.color != Palette.ImpostorRed);
                if (!roleInfo.isNeutral && roleInfo.color != Palette.ImpostorRed)
                    roleInfos.RemoveAll(x => x.color == Palette.ImpostorRed || x.isNeutral);
                var rnd = new Random(seed);
                roleInfo = roleInfos[rnd.Next(roleInfos.Count)];
            }

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
                        Helpers.cs(modifierInfo.color, $"\n{modifierInfo.introDescription}");
                }
                else
                {
                    var otherLover = CachedPlayer.LocalPlayer.PlayerControl == Lovers.lover1
                        ? Lovers.lover2
                        : Lovers.lover1;
                    __instance.RoleBlurbText.text += Helpers.cs(Lovers.color,
                        $"\n♥ 你和 {otherLover?.Data?.PlayerName ?? ""} 坠入了爱河♥");
                }
            }

            if (Deputy.knowsSheriff && Deputy.deputy != null && Sheriff.sheriff != null)
            {
                if (infos.Any(info => info.roleId == RoleId.Sheriff))
                    __instance.RoleBlurbText.text +=
                        Helpers.cs(Sheriff.color, $"\n你的捕快是 {Deputy.deputy?.Data?.PlayerName ?? ""}");
                else if (infos.Any(info => info.roleId == RoleId.Deputy))
                    __instance.RoleBlurbText.text += Helpers.cs(Sheriff.color,
                        $"\n你的警长是 {Sheriff.sheriff?.Data?.PlayerName ?? ""}");
            }
        }

        public static bool Prefix(IntroCutscene __instance)
        {
            if (!CustomOptionHolder.activateRoles.getBool()) return true;
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

/* Horses are broken since 2024.3.5 - keeping this code in case they return.
 * [HarmonyPatch(typeof(AprilFoolsMode), nameof(AprilFoolsMode.ShouldHorseAround))]
public static class ShouldAlwaysHorseAround {
    public static bool Prefix(ref bool __result) {
        __result = EventUtility.isEnabled && !EventUtility.disableEventMode;
        return false;
    }
}*/

[HarmonyPatch(typeof(AprilFoolsMode), nameof(AprilFoolsMode.ShouldShowAprilFoolsToggle))]
public static class ShouldShowAprilFoolsToggle
{
    public static void Postfix(ref bool __result)
    {
        __result = __result || EventUtility.isEventDate || EventUtility.canBeEnabled;  // Extend it to a 7 day window instead of just 1st day of the Month
    }
}