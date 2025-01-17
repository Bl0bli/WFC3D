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

        private Dictionary<List<Vector2>, string> _boundaryMap = new Dictionary<List<Vector2>, string>(); //Deja Serialisé
        [SerializeField, HideInInspector] private int lastID;
        [SerializeField, HideInInspector] private int lastSymmetricIndex;


        public string[] CheckBoundaries(Mesh mesh) {
            List<Vector2>[] boundaries = GetBoundaries(mesh);
            for (int i = 0; i < 6; i++) {
                if (TryGetBoundaryIndex(boundaries[i], out string index)) {
                    Debug.Log(index);
                }
                else {
                    if (i < 4) {
                        AddNewHBoundary(boundaries[i]);
                    }
                    else {
                        AddNewVBoundary(boundaries[i]);
                    }
                }
            }
            return GetBoundariesIndices(boundaries);
        }
        
        private string[] GetBoundariesIndices(List<Vector2>[] boundaries) {
            List<string> indices = new List<string>();

            foreach (List<Vector2> boundary in boundaries) {
                TryGetBoundaryIndex(boundary, out string index);
                indices.Add(index);
            }
            
            return indices.ToArray();
        }
        
        private void AddNewVBoundary(List<Vector2> boundary) {
            if (boundary.Count < 1) {
                Debug.Log("Empty socket Found");
                _boundaryMap.Add(boundary, "-1");
                return;
            }
            //TODO Finish
        }
        
        private void AddNewHBoundary(List<Vector2> boundary) {
            if (boundary.Count < 1) {
                Debug.Log("Empty socket Found");
                _boundaryMap.Add(boundary, "-1");
                return;
            }
            
            if (CheckForSymmetry(boundary)) {
                Debug.Log("Symmetric Face Found !");
                _boundaryMap.Add(boundary, GetLastSymmetricIndex() + "s");
                return;
            }

            if (CheckForFlippedVersion(boundary, out string index)) {
                Debug.Log("Flipped Face Found !");
                _boundaryMap.Add(boundary, index + "f");
                return;
            }
            
            _boundaryMap.Add(boundary, GetLastIndex().ToString());
        }
        
        private bool CheckForFlippedVersion(List<Vector2> boundary, out string s) {
            List<Vector2> flipped = new List<Vector2>();
            foreach (Vector2 vertex in boundary) {
                flipped.Add(new Vector2(-vertex.x, vertex.y));
            }

            return TryGetBoundaryIndex(flipped, out s);
        }
        
        private bool CheckForSymmetry(List<Vector2> boundary) {
            List<Vector2> flipped = new List<Vector2>();
            foreach (Vector2 vertex in boundary) {
                flipped.Add(new Vector2(-vertex.x, vertex.y));
            }
            return BoundsEqual(boundary, flipped);
        }
        
        private int GetLastSymmetricIndex() {
            lastSymmetricIndex++;
            return lastSymmetricIndex;
        }
        
        private int GetLastIndex() {
            lastID++;
            return lastID;
        }

        private List<Vector2>[] GetBoundaries(Mesh mesh) {
            List<Vector2>[] boundaries = new[] {
                new List<Vector2>(), new List<Vector2>(), new List<Vector2>(), new List<Vector2>(), //Horizontal X -X Z -Z
                new List<Vector2>(), new List<Vector2>() //Vertical Y -Y
            };

            List<Vector3> realVertices = new List<Vector3>(mesh.vertices);
            realVertices = realVertices.Distinct().ToList();
            

            foreach (Vector3 vertex in realVertices) {
                if (Mathf.Abs(vertex.x - _cellSize) <= _tolerance) { //TODO Cellsize/2
                    boundaries[0].Add(RoundVector(vertex.z, vertex.y));
                }
                if (Mathf.Abs(vertex.x + _cellSize) <= _tolerance) {
                    boundaries[1].Add(RoundVector(vertex.z, vertex.y));
                }
                
                if (Mathf.Abs(vertex.z - _cellSize) <= _tolerance) {
                    boundaries[2].Add(RoundVector(vertex.x, vertex.y));
                }
                if (Mathf.Abs(vertex.z + _cellSize) <= _tolerance) {
                    boundaries[3].Add(RoundVector(vertex.x, vertex.y));
                }
                
                if (Mathf.Abs(vertex.y - _cellSize) <= _tolerance) {
                    boundaries[4].Add(RoundVector(vertex.x, vertex.z));
                }
                if (Mathf.Abs(vertex.y + _cellSize) <= _tolerance) {
                    boundaries[5].Add(RoundVector(vertex.x, vertex.z));
                }
            }
            return boundaries;
        }

        private bool BoundsEqual(List<Vector2> a, List<Vector2> b) {
            return a.All(b.Contains) && a.Count == b.Count;
        }

        bool TryGetBoundaryIndex(List<Vector2> boundary, out string value) {
            foreach (KeyValuePair<List<Vector2>,string> pair in _boundaryMap) {
                if (BoundsEqual( pair.Key, boundary)) {
                    value = pair.Value;
                    return true;
                }
            }

            value = null;
            return false;
        }

        private Vector2 RoundVector(float x, float y) {
            x = Mathf.Round(x * 100f) / 100f;
            y = Mathf.Round(y * 100f) / 100;
            return new Vector2(x, y);
        }
    }
}

/*
 REMERCIEMENTS :
 
Je tiens à remercier Enzo, Franko et ma famille pour le soutiens apporté
mes bras pour toujour avoir été à mes côtés
mes jambes pour m'avoir supporté
et mes doigts car j'ai toujours pu compter sur eux
*/