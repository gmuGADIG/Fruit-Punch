using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Allows a field to be viewed but not edited in the inspector, providing a nicer alternative to certain print statements.
/// source: https://discussions.unity.com/t/how-to-make-a-readonly-property-in-inspector/75448
/// </summary>
public class ReadOnlyInInspectorAttribute : PropertyAttribute { }

[CustomPropertyDrawer(typeof(ReadOnlyInInspectorAttribute))]
public class ReadOnlyInInspectorDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
        GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}