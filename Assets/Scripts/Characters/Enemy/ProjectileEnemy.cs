using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

class ProjectileEnemy : Enemy {
    [Tooltip("Minimum distance from the player in order to shoot.")]
    [SerializeField] float minX = 1;

    [Tooltip("Maximum distance from the player in order to shoot.")]
    [SerializeField] float maxX = 5;

    [Tooltip("How far back/forth the enemy should be from the player before shooting.")]
    [SerializeField] float fuzzyZ = 0.1f;

    [Tooltip("How much damage the projectile will deal to the player.")]
    [SerializeField] float projectileDamage = 10;

    [Tooltip("How fast the projectile will move in meters per second.")]
    [SerializeField] float projectileSpeed = 1;

    [SerializeField] GameObject bulletPrefab;

    const float attackDuration = 2f;
    float currentAttackDuration;

    protected override EnemyState AggressiveUpdate()
    {
        var deltaX = Mathf.Abs(transform.position.x - aggressiveCurrentTarget.position.x);
        var deltaZ = Mathf.Abs(transform.position.z - aggressiveCurrentTarget.position.z);
        if (minX <= deltaX && deltaX <= maxX && deltaZ <= fuzzyZ) {
            return EnemyState.Attacking; // we can shoot them!
        }

        Vector3 goalPos = new();

        var signX = transform.position.x - aggressiveCurrentTarget.position.x / deltaX;
        var signZ = transform.position.x - aggressiveCurrentTarget.position.x / deltaZ;
    
        if (deltaX < minX) {
            goalPos.x = minX * signX + aggressiveCurrentTarget.position.x;
        } else {
            goalPos.x = maxX * signX + aggressiveCurrentTarget.position.x;
        }

        if (deltaZ < -fuzzyZ) {
            goalPos.z = -fuzzyZ * signZ + aggressiveCurrentTarget.position.z;
        } else {
            goalPos.z = fuzzyZ * signZ + aggressiveCurrentTarget.position.z;
        }

        goalPos.y = transform.position.y;

        transform.position = Vector3.MoveTowards(
            transform.position,
            goalPos,
            walkingSpeed * Time.deltaTime
        );

        return stateMachine.currentState;
    }

    protected override void AttackingEnter()
    {
        currentAttackDuration = attackDuration;

        var deltaX = Mathf.Abs(transform.position.x - aggressiveCurrentTarget.position.x);
        var signX = transform.position.x - aggressiveCurrentTarget.position.x / deltaX;

        Instantiate(
            bulletPrefab,
            transform.position,
            Quaternion.identity
        ).GetComponent<EnemyProjectile>()
        .Setup(projectileDamage, signX * projectileSpeed * Vector2.right);
    }

    protected override EnemyState AttackingUpdate()
    {
        currentAttackDuration -= Time.deltaTime;
        
        if (currentAttackDuration > 0) {
            return stateMachine.currentState;
        } else {
            return EnemyState.Wandering;
        }
    }
}