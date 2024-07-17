using UnityEngine;
using UnityEngine.Audio;

public abstract class AbstractMusicPlayer : MonoBehaviour, System.IEquatable<AbstractMusicPlayer> {
    [Range(0f, 1f)]
    [SerializeField] protected float volume = 1f;

    [SerializeField] protected AudioMixerGroup mixerGroup;

    #nullable enable
    public static AbstractMusicPlayer? CurrentMusicPlayer = null;

    protected abstract void Play();

    public abstract bool Equals(AbstractMusicPlayer? other);

    void Start() {
        print("CurrentMusicPlayer = " + CurrentMusicPlayer);
        if (this.Equals(CurrentMusicPlayer)) {
            Destroy(gameObject);
        } else {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);

            Destroy(CurrentMusicPlayer?.gameObject);
            CurrentMusicPlayer = this;
            Play();
        }
    }
}
