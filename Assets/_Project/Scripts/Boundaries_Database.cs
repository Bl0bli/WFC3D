using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


namespace WFC3D
{
    [CreateAssetMenu(menuName = "WFC3D/BoundaryDatabase")]
    public class Boundaries_Database : ScriptableObject
    {
        [SerializeField] private float _cellSize;
        [SerializeField] private float _tolerance;

        private Dictionary<List<Vector2>, string> _boundaryMap; //Deja Serialisé
        

        public void CheckBoundaries(Mesh mesh) {
            List<Vector2>[] boundaries = GetBoundaries(mesh); 
        }

        private List<Vector2>[] GetBoundaries(Mesh mesh) {
            List<Vector2>[] boundaries = new[] {
                new List<Vector2>(), new List<Vector2>(), new List<Vector2>(), new List<Vector2>(), //Horizontal X -X Z -Z
                new List<Vector2>(), new List<Vector2>() //Vertical Y -Y
            };

            foreach (Vector3 vertex in mesh.vertices) {
                if (Mathf.Abs(vertex.x - _cellSize) <= _tolerance) {
                    boundaries[0].Add(new Vector2(vertex.z, vertex.y));
                }
                if (Mathf.Abs(vertex.x + _cellSize) <= _tolerance) {
                    boundaries[1].Add(new Vector2(vertex.z, vertex.y));
                }
                
                if (Mathf.Abs(vertex.z - _cellSize) <= _tolerance) {
                    boundaries[2].Add(new Vector2(vertex.x, vertex.y));
                }
                if (Mathf.Abs(vertex.z + _cellSize) <= _tolerance) {
                    boundaries[3].Add(new Vector2(vertex.x, vertex.y));
                }
                
                if (Mathf.Abs(vertex.y - _cellSize) <= _tolerance) {
                    boundaries[4].Add(new Vector2(vertex.x, vertex.z));
                }
                if (Mathf.Abs(vertex.y + _cellSize) <= _tolerance) {
                    boundaries[5].Add(new Vector2(vertex.x, vertex.z));
                }
            }
            return boundaries;
        }

        private bool BoundsEqual(List<Vector2> a, List<Vector2> b) {
            return a.All(b.Contains) && a.Count == b.Count;
        }
    }
}