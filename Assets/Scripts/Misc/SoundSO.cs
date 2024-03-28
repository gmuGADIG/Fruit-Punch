using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//TEST by playing a sound upon button press
namespace ScriptableObjects
{
    [CreateAssetMenu(fileName ="NewSoundEffect",menuName ="Audio/New Sound Effect")]
public class SoundSO : ScriptableObject
{
    #region config
    public AudioClip[] clips;
    public Vector2 volume = new Vector2(0.5f, 0.5f);
    public Vector2 pitch = new Vector2(1, 1);
        #endregion
        #region PreviewCode
        #if UNITY_EDITOR
        private AudioSource previewer;
        private void OnEnable()
        {
            previewer = EditorUtility.CreateGameObjectWithHideFlags("AudioPreview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
        }

        private void OnDisable()
        {
            DestroyImmediate(previewer.gameObject);
        }
        //[ButtonGroup("previewControls")]
        //[Button]
        private void PlayPreview()
        {
            Play(previewer);
        }
        //[ButtonGroup("previewControls")]
        //[Button]
        private void stopPreview()
        {
            previewer.Stop();
        }
    #endif
    #endregion
        public AudioSource Play(AudioSource audioSourceParam = null)
        {
            if (clips.Length == 0)
            {
                Debug.Log($"Missing sound clips for {name}");
                return null;
            }
            var source = audioSourceParam;
            if (source == null)
            {
                var _obj = new GameObject("Sound", typeof(AudioSource));
                source = _obj.GetComponent<AudioSource>();
            }

            source.clip = clips[0];
            source.volume = Random.Range(volume.x, volume.y);
            source.pitch = Random.Range(pitch.x, pitch.y);

            source.Play();
            #if UNITY_EDITOR
            if (source == previewer)
            {
                Destroy(source.gameObject, source.clip.length / source.pitch);
            }
#else
            Destroy(source.gameObject, source.clip.length / source.pitch);
#endif
            return source;
        }
    }
}
