using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadPeanut : MonoBehaviour {
    [SerializeField] GameObject smallPeanutPrefab;
    [SerializeField] Transform spawnPosition;

    void SpawnSmallPeanuts() {
        Vector3[] vels = {
            new(-1f, 1f, Random.Range(0f, 1f)),
            new(1f, 1f, Random.Range(0f, 1f))
        };

        foreach (var v in vels) {
            Instantiate(smallPeanutPrefab, spawnPosition.position, Quaternion.identity)
                .GetComponent<Rigidbody>()
                .velocity = v;

        }
    }
}
