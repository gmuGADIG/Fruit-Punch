using UnityEngine;
using UnityEngine.AI;

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

    DebugMarker aggressiveMarker;

    [SerializeField] Transform shootPoint;

    protected override EnemyState AggressiveUpdate()
    {
        var pos = transform.position - aggressiveCurrentTarget.position;
        var absPos = pos.Abs();

        // if (minX <= absPos.x && absPos.x <= maxX && absPos.z <= fuzzyZ) {
        //     return EnemyState.Attacking; // we can shoot them!
        // }

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

        var atTarget = WalkTowards(goalPos, walkingSpeed);
        if (atTarget)
        {
            return EnemyState.Attacking; // We can shoot them!
        }

        if (Application.isEditor) {
            if (aggressiveMarker == null) {
                aggressiveMarker = DebugMarker.Instantiate(debugMarkerPrefab, goalPos, Color.red);
            } else {
                aggressiveMarker.transform.position = goalPos;
            }
        }

        return stateMachine.currentState;
    }

    protected override void AggressiveExit(EnemyState _newState)
    {
        if (aggressiveMarker != null) {
            Destroy(aggressiveMarker.gameObject);
        }
    }

    protected override void AttackingEnter()
    {
        if (transform.position.x - aggressiveCurrentTarget.position.x > 0) {
            transform.localEulerAngles = Vector3.up * 180;
        } else {
            transform.localEulerAngles = Vector3.zero;
        }
        
        // TODO: probably should cache this
        GetComponent<Animator>().Play("Shoot");

        currentAttackDuration = attackDuration;

    }

    void ShootProjectile() {
        Instantiate(
            bulletPrefab,
            shootPoint.position,
            Quaternion.identity
        ).GetComponent<EnemyProjectile>()
        .Setup(projectileDamage, transform.rotation * Vector3.right * projectileSpeed);
    }

    void AttackingAnimationOver() {
        stateMachine.SetState(EnemyState.Wandering);
    }

    protected override void AttackingExit(EnemyState _newState) {
        // TODO: probably should cache this
        GetComponent<Animator>().Play("Wander");
    }
}
