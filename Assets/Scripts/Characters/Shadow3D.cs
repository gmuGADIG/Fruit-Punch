using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// By default, projectors go through solid ground onto anything below it. <br/>
/// This script adjusts its far clip plane to only hit one thing. <br/>
/// (this is far from perfect but I can't imagine proper shadow casting would easy) 
/// </summary>
[ExecuteAlways, RequireComponent(typeof(Projector))]
public class Shadow3D : MonoBehaviour
{
    Projector proj;

    LayerMask groundLayer;

    RaycastHit[] hits = new RaycastHit[10];

    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
        this.GetComponentOrError(out proj);
    }

    void LateUpdate()
    {
        var dist = float.NegativeInfinity;
        
        var startPoint = transform.position;
        int hitCount = Physics.RaycastNonAlloc(new Ray(startPoint, Vector3.down), hits, 100f, groundLayer);
        
        if (hitCount== 0) dist = 100f;
        else
        {
            var highestPoint = hits.OrderBy(h => h.distance).First();
            dist = highestPoint.distance;
        }
        if (proj != null)
        {
            proj.farClipPlane = dist + 0.5f;
        }
    }
}
