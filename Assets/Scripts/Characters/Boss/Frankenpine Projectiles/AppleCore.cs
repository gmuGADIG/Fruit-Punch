using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleCore : EnemyProjectile {
    [Tooltip("Acceleration due to gravity in m/s^2.")]
    [SerializeField] float gravity = 16f;

    [Tooltip("The vertical velocity the projectile starts with.")]
    [SerializeField] float upForce = 5f;

    protected override void Start() {
        velocity += Vector3.up * upForce;
        base.Start();
    }

    protected override void Update()
    {
        velocity += Vector3.down * gravity * Time.deltaTime;
        base.Update();
    }
}
