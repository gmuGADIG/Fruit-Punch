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
        var startPoint = transform.position + Vector3.up * 10f; // slight offset so it doesn't start below the floor
        var hits = Physics.RaycastAll(startPoint, Vector3.down, 100f, LayerMask.GetMask("Ground"));
        var highestY = hits.OrderBy(h => h.distance).First().point.y + 0.05f;
        var pos = transform.position;
        pos.y = highestY;
        transform.position = pos;
    }
}
