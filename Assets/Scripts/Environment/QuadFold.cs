using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class QuadFold : MonoBehaviour
{
    void Start()
    {
        AdjustCollider();
    }

    void Update()
    {
        if (Application.isPlaying) return;
        else AdjustCollider();
    }
    
    void AdjustCollider()
    {
        // get box component
        var box = transform.GetChild(2).GetComponent<BoxCollider>();
        if (box == null || box.enabled == false) return;
        
        // get sprite components
        var topSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        var sideSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();

        // get extents and correction factor
        var topExtents = topSprite.bounds.extents;
        var sideExtents = sideSprite.bounds.extents;
        var correctionFactor = 1; // get the factor which SpriteFixer calculates
        
        // calculate box size
        var center = new Vector3();
        var size = new Vector3();

        var depth = topExtents.z * 2 * correctionFactor;
        center.z = depth / 2;
        size.z = depth;

        var height = sideExtents.y * 2 * correctionFactor;
        center.y = -height / 2; // the negative assumes the fold represents the top and front side of a box. for things like ground/sky folds, this is false.
        size.y = height;

        center.x = 0;
        size.x = topExtents.x * 2;

        // apply it
        box.center = center;
        box.size = size;
    }
}
