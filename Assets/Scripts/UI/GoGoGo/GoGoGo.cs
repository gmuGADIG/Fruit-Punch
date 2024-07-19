using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoGoGo : MonoBehaviour
{
    static GoGoGo instance;

    Animator animator;

    void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
    }


    public static void Go()
    {
        instance.animator.Play("GoGoGo");
    }
}
