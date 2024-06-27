using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Collider))]
public class AcidSpit : MonoBehaviour {
    EnemyProjectile enemyProjectile;
    Animator animator;
    Vector3 gravity;

    public void Setup(float damage, Vector3 velocity, Vector3 gravity) {
        this.gravity = gravity;

        this.GetComponentInChildrenOrError(out enemyProjectile);
        enemyProjectile.Setup(damage, velocity);
    }

    void Start() {
        this.GetComponentOrError(out animator);
    }

    void Update() {
        enemyProjectile.velocity += gravity * Time.deltaTime;

        if (enemyProjectile.velocity != Vector3.zero) {
            // look in direction of movement
            var velocity = (Vector3)(Vector2)enemyProjectile.velocity; // velocity with z=0
            transform.rotation = Quaternion.FromToRotation(Vector3.down, velocity);
        } else {
            transform.rotation = Quaternion.identity;
        }
    }
    
    public void OnGroundHit() {
        enemyProjectile.velocity = Vector3.zero;
        gravity = Vector3.zero;

        animator.Play("AcidSpitSplat");

        print("Hello, world!");
    }
}
