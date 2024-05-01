using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // singleton instance
    public static GameManager gameManager;

    //player's character
    public readonly Character[] characters = new[] {Character.None, Character.None};

    //player's Input Devices
    public readonly InputDevice[] playerInputDevices = {null, null};

    //player's Control Schemes
    public readonly string[] playerControlSchemes = {null, null};


    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // singleton check
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
        if (characters[0] == Character.None) return 0;
        else if (characters[1] == Character.None) return 1;
        else return 2;
    }
}
