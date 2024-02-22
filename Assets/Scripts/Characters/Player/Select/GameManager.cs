using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public Character playerOne = Character.None;
    public Character playerTwo = Character.None;
    public InputDevice playerOneInputDevice;
    public InputDevice playerTwoInputDevice;
    public string playerOneControlScheme;
    public string playerTwoControlScheme;


    void Start()
    {
        if (gameManager == null)
            gameManager = this;
    }

    /// <summary>
    /// check to see how many player there are.
    /// </summary>
    /// <returns> 0 if no </returns>
    public int PlayerCount()
    {
        int r = 0;
        if (playerOne != Character.None)
            r++;
        if (playerTwo != Character.None)
            r++;
        return r;
    }

    
    

}
