using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameOverScreen : MonoBehaviour
{
    [Tooltip ("Name of game over scene")]
    public string GameOverScene;

    //currently only set up for one player death, will work on making it work for 1 and 2 player

    Health playerHealth;
    // Start is called before the first frame update
    void Awake()
    {
        playerHealth = FindObjectOfType<Health>();
        
    }
    

    // Update is called once per frame
    void Update()
    {
        DamageInfo damageInfo = new DamageInfo(100, Vector2.right, AuraType.Strike);

        if (Input.GetKeyDown(KeyCode.J))
        {
            playerHealth.Damage(damageInfo);
            Debug.Log("Should be damaged");
        }
    }

    /// <summary>
    /// Load Scene when player dies
    /// </summary>
    private void DoLoadGameOverScreen()
    {
        SceneManager.LoadSceneAsync(GameOverScene, LoadSceneMode.Additive);
    }
    
   
    private void OnEnable()
    {
        playerHealth.onDeath += DoLoadGameOverScreen; 
    }

    
}
