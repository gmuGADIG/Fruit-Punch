using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class InnerBillboard : MonoBehaviour
{
    [SerializeField] Vector2Int pixelOffset;
    [SerializeField] Vector2 spriteRes;

    void Update()
    {
        var cam = Camera.main;
        var camOffsetPx = (Vector2)cam.WorldToScreenPoint(transform.position) - new Vector2(cam.pixelWidth, cam.pixelHeight)/2;
        var netOffsetPx = pixelOffset + camOffsetPx;
        
        var netOffsetUv = -new Vector2(netOffsetPx.x / spriteRes.x, netOffsetPx.y / spriteRes.y);
        var netOffset4 = new Vector4(netOffsetUv.x, netOffsetUv.y, 0, 0);

        var mat = GetComponent<Renderer>().material; 
        mat.SetVector("_UvOffset", netOffset4);
        mat.SetVector("_CamToTexScale", new Vector4(spriteRes.x / cam.pixelWidth,spriteRes.y / cam.pixelHeight,0,0));
    }
}
