using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerSelectorSpawner : MonoBehaviour
{
    public GameObject prefab;
    public GameObject blankPOneSelector;
    public GameObject blankPTwoSelector;
    public void ConnectPlayer(JoinContext context)
    {
        PlayerInput p = PlayerInput.Instantiate(
            prefab,
            controlScheme: context.ControlScheme,
            pairWithDevice: context.InputDevice
        );

        
        if (blankPOneSelector != null)
        {
            p.gameObject.transform.parent = GameObject.Find("Canvas").transform;
            p.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-250, -100, 0);
            Destroy(blankPOneSelector);
        }
        else if (blankPTwoSelector != null)
        {
            p.gameObject.transform.parent = GameObject.Find("Canvas").transform;
            p.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(250, -100, 0);
            Destroy(blankPTwoSelector);
        }
    }
}
