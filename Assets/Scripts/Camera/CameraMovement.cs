using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target; // Reference to the player's Transform component
    public Vector3 offset = new Vector3(0f, 2f); // Adjust as needed
    public GameObject player;
    public Vector3 startPosition = new Vector3(0f, 2f); // Adjust as needed
    public Transform playerTransform; // Reference to the player's transform
    public float smoothSpeed = 0.125f; // Smoothness of camera movement
    public bool isSpawn = false; // Boolean variable from another script
    private bool areEnemiesPresent = false; // Boolean to track if enemies are present



    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if enemies are present using the EnemyDetector script
        if (!EnemyDetector.AreEnemiesPresent && !isSpawn)
        {
            areEnemiesPresent = false;
        }
        else
        {
            areEnemiesPresent = true;
        }

        // Move the camera to follow the player if conditions are met
        if (!areEnemiesPresent && !isSpawn && playerTransform != null)
        {
            Vector3 desiredPosition = playerTransform.position;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }


}
