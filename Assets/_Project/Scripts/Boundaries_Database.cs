using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace WFC3D
{
    [CreateAssetMenu(menuName = "WFC3D/BoundaryDatabase")]
    public class Boundaries_Database : ScriptableObject
    {
        [SerializeField] private float cellSize;
        [SerializeField] private float tolerance;

        [SerializeField, HideInInspector] private int lastSymmetricID = 0;
        [SerializeField, HideInInspector] private int lastID = 0;

        private Dictionary<HashSet<Vector2>, string> _boundaryIndexMap = new Dictionary<HashSet<Vector2>, string>();

        public float CellSize => cellSize;
        public float Tolerance => tolerance;


        public string[] CheckBoundaries(Mesh mesh) {
            List<string> boundsIndices = new List<string>();
            foreach (HashSet<Vector2> horizontalBoundary in GetHorizontalBoundaries(mesh)) {
                if (MapTryGetValue(horizontalBoundary, out string index)) {
                    boundsIndices.Add(index);
                }
                else {
                    boundsIndices.Add(AddNewHorizontalBoundary(horizontalBoundary));
                }
            }
            foreach (HashSet<Vector2> verticalBoundary in GetVerticalBoundaries(mesh)) {
                if (MapTryGetValue(verticalBoundary, out string index)) {
                    boundsIndices.Add(index);
                }
            }
            return boundsIndices.ToArray();
        }
        private string AddNewHorizontalBoundary(HashSet<Vector2> horizontalBoundary) {
            string id;

            if (IsSymmetric(horizontalBoundary)) {
                id = GetLastSymmetricIndex() + "s";
            }
            else if (HasFlippedBoundary(horizontalBoundary, out string index)) {
                id = index + "f";
            }
            else if (horizontalBoundary.SetEquals(new HashSet<Vector2>())) {
                id = "-1";
            }
            else {
                id = GetLastIndex().ToString();
            }

            _boundaryIndexMap.Add(horizontalBoundary, id);
            return id;
        }
        private bool HasFlippedBoundary(HashSet<Vector2> horizontalBoundary, out string s) {
            HashSet<Vector2> flipped = new HashSet<Vector2>();
            foreach (Vector2 vertex in horizontalBoundary) {
                flipped.Add(new Vector2(-vertex.x , vertex.y));
            }
            if (MapTryGetValue(flipped, out string index)) {
                s = index;
                return true;
            }

            s = "no";
            return false;
        }
        private bool IsSymmetric(HashSet<Vector2> horizontalBoundary) {
            HashSet<Vector2> flipped = new HashSet<Vector2>();
            foreach (Vector2 vertex in horizontalBoundary) {
                flipped.Add(new Vector2(-vertex.x , vertex.y));
            }

            return flipped.SetEquals(horizontalBoundary);
        }
        private object GetLastIndex() {
            lastID++;
            return lastID;
        }
        private int GetLastSymmetricIndex() {
            lastSymmetricID++;
            return lastSymmetricID;
        }
        private HashSet<Vector2>[] GetVerticalBoundaries(Mesh mesh) {
            HashSet<Vector2>[] boundaries = new[] { new HashSet<Vector2>(), new HashSet<Vector2>() };
            foreach (Vector3 vertex in mesh.vertices) {
                if (vertex.y >= cellSize /2 - tolerance) { // Haut
                    boundaries[0].Add(new Vector2(vertex.x, vertex.z));
                }
            }
            foreach (Vector3 vertex in mesh.vertices) {
                if (vertex.y <= - cellSize /2 + tolerance) { // Bas
                    boundaries[1].Add(new Vector2(vertex.x, vertex.z));
                }
            }
            return boundaries;
        }
        private HashSet<Vector2>[] GetHorizontalBoundaries(Mesh mesh) {
            HashSet<Vector2>[] boundaries = new[] { new HashSet<Vector2>(), new HashSet<Vector2>(), new HashSet<Vector2>(), new HashSet<Vector2>() };
            foreach (Vector3 vertex in mesh.vertices) {
                // Right boundary
                if (Mathf.Abs(vertex.x - cellSize / 2) <= tolerance) {
                    boundaries[0].Add(new Vector2(vertex.z, vertex.y));
                }
                // Left boundary
                if (Mathf.Abs(vertex.x + cellSize / 2) <= tolerance) {
                    boundaries[1].Add(new Vector2(vertex.z, vertex.y));
                }
                // Front boundary
                if (Mathf.Abs(vertex.z - cellSize / 2) <= tolerance) {
                    boundaries[2].Add(new Vector2(vertex.x, vertex.y));
                }
                // Back boundary
                if (Mathf.Abs(vertex.z + cellSize / 2) <= tolerance) {
                    boundaries[3].Add(new Vector2(vertex.x, vertex.y));
                }
            }
            return boundaries;
        }

        bool MapTryGetValue(HashSet<Vector2> boundary, out string value) {
            foreach (KeyValuePair<HashSet<Vector2>,string> pair in _boundaryIndexMap) {
                if (pair.Key.SetEquals(boundary)) {
                    value = pair.Value;
                    return true;
                }
            }

            value = "no";
            return false;
        }
    }
}