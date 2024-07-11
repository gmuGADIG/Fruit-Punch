using UnityEngine;

public class LevelMusicPlayer: AbstractMusicPlayer {
    [SerializeField] float loopPoint;

    [SerializeField] AudioClip appleAudioClip;
    [SerializeField] AudioClip grapeAudioClip;
    [SerializeField] AudioClip bananaAudioClip;
    [SerializeField] AudioClip watermelonAudioClip;

    AudioSource audioSource;
    bool looping = false;

    public override bool Equals(AbstractMusicPlayer other) {
        if (other is LevelMusicPlayer) {
            var o = (LevelMusicPlayer)other;

            return 
                o.appleAudioClip == appleAudioClip && 
                o.grapeAudioClip == grapeAudioClip && 
                o.bananaAudioClip == bananaAudioClip && 
                o.watermelonAudioClip == watermelonAudioClip;
        } else {
            return false;
        }
    }

    AudioClip GetAudioClip() {
        #pragma warning disable CS8509
        return PlayerInfo.characters[0] switch {
            Character.Apple => appleAudioClip,
            Character.Grapes => grapeAudioClip,
            Character.Banana => bananaAudioClip,
            Character.Watermelon => watermelonAudioClip,
            _ => appleAudioClip, // play apple's music if we're entering this level through the editor
        };
    }

    protected override void Play() {
        audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = GetAudioClip();
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
