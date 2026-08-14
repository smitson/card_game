using UnityEngine;

/// <summary>
/// Singleton AudioManager. Place on a persistent GameObject in the scene.
/// Assign AudioClips in the Inspector. All calls are null-safe so tests
/// that run without a scene (no AudioManager instance) are unaffected.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sound Effects")]
    [Tooltip("Short whoosh/flutter played when a card is dealt from the deck")]
    public AudioClip cardFlipSound;

    [Tooltip("Positive ding/chime played on a successful card match removal")]
    public AudioClip matchSound;

    [Tooltip("Dull thud/bonk played when an invalid card is tapped")]
    public AudioClip invalidTapSound;

    [Tooltip("Short victory jingle played when the player wins")]
    public AudioClip winFanfareSound;

    [Tooltip("Descending tone played when the player loses")]
    public AudioClip loseStingSound;

    [Tooltip("Crisp click played when any UI button is pressed")]
    public AudioClip buttonClickSound;

    [Tooltip("Distinct sound played when the player undoes a move")]
    public AudioClip undoSound;

    [Header("Background Music")]
    public AudioClip backgroundMusic;

    [Tooltip("Whether to start background music automatically on scene load")]
    public bool playMusicOnStart = true;

    [Range(0f, 1f)]
    [Tooltip("Volume level for background music (0=silent, 1=full)")]
    public float musicVolume = 0.4f;

    [Header("SFX Volume")]
    [Range(0f, 1f)]
    public float sfxVolume = 1.0f;

    private AudioSource sfxSource;
    private AudioSource musicSource;
    private bool musicEnabled = true;

    void Awake()
    {
        // Singleton pattern - destroy duplicate if one already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // SFX source: PlayOneShot allows overlapping sounds
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        // Music source: loops continuously
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
    }

    void Start()
    {
        if (playMusicOnStart && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    // ---- Public SFX API ----

    public void PlayCardFlip()    => PlaySFX(cardFlipSound);
    public void PlayMatch()       => PlaySFX(matchSound);
    public void PlayInvalidTap()  => PlaySFX(invalidTapSound);
    public void PlayWinFanfare()  => PlaySFX(winFanfareSound);
    public void PlayLoseSting()   => PlaySFX(loseStingSound);
    public void PlayButtonClick() => PlaySFX(buttonClickSound);
    public void PlayUndo()        => PlaySFX(undoSound);

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // ---- Music Controls ----

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        if (musicEnabled)
            musicSource.UnPause();
        else
            musicSource.Pause();
    }

    public void SetMusicEnabled(bool enabled)
    {
        musicEnabled = enabled;
        if (enabled) musicSource.UnPause();
        else musicSource.Pause();
    }

    public bool IsMusicEnabled() => musicEnabled;
}
