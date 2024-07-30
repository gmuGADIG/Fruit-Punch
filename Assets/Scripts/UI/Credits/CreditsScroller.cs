using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float scrollSpeed = 1f;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            target.Translate(3f * scrollSpeed * Time.deltaTime * Vector3.up);
        }
        else
        {
            target.Translate(scrollSpeed * Time.deltaTime * Vector3.up);
        }
    }
}
