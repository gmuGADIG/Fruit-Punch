using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound : MonoBehaviour
{
    int id;
    string soundInfo;
    AudioClip soundAudio;
    public Sound(int ID, string info, AudioClip clip)
    {
        id = ID;
        soundInfo = info;
        soundAudio = clip;
    }
    public int getID()
    {
        return id;
    }
    public string getSoundInfo()
    {
        return soundInfo;
    }
    public AudioClip getAudio()
    {
        return soundAudio;
    }
}
