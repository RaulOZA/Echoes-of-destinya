using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

    public AudioMixer musicMixer, effectsMixer;

    public AudioSource swing, oscuroDeath, oscuroHit, mainMenu, gameOver, level1Music, level2Music, level3Music, level4Music;

    public static AudioManager instance;
    //public AudioSource musicSource;

    [Range(-80, 10)]
    public float masterVol, effectsVol;
    public Slider masterSlider, effectsSlider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    // Start is called before the first frame update
    void Start()
    {
        masterSlider.value = masterVol;
        effectsSlider.value = effectsVol;

        masterSlider.minValue = -80;
        masterSlider.maxValue = 10;

        effectsSlider.minValue = -80;
        effectsSlider.maxValue = 10;

        
        }

    // Update is called once per frame
    void Update()
    {
        MasterVolume();
        EffectsVolume();
    }

    public void MasterVolume()
    {
        musicMixer.SetFloat("masterVolume", masterSlider.value);
    }
    public void EffectsVolume()
    {
        effectsMixer.SetFloat("effectsVolume", effectsSlider.value);
    }

    public void PlayAudio(AudioSource audio)
    {
        audio.Play();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Stop all music first
        StopAllMusic();

        // Play the relevant music for the current scene
        if (scene.name == "Level 1")
        {
            instance.level1Music.Play();
        }
        if (scene.name == "Level2")
        {
            instance.level2Music.Play();
        }
        else if (scene.name == "Level3")
        {
            instance.level3Music.Play();
        }
        else if (scene.name == "FinalBoss")
        {
            instance.level4Music.Play();
        }
    }

    void StopAllMusic()
    {
        // Stop all music AudioSources in AudioManager
        if (AudioManager.instance.level1Music.isPlaying)
            AudioManager.instance.level1Music.Stop();
        if (AudioManager.instance.level2Music.isPlaying)
            AudioManager.instance.level2Music.Stop();
        if (AudioManager.instance.level3Music.isPlaying)
            AudioManager.instance.level3Music.Stop();
        if (AudioManager.instance.level4Music.isPlaying)
            AudioManager.instance.level4Music.Stop();
        // Add any other AudioSources that might need to be stopped
    }
}
