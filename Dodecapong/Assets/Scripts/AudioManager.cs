using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Instance setup
    public static AudioManager instance;

    private void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this);
    }
    #endregion

    public AudioSource audioSource;

    public AudioClip goal;
    public AudioClip playerElimination;
    public AudioClip selectUI;
    public AudioClip hoverUI;
    public AudioClip ballHover;
    public AudioClip towerMove;
    public AudioClip gameplayMusic;
    public AudioClip winMusic;

    public void PlaySound(AudioClip clip)
    {
        if (clip) audioSource.PlayOneShot(clip);
    }
}
