using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HurtBox))]
class EnemyProjectile : MonoBehaviour
{
    [SerializeField] float lifetime = 10;
    bool setupCalled = false;
    Vector3 velocity;

    public void Setup(float damage, Vector3 velocity) {
        var hurtBox = GetComponent<HurtBox>();
        hurtBox.damage = damage;
        hurtBox.onHurt += (_d) => Destroy(gameObject);

        this.velocity = velocity;
    
        setupCalled = true;
    }

    void Start() {
        if (!setupCalled) {
            Debug.LogError("EnemyProjectile.Setup not called!");
        }
        
        // destroy after lifetime expires
        Destroy(gameObject, lifetime);
    }

    public void Update() {
        transform.position += Time.deltaTime * (Vector3)velocity;
    }
}