using UnityEngine;

public class PlayerHealthBarHolder : MonoBehaviour
{
    static PlayerHealthBarHolder instance;

    PlayerHealthBarHolder()
    {
        instance = this;
    }
    
    /// <summary>
    /// Setup a health bar for a single player (index=0 for player one, index=1 for player two). <br/>
    /// Called by the player spawning when creating the players.
    /// </summary>
    public static void SetHealthBar(int index, Player player)
    {
        print($"setting up health bar for player idx {index}");
        var healthBar = instance.transform.GetChild(index).GetComponent<PlayerHealthBar>();
        healthBar.Setup(player);
    }

    public static void SetPlayerTwoVisible(bool visible)
    {
        print($"setting player two health UI visibility to {visible}");
        instance.transform.GetChild(1).gameObject.SetActive(visible);
    }
}
