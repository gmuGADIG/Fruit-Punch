using UnityEngine;

public class OneShotMusicPlayer : AbstractMusicPlayer {
    [SerializeField] AudioClip audioClip;

    public override bool Equals(AbstractMusicPlayer other) {
        if (other is OneShotMusicPlayer) {
            return ((OneShotMusicPlayer)other).audioClip == audioClip;
        } else {
            return false;
        }
    }

    protected override void Play() {
        var audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = mixerGroup;

        audioSource.Play();
    }
}
