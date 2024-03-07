using System.Collections;
using UnityEngine;

class EnemyProjectile : MonoBehaviour {
    bool setupCalled = false;

    Vector2 velocity;

    BeltCharacter bc;

    public void Setup(float damage, Vector2 velocity) {
        GetComponent<HurtBox>().damage = damage;
        this.velocity = velocity;
    }

    void Start() {
        this.GetComponentOrError(out bc);

        if (!setupCalled) {
            Debug.LogError("EnemyProjectile not called!");
        }
    }

    public void Update() {
        bc.internalPosition += Time.deltaTime * (Vector3)velocity;
    }
}