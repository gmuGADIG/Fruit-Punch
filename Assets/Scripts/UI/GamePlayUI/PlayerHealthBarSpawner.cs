using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        bar.gameObject.GetComponent<RectTransform>().position = new Vector3(bar.gameObject.GetComponent<RectTransform>().position.x, bar.gameObject.GetComponent<RectTransform>().position.y , -5);

        bar.GetComponent<PlayerHealthBarUI>().playerHealth = player.GetComponent<Health>();


        return bar;
    }


}
