using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DeadEnemyKnockback : MonoBehaviour {
    [SerializeField] float power = 2f;

    void Start() {
        GetComponent<Rigidbody>().velocity = transform.rotation * Vector3.left * power;
    }
}
