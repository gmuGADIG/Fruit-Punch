using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Important script for all objects with motion or hitboxes/hurtboxes. <br/>
/// Gives an object an internal 3d position and maps that to a real 2d position in the transform. <br/>
/// Also allows detecting collisions with other BeltController objects, filtering only the ones with a similar z-position. <br/>
/// Provides functionality for clamping the forward/backwards axis to certain values, simulating front and back walls. <br/>
/// Currently does not provide wall collision; may be added later if needed. <br/>
/// </summary>
[ExecuteAlways]
public class BeltCharacter : MonoBehaviour
{
    public static float zMin = -1, zMax = 0;
    
    /// <summary>
    /// Moves the real position by this much for each internal z movement. <br/>
    /// x means characters further back will be shifted left (negative) or right (position) <br/>
    /// y determines how quickly you move forward/backward compared to left/right. It should be less than 1 to sell the effect. <br/>
    /// z changes how much the z draw index is affected by internal z position. <br/>
    /// </summary>
    public static Vector3 zTranslation = new Vector3(0, .6f, 0.01f);
    
    /// <summary>
    /// The internal position storing left, right, and forward coordinates, separate from the 2d position at which the object appears.
    /// </summary>
    [ReadOnlyInInspector] public Vector3 internalPosition;
    
    private void Start()
    {
        // pos.xy = (internal.x + zTrans.x * internal.z, internal.y + zTrans.y * internal.z)
        // let internal.y and pos.z be 0, find internal.xz given pos.xy
        // internal.z = pos.y / zTrans.y
        // internal.x = pos.x - zTrans.x * internal.z
        internalPosition.z = transform.position.y / zTranslation.y;
        internalPosition.x = transform.position.x - zTranslation.x * internalPosition.z;
    }
    
    [ExecuteAlways]
    void Update()
    {
        // in the editor, edit the internal position and nothing else
        if (Application.isPlaying == false)
        {
            internalPosition.z = transform.position.y / zTranslation.y;
            internalPosition.x = transform.position.x - zTranslation.x * internalPosition.z;
            return;
        }
        
        // set transform position based on internal position, and clamp z-position
        internalPosition.z = Mathf.Clamp(internalPosition.z, zMin, zMax);
        transform.position = internalPosition + zTranslation * internalPosition.z;
    }

    /// <summary>
    /// Returns all colliding BeltCharacters, based on the z-position of `this`.
    /// </summary>
    public List<BeltCharacter> GetOverlappingBeltCharacters(Collider2D collider, LayerMask layers, float zTolerance = 0.5f)
    {
        var filter = new ContactFilter2D().NoFilter();
        filter.SetLayerMask(layers);
        var collisionHits = new List<Collider2D>(64);
        int hitCount = collider.OverlapCollider(filter, collisionHits);
        return collisionHits
            .Take(hitCount)
            .Select(x => x.GetComponent<BeltCharacter>())
            .Where(x => x != null)
            .Where(SimilarZPosition)
            .ToList();

        /* True if the belt character is in a similar z-position */
        bool SimilarZPosition(BeltCharacter beltChar)
        {
            var zDistance = Mathf.Abs(beltChar.internalPosition.z - this.internalPosition.z);
            return zDistance <= zTolerance;
        }
    }
}
