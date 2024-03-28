using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BasicCutsceneCamera;

public class GameSceneManager : MonoBehaviour
{   
    [System.Serializable]//The camera moves between scenes using a serializable object
    public class Scene
    {
        public enum Transition
        {
            Soft,
            Hard, //Note: enemy spawns are currently ignored on hard transition
            End
        }
        
        public Transition transitionType;
        public Vector3 transitionLocation;

        [System.Serializable]
        public class EnemySpawns
        {

            public Vector3 spawnLocation;
        }

        public EnemySpawns[] enemySpawns;
    }



    [SerializeField]
    public GameObject camera;
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
        if (camera == null)
            Debug.LogError("GameSceneManager: Main Camera not set, please set in inspector.");

        currentSceneNumber = 0;
        currentScene = scenes[0];
        camera.transform.position = startPosition;
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
        if (currentSceneNumber >= scenes.Length)
        {
            Debug.Log("Out of scenes, recycling last scene");//TODO: remove the recycle for when there is a proper end to the level
            currentSceneNumber--;
        }

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
            averagePos.z = camera.transform.position.z;
            Vector3 smoothedPosition = Vector3.Lerp(camera.transform.position, averagePos, smoothSpeed);
            smoothedPosition.y = offset.y;
            camera.transform.position = smoothedPosition;

            //Detects if the player is close enough to the next scene

            if (Vector3.Distance(camera.transform.position,currentScene.transitionLocation)<cameraFreezeThreshold)
            {
                Debug.Log("Freeze");
                FreezeCamera(currentScene.transitionLocation);
            }
        }
        else if (currentState == CameraState.frozen)
        {
            if (Vector3.Distance(camera.transform.position,frozenPos)>0.001) {
                Debug.Log("Smoothing");
                camera.transform.position = Vector3.Lerp(camera.transform.position, frozenPos, smoothSpeed);
            }
            if (Input.GetKey("space"))
            {
                UnfreezeCamera();
            }
        }
    }


}
