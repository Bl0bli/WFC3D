using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WFC3D
{
    public class Wave : MonoBehaviour
    {
        [SerializeField] Grid _gridScript;
        [SerializeField] Tile_Database _dtb;

        List<TileGridCell> _allCells;
        TileGridCell[,,] _grid;
        private bool[,,] _visited; 

        int range;

        private void Start()
        {
            _allCells = new List<TileGridCell>();
            range = _gridScript.GridSize;
            _grid = new TileGridCell[range, range, range];
            _visited = new bool[range, range, range];
            SetGrid();
            WFC();
        }

        void SetGrid()
        {
            for (int i = 0; i < range; i++)
            {
                for (int j = 0; j < range; j++)
                {
                    for (int k = 0; k < range; k++)
                    {
                        _grid[i, j, k] = new TileGridCell(_dtb.Tiles, new Vector3Int(i, j, k));
                        _allCells.Add(_grid[i, j, k]);
                    }
                }
            }
        }

        void WFC() {
            // while (!_allCells.TrueForAll(cell => cell.Collapsed))
            // {
            //     foreach (TileGridCell cell in _allCells) {
            //         foreach (TileStruct tile in cell.PossibleTiles) {
            //             Debug.Log(tile.Mesh.GetInstanceID());
            //         }
            //     }
            //     
            //     if (!Propagate(CollapseCell(SelectCellWithSmallestEntropy())))
            //     {
            //         ResetAlgo();
            //         Debug.Log("Retry");
            //     }
//
            //     _visited = new bool[range, range, range];
            // }
            for (int i = 0; i < 40; i++) {
                if (_allCells.TrueForAll(cell => cell.Collapsed)) {
                    Debug.Log("end");
                    break;
                }
                if (!Propagate(CollapseCell(SelectCellWithSmallestEntropy()))) {
                    ResetAlgo();
                    Debug.Log("Retry");
                }

                _visited = new bool[range, range, range];
                
            }
            foreach (var VARIABLE in _allCells) {
                InstantialeObj(VARIABLE);
            }
        }
        void ResetAlgo()
        {
            _allCells = new List<TileGridCell>();
            range = _gridScript.GridSize;
            _grid = new TileGridCell[range, range, range];
            SetGrid();
            /*foreach (TileGridCell cell in _allCells)
            {
                Debug.Log(cell.PossibleTiles.Count);
            }*/
        }
        TileGridCell SelectCellWithSmallestEntropy()
        {
            List<TileGridCell> _uncollapsedCells = new List<TileGridCell>();
            foreach (TileGridCell cell in _allCells)
            {
                if (!cell.Collapsed)
                {
                    _uncollapsedCells.Add(cell); //ajoute ref
                    //Debug.Log(cell.GridPos);
                }
            }

            int minEntropy = int.MaxValue;
            List<TileGridCell> _smallestEntropyCells = new List<TileGridCell>();

            foreach (TileGridCell cell in _uncollapsedCells)
            {
                if (cell.PossibleTiles.Count < minEntropy && cell.PossibleTiles.Count > 0)
                {
                    minEntropy = cell.PossibleTiles.Count;
                    _smallestEntropyCells.Clear();
                    _smallestEntropyCells.Add(cell); //ajoute ref
                }
                else if (cell.PossibleTiles.Count == minEntropy)
                {
                    _smallestEntropyCells.Add(cell);
                }
            }
            return _smallestEntropyCells[UnityEngine.Random.Range(0, _smallestEntropyCells.Count - 1)]; //là c une ref
        }

        TileGridCell CollapseCell(TileGridCell _cellToCollapse)
        {
            TileStruct _chosenTile = _cellToCollapse.PossibleTiles[UnityEngine.Random.Range(0, _cellToCollapse.PossibleTiles.Count - 1)]; //ref

            List<TileStruct> _newPossibleTiles = new List<TileStruct>();
            _newPossibleTiles.Add(_chosenTile); //ajout ref
            int index = _allCells.IndexOf(_cellToCollapse);
            _allCells[index] = new TileGridCell(_newPossibleTiles, _cellToCollapse.GridPos, true); // On remplace l'ancienne cellule par la nouvelle dans la liste
            _grid[_cellToCollapse.GridPos.x, _cellToCollapse.GridPos.y, _cellToCollapse.GridPos.z] = new TileGridCell(_newPossibleTiles, _cellToCollapse.GridPos, true);
            //InstantialeObj(new TileGridCell(_newPossibleTiles, _cellToCollapse.GridPos, true));
            return new TileGridCell(_cellToCollapse); //return une ref
        }

        void InstantialeObj(TileGridCell cellToInstantiate)

        {
            GameObject go = new GameObject();
            go.AddComponent<MeshFilter>().mesh = cellToInstantiate.PossibleTiles[0].Mesh;
            go.AddComponent<MeshRenderer>();
            go.transform.position = new Vector3
                (
                cellToInstantiate.GridPos.x * _gridScript.CellSize, 
                cellToInstantiate.GridPos.y * _gridScript.CellSize, 
                cellToInstantiate.GridPos.z * _gridScript.CellSize
                );
            go.transform.rotation = Quaternion.Euler(0, cellToInstantiate.PossibleTiles[0].Rotation * 90, 0);
        }
        
        bool Propagate(TileGridCell _collapsedCell) {
            if (_collapsedCell.PossibleTiles.Count <= 0) return false;
            List<Vector3Int> directions = new List<Vector3Int>
            {
                new Vector3Int(1, 0, 0), //droite
                new Vector3Int(-1, 0, 0), //gauche
                new Vector3Int(0, 1, 0), //haut
                new Vector3Int(0, -1, 0), //bas
                new Vector3Int(0, 0, 1), //devant
                new Vector3Int(0, 0, -1) //derri�re
            };
            //Debug.Log(_collapsedCell.PossibleTiles.Count);
            for (int i = 0; i < 6; i++)
            {
                Vector3Int neighborPos = new Vector3Int(_collapsedCell.GridPos.x, _collapsedCell.GridPos.y, _collapsedCell.GridPos.z) + directions[i];
                if (NeighborInGrid(neighborPos))
                {
                    //Debug.Log(_collapsedCell.PossibleTiles.Count);
                    TileGridCell neighbor = _grid[neighborPos.x, neighborPos.y, neighborPos.z];
                    if(!neighbor.Collapsed && !_visited[neighborPos.x, neighborPos.y, neighborPos.z]) {
                        _visited[neighborPos.x, neighborPos.y, neighborPos.z] = true;
                        bool _changed = false;
                        List<TileStruct> _possibleTiles_COPY = new List<TileStruct>(neighbor.PossibleTiles);

                        foreach (TileStruct tile in _possibleTiles_COPY) {

                            if (neighbor.PossibleTiles.Count == 0)
                            {
                                return false;
                            }
                            Debug.Log(_dtb.Tiles.IndexOf(tile) + " compared with " + _dtb.Tiles.IndexOf(_collapsedCell.PossibleTiles[0]) + "at " + i);
                            bool b = _dtb.CheckNeighboor(tile.Faces[i], _collapsedCell.PossibleTiles[0].Faces[GetOppositeFace(i)]);
                            Debug.Log(b);
                            if(!b)
                            {
                                //Debug.Log(_dtb.Tiles.IndexOf(tile));
                                neighbor.Remove(tile);
                                _changed = true;

                            }
                        }
                        _grid[neighborPos.x, neighborPos.y, neighborPos.z] = neighbor;
                        if (_changed)
                        {
                            if (!Propagate(neighbor)) {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        int GetOppositeFace(int face) {
            switch (face) {
                case 0: return 1;
                case 1: return 0;
                case 2: return 3;
                case 3: return 2;
                case 4: return 5;
                case 5: return 4;
                default: return -1;
            }
        }
        bool NeighborInGrid(Vector3Int pos) => pos.x >= 0 && pos.x < range && pos.y >= 0 && pos.y < range && pos.z >= 0 && pos.z < range;

    }
}

