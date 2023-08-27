using System;
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
    
    [HideInInspector] public UnityEvent goalScoredEvent;
    [HideInInspector] public UnityEvent playerEliminatedEvent; //
    [HideInInspector] public UnityEvent towerMoveEvent; // 
    [HideInInspector] public UnityEvent shieldBreakEvent; // 
    [HideInInspector] public UnityEvent shieldHitEvent; //

    [Header("UI Clips")]
    [SerializeField] AudioClip selectUI;
    [SerializeField] AudioClip hoverUI;

    [HideInInspector] public UnityEvent selectUIEvent; //
    [HideInInspector] public UnityEvent hoverUIEvent; //

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
        selectUIEvent ??= new UnityEvent();
        hoverUIEvent ??= new UnityEvent();
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
        selectUIEvent.AddListener(SelectUICallback);
        hoverUIEvent.AddListener(HoverUICallback);
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
    void SelectUICallback() => SafePlayOneShot(audioSource, selectUI);
    void HoverUICallback() => SafePlayOneShot(audioSource, hoverUI);
    void MainMenuCallback() => SafePlayOneShot(audioSource, mainMenu);
    void JoinMenuCallback() => SafePlayOneShot(audioSource, joinMenu);
    void SettingsMenuCallback() => SafePlayOneShot(audioSource, settingsMenu);
    void GameplayCallback() => SafePlayOneShot(audioSource, gameplay);
    void GamePausedCallback() => SafePlayOneShot(audioSource, gamePaused);
    void GameOverCallback() => SafePlayOneShot(audioSource, gameOver);
}
