using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WFC3D
{
    public class Wave2 : MonoBehaviour
    {
        [SerializeField] Grid _gridScript;
        [SerializeField] Tile_Database _dtb;

        TileGridCell[,,] _grid;
        private bool[,,] _visited;
        private int range;

        private void Start() {
            Reset();

            for (int i = 0; i < 100000; i++) {
                Debug.Log(i);
                if (AllCellCollapsed()) break;
                Vector3Int cellToCollapse = FindLowestEntropyCell();
                Collapse(cellToCollapse);
                if (!Propagate(cellToCollapse)) {
                    Reset();
                }

            }
            
        }
        
        private bool Propagate(Vector3Int cellToCollapse) {
            Vector3Int[] neighbors = GetNeighbors(cellToCollapse);
            if (AllConditionMatch(neighbors, cellToCollapse) ||
                neighbors.All(IsVisited)) {
                return true;
            }

            if (neighbors.Any(HasNoPossibleTile)) {
                return false;
            }

            for (int i = 0; i < neighbors.Length; i++) {
                TileGridCell cell = _grid[neighbors[i].x, neighbors[i].y, neighbors[i].z];
                cell.UpdatePossibleTilesFromCell(i, _dtb);
                cell.Visisted = true;
                _grid[neighbors[i].x, neighbors[i].y, neighbors[i].z] = cell;
                if (!Propagate(cell.GridPos)) {
                    return false;
                }
            }

            return true;
        }
        private bool HasNoPossibleTile(Vector3Int neighbor) {
            if (neighbor.x > 0 && neighbor.x < range) {
                if (neighbor.y > 0 && neighbor.y < range) {
                    if (neighbor.z > 0 && neighbor.z < range) {
                        return _grid[neighbor.x, neighbor.y, neighbor.z].PossibleTiles.Count <= 0;
                    }
                }
            }

            return false;
        }
        private bool IsVisited(Vector3Int neighbor) {
            if (neighbor.x > 0 && neighbor.x < range) {
                if (neighbor.y > 0 && neighbor.y < range) {
                    if (neighbor.z > 0 && neighbor.z < range) {
                        return _grid[neighbor.x, neighbor.y, neighbor.z].Visisted;
                    }
                }
            }

            return true;
        }

        private bool AllConditionMatch(Vector3Int[] neighbors, Vector3Int cellToCollapse) {
            List<Vector3Int> neigh = neighbors.ToList();
            foreach (Vector3Int neighbor in neigh) {
                if (neighbor.x > 0 && neighbor.x < range) {
                    if (neighbor.y > 0 && neighbor.y < range) {
                        if (neighbor.z > 0 && neighbor.z < range) {
                            foreach (TileStruct tile in _grid[neighbor.x, neighbor.y, neighbor.z].PossibleTiles) {
                                foreach (TileStruct tilet1 in _grid[cellToCollapse.x, cellToCollapse.y, cellToCollapse.z].PossibleTiles) {
                                    
                                    Debug.Log(tile.Faces[neigh.IndexOf(neighbor)]);
                                    Debug.Log(tilet1.Faces[neigh.IndexOf(neighbor)]);
                                    Debug.Log(_dtb.CheckNeighboor(
                                        tile.Faces[neigh.IndexOf(neighbor)],
                                        tilet1.Faces[neigh.IndexOf(neighbor)]));
                                    if (!_dtb.CheckNeighboor(
                                            tile.Faces[neigh.IndexOf(neighbor)],
                                            tilet1.Faces[neigh.IndexOf(neighbor)])) {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }
        private Vector3Int[] GetNeighbors(Vector3Int cellToCollapse) {
            Vector3Int[] neighbors = new[]
            {
                cellToCollapse + Vector3Int.right,
                cellToCollapse + Vector3Int.left,
                cellToCollapse + Vector3Int.up,
                cellToCollapse + Vector3Int.down,
                cellToCollapse + Vector3Int.forward,
                cellToCollapse + Vector3Int.back
            };
            return neighbors;
        }
        private void Collapse(Vector3Int cellToCollapse) {
            _grid[cellToCollapse.x, cellToCollapse.y, cellToCollapse.z].Collapse();
        }
        private Vector3Int FindLowestEntropyCell() {
            Vector3Int lowestEntropyIndex = new Vector3Int();
            int lowestEntropy = int.MaxValue;

            foreach (TileGridCell cell in _grid) {
                int cellEntropy = cell.PossibleTiles.Count;
                if (cellEntropy < lowestEntropy && cellEntropy > 0) {
                    lowestEntropy = cellEntropy;
                    lowestEntropyIndex = cell.GridPos;
                }
            }

            //TODO Aléatoire en cas d'égalité
            return lowestEntropyIndex;
        }

        private bool AllCellCollapsed() {
            foreach (TileGridCell cell in _grid) {
                if (!cell.Collapsed) return false;
            }

            return true;
        }
        private void Reset() {
            range = _gridScript.GridSize / _gridScript.CellSize;
            _grid = new TileGridCell[range, range, range];
            for (int i = 0; i < range; i++) {
                for (int j = 0; j < range; j++) {
                    for (int k = 0; k < range; k++) {
                        _grid[i, j, k] = new TileGridCell(_dtb.Tiles, new Vector3Int(i, j, k));
                    }
                }
            }
        }
    }
}