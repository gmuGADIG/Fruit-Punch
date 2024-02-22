using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameOverScreen : MonoBehaviour
{
    [Tooltip ("Name of game over scene")]
    public string GameOverScene;

    Health playerHealth;
    // Start is called before the first frame update
    void Start()
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
    /// Should Load Scene when player dies?
    /// </summary>
    public void onDeath()
    {
        SceneManager.LoadSceneAsync(GameOverScene, LoadSceneMode.Additive);
    }
}
