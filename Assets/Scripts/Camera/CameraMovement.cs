using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 2f); // Adjust as needed
    public Vector3 startPosition = new Vector3(0f, 2f); // Adjust as needed
    public float smoothSpeed = 0.125f; // Smoothness of camera movement
    public bool isSpawn = false; // Boolean variable from another script
    private bool areEnemiesPresent = false; // Boolean to track if enemies are present
    public enum CameraState
    {
        follow, frozen
    }
    private CameraState currentState;
    private float avgPos;
    private List<Player> players = new List<Player>();

    // Start is called before the first frame updae
    void Start()
    {
        transform.position = startPosition;
        currentState = CameraState.follow;
        players = FindObjectsOfType<Player>().ToList();
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
        if (!areEnemiesPresent && !isSpawn)
        {
            Vector3 averagePos = Vector3.zero;
            for (int i = 0; i < players.Count; i++)
            {
                averagePos += players[i].transform.position;
            }
            averagePos /= players.Count;
            averagePos += offset;
            averagePos.z = transform.position.z;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, averagePos, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }


}
