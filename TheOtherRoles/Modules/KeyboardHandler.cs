using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Buttons;
using TheOtherRoles.Patches;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Modules;

[HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
public class KeyboardHandler
{
    private static readonly List<PlayerControl> bots = new();

    private static void Postfix(KeyboardJoystick __instance)
    {
        if (AmongUsClient.Instance && (AmongUsClient.Instance.AmHost || ModOption.DebugMode))
        {
            // 生成假人
            /*if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.F) && Input.GetKeyDown(KeyCode.Return)
                && AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame && InGame)
            {
                var playerControl = UnityEngine.Object.Instantiate(AmongUsClient.Instance.PlayerPrefab);
                _ = playerControl.PlayerId = (byte)GameData.Instance.GetAvailableId();

                bots.Add(playerControl);
                GameData.Instance.AddPlayer(playerControl);
                AmongUsClient.Instance.Spawn(playerControl);

                playerControl.transform.position = CachedPlayer.LocalPlayer.transform.position;
                playerControl.GetComponent<DummyBehaviour>().enabled = true;
                playerControl.NetTransform.enabled = false;
                playerControl.SetName(RandomString(6));
                playerControl.SetColor((byte)random.Next(Palette.PlayerColors.Length));
                GameData.Instance.RpcSetTasks(playerControl.PlayerId, Array.Empty<byte>());
            }*/
            // 强制开始会议或结束会议
            if (Input.GetKey(ModInputManager.metaControlInput.keyCode) && Input.GetKeyDown(ModInputManager.meetingInput.keyCode) && InGame)
            {
                if (InMeeting) MeetingHud.Instance.RpcClose();
                else CachedPlayer.LocalPlayer.PlayerControl.NoCheckStartMeeting(null, true);
            }
            // 强制结束游戏
            if (Input.GetKey(ModInputManager.metaControlInput.keyCode) && Input.GetKeyDown(ModInputManager.endGameInput.keyCode) && InGame)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.Canceled, false);
            }
            if (Input.GetKey(ModInputManager.metaControlInput.keyCode) && Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.Return) && ModOption.DebugMode && InGame)
            {
                CustomButton.ResetAllCooldowns(0f);
            }
            // 快速开始游戏
            if (Input.GetKeyDown(KeyCode.LeftShift) && IsCountDown)
            {
                GameStartManager.Instance.countDownTimer = 0;
            }
        }

        if (Input.GetKeyDown(ModInputManager.helpInput.keyCode))
        {
            if (LobbyRoleInfo.RolesSummaryUI == null) LobbyRoleInfo.RoleSummaryOnClick();
            else
            {
                Object.Destroy(LobbyRoleInfo.RolesSummaryUI);
                LobbyRoleInfo.RolesSummaryUI = null;
            }
        }
    }
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[rnd.Next(s.Length)]).ToArray());
    }
}

[HarmonyPatch(typeof(ControllerManager), nameof(ControllerManager.Update))]
public class ControllerManagerUpdate
{
    private static int resolutionIndex;
    private static readonly (int, int)[] resolutions =
    [
        (640, 360),
        (960, 540),
        (1280, 720),
        (1600, 900),
        (1920, 1080),
        (Screen.currentResolution.width, Screen.currentResolution.height),
    ];

    private static void Postfix(ControllerManager __instance)
    {

        if (Input.GetKeyDown(ModInputManager.screenResolution.keyCode))
        {
            resolutionIndex++;
            if (resolutionIndex >= resolutions.Length) resolutionIndex = 0;
            bool fullScreen = resolutionIndex == resolutions.Length - 1;
            ResolutionManager.SetResolution(resolutions[resolutionIndex].Item1, resolutions[resolutionIndex].Item2, fullScreen);
        }
    }
}
