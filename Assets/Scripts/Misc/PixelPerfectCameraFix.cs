
using System;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// Attaching this script next to a PixelPerfectCamera allows `Stretch Fill` to be used without enabling bilinear sampling (blurs and ruins pixel art). <br/>
/// (I'm pissed af this needs it's own script. If you want to join in that misery: https://forum.unity.com/threads/pixel-perfect-camera-should-not-have-bilinear-as-a-filter-mode-option.721760/)
/// </summary>
[RequireComponent(typeof(PixelPerfectCamera))]
[ExecuteAlways]
public class PixelPerfectCameraFix : MonoBehaviour
{
#if UNITY_EDITOR
    void OnValidate()
    {
        GetComponent<PixelPerfectCamera>().runInEditMode = true;
    }
#endif
    void OnPostRender()
    {
        Camera.main.activeTexture.filterMode = FilterMode.Point;
    }
}
