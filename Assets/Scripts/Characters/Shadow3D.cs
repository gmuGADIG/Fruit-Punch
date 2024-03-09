using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class Shadow3D : MonoBehaviour
{
    void LateUpdate()
    {
        var pos = transform.position;
        
        var startPoint = transform.parent.position + Vector3.up * 0.1f; // start slightly above the parent
        var hits = Physics.RaycastAll(startPoint, Vector3.down, 100f, LayerMask.GetMask("Ground"));
        
        if (hits.Length == 0) pos.y = -100f;
        else
        {
            var highestPoint = hits.OrderBy(h => h.distance).First();
            pos.y = highestPoint.point.y + 0.05f;
        }

        transform.position = pos;
    }
}
