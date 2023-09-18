using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource), typeof(AudioSource), typeof(AudioSource))]
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

    [SerializeField] AudioSource menuMusicSource;
    [SerializeField] AudioSource gameplayMusicSource;

    [Header("Gameplay Clips")]
    [SerializeField] AudioClip goalScored;
    [SerializeField] AudioClip playerEliminated;
    [SerializeField] AudioClip towerMove;
    [SerializeField] AudioClip shieldBreak;
    [SerializeField] AudioClip shieldHit;
    [SerializeField] AudioClip ballCountdown;
    [SerializeField] AudioClip ballBounce;
    [SerializeField] AudioClip ballHit;
    [SerializeField] AudioClip ballHitPillar;
    [SerializeField] AudioClip ballHitBlackHole;
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
    [HideInInspector] public UnityEvent ballHitPillarEvent; 
    [HideInInspector] public UnityEvent ballHitBlackHoleEvent; //
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

    [HideInInspector] public UnityEvent menuEvent; //
    [HideInInspector] public UnityEvent gameplayEvent; //

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
        ballHitPillarEvent ??= new UnityEvent();
        ballHitBlackHoleEvent ??= new UnityEvent();
        ballGrabEvent ??= new UnityEvent();
        dashEvent ??= new UnityEvent();

        selectUIEvent ??= new UnityEvent();
        hoverUIEvent ??= new UnityEvent();
        playerJoinEvent ??= new UnityEvent();
        playerLeaveEvent ??= new UnityEvent();

        menuEvent ??= new UnityEvent();
        gameplayEvent ??= new UnityEvent();


        goalScoredEvent.AddListener(GoalScoredCallback);
        playerEliminatedEvent.AddListener(PlayerElminatedCallback);
        towerMoveEvent.AddListener(TowerMoveCallback);
        shieldBreakEvent.AddListener(ShieldBreakCallback);
        shieldHitEvent.AddListener(ShieldHitCallback);
        ballCountdownEvent.AddListener(BallCountdownCallback);
        ballBounceEvent.AddListener(BallBounceCallback);
        ballHitEvent.AddListener(BallHitCallback);
        ballHitPillarEvent.AddListener(BallHitPillarCallback);
        ballHitBlackHoleEvent.AddListener(BallHitBlackHoleCallback);
        ballGrabEvent.AddListener(BallGrabCallback);
        dashEvent.AddListener(DashCallback);

        selectUIEvent.AddListener(SelectUICallback);
        hoverUIEvent.AddListener(HoverUICallback);
        playerJoinEvent.AddListener(PlayerJoinCallback);
        playerLeaveEvent.AddListener(PlayerLeaveCallback);

        menuEvent.AddListener(MainMenuCallback);
        gameplayEvent.AddListener(GameplayCallback);
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
    void BallHitPillarCallback() => SafePlayOneShot(audioSource, ballHitPillar);
    void BallHitBlackHoleCallback() => SafePlayOneShot(audioSource, ballHitBlackHole);
    void BallGrabCallback() => SafePlayOneShot(audioSource, ballGrab);
    void DashCallback() => SafePlayOneShot(audioSource, dash);

    void SelectUICallback() => SafePlayOneShot(audioSource, selectUI);
    void HoverUICallback() => SafePlayOneShot(audioSource, hoverUI);
    void PlayerJoinCallback() => SafePlayOneShot(audioSource, playerJoin);
    void PlayerLeaveCallback() => SafePlayOneShot(audioSource, playerLeave);

    void MainMenuCallback()
    {
        if (!menuMusicSource.isPlaying)
        {
            menuMusicSource.Play();
            gameplayMusicSource.Stop();
        }
    }

    void GameplayCallback()
    {
        if (!gameplayMusicSource.isPlaying)
        {
            gameplayMusicSource.Play();
            menuMusicSource.Stop();
        }
    }

}
