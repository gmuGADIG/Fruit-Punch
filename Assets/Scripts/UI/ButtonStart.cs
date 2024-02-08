using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonStart : MonoBehaviour
{
    public void GameStart()
    {
        Debug.Log("Start!");
    }

    public void GameOptions()
    {
        Debug.Log("Options!");
    }
    
    public void GameQuit()
    {
        Application.Quit();
    }
}
