using UnityEditor;
using UnityEngine;

namespace WFC3D.Editor
{
    [CustomEditor(typeof(Tile_Database))]
    public class Tile_DatabaseEditor : UnityEditor.Editor
    {
        UnityEditor.Editor[] previews;
        bool init = false;

        private void OnEnable()
        {
            previews = new UnityEditor.Editor[((Tile_Database)target).Tiles.Count];
            init = false;
        }
        public override void OnInspectorGUI()
        {
            Tile_Database database = (Tile_Database)target;
            if(!init)
            {
                for (int i = 0; i < database.Tiles.Count; i++)
                {
                    if (database.Tiles[i].Mesh != null)
                    {
                        previews[i] = UnityEditor.Editor.CreateEditor(database.Tiles[i].Mesh);
                    }
                }
            }

            init = true;
            for (int i = 0; i < database.Tiles.Count; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label("Tile ID : " + database.Tiles[i].Id);
                if (database.Tiles[i].Mesh != null)
                {
                    previews[i].OnInteractivePreviewGUI(GUILayoutUtility.GetRect(50, 50), new GUIStyle());
                }
                if(GUILayout.Button("Delete"))
                {
                    database.Tiles.Remove(database.Tiles[i]);
                    break;
                }

                GUILayout.EndHorizontal();
            }

        }
    
    }
}
