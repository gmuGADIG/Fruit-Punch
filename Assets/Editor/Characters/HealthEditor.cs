using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Health))]
public class HealthEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Kill"))
        {
            Debug.Log("Killing " + target.name);
            (target as Health).Damage(new DamageInfo(float.MaxValue, Vector2.zero, (AuraType)(-1)));
            Debug.LogWarning((target as Health).CurrentHealth + " left.");
        }
    }
}
