using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private bool enemyOnScreen = true;
    private int NumEnemy = 0; //do the tags for the enemies
    public Transform target; // Reference to the player's Transform component
    public Vector3 offset = new Vector3(0f, 2f); // Adjust as needed
    public GameObject player;
    public Vector3 startPosition = new Vector3(0f, 2f); // Adjust as needed


    // Start is called before the first frame update
    void Start()
    {
        enemyOnScreen = true;
        transform.position = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (NumEnemy == 0)
        {
            enemyOnScreen = false; //makes enemyOnScren false when there are no enemies
        }

        if (!enemyOnScreen || target == null)
            return; //stops if player is dead or there are no enemies on screen
        transform.position = target.position + offset;
    }


}
