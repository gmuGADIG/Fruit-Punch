using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float scrollSpeed = 1f;

    private void Update()
    {
        target.Translate(scrollSpeed * Time.deltaTime * Vector3.up);
    }
}
