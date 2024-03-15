using System;
using UnityEngine;

[ExecuteAlways]
public class InnerBillboard : MonoBehaviour
{
    [SerializeField] Vector2Int pixelOffset;
    [SerializeField] Texture texture;
    readonly Vector2 referenceRes = new Vector2(448, 252); // would be better to fetch this from the main camera but.. eh

    Material _mat;
    Material Mat
    {
        get
        {
            if (_mat == null)
            {
                _mat = new Material(Shader.Find("Custom/InnerBillboard"));
                GetComponent<Renderer>().material = _mat;
            }

            return _mat;
        }
        set => _mat = value;
    }
    
    void Update()
    {
        // TODO: this needs to use reference pixels, not final screen pixels
        var cam = Camera.main;
        // get the screen-distance from the center of the camera to the transform's anchor. this uses final screen pixels, NOT reference pixels
        var camOffsetScreenPx = (Vector2)cam.WorldToScreenPoint(transform.position) - new Vector2(cam.pixelWidth, cam.pixelHeight)/2;
        // convert to reference pixels
        var camOffsetRefPx = camOffsetScreenPx * new Vector2(referenceRes.x / cam.pixelWidth, referenceRes.y / cam.pixelHeight);
        // apply serialized offset
        var netOffsetPx = pixelOffset + camOffsetRefPx;
        
        
        // convert to the uv coordinates of the texture
        var netOffsetUv = -new Vector2(netOffsetPx.x / texture.width, netOffsetPx.y / texture.height);
        var netOffset4 = new Vector4(netOffsetUv.x, netOffsetUv.y, 0, 0);

        // apply to material
        Mat.SetVector("_UvOffset", netOffset4);
        Mat.SetVector("_CamToTexScale", new Vector4(texture.width / referenceRes.x,texture.height / referenceRes.y,0,0));
    }

    void OnValidate()
    {
        Mat.SetTexture("_MainTex", texture);
    }
}
