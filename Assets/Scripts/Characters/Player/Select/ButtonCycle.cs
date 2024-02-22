using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCycle : MonoBehaviour
{
    [SerializeField]
    public Sprite[] images;
    Image buttonImage;
    private int index;
    // Start is called before the first frame update
    void Start()
    {
        buttonImage = gameObject.GetComponent<Image>();
        index = 0;
        StartCoroutine(Cycle());
    }

    IEnumerator Cycle()
    {
        buttonImage.sprite = images[index];
        index++;
        if (index >= images.Length)
            index = 0;
        yield return new WaitForSeconds(1);
        StartCoroutine(Cycle());
    }


}
