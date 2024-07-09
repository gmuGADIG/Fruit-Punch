// the following line suppresses a warning about inexhaustive switch expressions,
// because the switch statement does not handle cursed conditions like `(Side) 2`
#pragma warning disable CS8524

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineCurtain : MonoBehaviour
{
    enum Side { Left, Right }

    // https://www.youtube.com/watch?v=LSNQuFEDOyQ
    float expDecay(float a, float b, float decay, float dt) {
        return b + (a - b) * Mathf.Exp(-decay * dt);
    }

    Side RandomSide() { return Random.Range(0f, 1f) > 0.5f ? Side.Left : Side.Right; }
    string AnimationNameOfSide(Side side) {
        return side switch {
            Side.Left => LeftAnimation,
            Side.Right => RightAnimation,
        };
    }

    [SerializeField] Transform LeftTransform;
    [SerializeField] Transform RightTransform;

    Animator animator;

    const string LeftAnimation = "Left";
    const string RightAnimation = "Right";

    readonly Vector3 LeftStartingPosition = new Vector3(-5.89f, 0f, -0.782f);
    readonly Vector3 RightStartingPosition = new Vector3(6.01f, 0f, -0.782f);

    Side attackingSide;

    public event System.Action AttackFinished;

    void Start() {
        animator = GetComponent<Animator>();

        foreach (var hurtBox in GetComponentsInChildren<HurtBox>(true)) {
            hurtBox.onPearried += OnPearry;
        }
    }

    public void OnPearry(DamageInfo _d) {
        animator.enabled = false;

        var curtain = attackingSide switch {
            Side.Left => LeftTransform,
            Side.Right => RightTransform
        };

        curtain.GetComponent<HurtBox>().enabled = false;

        IEnumerator Coroutine() {
            var goal = attackingSide switch {
                Side.Left => LeftStartingPosition,
                Side.Right => RightStartingPosition 
            };
            
            while (Mathf.Abs(curtain.position.x - goal.x) > 0.05f) {
                var pos = curtain.position;
                curtain.position = new(
                    expDecay(pos.x, goal.x, 10, Time.deltaTime),
                    pos.y,
                    pos.z
                );

                yield return null;
            }
            print("done");

            AttackFinished?.Invoke();
        }

        StartCoroutine(Coroutine());
    }

    public void Attack() {
        attackingSide = RandomSide();
        animator.enabled = true;
        animator.Play(AnimationNameOfSide(attackingSide), -1, 0f);
    }

    void SignalAttackFinished() {
        AttackFinished?.Invoke();
    }
}
