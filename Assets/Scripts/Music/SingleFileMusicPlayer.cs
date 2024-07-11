using UnityEngine;

public class SingleFileMusicPlayer : AbstractMusicPlayer {
    [SerializeField] float loopPoint;
    [SerializeField] AudioClip audioClip;

    bool looping = false;
    AudioSource audioSource;

    public override bool Equals(AbstractMusicPlayer other) {
        if (other is SingleFileMusicPlayer) {
            return ((SingleFileMusicPlayer)other).audioClip == audioClip;
        } else {
            return false;
        }
    }

    protected override void Play() {
        audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = mixerGroup;

        audioSource.Play();
    }

    void Update() {
        if (!looping) {
            looping = audioSource.time >= loopPoint;
            return;
        }

        if (!audioSource.isPlaying) {
            audioSource.Play();
            audioSource.time = loopPoint;
        }
    }
}
