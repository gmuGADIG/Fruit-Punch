using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    /// <summary>
    /// Unloads the Options Screen
    /// </summary>

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    private float masterVolume = 1f;
    private float sfxVolume = 1f;
    private float musicVolume = 1f;

    private void Start()
    {
        if(masterVolumeSlider == null)
        {
            Debug.LogError("Master Volume slider not assigned");
        }
        if(sfxVolumeSlider == null)
        {
            Debug.LogError("SFX Volume slider not assigned");
        }
        if(musicVolumeSlider == null)
        {
            Debug.LogError("Music Volume slider not assigned");
        }

        masterVolumeSlider.onValueChanged.AddListener((value) =>
        {
            masterVolume = value;
            Debug.Log(masterVolume);
        });

        sfxVolumeSlider.onValueChanged.AddListener((value) =>
        {
            sfxVolume = value;
            Debug.Log(sfxVolume);
        });

        musicVolumeSlider.onValueChanged.AddListener((value) =>
        {
            musicVolume = value;
            Debug.Log(musicVolume);
        });
    }

    public void backButton()
    {
        SceneManager.UnloadSceneAsync(SwitchScene.optionsMenu);
    }
}