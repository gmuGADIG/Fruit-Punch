using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HurtBox))]
class EnemyProjectile : MonoBehaviour {
    bool setupCalled = false;

    Vector2 velocity;

    public void Setup(float damage, Vector2 velocity) {
        GetComponent<HurtBox>().damage = damage;
        this.velocity = velocity;
    }

    void Start() {
        if (!setupCalled) {
            Debug.LogError("EnemyProjectile not called!");
        }
    }

    public void Update() {
        transform.position += Time.deltaTime * (Vector3)velocity;
    }
}