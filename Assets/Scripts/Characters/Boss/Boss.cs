using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all bosses. <br/>
/// Does not handle state machine, but provides some functions which subclasses may want to call.
/// </summary>
public class Boss : MonoBehaviour
{
    protected IEnumerator WalkToPlayer()
    {
        // TODO: find player and approach them. exit when close enough or can't get closer.
        yield return null;
    }
}
