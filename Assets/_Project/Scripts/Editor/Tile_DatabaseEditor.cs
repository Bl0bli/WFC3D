using UnityEditor;
using UnityEngine;

namespace WFC3D.Editor
{
    [CustomEditor(typeof(Tile_Database))]
    public class Tile_DatabaseEditor : UnityEditor.Editor
    {
        UnityEditor.Editor[] previews;
        bool init = false;
        bool updating = true;

        private void OnEnable()
        {
            Tile_Database database = (Tile_Database)target;
            database._UpdateDtb += StopUpdatingInspector;
            ReDraw();
        }
        private void OnDisable()
        {
            Debug.Log("OnDisable");
            foreach (UnityEditor.Editor item in previews)
            {
                DestroyImmediate(item);
                Debug.Log("Destroy Editor");
            }
        }
        public override void OnInspectorGUI()
        {
            Tile_Database database = (Tile_Database)target;
            if (updating)
            {
                if (!init)
                {
                    for (int i = 0; i < database.Tiles.Count; i++)
                    {
                        if (database.Tiles[i].Mesh != null)
                        {
                            Debug.Log("Create Editor");
                            previews[i] = UnityEditor.Editor.CreateEditor(database.Tiles[i].Mesh);
                        }
                        if (database.Tiles[i].Mesh == null)
                        {
                            Debug.Log("No Mesh Found " + database.Tiles[i].Id);
                        }
                    }
                }

                init = true;
                for (int i = 0; i < database.Tiles.Count; i++)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Label("Tile ID : " + database.Tiles[i].Id);
                    Debug.Log(database.Tiles[i].Id);
                    if (database.Tiles[i].Mesh != null)
                    {
                        previews[i].OnInteractivePreviewGUI(GUILayoutUtility.GetRect(50, 50), new GUIStyle());
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        database.RemoveTile(database.Tiles[i]);
                        break;
                    }

                    GUILayout.EndHorizontal();
                }
            }
            

        }

        private void ReDraw()
        {
            Tile_Database database = (Tile_Database)target;
            previews = new UnityEditor.Editor[(database).Tiles.Count];
            init = false;
            updating = true;
        }

        private void StopUpdatingInspector(bool freeze)
        {
            if(freeze)
            {
                updating = false;
            }
            else
            {
                ReDraw();
            }
        }
    
    }
}
