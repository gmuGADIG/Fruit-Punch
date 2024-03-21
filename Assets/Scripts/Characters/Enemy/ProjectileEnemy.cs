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

    GameObject aggressiveMarker;

    protected override EnemyState AggressiveUpdate()
    {
        var pos = transform.position - aggressiveCurrentTarget.position;
        var absPos = pos.Abs();

        var deltaX = Mathf.Abs(transform.position.x - aggressiveCurrentTarget.position.x);
        var deltaZ = Mathf.Abs(transform.position.z - aggressiveCurrentTarget.position.z);
        if (minX <= absPos.x && absPos.x <= maxX && absPos.z <= fuzzyZ) {
            return EnemyState.Attacking; // we can shoot them!
        }

        Vector3 goalPos = new();
        var signs = new Vector3(
            pos.x / absPos.x,
            pos.y / absPos.y,
            pos.z / absPos.z
        );

        if (minX > absPos.x) { // too close!
            goalPos.x = minX * signs.x;
        } else if (maxX < absPos.x) { // too far!
            goalPos.x = maxX * signs.x;
        }

        // not lined up!
        if (absPos.z > fuzzyZ) {
            goalPos.z = fuzzyZ * signs.z;
        }

        // goalPos is relative to target until this point
        goalPos += aggressiveCurrentTarget.position;
        goalPos.y = transform.position.y;

        transform.position = Vector3.MoveTowards(
            transform.position,
            goalPos,
            walkingSpeed * Time.deltaTime
        );

        if (Application.isEditor) {
            if (aggressiveMarker == null) {
                aggressiveMarker = Instantiate(debugMarkerPrefab, goalPos, Quaternion.identity);
            } else {
                aggressiveMarker.transform.position = goalPos;
            }
        }

        return stateMachine.currentState;
    }

    protected override void AggressiveExit(EnemyState _newState)
    {
        Destroy(aggressiveMarker);
    }

    protected override void AttackingEnter()
    {
        currentAttackDuration = attackDuration;

        var deltaX = Mathf.Abs(transform.position.x - aggressiveCurrentTarget.position.x);
        var signX = transform.position.x - aggressiveCurrentTarget.position.x / deltaX;

        Instantiate(
            bulletPrefab,
            transform.position + Vector3.up * .5f,
            Quaternion.identity
        ).GetComponent<EnemyProjectile>()
        .Setup(projectileDamage, signX * projectileSpeed * Vector2.left);
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