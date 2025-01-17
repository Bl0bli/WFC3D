using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFC3D
{
    public class Wave : MonoBehaviour
    {
        [SerializeField] Grid _gridScript;
        [SerializeField] Tile_Database _dtb;

        List<TileGridCell> _allCells;
        TileGridCell[,,] _grid;

        int range;

        private void Start()
        {
            _allCells = new List<TileGridCell>();
            range = _gridScript.GridSize / _gridScript.CellSize;
            _grid = new TileGridCell[range, range, range];
            SetGrid();
            //for(int i = 0; i < range  * range * range; i++) 
            //{
            //    Propagate(CollapseCell(SelectCellWithSmallestEntropy()));
            //    foreach (TileGridCell cell in _allCells)
            //    {
            //        Debug.Log(cell.Collapsed);
            //    }
            //}
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

        void WFC()
        {
            while (!_allCells.TrueForAll(cell => cell.Collapsed))
            {
                if (!Propagate(CollapseCell(SelectCellWithSmallestEntropy())))
                {
                    ResetAlgo();
                    Debug.Log("Retry");
                }
            }
        }
        void ResetAlgo()
        {
            _allCells = new List<TileGridCell>();
            range = _gridScript.GridSize / _gridScript.CellSize;
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
                    _uncollapsedCells.Add(cell);
                    //Debug.Log(cell.GridPos);
                }
            }

            int minEntropy = int.MaxValue;
            List<TileGridCell> _smallestEntropyCells = new List<TileGridCell>();

            foreach (TileGridCell cell in _uncollapsedCells)
            {
                if (cell.PossibleTiles.Count < minEntropy)
                {
                    minEntropy = cell.PossibleTiles.Count;
                    _smallestEntropyCells.Clear();
                    _smallestEntropyCells.Add(cell);
                }
                else if (cell.PossibleTiles.Count == minEntropy)
                {
                    _smallestEntropyCells.Add(cell);
                }
            }
            return _smallestEntropyCells[UnityEngine.Random.Range(0, _smallestEntropyCells.Count - 1)];
        }

        TileGridCell CollapseCell(TileGridCell _cellToCollapse)
        {
            TileStruct _chosenTile = _cellToCollapse.PossibleTiles[UnityEngine.Random.Range(0, _cellToCollapse.PossibleTiles.Count - 1)];

            List<TileStruct> _newPossibleTiles = new List<TileStruct>();
            _newPossibleTiles.Add(_chosenTile);
            int index = _allCells.IndexOf(_cellToCollapse);
            _allCells[index] = new TileGridCell(_newPossibleTiles, _cellToCollapse.GridPos, true); // On remplace l'ancienne cellule par la nouvelle dans la liste

            InstantialeObj(_cellToCollapse);
            return _cellToCollapse;
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
        
        bool Propagate(TileGridCell _collapsedCell)
        {
            List<Vector3Int> directions = new List<Vector3Int>
            {
                new Vector3Int(1, 0, 0), //droite
                new Vector3Int(-1, 0, 0), //gauche
                new Vector3Int(0, 1, 0), //haut
                new Vector3Int(0, -1, 0), //bas
                new Vector3Int(0, 0, 1), //devant
                new Vector3Int(0, 0, -1) //derrière
            };

            for (int i = 0; i < directions.Count; i++)
            {
                Vector3Int neighborPos = _collapsedCell.GridPos + directions[i];
                if (NeighborInGrid(neighborPos))
                {
                    TileGridCell neighbor = _grid[neighborPos.x, neighborPos.y, neighborPos.z];
                    if(!neighbor.Collapsed)
                    {
                        bool _changed = false;
                        List<TileStruct> _possibleTiles_COPY = new List<TileStruct>(neighbor.PossibleTiles);

                        foreach (TileStruct tile in _possibleTiles_COPY)
                        {
                            if(!_dtb.CheckNeighboor(tile.Faces[i], _collapsedCell.PossibleTiles[0].Faces[i]))
                            {
                                neighbor.PossibleTiles.Remove(tile);
                                _changed = true;
                                Debug.Log("removed");
                            }
                        }
                        if (neighbor.PossibleTiles.Count == 0)
                        {
                            return false;
                        }
                        if (_changed)
                        {
                            Propagate(neighbor);
                        }
                    }
                }
            }
            return true;
        }

        bool NeighborInGrid(Vector3Int pos) => pos.x >= 0 && pos.x < range && pos.y >= 0 && pos.y < range && pos.z >= 0 && pos.z < range;

    }
}

