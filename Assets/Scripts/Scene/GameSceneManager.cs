using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

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
        public ScreenSpawner spawner;

    }

    public GameObject mainCamera;
    [SerializeField]
    float cameraFreezeThreshold = 1.0f; //The distance from the freeze location before it triggers the start.
    [SerializeField]
    public float cameraSmoothSpeed = 0.125f; // Smoothness of camera movement
    [SerializeField]
    public Vector3 startPosition = new Vector3(0f, 2f); // Adjust as needed
    [SerializeField]
    public Vector3 cameraOffset = new Vector3(0f, 2f); // Adjust as needed

    [SerializeField]
    public Scene[] scenes;

    private int currentSceneNumber;
    private Scene currentScene;
    
    private bool areEnemiesPresent = false; // Boolean to track if enemies are present
    private Vector3 frozenPos;

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
        mainCamera = Camera.main.gameObject;
        if (mainCamera == null)
            Debug.LogError("GameSceneManager: Main Camera not set, please set in inspector.", gameObject);
        if (scenes.Length <= 0)
        {
            Debug.LogWarning("Invalid number of scenes");
        }
        else
        {
            currentSceneNumber = 0;
            currentScene = scenes[0];
        }
        mainCamera.transform.position = startPosition;
        currentState = CameraState.follow;
        players = FindObjectsOfType<Player>().ToList();
    }
    public void FreezeCamera(Vector3 pos)
    {
        currentState = CameraState.frozen;
        frozenPos = pos;
        Debug.Log("Spawner exits?");
        if (currentScene.spawner != null)
        {
            Debug.Log("Yes!!");
            currentScene.spawner.StartSpawning();
        }
    }

    public void UnfreezeCamera()
    {
        currentSceneNumber++;
        if (currentSceneNumber >= scenes.Length)
        {
            Debug.LogWarning("GameSceneManager: Out of scenes, recycling last scene");//TODO: remove the recycle for when there is a proper end to the level
            currentSceneNumber--;
        }

        currentScene = scenes[currentSceneNumber];

        if (currentScene.transitionType==Scene.Transition.Soft) {
            currentState = CameraState.follow;
        }
        else
        {
            currentState = CameraState.follow;
            HardTransition();
        }
    }

    public void HardTransition()
    {
        Vector3 oldCameraLocation = mainCamera.transform.position;
        foreach (Player p in players)
        {
            Vector3 playerOffset = p.transform.position - (oldCameraLocation-cameraOffset);
            p.transform.position = currentScene.transitionLocation+ playerOffset;
        }
        mainCamera.transform.position = currentScene.transitionLocation+cameraOffset;
        UnfreezeCamera();
    }

    // Update is called once per frame
    void Update()
    {
        // Move the camera to follow the player if conditions are met
        if (currentState == CameraState.follow)
        {
            Vector3 averagePos = Vector3.zero;
            for (int i = 0; i < players.Count; i++)
            {
                averagePos += players[i].transform.position;
            }
            averagePos /= players.Count;
            averagePos += cameraOffset;
            averagePos.z = mainCamera.transform.position.z;

            Camera cam = Camera.main;
            float height = 2f * cam.orthographicSize;
            float width = height * cam.aspect;
            var halfWidth = width * .5f;

            var x = Mathf.Clamp(
                mainCamera.transform.position.x,
                averagePos.x - halfWidth * .3f,
                averagePos.x + halfWidth * .8f
            );

            var newPos = mainCamera.transform.position;
            newPos.x = x;
            mainCamera.transform.position = newPos;

            // Vector3 smoothedPosition = Vector3.Lerp(mainCamera.transform.position, averagePos, cameraSmoothSpeed);
            // smoothedPosition.y = Mathf.Round(cameraOffset.y);
            // mainCamera.transform.position = smoothedPosition;

            //Detects if the player is close enough to the next scene
            if (Vector3.Distance(mainCamera.transform.position-cameraOffset,currentScene.transitionLocation)<cameraFreezeThreshold)
            {
                FreezeCamera(currentScene.transitionLocation+cameraOffset);
            }
        }
        else if (currentState == CameraState.frozen)
        {
            if (Vector3.Distance(mainCamera.transform.position,frozenPos)>0.001) {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, frozenPos, cameraSmoothSpeed);
            }
            if (Input.GetKey("space"))
            {
                UnfreezeCamera();
            }
        }
    }


}
