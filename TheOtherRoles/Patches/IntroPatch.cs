using TheOtherRoles.Objects.Map;

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
                var player = UObject.Instantiate(__instance.PlayerPrefab,
                    FastDestroyableSingleton<HudManager>.Instance.transform);
                playerPrefab = __instance.PlayerPrefab;
                p.SetPlayerMaterialColors(player.cosmetics.currentBodySprite.BodySprite);
                player.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
                player.cosmetics.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
                player.cosmetics.nameText.text = data.PlayerName;
                player.SetFlipX(true);
                ModOption.playerIcons[p.PlayerId] = player;
                player.gameObject.SetActive(false);

                if (PlayerControl.LocalPlayer.Is(RoleId.Arsonist) && p != PlayerControl.LocalPlayer)
                {
                    player.transform.localPosition = bottomLeft + new Vector3(-0.25f, -0.25f, 0) +
                                                     (Vector3.right * playerCounter++ * 0.35f);
                    player.transform.localScale = Vector3.one * 0.2f;
                    player.setSemiTransparent(true);
                    player.gameObject.SetActive(true);
                }
                else
                {
                    // This can be done for all players not just for the bounty hunter as it was before. Allows the thief to have the correct position and scaling
                    player.transform.localPosition = bottomLeft;
                    player.transform.localScale = Vector3.one * 0.4f;
                    player.gameObject.SetActive(false);
                }
            }
        }

        CustomRoleManager.OnGameStart();

        // 管道追加
        AdditionalVents.AddAdditionalVents();

        // Add Electrical
        FungleAdditionalElectrical.CreateElectrical();

        if (CustomOptionHolder.randomGameStartPosition.GetBool()) MapData.RandomSpawnPlayers();

        if (AmongUsClient.Instance.AmHost)
        {
            var mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
            var writerS = StartRPC(CustomRPC.DynamicMapOption);
            writerS.Write(mapId);
            writerS.EndRPC();

            // First kill
            if (ModOption.shieldFirstKill && ModOption.firstKillName != "")
            {
                var target = PlayerControl.AllPlayerControls.ToList().FirstOrDefault(x => x.Data.PlayerName.Equals(ModOption.firstKillName));
                if (target != null)
                {
                    var writer = StartRPC(CustomRPC.SetFirstKill);
                    writer.Write(target.PlayerId);
                    writer.EndRPC();
                    RPCProcedure.setFirstKill(target.PlayerId);
                }
            }
        }

        foreach (var role in CustomRoleManager.AllRoles.ToArray())
        {
            try
            {
                role.Initialize();

            }
            catch (Exception e)
            {
                Message($"初始化角色 {role.RoleName} 时出错: {e.Message}\n{e.StackTrace}");
            }
        }

        //游戏开始时重置cd
        CustomButton.ResetAllCooldowns(ModOption.ButtonCooldown);

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
    public static void setupIntroTeamIcons(IntroCutscene __instance, ref ISystem.List<PlayerControl> yourTeam)
    {
        // Intro solo teams
        if (PlayerControl.LocalPlayer.IsNeutral())
        {
            var soloTeam = new ISystem.List<PlayerControl>();
            soloTeam.Add(PlayerControl.LocalPlayer);
            yourTeam = soloTeam;
        }

        // Add the Spy to the Impostor team (for the Impostors)
        if (PlayerControl.LocalPlayer.IsImpostor() && GetRoles(RoleId.Spy).Any())
        {
            var players = PlayerControl.AllPlayerControls.ToArray().OrderBy(x => Guid.NewGuid()).ToList();
            // The local player always has to be the first one in the list (to be displayed in the center)
            var fakeImpostorTeam = new ISystem.List<PlayerControl>();
            fakeImpostorTeam.Add(PlayerControl.LocalPlayer);
            foreach (var p in players)
                if (PlayerControl.LocalPlayer != p && p.IsImpostor(AndSpy: true))
                    fakeImpostorTeam.Add(p);
            yourTeam = fakeImpostorTeam;
        }

        // Role draft: If spy is enabled, don't show the team
        /*if (CustomOptionHolder.spySpawnRate.GetSelection() > 0 && PlayerControl.AllPlayerControls.ToArray().Where(x => x.Data.Role.IsImpostor).Count() > 1)
        {
            // The local player always has to be the first one in the list (to be displayed in the center)
            var fakeImpostorTeam = new List<PlayerControl>();
            fakeImpostorTeam.Add(PlayerControl.LocalPlayer);
            yourTeam = fakeImpostorTeam;
        }*/
    }

    public static void setupIntroTeam(IntroCutscene __instance, ref ISystem.List<PlayerControl> yourTeam)
    {
        var infos = PlayerControl.LocalPlayer.GetRoleInfo();
        var neutralColor = new Color32(76, 84, 78, 255);
        if (infos == null) return;
        if (RoleDraft.isEnabled && !PlayerControl.LocalPlayer.IsImpostor())
        {
            __instance.BackgroundBar.material.color = neutralColor;
            __instance.TeamTitle.text = "UnknownTeam".Translate();
            __instance.TeamTitle.color = Palette.CrewmateBlue;
            return;
        }
        if (infos.RoleType == RoleType.Neutral)
        {
            __instance.BackgroundBar.material.color = infos.Color;
            __instance.TeamTitle.text = "NeutralTeam".Translate();
            __instance.TeamTitle.color = neutralColor;
        }
        else
        {
            var isCrew = true;
            if (infos.Color == Palette.ImpostorRed) isCrew = false;
            if (isCrew)
            {
                __instance.BackgroundBar.material.color = infos.Color;
                __instance.TeamTitle.text = "CrewmateTeam".Translate();
                __instance.TeamTitle.color = Palette.CrewmateBlue;
            }
            else
            {
                __instance.BackgroundBar.material.color = infos.Color;
                __instance.TeamTitle.text = "ImpostorTeam".Translate();
                __instance.TeamTitle.color = Palette.ImpostorRed;
            }
        }
    }

    public static IEnumerator<WaitForSeconds> EndShowRole(IntroCutscene __instance)
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
            var info = PlayerControl.LocalPlayer.GetRoleInfo();
            var abilityInfo = PlayerControl.LocalPlayer.GetModifierBase().Select(x => x.RoleInfo);

            __instance.RoleBlurbText.text = "";
            if (info != null)
            {
                __instance.RoleText.text = info.Name;
                __instance.RoleText.color = info.Color;
                __instance.RoleBlurbText.text = info.IntroDescription;
                __instance.RoleBlurbText.color = info.Color;
            }

            var role = PlayerControl.LocalPlayer.GetRoleBase();

            if (role is Sheriff sheriff && sheriff.MyDeputy != null)
            {
                __instance.RoleBlurbText.text = Cs(Sheriff.color,
                        string.Format(GetString("IntroShowRole.Sheriff"), sheriff.MyDeputy?.Data?.PlayerName ?? "NULL"));
            }
            else if (role is Deputy deputy && deputy.MySheriff != null)
            {
                __instance.RoleBlurbText.text = Cs(Sheriff.color,
                        string.Format(GetString("IntroShowRole.Deputy"), deputy.Player?.Data?.PlayerName ?? "NULL"));
            }
            else if (role is Executioner executioner && executioner.Target != null)
            {
                __instance.RoleBlurbText.text = Cs(Executioner.color,
                    string.Format(GetString("IntroShowRole.Executioner"), executioner.Target?.Data?.PlayerName ?? "NULL"));
            }
            else if (role is Lawyer lawyer && lawyer.Target != null)
            {
                __instance.RoleBlurbText.text = Cs(Lawyer.color,
                    string.Format(GetString("IntroShowRole.Lawyer"), lawyer.Target?.Data?.PlayerName ?? "NULL"));
            }

            abilityInfo?.Do((x) =>
            {
                if (x.RoleId != RoleId.Lover)
                {
                    __instance.RoleBlurbText.text += Cs(x.Color, $"<size=80%>\n{x.IntroDescription}</size>");
                }
                else
                {
                    var otherLover = PlayerControl.LocalPlayer.OtherLover();
                    __instance.RoleBlurbText.text += Cs(Lovers.color, $"<size=80%>\n♥ 你和 {otherLover?.Data?.PlayerName ?? ""} 坠入了爱河 ♥</size>");
                }
            });
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
        public static void Prefix(IntroCutscene __instance, ref ISystem.List<PlayerControl> teamToDisplay)
        {
            setupIntroTeamIcons(__instance, ref teamToDisplay);
        }

        public static void Postfix(IntroCutscene __instance, ref ISystem.List<PlayerControl> teamToDisplay)
        {
            setupIntroTeam(__instance, ref teamToDisplay);
        }
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    private class BeginImpostorPatch
    {
        public static void Prefix(IntroCutscene __instance, ref ISystem.List<PlayerControl> yourTeam)
        {
            setupIntroTeamIcons(__instance, ref yourTeam);
        }

        public static void Postfix(IntroCutscene __instance, ref ISystem.List<PlayerControl> yourTeam)
        {
            setupIntroTeam(__instance, ref yourTeam);
        }
    }
}

