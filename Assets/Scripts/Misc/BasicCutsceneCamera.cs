using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public bool panelSkipped = false;


    [SerializeField] InputActionAsset actions;


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
///     We use a for loop to iterate through this array and use the PanelData within to animate the camera
///     
///     TODO: The player should have the option of skipping the hang time by using their primary strike input, immediately going to the next panel.
/// 
///     To make your own comic, simply make a new PanelData[] array and edit the values in the inspector! Watch the camera zip around! Wow!
/// 
///     The game window is 448x252 pixels, so panels ideally should be at least that much, but can go as high 
///     resolution as the artist wants (within reason) because the camera can zoom as far out as we want.
/// 
/// 
/// 
/// </summary>



    // Start is called before the first frame update
    void Start()
    {
        actions["gameplay/Strike"].performed += Skip;
        comicCamera=this.GetComponent<Camera>();
        StartCoroutine(testComicRoutine());
    }


    IEnumerator testComicRoutine()
    {
        // The main loop of the comic system is to go somewhere from a prior position. There is no prior position to the first panel, and so
        // the first panel gets special treatment and is handled before entering the main loop.

        // Start camera in first position.
        this.transform.position = testComic[0].panelPosition;
        // Set camera size for first panel.
        comicCamera.orthographicSize = testComic[0].cameraScale;
        // Hang on first panel for set time.        
        yield return new WaitForSeconds(testComic[0].hangTime);

        //this.transform.position=Vector3.Lerp(transform.position, testComic[1].panelPosition, testComic[1].approachSpeed*Time.deltaTime);


        // For each item in the array, i.e., for each panel in your comic, AFTER THE FIRST:
        for(int i = 1; i < testComic.Length; i++)
        {
            float elapsedTime = 0.0f;

            // We lerp on a loop until the lerps are done. i.e., we move the camera.
            while (elapsedTime < testComic[i].approachTime)
            {
                // Lerp POSITION with approachtime to target panel destination
                transform.position = Vector3.Lerp(transform.position, testComic[i].panelPosition, (elapsedTime / testComic[i].approachTime));
                // Lerp CAMERA SCALE with approachtime to target camerascale
                comicCamera.orthographicSize = Mathf.Lerp(comicCamera.orthographicSize, testComic[i].cameraScale, (elapsedTime / testComic[i].approachTime));
                // Keeping track of the change in time for framerate independence.
                elapsedTime += Time.deltaTime;

                // I don't remember why I put this in here but I'm not touching it because it's late and I'm scared.
                yield return null;
            }

            // yield for hangtime before running again
            yield return new WaitForSeconds(testComic[i].hangTime);
            
            //float elapsedHangTime = 0.0f;
            //panelSkipped=false;
            //while(elapsedHangTime<=testComic[i].hangTime&&panelSkipped==false)
            //{
            //    elapsedHangTime+=Time.deltaTime;
            //}

            // PLAN FOR REPLACING THE YIELD (THIS COULD GET UGLY):
            // panelSkipped=false
            // while [variable we add deltatime to] <= hangTime && skipped==false:
            //              [that variable]+=deltatime


            // If there are any panels left, we return to the top of the foor loop on the next panel.
        }
        // I do remember why I put this in here and it's because IEnumerator wants it apparently.
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Skip(InputAction.CallbackContext _context)
    {
        Debug.Log("aaaaaa");
        panelSkipped=true;
    }
}
