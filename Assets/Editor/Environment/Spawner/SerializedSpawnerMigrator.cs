using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SerializedSpawnerMigrator : Editor
{
    //Doesn't work for some reason, leaving for future reference
    //[MenuItem("Tools/Update Spawner Serialized Fields In All")]
    //private static void UpdateSerializedPropertyInAllScenes()
    //{
    //    bool proceed = EditorUtility.DisplayDialog(
    //        "Warning",
    //        "This tool will touch every scene and save without further warning. You probably shouldn't be running this.",
    //        "I understand the consequences and wish to proceed",
    //        "Cancel");
    //    if (!proceed) return;
    //    for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
    //    {
    //        string path = SceneUtility.GetScenePathByBuildIndex(i);
    //        Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                
    //            var rootGameObjects = scene.GetRootGameObjects();
    //            foreach (var rootGameObject in rootGameObjects)
    //            {
    //                var screenSpawners = rootGameObject.GetComponentsInChildren<ScreenSpawner>(true);
    //                foreach (var screenSpawner in screenSpawners)
    //                {
    //                    SpawnerEnemyToGameObject(screenSpawner);
    //                }
    //            }
    //        EditorSceneManager.MarkSceneDirty(scene);
    //        EditorSceneManager.SaveScene(scene);
    //    }
    //    AssetDatabase.Refresh();
    //}

    [MenuItem("Tools/Update Spawner Serialized Fields In Current")]
    private static void UpdateSerializedPropertyInCurrentScene()
    {
        bool proceed = EditorUtility.DisplayDialog(
            "Warning",
            "This tool will touch the current scene and save without further warning. You probably shouldn't be running this.",
            "I understand the consequences and wish to proceed",
            "Cancel");
        if (!proceed) return;
        Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var rootGameObject in rootGameObjects)
        {
            var screenSpawners = rootGameObject.GetComponentsInChildren<ScreenSpawner>(true);
            foreach (var screenSpawner in screenSpawners)
            {
                SpawnerEnemyToGameObject(screenSpawner);
            }
        }
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.Refresh();
    }
    private static void SpawnerEnemyToGameObject(ScreenSpawner spawner)
    {
        SerializedObject serializedObject = new SerializedObject(spawner);
        SerializedProperty enemiesToSpawn = serializedObject.FindProperty("enemiesToSpawn");
        if (enemiesToSpawn != null && enemiesToSpawn.isArray)
        {
            for(int i = 0; i < enemiesToSpawn.arraySize; i++)
            {
                SerializedProperty enemySpawnData = enemiesToSpawn.GetArrayElementAtIndex(i);
                SerializedProperty enemyProperty = enemySpawnData.FindPropertyRelative("enemy");
                SerializedProperty enemyPrefab = enemySpawnData.FindPropertyRelative("enemyPrefab");
                
                if(enemyProperty != null && enemyProperty.propertyType == SerializedPropertyType.ObjectReference)
                {
                    if(enemyProperty.objectReferenceValue != null)
                    {
                        Enemy enemy = enemyProperty.objectReferenceValue as Enemy;
                        if(enemy != null)
                        {
                            enemyPrefab.objectReferenceValue = enemy.gameObject;
                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                }
            }
        }
    }
}
