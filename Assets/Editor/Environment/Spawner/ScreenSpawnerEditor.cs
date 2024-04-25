using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(ScreenSpawner))]
public class ScreenSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Start Spawner"))
        {
            (target as ScreenSpawner).StartSpawning();
        }
    }
}
