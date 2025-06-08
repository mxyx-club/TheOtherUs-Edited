namespace TheOtherRoles.Objects;

public class KillTrap
{
    public static List<KillTrap> AllTraps = new();

    public static Sprite trapSprite = new ResourceSprite("Trap.png", 300);
    public static Sprite trapActiveSprite = new ResourceSprite("TrapActive.png", 300);
    public static AudioClip place;
    public static AudioClip activate;
    public static AudioClip disable;
    public static AudioClip countdown;
    public static AudioClip kill;
    public static AudioRolloffMode rollOffMode = AudioRolloffMode.Linear;
    public GameObject killtrap;
    public AudioSource audioSource;
    public bool isActive;
    public bool isDisabled;
    public PlayerControl trapper;
    public PlayerControl target;
    public DateTime placedTime;

    private static int maxId;
    private int Id;

    public KillTrap(PlayerControl trapper, Vector3 pos)
    {
        var traps = AllTraps.Where(x => x.trapper == trapper);
        // 最初の罠を消す
        if (traps.Count() >= EvilTrapper.numTrap)
        {
            var oldestTrap = traps.OrderBy(t => t.Id).FirstOrDefault();
            oldestTrap?.Destroy();
        }

        // 罠を設置
        killtrap = new GameObject("Trap");
        var trapRenderer = killtrap.AddComponent<SpriteRenderer>();
        killtrap.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        trapRenderer.sprite = trapSprite;
        Vector3 position = new(pos.x, pos.y, (pos.y / 1000) + 0.001f);
        killtrap.transform.position = position;
        // this.trap.transform.localPosition = pos;
        killtrap.SetActive(true);
        isDisabled = false;

        // 音を鳴らす
        audioSource = killtrap.gameObject.AddComponent<AudioSource>();
        audioSource.priority = 0;
        audioSource.spatialBlend = 1;
        audioSource.volume = 0.75f;
        audioSource.clip = place;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.maxDistance = 2 * EvilTrapper.maxDistance / 3;
        audioSource.minDistance = EvilTrapper.minDistance;
        audioSource.rolloffMode = rollOffMode;
        audioSource.PlayOneShot(place);
        this.trapper = trapper;
        Id = ++maxId;
        // 設置時刻を設定
        placedTime = DateTime.UtcNow;

        AllTraps.Add(this);
        Message($"创建陷阱 {Id}");
    }

    public void Destroy()
    {
        Message($"销毁陷阱 {Id}");
        audioSource?.Stop();
        if (killtrap != null) UObject.Destroy(killtrap);
        AllTraps.Remove(this);

    }

    public static void ClearAllTraps()
    {
        Message("清除所有陷阱");
        foreach (var t in AllTraps.ToArray())
        {
            t?.Destroy();
        }
        AllTraps = new();
        maxId = 0;
    }

    public static void ClearAllTraps(PlayerControl trapper)
    {
        Message($"清除玩家 {trapper?.Data?.PlayerName ?? "NULL"} 的所有陷阱");
        foreach (var t in AllTraps.Where(x => x.trapper == trapper))
        {
            t?.Destroy();
        }
        AllTraps.RemoveAll(t => t.trapper == trapper);
    }

    public static void activateTrap(PlayerControl trapper, PlayerControl target, int trapId)
    {
        Message($"激活陷阱 {trapId}，目标: {target.Data.PlayerName}");
        var trap = AllTraps.FirstOrDefault(x => x.Id == trapId);
        if (trap == null || trap.isDisabled) return;
        // 有効にする

        trap.isActive = true;
        trap.target = target;
        var spriteRenderer = trap.killtrap.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = trapActiveSprite;

        ClearAllTraps(trapper);

        if (PlayerControl.LocalPlayer == trapper)
        {
            Message($"玩家 {target?.Data?.PlayerName ?? "NULL"} 触发陷阱");
            TMP_Text text;
            RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
            GameObject gameObject = UObject.Instantiate(roomTracker.gameObject);
            UObject.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
            gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
            gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
            gameObject.transform.localScale = Vector3.one * 1.5f;
            text = gameObject.GetComponent<TMP_Text>();
            text.text = string.Format(GetString("trapperGotTrapText"), target?.Data?.PlayerName ?? "NULL");
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(3f, new Action<float>((p) =>
            {
                if (p == 1f && text != null && text.gameObject != null)
                {
                    UObject.Destroy(text.gameObject);
                }
            })));
        }

        // 音を鳴らす
        trap.audioSource.Stop();
        trap.audioSource.loop = true;
        trap.audioSource.priority = 0;
        trap.audioSource.spatialBlend = 1;
        trap.audioSource.maxDistance = EvilTrapper.maxDistance;
        trap.audioSource.clip = countdown;
        trap.audioSource.Play();

        // ターゲットを動けなくする
        target.NetTransform.Halt();

