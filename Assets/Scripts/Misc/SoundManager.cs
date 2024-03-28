using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    // placeholder
    public string sound = "ArcadeTest";

    // May be redeveloped in a cleaner Soundbank.cs file
    public AudioClip[] soundBank;

    public static SoundManager Current { get; private set; }

    public void Start()
    {
        if (Current == null)
        {
            Current = this;
        }
        else
        {
            Destroy(this);
        }
        // warnings should go away when a sound name is linked?
        // May be redeveloped in a cleaner Soundbank.cs file
        //soundBank[0] = new Sound(0, "playerWalk", AudioClip.Create("ArcadeTest.wav", 44100 * 2, 1, 44100, true, true, OnAudioRead));
        //soundBank[1] = new Sound(1, "playerJump", AudioClip.Create("ArcadeTest.wav", 44100 * 2, 1, 44100, true, true, OnAudioRead));
        //soundBank[2] = new Sound(2, "playerStrike", AudioClip.Create("ArcadeTest.wav", 44100 * 2, 1, 44100, true, true, OnAudioRead));
        //soundBank[3] = new Sound(3, "playerPearry", AudioClip.Create("ArcadeTest.wav", 44100 * 2, 1, 44100, true, true, OnAudioRead));
        //soundBank[4] = new Sound(4, "playerDie", AudioClip.Create("ArcadeTest.wav", 44100 * 2, 1, 44100, true, true, OnAudioRead));
    }
    public void Update()
    {

        playSound(sound);
    }
    public static void playSound(string sound)
    {
        foreach(AudioClip s in Current.soundBank)
        {
            if (s.name.Contains(sound))
            {
                Debug.Log("Found match with " + s.name);
                AudioSource audio = Current.GetComponent<AudioSource>();
                audio.clip = s;
                audio.Play();
                return;
            }
        }
        Debug.Log("No match found with " + sound);
    }
    static void OnAudioRead(float[] data)
    {
        // PCM callback?
    }
}
