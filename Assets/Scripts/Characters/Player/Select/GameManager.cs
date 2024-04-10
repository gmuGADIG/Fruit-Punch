using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    //singlton instance
    public static GameManager gameManager;

    //player's character
    public Character playerOne = Character.None;
    public Character playerTwo = Character.None;

    //player's Input Devices
    public InputDevice playerOneInputDevice;
    public InputDevice playerTwoInputDevice;

    //player's Control Schemes
    public string playerOneControlScheme;
    public string playerTwoControlScheme;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        //singlton check
        if (gameManager == null)
            gameManager = this;
        else
            Destroy(this);
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
