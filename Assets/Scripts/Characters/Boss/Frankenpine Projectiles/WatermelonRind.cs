using UnityEngine;

public class WatermelonRind : EnemyProjectile
{
    [Tooltip("How far should the projectile go across the screen.")]
    [SerializeField] float xSpan = 3f;

    [Tooltip("How far should the projectile go up and down (z-axis) the screen.")]
    [SerializeField] float zSpan = 2f;

    [Tooltip("How fast does the \"animation\" of the projectile go.")]
    [SerializeField] float speedModifier = 1f;

    float angle = 0f;

    protected override void Update()
    {
        velocity = new Vector3(
            -xSpan * Mathf.Cos(angle),
            0,
            zSpan * Mathf.Sin(angle)
        );

        angle += Time.deltaTime * speedModifier;
        angle = Mathf.Min(angle, Mathf.PI * 2 * 0.5f);

        base.Update();
    }
}
