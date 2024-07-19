using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUITrigger : MonoBehaviour
{
    [SerializeField] string bossName;
    
    void Start()
    {
        BossUI.instance.Setup(bossName, GetComponent<Health>());
    }
}
