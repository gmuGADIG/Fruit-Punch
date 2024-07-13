using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushParticleSpawner : MonoBehaviour
{
    public ParticleSystem system;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        system.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
