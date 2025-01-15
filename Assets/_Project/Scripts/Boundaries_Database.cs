using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace WFC3D
{
    public struct Boundaries
    {
        public List<List<Vector2>> AxisBoundaries;
    }
    [CreateAssetMenu(menuName = "WFC3D/BoundaryDatabase")]
    public class Boundaries_Database : ScriptableObject
    {
        [SerializeField] private float tileBoundsHalfSize;
        [SerializeField] private float boundaryTolerance;
        
        [SerializeField, HideInInspector] private Dictionary<List<Vector2>, string> _boundaryMap;
        [SerializeField, HideInInspector] private int lastIndex;
        [SerializeField, HideInInspector] private int lastSymmetricIndex;

        public float TileBoundsHalfSize => tileBoundsHalfSize;
        public float BoundaryTolerance => boundaryTolerance;

        public string[] CheckBoundaries(Boundaries boundaries) {
            string[] boundaryIndices = new string[6];
            
            for(int i = 0; i < 6; i++) {
                if (_boundaryMap.ContainsKey(boundaries.AxisBoundaries[i])) {
                    boundaryIndices[i] = _boundaryMap[boundaries.AxisBoundaries[i]];
                    continue;
                }

                if (i < 4) {
                    boundaryIndices[i] = AddNewHBoundary(boundaries.AxisBoundaries[i]);
                }
                else {
                    //TODO boundaryIndices[i] = AddNewVBoundary(boundaries.AxisBoundaries[i]);
                }
            }
            
            return boundaryIndices;
        }
        private string AddNewVBoundary(List<Vector2> boundariesAxisBoundary) {
            throw new System.NotImplementedException(); //TODO Implement
        }

        private string AddNewHBoundary(List<Vector2> boundary) {
            string index;
            if (HasFlippedVersion(boundary, out int unFlippedIndex)) {
                index = unFlippedIndex.ToString() + "f";
            }
            else if (CheckForSymmetry(boundary)) {
                index = GetNewSymmetricalIndex() + "s";
            }
            else {
                index = GetNewIndex().ToString();
            }
            
            return index;
        }
        
        private int GetNewSymmetricalIndex() {
            lastSymmetricIndex++;
            return lastSymmetricIndex;
        }
        
        private int GetNewIndex() {
            lastIndex++;
            return lastIndex;
        }
        
        private bool CheckForSymmetry(List<Vector2> boundary) {
            for (int i = 0; i < boundary.Count; i++) {
                for (int j = 0; j < boundary.Count; j++) {
                    Vector2 symm = new Vector2(-boundary[j].x, boundary[j].y);
                    if ((i != j) && boundary[i] == symm) { //TODO Equal with epsilons
                        break;
                    }
                }
                if (boundary[i].x == 0) {
                    continue;
                }
                return false; 
            }
            return true;
        }
        
        private bool HasFlippedVersion(List<Vector2> boundary, out int i) {
            List<Vector2> flippedBoundary = new List<Vector2>();
            foreach (Vector2 vertex in boundary) {
                flippedBoundary.Add(new Vector2(-vertex.x, vertex.y));
            }

            if (_boundaryMap.TryGetValue(flippedBoundary, out string value)) {
                i = -1; //TODO GetIndex(value);
                return true;
            }

            i = -1;
            return false;
        }
    }
}