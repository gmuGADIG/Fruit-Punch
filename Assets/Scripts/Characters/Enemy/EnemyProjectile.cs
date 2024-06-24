using UnityEngine;

[RequireComponent(typeof(HurtBox), typeof(Lifetime))]
public class EnemyProjectile : MonoBehaviour
{
    [HideInInspector] public Vector3 velocity;

    bool setupCalled = false;

    /// <summary>
    /// Sets up the enemy projectile, including the velocity and damage.
    /// </summary>
    public virtual void Setup(float damage, Vector3 velocity) {
        var hurtBox = GetComponent<HurtBox>();
        hurtBox.onHurt += (_d) => Destroy(gameObject);

        hurtBox.damage = damage;

        this.velocity = velocity;

        setupCalled = true;
    }

    protected virtual void Start() {
        if (!setupCalled) {
            throw new System.Exception("EnemyProjectile.Setup not called!");
        }
    }

    protected virtual void Update() {
        transform.position += Time.deltaTime * (Vector3)velocity;
    }
}
