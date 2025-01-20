using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace WFC3D.Editor
{
    [CustomEditor(typeof(Wave))]
    public class WaveEditor : UnityEditor.Editor
    {
        private GameObject[] _spawnedObjects;
        
        private void OnEnable() {
            ((Wave)target).InstanciatedTilesEvent += GetTiles;
        }

        private void OnDisable() {
            ((Wave)target).InstanciatedTilesEvent -= GetTiles;
        }

        private void GetTiles(GameObject[] obj) {
            _spawnedObjects = obj;
        }

        public override void OnInspectorGUI()
        {
            Wave wave = (Wave)target;
            wave._dtb = (Tile_Database)EditorGUILayout.ObjectField(
                "Tile DataBase : ",
                wave._dtb,
                typeof(Tile_Database),
                false
            );
            
            
            if (GUILayout.Button("Generate"))
            {
                wave.WFC();
            }
            
            if (_spawnedObjects != null) {
                if (GUILayout.Button("Export")) {
                    GameObject prefabRoot = new GameObject();
                    foreach (GameObject go in _spawnedObjects) {
                        go.transform.SetParent(prefabRoot.transform);
                    }
                    string path = "Assets/WFC3D/Prefabs/WFCasset.prefab";
                    path = AssetDatabase.GenerateUniqueAssetPath(path);
                    bool success;
                    PrefabUtility.SaveAsPrefabAssetAndConnect(prefabRoot, path, InteractionMode.UserAction, out success);

                    if (success) {
                        Debug.Log("Prefab saved successfully");
                    }
                    else {
                        Debug.LogWarning("Prefab failed to save");
                    }
                }
            }
        }
    }
}
