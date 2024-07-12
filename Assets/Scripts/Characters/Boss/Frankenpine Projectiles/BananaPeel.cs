using UnityEngine;

public class BananaPeel : EnemyProjectile {
    protected override void Update() {
        if (Physics.Raycast(
                transform.position + Vector3.up,
                Vector3.down,
                out var hit,
                float.PositiveInfinity,
                LayerMask.GetMask("World")
        )) {
            transform.position = new(
                transform.position.x,
                hit.point.y,
                transform.position.z
            );
        }

        base.Update();
    }
}
