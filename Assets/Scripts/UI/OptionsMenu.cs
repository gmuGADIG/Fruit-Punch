using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider musicSlider;

    [SerializeField] TMP_Text masterValueLabel;
    [SerializeField] TMP_Text sfxValueLabel;
    [SerializeField] TMP_Text musicValueLabel;

    float timeSinceSoundDemo = 0f;
    bool setupFinished = false;
    
    private void Start()
    {
        Utils.Assert(masterSlider != null && sfxSlider != null && musicSlider != null);
        Utils.Assert(masterValueLabel != null && sfxValueLabel != null && musicValueLabel != null);
        masterSlider.onValueChanged.AddListener((value) =>
        {
            PlayDemoSound();
            masterValueLabel.text = Mathf.RoundToInt(value * 100) + "%";
            PlayerPrefs.SetFloat("VolumeMaster", value);
            mixer.SetFloat("VolumeMaster", Mathf.Log10(value) * 20);
            print("set VolumeMain to " + Mathf.Log10(value) * 20);
        });

        sfxSlider.onValueChanged.AddListener((volume) => SetVolume("VolumeSfx", volume, sfxValueLabel, true));

        musicSlider.onValueChanged.AddListener((volume) => SetVolume("VolumeMusic", volume, musicValueLabel, false));
        
        masterSlider.value = PlayerPrefs.GetFloat("VolumeMaster", 0.8f);
        sfxSlider.value = PlayerPrefs.GetFloat("VolumeSfx", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("VolumeMusic", 1f);

        setupFinished = true;
    }

    public static void SetStartUpVolumes(AudioMixer mixer)
    {
        mixer.SetFloat("VolumeMaster", Mathf.Log10(PlayerPrefs.GetFloat("VolumeMaster")) * 20);
        mixer.SetFloat("VolumeMaster", Mathf.Log10(PlayerPrefs.GetFloat("VolumeMaster")) * 20);
        mixer.SetFloat("VolumeMaster", Mathf.Log10(PlayerPrefs.GetFloat("VolumeMaster")) * 20);
    }

    void SetVolume(string key, float value, TMP_Text label, bool playDemoSound)
    {
        if (playDemoSound && setupFinished) PlayDemoSound();
        label.text = Mathf.RoundToInt(value * 100) + "%";
        PlayerPrefs.SetFloat(key, value);
        var decibels = Mathf.Log10(value) * 20;
        mixer.SetFloat(key, decibels);
        print($"set {key} to {decibels}");
    }

    void PlayDemoSound()
    {
        if (timeSinceSoundDemo <= 0.2) return;
        SoundManager.Instance.PlaySoundGlobal("EnemyHurt");
        timeSinceSoundDemo = 0f;
    }

    void Update()
    {
        timeSinceSoundDemo += Time.deltaTime;
    }

    public void backButton()
    {
        var pauseManager = GameObject.Find("PauseManager");
        if (pauseManager != null)
        {
            pauseManager.GetComponent<PauseManager>().OnBack();
        }
        else
        {
            SceneManager.UnloadSceneAsync("OptionsMenu");
        }
    }
}