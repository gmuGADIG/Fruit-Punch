using System.Collections;
using UnityEngine;

/// <summary>
/// Destroys game object after a duration of time.
/// </summary>
public class Lifetime : MonoBehaviour
{
    [Tooltip("How long the object will exist before being automatically destroyed.")]
    [SerializeField] float lifetime = 10f;

    IEnumerator Start() {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
