using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace WFC3D.Editor
{
    public class MeshImporterEditorWindow : EditorWindow
    {
        //TODO Add DatabaseMember
        private Mesh _mesh;
        private UnityEditor.Editor _meshEditor;
        private Boundaries_Database _boundariesDatabase;
        
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
            _boundariesDatabase = (Boundaries_Database)EditorGUILayout.ObjectField(
                "Boundaries DataBase : ",
                _boundariesDatabase,
                typeof(Boundaries_Database),
                false);
            
            AddMeshField();
            AddMeshPreview();

            if (_mesh) {
                if (GUILayout.Button("Import")) {
                    if (_boundariesDatabase) {
                        Import();
                    }
                }
            }

        }

        #region MeshPreviewEditor

        private void AddMeshPreview() {

            GUIStyle bgColor = new GUIStyle();

            if (_mesh)
            {
                if (!_meshEditor) {
                    _meshEditor = UnityEditor.Editor.CreateEditor(_mesh);

                }
                _meshEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect (200,200),bgColor);
            }
        }
        private void AddMeshField() {
            EditorGUI.BeginChangeCheck();
            _mesh = (Mesh) EditorGUILayout.ObjectField("Mesh to Import :", _mesh, typeof(Mesh), true);
            if(EditorGUI.EndChangeCheck())
            {
                if(_meshEditor) DestroyImmediate(_meshEditor);
            }
        }


        #endregion
        
        private void Import() {
            string[] boundaryIndices = _boundariesDatabase.CheckBoundaries(_mesh);
            foreach (string index in boundaryIndices) {
                Debug.Log(index);
            }
        }

    }
}
