using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Sets up various start-up functionality, including adjusting volume based on PlayerPrefs. <br/>
/// This must be added to the main menu. It may be added to other scenes as well if it helps while developing.
/// </summary>
public class StartUp : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;

    static bool hasRun = false;
    
    void Start()
    {
        if (hasRun) return;
        hasRun = true;
        
        OptionsMenu.SetStartUpVolumes(mixer);
    }
}
