using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Health)), CanEditMultipleObjects]
public class HealthEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Kill"))
        {
            //make them vulnerable to all types before dealing max damage
            foreach (var obj in targets)
            {
                (obj as Health).vulnerableTypes = (AuraType)(-1);
                (obj as Health).Damage(new DamageInfo(null, float.MaxValue, Vector2.zero, (AuraType)(-1)));
            }
        }
    }
}
