using UnityEngine;

public class MultiFileMusicPlayer : AbstractMusicPlayer {
    [SerializeField] AudioClip introClip;
    [SerializeField] AudioClip loopClip;

    AudioSource introSource;
    AudioSource loopSource;

    public override bool Equals(AbstractMusicPlayer other) {
        if (other is MultiFileMusicPlayer) {
            var o = (MultiFileMusicPlayer)other; // i hate c# for forcing me to do this
            return o.introClip == introClip && o.loopClip == loopClip;
        } else {
            return false;
        }
    }

    protected override void Play() {
        introSource = gameObject.AddComponent<AudioSource>();
        loopSource = gameObject.AddComponent<AudioSource>();

        introSource.clip = introClip;
        introSource.volume = volume;
        introSource.outputAudioMixerGroup = mixerGroup;

        loopSource.clip = loopClip;
        loopSource.loop = true;
        loopSource.volume = volume;
        loopSource.outputAudioMixerGroup = mixerGroup;

        introSource.Play();

        // convince Unity to load our audio
        loopSource.Play();
        loopSource.Stop();
    }

    void Update() {
        if (!introSource.isPlaying) {
            loopSource.Play();
            enabled = false; // stops Update from being run
        }
    }
}
