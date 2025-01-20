using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WFC3D
{
    public class Wave : MonoBehaviour
    {
        [SerializeField] private Grid _gridScript;
        [SerializeField] public Tile_Database _dtb;

        public event Action<GameObject[]> InstanciatedTilesEvent;

        #region Grid
        private List<TileGridCell> _allCells;
        private TileGridCell[,,] _grid;

        private List<TileGridCell> _DefaultallCells;
        private TileGridCell[,,] _Defaultgrid;
        #endregion

        #region Propriétés locales
        List<TileGridCell> smallestEntropyCells = new List<TileGridCell>();
        List<TileStruct> newPossibleTiles = new List<TileStruct>();
        List<TileStruct> propagatePossibleTiles = new List<TileStruct>();
        TileGridCell neighbor = new TileGridCell(false);
        Vector3Int neighborPos = new Vector3Int(0, 0, 0);


        private Vector3Int _range;
        Vector3Int[] directions = 
            {
                new Vector3Int(1, 0, 0), //droite
                new Vector3Int(-1, 0, 0), //gauche
                new Vector3Int(0, 1, 0), //haut
                new Vector3Int(0, -1, 0), //bas
                new Vector3Int(0, 0, 1), //devant
                new Vector3Int(0, 0, -1) //derri�re
            };
        #endregion//Ces propriétés pourrait être déclarées localement mais on évite de le faire pour ne pas faire de l'allocation mémoire à chaque itération

        private void Setup()
        {
            _DefaultallCells = new List<TileGridCell>();
            _allCells = new List<TileGridCell>();
            _range = _gridScript.GridSize;
            _grid = new TileGridCell[_range.x, _range.y, _range.z];
            _Defaultgrid = new TileGridCell[_range.x, _range.y, _range.z];
            SetGrid();
        }
        
        private void SetGrid()
        {
             int index = 0;
            for (int i = 0; i < _range.x; i++)
            {
                for (int j = 0; j < _range.y; j++)
                {
                    for (int k = 0; k < _range.z; k++)
                    {
                        _grid[i, j, k] = new TileGridCell(_dtb.Tiles, new Vector3Int(i, j, k), index);
                        _Defaultgrid[i, j, k] = new TileGridCell(_dtb.Tiles, new Vector3Int(i, j, k), index);

                        _allCells.Add(_grid[i, j, k]);
                        _DefaultallCells.Add(_Defaultgrid[i, j, k]);
                        index++;
                    }
                }
            }
        }

        public void WFC() {
            Setup();
            while (!_allCells.TrueForAll(cell => cell.Collapsed))
            {
                if (!Propagate(CollapseCell(SelectCellWithSmallestEntropy())))
                {
                    ResetAlgo();
                }
            }
            List<GameObject> gos = new List<GameObject>();
            foreach (var tile in _allCells) {
                gos.Add(InstantiateObj(tile));
            }
            InstanciatedTilesEvent?.Invoke(gos.ToArray());
        }

        private void ResetAlgo()
        {
            _allCells = new List<TileGridCell>();
            _range = _gridScript.GridSize;
            _grid = new TileGridCell[_range.x, _range.y, _range.z];
            SetGrid();
        }

        private TileGridCell SelectCellWithSmallestEntropy()
        {
            List<TileGridCell> uncollapsedCells = _allCells.Where(cell => !cell.Collapsed).ToList();

            int minEntropy = int.MaxValue;
            smallestEntropyCells.Clear();

            for( int i = 0; i < uncollapsedCells.Count; i++)
            {
                TileGridCell cell = uncollapsedCells[i];
                if(cell.PossibleTiles.Count < minEntropy && cell.PossibleTiles.Count > 0)
                {
                    minEntropy = cell.PossibleTiles.Count;
                    smallestEntropyCells.Clear();
                    smallestEntropyCells.Add(cell);
                }
                else if (cell.PossibleTiles.Count == minEntropy)
                {
                    smallestEntropyCells.Add(cell);
                }
            }
            return smallestEntropyCells[UnityEngine.Random.Range(0, smallestEntropyCells.Count - 1)]; //là c une ref
        }

        private TileGridCell CollapseCell(TileGridCell cellToCollapse)
        {
            TileStruct chosenTile = cellToCollapse.PossibleTiles[UnityEngine.Random.Range(0, cellToCollapse.PossibleTiles.Count - 1)]; //ref

            newPossibleTiles.Clear();
            newPossibleTiles.Add(chosenTile); //ajout ref
            int index = cellToCollapse.IndexInList;
            TileGridCell newCell = new TileGridCell(newPossibleTiles, cellToCollapse.GridPos, true);
            _allCells[index] = newCell; // On remplace l'ancienne cellule panr la nouvelle dans la liste
            _grid[cellToCollapse.GridPos.x, cellToCollapse.GridPos.y, cellToCollapse.GridPos.z] = new TileGridCell(newPossibleTiles, cellToCollapse.GridPos, true);
            return new TileGridCell(cellToCollapse); //return une ref
        }

        private GameObject InstantiateObj(TileGridCell cellToInstantiate)
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
            go.transform.rotation = Quaternion.Euler(0, cellToInstantiate.PossibleTiles[0].Rotation * -90, 0);
            return go;
        }

        private bool Propagate(TileGridCell collapsedCell) {
            if (collapsedCell.PossibleTiles.Count == 0) return false;

            for (int i = 0; i < 6; i++)
            {
                neighborPos = collapsedCell.GridPos + directions[i];
                if (!NeighborInGrid(neighborPos)) {
                    continue;
                }
                
                neighbor = _grid[neighborPos.x, neighborPos.y, neighborPos.z];
                if (neighbor.PossibleTiles.Count == 0)
                {
                    return false;
                }
                    
                bool changed = false;
                propagatePossibleTiles.Clear();

                for (int j = 0; j < neighbor.PossibleTiles.Count; j++)
                {
                    if (collapsedCell.PossibleTiles[0].Neighboors.GetNeighborList(i).Contains(neighbor.PossibleTiles[j].Id))
                    {
                        propagatePossibleTiles.Add(neighbor.PossibleTiles[j]);
                    }
                    else
                    {
                        changed = true;
                    }
                }
                neighbor.PossibleTiles = new List<TileStruct>(propagatePossibleTiles);

                if (!changed) {
                    continue;
                }

                if (!Propagate(neighbor)) {
                    return false;
                }
            }
            return true;
        }

        private bool NeighborInGrid(Vector3Int pos) => (pos.x >= 0) && (pos.x < _range.x) && (pos.y >= 0) &&
                                                       (pos.y < _range.y) && (pos.z >= 0) && (pos.z < _range.z);
    }
}

