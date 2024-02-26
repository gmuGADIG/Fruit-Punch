using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Billboard : MonoBehaviour
{
    void Update()
    {
        this.transform.localRotation = Camera.main.transform.rotation;
    }
}
