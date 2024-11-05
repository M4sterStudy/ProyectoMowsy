using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio")]
    private AudioSource musicSource;
    private AudioSource sfxSource;

    [Header("Sliders de Volumen")]
    private Slider musicVolumeSlider;
    private Slider sfxVolumeSlider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        sfxSource = gameObject.AddComponent<AudioSource>();

        float savedMusicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicSource.volume = savedMusicVolume;
        sfxSource.volume = savedSFXVolume;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupVolumeControls();
    }

    private void SetupVolumeControls()
    {
        musicVolumeSlider = FindSliderByName("SliderMusica");
        sfxVolumeSlider = FindSliderByName("SliderEfectosDeSonido");

        float savedMusicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.gameObject.SetActive(true);
            musicVolumeSlider.value = savedMusicVolume;
            musicVolumeSlider.onValueChanged.RemoveAllListeners();
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        else
        {
            Debug.LogWarning("No se encontró el SliderMusica en la escena.");
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.gameObject.SetActive(true);
            sfxVolumeSlider.value = savedSFXVolume;
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        else
        {
            Debug.LogWarning("No se encontró el SliderEfectosDeSonido en la escena.");
        }
    }

    private Slider FindSliderByName(string name)
    {
        Slider[] allSliders = Resources.FindObjectsOfTypeAll<Slider>();
        foreach (Slider slider in allSliders)
        {
            if (slider.name == name)
            {
                return slider;
            }
        }
        return null;
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioClip de música no asignado.");
        }
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxClip != null)
        {
            sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxSource.PlayOneShot(sfxClip);
        }
        else
        {
            Debug.LogWarning("AudioClip de efectos de sonido no asignado.");
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}