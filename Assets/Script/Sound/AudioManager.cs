using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("----------Audio Source---------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource foleySource;
    [SerializeField] AudioSource uiSource;

    [Header("----------Audio Clip---------")]
    public AudioClip[] theme;
    public AudioClip[] sfx;
    public AudioClip[] Enviroment;
    public AudioClip[] Enemy;
    public AudioClip Click;
    // Start is called before the first frame update
    void Start()
    {
        SelectTheme();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void SelectTheme()
    {
        if (musicSource == null)
        {
            Debug.LogWarning("AudioManager.SelectTheme: musicSource is null.");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;

        if (theme == null || theme.Length == 0)
        {
            Debug.LogWarning("AudioManager.SelectTheme: theme array is empty. Assign AudioClips in the Inspector.");
            musicSource.clip = null;
            return;
        }

        AudioClip clip = null;

        if (currentScene == "Main Menu")
        {
            clip = GetThemeAt(0);
        }
        else if (currentScene == "Tutorial Map")
        {
            clip = GetThemeAt(1);
        }
        else if (currentScene == "Map 1")
        {
            clip = GetThemeAt(2);
        }
        else if (currentScene == "Map 2")
        {
            clip = GetThemeAt(3);
        }
        else if (currentScene == "Map 3")
        {
            clip = null;
        }
        else if (currentScene == "Map 4")
        {
            clip = GetThemeAt(5);
        }
        else if (currentScene == "Map 5")
        {      
            clip = GetThemeAt(6);
        }
        else if (currentScene == "The End")
        {
            clip = GetThemeAt(6);
        }

        musicSource.clip = clip;
        if (musicSource.clip != null)
        {
            musicSource.Play();
        }
    }

    private AudioClip GetThemeAt(int index)
    {
        if (theme != null && index >= 0 && index < theme.Length) return theme[index];
        Debug.LogWarning($"AudioManager: requested theme[{index}] out of range (length={theme?.Length ?? 0}).");
        return null;
    }

    public void PlaySfx(AudioClip audioclip)
    {
        if (audioclip != null)
        {
            sfxSource.PlayOneShot(audioclip);
        }
        else
        {
            Debug.Log("null");
        }
    }
    public void PlayFoley(AudioClip audioclip)
    {
        foleySource.PlayOneShot(audioclip);
    }
}
