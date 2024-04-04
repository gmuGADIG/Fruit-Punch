using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBarSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject playerHealthBarUIPrefab;
    [SerializeField]
    private GameObject healthBarCanvasObject;


    public GameObject SpawnHealthBar(Character character, GameObject player)
    {
        GameObject bar = Instantiate<GameObject>(playerHealthBarUIPrefab);

        bar.gameObject.transform.parent = healthBarCanvasObject.transform;
        bar.transform.localScale = playerHealthBarUIPrefab.transform.localScale;

        bar.GetComponent<PlayerHealthBarUI>().playerHealth = player.GetComponent<Health>();


        return bar;
    }


}
