using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AudioEvent : UnityEvent<AudioClip>
{
}

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

    public AudioSource audioSource;

    [Header("Gameplay Events")]
    public UnityEvent goalScored;
    public UnityEvent playerEliminated;
    public UnityEvent ballHover;
    public UnityEvent towerMove;
    public AudioEvent shieldBreak;

    [Header("UI Events")]
    public UnityEvent selectUI;
    public UnityEvent hoverUI;

    [Header("Game State Events")]
    public UnityEvent mainMenu;
    public UnityEvent joinMenu;
    public UnityEvent settingsMenu;
    public UnityEvent gameplay;
    public UnityEvent gamePaused;
    public UnityEvent gameOver;

    private void Start()
    {
        // Gameplay
        goalScored ??= new UnityEvent();
        playerEliminated ??= new UnityEvent();
        ballHover ??= new UnityEvent();
        towerMove ??= new UnityEvent();
        shieldBreak ??= new AudioEvent();

        // UI
        selectUI ??= new UnityEvent();
        hoverUI ??= new UnityEvent();

        // Game State
        mainMenu ??= new UnityEvent();
        joinMenu ??= new UnityEvent();
        settingsMenu ??= new UnityEvent();
        gameplay ??= new UnityEvent();
        gamePaused ??= new UnityEvent();
        gameOver ??= new UnityEvent();

        shieldBreak.AddListener(PlayClip);

        //shieldBreak.Invoke(null);
    }

    public void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}
