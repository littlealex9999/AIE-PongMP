using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    #region Instance setup
    public static EventManager instance;

    private void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this);
    }
    #endregion

    [SerializeField] AudioSource audioSource;

    [Header("Gameplay Clips")]
    [SerializeField] AudioClip goalScored;
    [SerializeField] AudioClip playerEliminated;
    [SerializeField] AudioClip towerMove;
    [SerializeField] AudioClip shieldBreak;
    [SerializeField] AudioClip shieldHit;
    [SerializeField] AudioClip ballCountdown;
    [SerializeField] AudioClip ballBounce;
    [SerializeField] AudioClip ballHit;
    [SerializeField] AudioClip ballGrab;
    [SerializeField] AudioClip dash;

    [HideInInspector] public UnityEvent goalScoredEvent; //
    [HideInInspector] public UnityEvent playerEliminatedEvent; //
    [HideInInspector] public UnityEvent towerMoveEvent; // 
    [HideInInspector] public UnityEvent shieldBreakEvent; // 
    [HideInInspector] public UnityEvent shieldHitEvent; //
    [HideInInspector] public UnityEvent ballCountdownEvent; //
    [HideInInspector] public UnityEvent ballBounceEvent; //
    [HideInInspector] public UnityEvent ballHitEvent; //
    [HideInInspector] public UnityEvent ballGrabEvent; //
    [HideInInspector] public UnityEvent dashEvent; //

    [Header("UI Clips")]
    [SerializeField] AudioClip selectUI;
    [SerializeField] AudioClip hoverUI;
    [SerializeField] AudioClip playerJoin;
    [SerializeField] AudioClip playerLeave;

    [HideInInspector] public UnityEvent selectUIEvent; //
    [HideInInspector] public UnityEvent hoverUIEvent; //
    [HideInInspector] public UnityEvent playerJoinEvent; //
    [HideInInspector] public UnityEvent playerLeaveEvent; //

    [Header("Game State Clips")]
    [SerializeField] AudioClip mainMenu;
    [SerializeField] AudioClip joinMenu;
    [SerializeField] AudioClip settingsMenu;
    [SerializeField] AudioClip gameplay;
    [SerializeField] AudioClip gamePaused;
    [SerializeField] AudioClip gameOver;

    [HideInInspector] public UnityEvent mainMenuEvent; //
    [HideInInspector] public UnityEvent joinMenuEvent; //
    [HideInInspector] public UnityEvent settingsMenuEvent; //
    [HideInInspector] public UnityEvent gameplayEvent; //
    [HideInInspector] public UnityEvent gamePausedEvent; //
    [HideInInspector] public UnityEvent gameOverEvent; //

    private void Start()
    {
        goalScoredEvent ??= new UnityEvent();
        playerEliminatedEvent ??= new UnityEvent();
        towerMoveEvent ??= new UnityEvent();
        shieldBreakEvent ??= new UnityEvent();
        shieldHitEvent ??= new UnityEvent();
        ballCountdownEvent ??= new UnityEvent();
        ballBounceEvent ??= new UnityEvent();
        ballHitEvent ??= new UnityEvent();
        ballGrabEvent ??= new UnityEvent();
        dashEvent ??= new UnityEvent();

        selectUIEvent ??= new UnityEvent();
        hoverUIEvent ??= new UnityEvent();
        playerJoinEvent ??= new UnityEvent();
        playerLeaveEvent ??= new UnityEvent();

        mainMenuEvent ??= new UnityEvent();
        joinMenuEvent ??= new UnityEvent();
        settingsMenuEvent ??= new UnityEvent();
        gameplayEvent ??= new UnityEvent();
        gamePausedEvent ??= new UnityEvent();
        gameOverEvent ??= new UnityEvent();

        goalScoredEvent.AddListener(GoalScoredCallback);
        playerEliminatedEvent.AddListener(PlayerElminatedCallback);
        towerMoveEvent.AddListener(TowerMoveCallback);
        shieldBreakEvent.AddListener(ShieldBreakCallback);
        shieldHitEvent.AddListener(ShieldHitCallback);
        ballCountdownEvent.AddListener(BallCountdownCallback);
        ballBounceEvent.AddListener(BallBounceCallback);
        ballHitEvent.AddListener(BallHitCallback);
        ballGrabEvent.AddListener(BallGrabCallback);
        dashEvent.AddListener(DashCallback);

        selectUIEvent.AddListener(SelectUICallback);
        hoverUIEvent.AddListener(HoverUICallback);
        playerJoinEvent.AddListener(PlayerJoinCallback);
        playerLeaveEvent.AddListener(PlayerLeaveCallback);

        mainMenuEvent.AddListener(MainMenuCallback);
        joinMenuEvent.AddListener(JoinMenuCallback);
        settingsMenuEvent.AddListener(SettingsMenuCallback);
        gameplayEvent.AddListener(GameplayCallback);
        gamePausedEvent.AddListener(GamePausedCallback);
        gameOverEvent.AddListener(GameOverCallback);
    }

    void SafePlayOneShot(AudioSource audioSource, AudioClip audioClip)
    {
        if (audioClip != null) audioSource.PlayOneShot(audioClip);
    }

    void GoalScoredCallback() => SafePlayOneShot(audioSource, goalScored);
    void PlayerElminatedCallback() => SafePlayOneShot(audioSource, playerEliminated);
    void TowerMoveCallback() => SafePlayOneShot(audioSource, towerMove);
    void ShieldBreakCallback() => SafePlayOneShot(audioSource, shieldBreak);
    void ShieldHitCallback() => SafePlayOneShot(audioSource, shieldHit);
    void BallCountdownCallback() => SafePlayOneShot(audioSource, ballCountdown);
    void BallBounceCallback() => SafePlayOneShot(audioSource, ballBounce);
    void BallHitCallback() => SafePlayOneShot(audioSource, ballHit);
    void BallGrabCallback() => SafePlayOneShot(audioSource, ballGrab);
    void DashCallback() => SafePlayOneShot(audioSource, dash);

    void SelectUICallback() => SafePlayOneShot(audioSource, selectUI);
    void HoverUICallback() => SafePlayOneShot(audioSource, hoverUI);
    void PlayerJoinCallback() => SafePlayOneShot(audioSource, playerJoin);
    void PlayerLeaveCallback() => SafePlayOneShot(audioSource, playerLeave);

    void MainMenuCallback() => SafePlayOneShot(audioSource, mainMenu);
    void JoinMenuCallback() => SafePlayOneShot(audioSource, joinMenu);
    void SettingsMenuCallback() => SafePlayOneShot(audioSource, settingsMenu);
    void GameplayCallback() => SafePlayOneShot(audioSource, gameplay);
    void GamePausedCallback() => SafePlayOneShot(audioSource, gamePaused);
    void GameOverCallback() => SafePlayOneShot(audioSource, gameOver);
}
