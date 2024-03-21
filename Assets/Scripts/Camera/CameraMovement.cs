using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BasicCutsceneCamera;

public class CameraMovement : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawns
    {

        public Vector3 spawnLocation;
    }

    [System.Serializable]//The camera moves between scenes using a serializable object
    public class Scene
    {
        public enum Transition
        {
            Soft,
            Hard //Note: enemy spawns are currently ignored on hard transition
        }
        
        public Transition transitionType;
        public Vector3 transitionLocation;

        public EnemySpawns[] enemySpawns;
    }
    [SerializeField]
    public Scene[] scenes;
    public int currentSceneNumber;
    private Scene currentScene;


    [SerializeField]
    float cameraFreezeThreshold = 1.0f; //The distance from the freeze location before it triggers the start.


    public Vector3 offset = new Vector3(0f, 2f); // Adjust as needed
    public Vector3 startPosition = new Vector3(0f, 2f); // Adjust as needed
    public float smoothSpeed = 0.125f; // Smoothness of camera movement
    public bool isSpawn = false; // Boolean variable from another script
    private bool areEnemiesPresent = false; // Boolean to track if enemies are present
    public Vector3 frozenPos;

   
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
        currentSceneNumber = 0;
        currentScene = scenes[0];
        transform.position = startPosition;
        currentState = CameraState.follow;
        players = FindObjectsOfType<Player>().ToList();
    }
    public void FreezeCamera(Vector3 pos)
    {
        currentState = CameraState.frozen;
        frozenPos = pos;
    }

    public void UnfreezeCamera()
    {
        currentSceneNumber++;
        currentScene = scenes[currentSceneNumber];

        if (currentScene.transitionType==Scene.Transition.Soft) {
            currentState = CameraState.follow;
        }
        else
        {
            HardTransition();
        }
    }

    public void HardTransition()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*// Check if enemies are present using the EnemyDetector script
        if (!EnemyDetector.AreEnemiesPresent && !isSpawn)
        {
            areEnemiesPresent = false;
        }
        else
        {
            areEnemiesPresent = true;
        }*/

        // Move the camera to follow the player if conditions are met
        if (currentState == CameraState.follow)
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
            smoothedPosition.y = offset.y;
            transform.position = smoothedPosition;

            //Detects if the player is close enough to the next scene

            if (Vector3.Distance(transform.position,currentScene.transitionLocation)<cameraFreezeThreshold)
            {
                FreezeCamera(currentScene.transitionLocation);
            }
        }
        else if (currentState == CameraState.frozen)
        {
            if (Vector3.Distance(transform.position,frozenPos)>0.001) {
                Debug.Log("Smoothing");
                transform.position = Vector3.Lerp(transform.position, frozenPos, smoothSpeed);
            }
            if (Input.GetKey("space"))
            {
                UnfreezeCamera();
            }
        }
    }


}
