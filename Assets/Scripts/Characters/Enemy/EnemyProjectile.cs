using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HurtBox))]
class EnemyProjectile : MonoBehaviour {
    bool setupCalled = false;
    Vector2 velocity;

    public void Setup(float damage, Vector2 velocity) {
        var hurtBox = GetComponent<HurtBox>();
        hurtBox.damage = damage;
        hurtBox.onHurt += (_d) => Destroy(gameObject);

        this.velocity = velocity;
    
        setupCalled = true;
    }

    void Start() {
        //yield return null;
        if (!setupCalled) {
            Debug.LogError("EnemyProjectile.Setup not called!");
        }
    }

    public void Update() {
        transform.position += Time.deltaTime * (Vector3)velocity;
    }
}