using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Processors;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{   
    [System.Serializable]//The camera moves between scenes using a serializable object
    public class Scene
    {
        public Vector3 transitionLocation;
        public ScreenSpawner spawner;
    }
    
    [SerializeField]
    float cameraFreezeThreshold = 1.0f; //The distance from the freeze location before it triggers the start.
    [SerializeField]
    public float cameraSmoothSpeed = 0.125f; // Smoothness of camera movement
    [SerializeField]
    public Vector3 cameraOffset = new Vector3(0f, 2f); // Adjust as needed

    [SerializeField]
    public Scene[] scenes;

    [Tooltip("When the final battle is finished, the scene with this name will be loaded.")]
    public string nextStage;

    private int currentSceneNumber;
    private Scene currentScene;
    
    // private bool areEnemiesPresent = false; // Boolean to track if enemies are present
    private Vector3 frozenPos;
    
    private GameObject mainCamera;

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

        currentState = CameraState.follow;
    }

    public void FreezeCamera(Vector3 pos)
    {
        currentState = CameraState.frozen;
        frozenPos = pos;
        if (currentScene.spawner == null) Debug.LogError($"screen {currentSceneNumber} has null spawner!");
        else if (currentScene.spawner.gameObject.activeInHierarchy)
        {
            currentScene.spawner.StartSpawning();
            currentScene.spawner.onWaveComplete.AddListener(UnfreezeCamera);
        } else {
            UnfreezeCamera();
        }
    }

    public void UnfreezeCamera()
    {
        currentSceneNumber++;
        if (currentSceneNumber >= scenes.Length) // final screen; open final results menu
        {
            FindAnyObjectByType<EndLevelResultsUI>().ShowMenu();
            return;
        }

        currentScene = scenes[currentSceneNumber];
        currentState = CameraState.follow;
    }

    /// <summary>
    /// Gets the average player position offset by cameraOffset and the z component set to mainCamera's z position.
    /// </summary>
    /// <returns>Definitely the average player position.</returns>
    Vector3 GetAveragePlayerPosition() {
        Vector3 averagePos = Vector3.zero;
        for (int i = 0; i < players.Count; i++)
        {
            averagePos += players[i].transform.position;
        }
        averagePos /= players.Count;
        averagePos += cameraOffset;
        averagePos.z = mainCamera.transform.position.z;

        return averagePos;
    }

    // Update is called once per frame
    void Update()
    {
        if (players.Count == 0)
        {
            players = FindObjectsOfType<Player>().ToList();
        }
        
        // Move the camera to follow the player if conditions are met
        if (currentState == CameraState.follow)
        {
            Vector3 averagePos = GetAveragePlayerPosition();

            Camera cam = Camera.main;
            float height = 2f * cam.orthographicSize;
            float width = height * cam.aspect;
            var halfWidth = width * .5f;

            var x = Mathf.Clamp(
                mainCamera.transform.position.x,
                averagePos.x - halfWidth * .3f,
                mainCamera.transform.position.x
                //averagePos.x + halfWidth * .8f
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
            
            // Debug advance
            if (Application.isEditor && Input.GetKey(KeyCode.Tilde))
            {
                UnfreezeCamera();
            }
        }
    }

    public void GoToNextScene()
    {
        FindAnyObjectByType<FadeOutUI>().FadeOutToNextScene(nextStage);
    }
}
