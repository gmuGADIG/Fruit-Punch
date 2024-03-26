using UnityEngine;

/// <summary>
/// Toggles a collider on and off at regular intervals. <br/>
/// Does not handle hazard code. That will either be done with HurtBox or a similar script.
/// </summary>
[RequireComponent(typeof(Collider))]
public class OnOffHazard : MonoBehaviour
{
    public float onTime = 2;
    public float offTime = 2;

    float timeTillChange = 0f;
    bool currentlyOn;

    private Collider col;

    private void Start()
    {
        this.GetComponentOrError(out col);
    }

    void Update()
    {
        timeTillChange -= Time.deltaTime;
        if (timeTillChange <= 0)
        {
            currentlyOn = !currentlyOn; // toggle it
            col.enabled = currentlyOn;
            timeTillChange += currentlyOn ? onTime : offTime;
        }
    }
}
