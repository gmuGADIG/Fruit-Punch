using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameOverScreen : MonoBehaviour
{
    [Tooltip ("Name of game over scene")]
    public string GameOverScene;

    /*
     * On Awake gets num of living players
     * When a player dies, it subtracts the number of living players
     * If living players equal zero load the Game over screen
     * 
     */

    Health[] playersHealth;
    int numOfLivingPlayers;
    // Start is called before the first frame update
    void Awake()
    {
        playersHealth = FindObjectsOfType<Health>();
        numOfLivingPlayers = playersHealth.Length;
        Debug.Log("num of lliving players " + numOfLivingPlayers);
    }
    


    /// <summary>
    /// Load Scene when player dies
    /// </summary>
    private void DoLoadGameOverScreen()
    {
        SceneManager.LoadSceneAsync(GameOverScene, LoadSceneMode.Additive);
    }
    /// <summary>
    /// Will reduce number of living players, if no living players, GAME OVER
    /// </summary>
    private void APlayerDied()
    {
        numOfLivingPlayers--;
        Debug.Log("A player has died");
        if(numOfLivingPlayers == 0)
        { DoLoadGameOverScreen(); }
    }
   
    private void OnEnable()
    {
        for(int i = 0; i < numOfLivingPlayers; i++)
        {
            playersHealth[i].onDeath += APlayerDied;
        }
        
    }

    
}
