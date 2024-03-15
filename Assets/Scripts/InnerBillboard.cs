using System;
using UnityEngine;

/**
 * Gives a 3d mesh renderer an inner billboard, allowing 2d art to combine with the 3d world. <br/>
 * Replaces the material with a new InnerBillboard shader and tweaks its shader parameters. <br/>
 * Runs in the editor.
 */
[ExecuteAlways]
public class InnerBillboard : MonoBehaviour
{
    public Vector2 tempOffset;
    
    [Tooltip("Texture to billboard. Generally, this will be the sprite of the entire level. Texture must be its own image, not part of an atlas. Transparency not supported.")]
    [SerializeField] Texture texture;
    
    [Tooltip("This point is where the bottom-left of the image will be drawn. If placed at the bottom-left of the level, level objects don't need individual pixel offsets.")]
    [SerializeField] Transform anchor;
    
    [Tooltip("Shifts the texture by some pixels. Often necessary for objects with an anchor of itself.")]
    [SerializeField] Vector2Int pixelOffset;
    
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
    }
    
    void Update()
    {
        var cam = Camera.main;
        // get the anchor screen position (screen-space uv) then convert to texture uv
        var camOffsetScreenUv = (Vector2)cam.WorldToViewportPoint(anchor.position) + tempOffset;
        var camOffsetTexUv = -camOffsetScreenUv * new Vector2(referenceRes.x / texture.width, referenceRes.y / texture.height);

        // convert serialized offset to texture uv
        var paramOffsetTexUv = -new Vector2((float)pixelOffset.x / texture.width, (float)pixelOffset.y / texture.height);
        
        // convert to the uv coordinates of the texture
        var netOffset = camOffsetTexUv + paramOffsetTexUv;
        var netOffset4 = new Vector4(netOffset.x, netOffset.y, 0, 0);

        // apply to material
        Mat.SetVector("_UvOffset", netOffset4);
        Mat.SetVector("_CamToTexScale", new Vector4(texture.width / referenceRes.x,texture.height / referenceRes.y,0,0));
    }

    void OnValidate()
    {
        Mat.SetTexture("_MainTex", texture);
    }
}
