using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Impostor;

public static class Bomber
{
    public static PlayerControl Player;
    public static Color color = Palette.ImpostorRed;

    public static float cooldown = 30f;
    public static float bombDelay = 10f;
    public static float bombTimer = 10f;
    public static bool triggerBothCooldowns;
    public static bool canGiveToBomber;
    public static bool hotPotatoMode;

    public static PlayerControl currentTarget;
    public static PlayerControl currentBombTarget;

    public static Bomb ActiveBomb;
    public static Sprite buttonSprite = new ResourceSprite("Bomber2.png");

    public static void clearAndReload()
    {
        Player = null;
        currentTarget = null;
        currentBombTarget = null;
        ActiveBomb?.Destroy();
        ActiveBomb = null;
        cooldown = CustomOptionHolder.bomberBombCooldown.GetFloat();
        bombDelay = CustomOptionHolder.bomberDelay.GetFloat();
        bombTimer = CustomOptionHolder.bomberTimer.GetFloat();
        triggerBothCooldowns = CustomOptionHolder.bomberTriggerBothCooldowns.GetBool();
        canGiveToBomber = CustomOptionHolder.bomberCanGiveToBomber.GetBool();
        hotPotatoMode = CustomOptionHolder.bomberHotPotatoMode.GetBool();
    }

    public static void Update()
    {
        ActiveBomb?.Update();
    }

    public static void giveBomb(byte playerId, byte targetId = byte.MaxValue, bool giving = false)
    {
        if (targetId == byte.MaxValue)
        {
            ActiveBomb?.Destroy();
            ActiveBomb = null;
            return;
        }

        var player = PlayerById(playerId);
        var target = PlayerById(targetId);
        if (target == null) return;

        if (giving)
        {
            if (ActiveBomb == null)
            {
                ActiveBomb = new Bomb(target, bombDelay, bombTimer);
                return;
            }

            ActiveBomb.HasBombPlayer = target;
            ActiveBomb.TimeLeft += 0.5f;
            GameDataManager.RecordEvent("giveBomb", player?.PlayerId, target?.PlayerId);
        }
        else
        {
            ActiveBomb?.Destroy();
            ActiveBomb = new Bomb(target, bombDelay, bombTimer);
            GameDataManager.RecordEvent("giveBomb", player?.PlayerId, target?.PlayerId);
        }
    }

    public class Bomb
    {
        public PlayerControl HasBombPlayer
        {
            get;
            set
            {
                if (field == value) return;

                var oldPlayer = field;
                field = value;
                OnBombPlayerChanged(oldPlayer, value);
            }
        }

        public bool IsActive { get; private set; }

        public float TimeLeft
        {
            get => _bombDelay + _bombTimer - _elapsedTime;
            set => _elapsedTime = _bombDelay + _bombTimer - value;
        }

        private readonly float _bombDelay;
        private readonly float _bombTimer;
        private float _elapsedTime;
        private bool _hasAlerted;
        private int _lastDisplayedTime = -1;
        private bool _destroyed;

        public Bomb(PlayerControl player, float bombDelay, float bombTimer)
        {
            _bombDelay = bombDelay;
            _bombTimer = bombTimer;
            _elapsedTime = 0;
            IsActive = false;
            HasBombPlayer = player;
        }

        private static void OnBombPlayerChanged(PlayerControl oldPlayer, PlayerControl newPlayer)
        {
            if (oldPlayer == PlayerControl.LocalPlayer)
            {
                SoundEffectsManager.stop("timemasterShield");
            }
            else if (newPlayer == PlayerControl.LocalPlayer)
            {
                SoundEffectsManager.play("timemasterShield");
            }
        }

        public void Update()
        {
            if (_destroyed) return;

            _elapsedTime += Time.deltaTime;

            if (!IsActive && _elapsedTime >= _bombDelay)
            {
                IsActive = true;
            }

            if (Player.IsDead() || HasBombPlayer.IsDead())
            {
                ActiveBomb = null;
                Destroy();
                return;
            }

            if (IsActive && TimeLeft <= 0)
            {
                if (PlayerControl.LocalPlayer == Player)
                    RpcCustomMurderPlayer(Player, HasBombPlayer, false);
                ActiveBomb = null;
                Destroy();
                return;
            }

            if (IsActive && PlayerControl.LocalPlayer == HasBombPlayer)
            {
                var remaining = Mathf.CeilToInt(TimeLeft);
                if (remaining < 0) remaining = 0;

                if (_lastDisplayedTime != remaining)
                {
                    _ = new CustomMessage($"你手中的炸弹将在 {remaining} 秒后引爆!", 1f);
                    _lastDisplayedTime = remaining;
                }

                if (remaining % 5 == 0)
                {
                    if (!_hasAlerted)
                    {
                        Coroutines.Start(showFlashCoroutine(Palette.ImpostorRed, 0.75f));
                        _hasAlerted = true;
                    }
                }
                else
                {
                    _hasAlerted = false;
                }
            }
        }

        public void Destroy()
        {
            if (_destroyed) return;
            _destroyed = true;
            IsActive = false;
            SoundEffectsManager.stop("timemasterShield");
            HasBombPlayer = null;
        }
    }

}