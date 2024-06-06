using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BasicCutsceneCamera : MonoBehaviour
{
    [System.Serializable]
    public class PanelData
    {
        public float approachTime;
        public Vector3 panelPosition;
        public float hangTime;
        public float cameraScale;
    }

    public PanelData[] testComic;

    public Camera comicCamera;

    public bool cameraMoving = false;

    // panelSkipped is constantly reset to false to prepare for the user(s) to set it to true again.
    // It gets set to true when any player hits their strike input.
    public bool panelSkipped = false;

    public string nextScene = "Scenes/Build Scenes/DemoLevel02";

/// <summary>
/// 
/// 
///     PanelData class tells us:
///             - approachTime: Time (seconds) camera spends tweening towards this panel
///             - panelPosition: Vector2 position of each panel
///             - hangTime: Time (seconds) camera hangs on panel
///             - cameraScale: Scale (float) of camera (amount of camera zoom per panel, def. 5) for panel
///     
///     We have an Array of PanelData. We make an array for every comic (at least 4)
///     We use a for loop to iterate through this array and use the PanelData within to animate the camera.
///     
/// 
///     To make your own comic, simply make a new PanelData[] array and edit the values in the inspector! Watch the camera zip around! Wow!
/// 
///     The game window is 448x252 pixels, so panels ideally should be at least that much, but can go as high 
///     resolution as the artist wants (within reason) because the camera can zoom as far out as we want.
/// 
///     Players can use a strike attack to skip wait times and advance panels early if they want.
///     The cutscene only moves forward, there's no logic for going backwards.
/// 
/// 
/// </summary>



    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AnyInput>().performed += Skip;
        comicCamera=this.GetComponent<Camera>();
        StartCoroutine(TestComicRoutine());
    }


    IEnumerator TestComicRoutine()
    {
        // The main loop of the comic system is to go somewhere from a prior position. There is no prior position to the first panel, and so
        // the first panel gets special treatment and is handled before entering the main loop.

        // Start camera in first position.
        this.transform.position = testComic[0].panelPosition;
        // Set camera size for first panel.
        comicCamera.orthographicSize = testComic[0].cameraScale;
        
        
        
        // Hang on first panel for set time.        
        //OLD METHOD: yield return new WaitForSeconds(testComic[0].hangTime);
        //NEW MORE COMPLICATED METHOD THAT LETS YOU SKIP:
        float elapsedHangTime = 0.0f;
        panelSkipped=false;

        while ( elapsedHangTime <= testComic[0].hangTime && !panelSkipped )
        {
            elapsedHangTime+=Time.deltaTime;
            
            // Wait for the next frame to keep counting.
            yield return null;
        }

        panelSkipped=false;

        // For each item in the array, i.e., for each panel in your comic, AFTER THE FIRST (i = 1):
        for(int i = 1; i < testComic.Length; i++)
        {
            float elapsedTime = 0.0f;

            // We lerp on a loop until the lerps are done. i.e., we move the camera.
            while (elapsedTime < testComic[i].approachTime && !panelSkipped)
            {
                // Lerp POSITION with approachtime to target panel destination
                transform.position = Vector3.Lerp(transform.position, testComic[i].panelPosition, (elapsedTime / testComic[i].approachTime));
                // Lerp CAMERA SCALE with approachtime to target camerascale
                comicCamera.orthographicSize = Mathf.Lerp(comicCamera.orthographicSize, testComic[i].cameraScale, (elapsedTime / testComic[i].approachTime));
                // Keeping track of the change in time for framerate independence.
                elapsedTime += Time.deltaTime;
                
                // Wait for the next frame to continue lerping.
                yield return null;
            }
            // These next 2 lines are redundant- UNLESS they hit an input to skip the transition.
            // They're not in a "if panelSkipped" check because they're also a good fallback for safety.
            transform.position = testComic[i].panelPosition;
            comicCamera.orthographicSize = testComic[i].cameraScale;

            // yield for hangtime before running again
            //OLD: yield return new WaitForSeconds(testComic[i].hangTime);
            //NEW:
            elapsedHangTime = 0.0f;
            panelSkipped=false;
            while ( elapsedHangTime <= testComic[i].hangTime && !panelSkipped )
            {
                elapsedHangTime += Time.deltaTime;
                // Wait for the next frame to keep counting.
                yield return null;
            }

            // If there are any panels left, we return to the top of the foor loop on the next panel.
        }
        // Wait a frame, though, because IEnumerator said so.
        yield return null;
        SceneManager.LoadScene(nextScene);
    }

    void Skip(InputAction.CallbackContext _context)
    {
        panelSkipped = true;
    }
}