        var moveableFlag = false;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(EvilTrapper.killTimer, new Action<float>((p) =>
        {
            try
            {
                if (InMeeting) return;
                if (trap == null || trap.killtrap == null || !trap.isActive) //　解除された場合の処理
                {
                    if (!moveableFlag)
                    {
                        target.moveable = true;
                        moveableFlag = true;
                    }
                    return;
                }
                else if ((p == 1f) && !target.Data.IsDead)
                {
                    // 正常にキルが発生する場合の処理
                    target.moveable = true;
                    if (PlayerControl.LocalPlayer == trap.trapper)
                    {
                        var writer = StartRPC(CustomRPC.TrapperKill);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(target.PlayerId);
                        writer.Write(trapId);
                        writer.EndRPC();
                        trapKill(PlayerControl.LocalPlayer, target, trapId);
                    }
                }
                else
                { // カウントダウン中の処理
                    target.moveable = false;
                    target.NetTransform.Halt();
                    target.transform.position = trap.killtrap.transform.position + new Vector3(0, 0.3f, 0);
                }
            }
            catch (Exception e)
            {
                Error("An error occured during the countdown");
                Error(e.Message);
            }
        })));
    }

    public static void disableTrap(int trapId)
    {
        Message($"解除陷阱 {trapId}");
        var trap = AllTraps.FirstOrDefault(x => x.Id == trapId);
        trap.isActive = false;
        trap.isDisabled = true;
        trap.audioSource.Stop();
        trap.audioSource.PlayOneShot(disable);
        _ = new LateTask(trap.Destroy, disable.length, "Destroy KillTrap");
    }

    public static void UpdateTrap()
    {
        foreach (var t in AllTraps)
        {
            t.Update();
            bool canSee = t.isActive || PlayerControl.LocalPlayer.IsImpostor() || CanSeeRoleInfo;
            var opacity = canSee ? 1.0f : 0.0f;

            if (t.killtrap != null)
                t.killtrap.GetComponent<SpriteRenderer>().material.color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
        }
    }

    public void Update()
    {
        if (killtrap != null && isActive && !InMeeting)
        {
            Vector3 p1 = killtrap.transform.position;
            Vector3 p2 = PlayerControl.LocalPlayer.transform.position;
            float distance = Vector3.Distance(p1, p2);
            if (PlayerControl.LocalPlayer != target && PlayerControl.LocalPlayer.IsAlive() && distance < 0.5)
            {
                var writer = StartRPC(CustomRPC.DisableTrap);
                writer.Write(Id);
                writer.EndRPC();
                disableTrap(Id);
            }
        }

        if (!hasTrappedPlayer() && !InMeeting)
        {
            if (DateTime.UtcNow.Subtract(placedTime).TotalSeconds < EvilTrapper.extensionTime) return;
            if (isActive || PlayerControl.LocalPlayer.IsDead() || PlayerControl.LocalPlayer.inVent || isDisabled || InMeeting) return;
            var p1 = PlayerControl.LocalPlayer.transform.localPosition;
            Dictionary<GameObject, byte> listActivate = new();
            var p2 = killtrap.transform.localPosition;
            var distance = Vector3.Distance(p1, p2);
            if (distance < EvilTrapper.trapRange)
            {
                target = PlayerControl.LocalPlayer;
                if (PlayerControl.LocalPlayer.AmOwner)
                {
                    Message("本地玩家触发陷阱");
                    var writer = StartRPC(CustomRPC.ActivateTrap);
                    writer.Write(trapper.PlayerId);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(Id);
                    writer.EndRPC();
                    RPCProcedure.activateTrap(trapper.PlayerId, PlayerControl.LocalPlayer.PlayerId, Id);
                }
            }
        }
    }

    public static void OnMeetingStart()
    {
        Message("会议开始，停止所有陷阱");
        foreach (var trap in AllTraps)
        {
            trap.audioSource.PlayOneShot(kill);
            if (trap.target.IsAlive() && PlayerControl.LocalPlayer == trap.target)
            {
                checkMurderAttemptAndKill(trap.trapper, trap.target, false);
            }
            _ = new LateTask(() =>
            {
                trap.audioSource.Stop();
                trap.Destroy();
            }, kill.length);
        }
    }

    public static bool hasTrappedPlayer()
    {
        foreach (var trap in AllTraps)
        {
            if (trap.target != null) return true;
        }
        return false;
    }

    public static bool isTrapped(PlayerControl p)
    {
        foreach (var trap in AllTraps)
        {
            if (trap.target == p) return true;
        }
        return false;
    }

    public static void trapKill(PlayerControl trapper, PlayerControl target, int trapId)
    {
        Message($"陷阱 {trapId} 击杀 {target?.Data?.PlayerName ?? "NULL"}");
        var trap = AllTraps.FirstOrDefault(x => x.Id == trapId);
        var audioSource = trap.audioSource;

        audioSource.Stop();
        audioSource.maxDistance = EvilTrapper.maxDistance;
        audioSource.PlayOneShot(kill);
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(kill.length, new Action<float>((p) =>
        {
            if (p == 1f)
            {
                Message("Destroy On trapKill");
                trap?.Destroy();
            }
        })));
        if (PlayerControl.LocalPlayer == trapper) checkMurderAttemptAndKill(trapper, target, false);

        EvilTrapper.isTrapKill = true;
    }


    private static readonly Assembly dll = Assembly.GetExecutingAssembly();
    public static void LoadAudioAssets()
    {
        var resourceAudioAssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.audiobundle");
        var assetBundleBundle = AssetBundle.LoadFromMemory(resourceAudioAssetBundleStream.ReadFully());
        activate = assetBundleBundle.LoadAsset<AudioClip>("TrapperActivate.mp3").DontUnload();
        countdown = assetBundleBundle.LoadAsset<AudioClip>("TrapperCountdown.mp3").DontUnload();
        disable = assetBundleBundle.LoadAsset<AudioClip>("TrapperDisable.mp3").DontUnload();
        kill = assetBundleBundle.LoadAsset<AudioClip>("TrapperKill.mp3").DontUnload();
        place = assetBundleBundle.LoadAsset<AudioClip>("TrapperPlace.mp3").DontUnload();
    }
}