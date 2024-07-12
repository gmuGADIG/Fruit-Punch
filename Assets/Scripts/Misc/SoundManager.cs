using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public int numOfPositionalSources = 100;

    public AudioMixerGroup mixerGroup;

    private Queue<PositionalAudioSource> positionalSources = new Queue<PositionalAudioSource>();
    private List<AudioSource> toUnpause = new List<AudioSource>();

    Sound[] sounds = {};

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }

        sounds = Resources.LoadAll<Sound>("Sounds");

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.GetRandomAudioClip();
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mixerGroup;
        }
        Transform positionalSourceParent = new GameObject("Positional Audio Sources").transform;
        for (int i = 0; i < numOfPositionalSources; i++)
        {
            PositionalAudioSource newPositional = new GameObject("Positional Audio Source").AddComponent<PositionalAudioSource>();
            newPositional.transform.parent = positionalSourceParent;
            positionalSources.Enqueue(newPositional);
        }
    }

    /// <summary>
    /// Play sound with name globally. This means that the sound will be the same volume regardless of where the audio listener is located
    /// and will not sound 3D.
    /// </summary>
    /// <param soundName="soundName">The name used to identify the sound. Should match the Sound asset filename.</param>
    /// <returns>The AudioSource that is playing the sound. null if sound not found.</returns>
    public AudioSource PlaySoundGlobal(string soundName)
    {
        int soundID = GetSoundIndex(soundName);
        if (soundID < 0 || soundID >= sounds.Length)
        {
            Debug.LogWarning("Invalid Sound ID: " + soundID);
            return null;
        }
        Sound sound = sounds[soundID];

        sound.source.clip = sound.GetRandomAudioClip();
        sound.source.volume = sound.volume * (1f + UnityEngine.Random.Range(-sound.volumeVariance / 2f, sound.volumeVariance / 2f));
        sound.source.pitch = sound.pitch * (1f + UnityEngine.Random.Range(-sound.pitchVariance / 2f, sound.pitchVariance / 2f));

        sound.source.Play();
        return sound.source;
    }

    /// <summary>
    /// Play sound with name soundName at position. The AudioSource will use 3D sound settings.
    /// </summary>
    /// <param soundName="soundName">The name used to identify the sound. Should match the Sound asset filename.</param>
    /// <returns>The AudioSource that is playing the sound. null if sound not found.</returns>
    public PositionalAudioSource PlaySoundAtPosition(string soundName, Vector3 position)
    {
        int soundID = GetSoundIndex(soundName);
        if (soundID < 0 || soundID >= sounds.Length)
        {
            Debug.LogWarning("Invalid Sound ID: " + soundID);
            return null;
        }
        Sound sound = sounds[soundID];

        PositionalAudioSource source = positionalSources.Dequeue();
        source.transform.SetParent(null);
        source.SetSound(sound);
        source.transform.position = position;
        source.Play(sound.mixerGroup);
        positionalSources.Enqueue(source);
        return source;
    }

    /// <summary>
    /// Play sound with name soundName at position. Takes an extra parameter for parenting the AudioSource.
    /// </summary>
    /// <param soundName="soundName">The name used to identify the sound. Should match the Sound asset filename.</param>
    /// <param parent="parent"> The transform the AudioSource should be parented to. </param>
    /// <returns>The AudioSource that is playing the sound. null if not found.</returns>
    public PositionalAudioSource PlaySoundAtPosition(string soundName, Vector3 position, Transform parent)
    {
        PositionalAudioSource source = PlaySoundAtPosition(soundName, position);
        //source.SetFollowTarget(parent);
        source.transform.SetParent(parent);
        return source;
    }

    /// <summary>
    /// Stops playing the sound with name soundName.
    /// </summary>
    /// <param soundName="soundName">The name used to identify the sound. Should match the Sound asset filename.</param>
    public void StopPlayingGlobal(string soundName)
    {
        int soundID = GetSoundIndex(soundName);
        Sound sound = sounds[soundID];
        if (sound.source != null)
        {
            sound.source.volume = sound.volume * (1f + UnityEngine.Random.Range(-sound.volumeVariance / 2f, sound.volumeVariance / 2f));
            sound.source.pitch = sound.pitch * (1f + UnityEngine.Random.Range(-sound.pitchVariance / 2f, sound.pitchVariance / 2f));

            sound.source.Stop();
        }
    }

    /// <summary>
    /// Gets the index of a sound with string name from the sounds[] array. The index will be greater than 0 and less than
    /// the total number of sounds.
    /// Logs a warning if the sound is not found.
    /// </summary>
    /// <param name="name">The name used to identify the sound. Should match the Sound asset filename.</param>
    /// <returns>The sound index. -1 if the sound is not found.</returns>
    int GetSoundIndex(string name)
    {
        int id = Array.FindIndex(sounds, sound => sound.name == name);
        if (id == -1)
        {
            Debug.LogWarning("Sound with name: " + name + " does not exist!");
        }
        return id;
    }

    /// <summary>
    /// Pauses all audio sources in the scene.
    /// </summary>
    public void PauseAllSounds()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i].isPlaying)
            {
                sources[i].Pause();
                toUnpause.Add(sources[i]);
            }
        }
    }

    /// <summary>
    /// Unpauses all audio sources that were paused with PauseAllSounds(). If this method is called before PauseAllSounds() it will
    /// do nothing.
    /// </summary>
    public void UnPauseAllSounds()
    {
        for (int i = 0; i < toUnpause.Count; i++)
        {
            toUnpause[i].UnPause();
        }
        toUnpause.Clear();
    }
}
