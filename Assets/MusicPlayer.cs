using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{
    [Range(0f, 2f)]
    [SerializeField] float volume = 1f;
    [SerializeField] AudioMixerGroup mixerGroup;

    [SerializeField] AudioClip intro;
    [SerializeField] AudioClip loop;

    AudioSource introSource;
    AudioSource loopSource;

    #nullable enable
    public static MusicPlayer? Instance { get; private set; } = null;

    void PlayLoop() {
        if (loop != null) {
            loopSource.Play();
        }

        // by disabling ourselves, we disable the busyloop in Update
        enabled = false;
    }

    void Start() {
        if (Instance != null) {
            if (intro == Instance.intro && loop == Instance.loop) {
                Destroy(gameObject);
                return;
            }
        } 

        DontDestroyOnLoad(gameObject);
        if (Instance != null) {
            Destroy(Instance.gameObject);
        }

        Instance = this;

        introSource = gameObject.AddComponent<AudioSource>();
        loopSource = gameObject.AddComponent<AudioSource>();

        introSource.clip = intro;
        introSource.outputAudioMixerGroup = mixerGroup;
        introSource.volume = volume;

        loopSource.clip = loop;
        loopSource.loop = true;
        loopSource.outputAudioMixerGroup = mixerGroup;
        loopSource.volume = volume;
        
        // this is a trick to get Unity to load the clip now instead 
        // of lazily loading when we try to play the audio
        loopSource.Play();
        loopSource.Pause();

        if (intro != null) {
            introSource.Play();
        } else {
            PlayLoop();
        }
    }

    void Update() {
        // Will the audio clip end next frame?
        if (!introSource.isPlaying) {
            PlayLoop();
        }
    }
}
