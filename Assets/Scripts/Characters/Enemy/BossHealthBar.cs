using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBar : EnemyHealthBar
{
    public Player player;
    // Override -- Health Bar displays at top of screen instead of bottom
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, 2.25f, 0);
    }
}
