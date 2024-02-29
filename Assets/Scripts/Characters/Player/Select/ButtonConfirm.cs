using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonConfirm : MonoBehaviour
{
    [SerializeField]
    public Sprite[] images;
    // Start is called before the first frame update
    public void SetButton(string button)
    {

        Image img = gameObject.GetComponent<Image>();
        switch (button)
        {
            case "KeyboardLeft":
                img.sprite = images[0];
                break;
            case "KeyboardRight":
                img.sprite = images[1];
                break;

            case "Controller":
                img.sprite = images[2];
                break;
        }
    }


}
