using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Shadow3D : MonoBehaviour
{
    void LateUpdate()
    {
        var pos = transform.position;
        pos.y = 0;
        transform.position = pos;
    }
}
