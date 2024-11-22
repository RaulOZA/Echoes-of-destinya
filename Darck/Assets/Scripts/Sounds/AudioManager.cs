using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public AudioMixer musicMixer, effectsMixer;

    public AudioSource swing, oscuroDeath, oscuroHit, mainMenu, gameOver, lvl1bgmsc;

    public static AudioManager instance;

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
}
