using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace WFC3D.Editor
{
    public class MeshImporterEditorWindow : EditorWindow
    {
        //TODO Add DatabaseMember
        public List<Mesh> _mesh;
        private UnityEditor.Editor _meshEditor;
        private Boundaries_Database _boundariesDatabase;
        private Tile_Database _tileDatabase;
        private int _meshPreviewIndex;
        

        [MenuItem("WFC3D/MeshImporter")]
        public static void Open() {

            MeshImporterEditorWindow[] windows = Resources.FindObjectsOfTypeAll<MeshImporterEditorWindow>();
            if (windows.Length > 0) {
                windows[0].Focus();
                return;
            }

            MeshImporterEditorWindow window = CreateWindow<MeshImporterEditorWindow>(typeof(SceneView));

            window.titleContent = new GUIContent("Mesh Importer");
        }

        private void OnGUI() {
            AddBoundaryDatabase();
            AddTileDatabase();
            AddMeshField();
            AddMeshPreview();
            AddMeshPreviewSwitchButtons();
            
            

            if (_mesh.Count > 0 && _mesh.All(mesh => mesh != null)) {
                if (GUILayout.Button("Import")) {
                    if (_boundariesDatabase) {
                        Import();
                    }
                }
            }

        }

        private void AddMeshPreviewSwitchButtons() {
            if (_meshEditor) {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous")) {
                    DestroyImmediate(_meshEditor);
                    _meshPreviewIndex--;
                    _meshEditor = UnityEditor.Editor.CreateEditor(_mesh[Mathf.Abs(_meshPreviewIndex%_mesh.Count)]);
                }
                
                GUIStyle centered = new GUIStyle(GUI.skin.label);
                centered.alignment = TextAnchor.MiddleCenter;
                
                GUILayout.Label(Mathf.Abs(_meshPreviewIndex%_mesh.Count).ToString(), centered);
                if (GUILayout.Button("Next")) {
                    DestroyImmediate(_meshEditor);
                    _meshPreviewIndex++;
                    _meshEditor = UnityEditor.Editor.CreateEditor(_mesh[Mathf.Abs(_meshPreviewIndex%_mesh.Count)]);
                }
                
                EditorGUILayout.EndHorizontal();
            }
        }

        private void AddTileDatabase() {
            EditorGUILayout.BeginHorizontal();
            _tileDatabase = (Tile_Database)EditorGUILayout.ObjectField(
                "Tile DataBase : ",
                _tileDatabase,
                typeof(Tile_Database),
                false
            );
            
            if (GUILayout.Button("Create New")) {
                Tile_Database asset = CreateInstance<Tile_Database>();
                string path = AssetDatabase.GenerateUniqueAssetPath("Assets/WFC3D/Databases/TileData.asset");
                AssetDatabase.CreateAsset(asset, path);
                _tileDatabase = asset;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void AddBoundaryDatabase() {
            EditorGUILayout.BeginHorizontal();
            _boundariesDatabase = (Boundaries_Database)EditorGUILayout.ObjectField(
                "Boundaries DataBase : ",
                _boundariesDatabase,
                typeof(Boundaries_Database),
                false);

            if (GUILayout.Button("Create New")) {
                Boundaries_Database asset = CreateInstance<Boundaries_Database>();
                string path = AssetDatabase.GenerateUniqueAssetPath("Assets/WFC3D/Databases/BoundariesData.asset");
                AssetDatabase.CreateAsset(asset, path);
                _boundariesDatabase = asset;
            }
            EditorGUILayout.EndHorizontal();
        }

        #region MeshPreviewEditor

        private void AddMeshPreview() {

            GUIStyle bgColor = new GUIStyle();

            if (_mesh.Count > 0 && _mesh.All(mesh => mesh != null))
            {
                if (!_meshEditor) {
                    _meshEditor = UnityEditor.Editor.CreateEditor(_mesh[Mathf.Abs(_meshPreviewIndex%_mesh.Count)]);
                }
                _meshEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect (200,200),bgColor);
            }
        }
        private void AddMeshField() {
            EditorGUI.BeginChangeCheck();
            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty meshs = so.FindProperty("_mesh");
            EditorGUILayout.PropertyField(meshs, true);
            so.ApplyModifiedProperties();

            if(EditorGUI.EndChangeCheck())
            {
                if(_meshEditor) DestroyImmediate(_meshEditor);
            }
        }


        #endregion
        
        private void Import() {
            foreach (Mesh mesh in _mesh) {
                string[] indices =  _boundariesDatabase.CheckBoundaries(mesh);
                
                TileStruct tile = new TileStruct(mesh, 0, indices[0], indices[1], indices[4], indices[5], indices[2], indices[3] , _tileDatabase.ID);
                for (int i = 0; i < 4; i++) 
                {
                    _tileDatabase.AddTile(tile);
                    tile.Rotate(_tileDatabase.ID);
                }
            }
        }

    }
}
