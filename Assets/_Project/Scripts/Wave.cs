using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WFC3D
{
    public class Wave : MonoBehaviour
    {
        [SerializeField] private Grid _gridScript;
        [SerializeField] private Tile_Database _dtb;

        private List<TileGridCell> _allCells;
        private TileGridCell[,,] _grid;

        private int _range;

        private void Start()
        {
            _allCells = new List<TileGridCell>();
            _range = _gridScript.GridSize;
            _grid = new TileGridCell[_range, _range, _range];
            SetGrid();
            WFC();
        }

        private void SetGrid()
        {
             int index = 0;
            for (int i = 0; i < _range; i++)
            {
                for (int j = 0; j < _range; j++)
                {
                    for (int k = 0; k < _range; k++)
                    {
                        _grid[i, j, k] = new TileGridCell(_dtb.Tiles, new Vector3Int(i, j, k), index);
                        _allCells.Add(_grid[i, j, k]);
                        index++;
                    }
                }
            }
        }

        private void WFC() {
            while (!_allCells.TrueForAll(cell => cell.Collapsed))
            {
               
                if (!Propagate(CollapseCell(SelectCellWithSmallestEntropy())))
                {
                    ResetAlgo();
                }
            }
            foreach (var tile in _allCells) {
                InstantiateObj(tile);
            }
        }

        private void ResetAlgo()
        {
            _allCells = new List<TileGridCell>();
            _range = _gridScript.GridSize;
            _grid = new TileGridCell[_range, _range, _range];
            SetGrid();
        }

        private TileGridCell SelectCellWithSmallestEntropy()
        {
            List<TileGridCell> uncollapsedCells = _allCells.Where(cell => !cell.Collapsed).ToList();

            int minEntropy = int.MaxValue;
            List<TileGridCell> smallestEntropyCells = new List<TileGridCell>();

            foreach (TileGridCell cell in uncollapsedCells)
            {
                if (cell.PossibleTiles.Count < minEntropy && cell.PossibleTiles.Count > 0)
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

            List<TileStruct> newPossibleTiles = new List<TileStruct>();
            newPossibleTiles.Add(chosenTile); //ajout ref
            int index = cellToCollapse.IndexInList;
            _allCells[index] = new TileGridCell(newPossibleTiles, cellToCollapse.GridPos, true); // On remplace l'ancienne cellule panr la nouvelle dans la liste
            _grid[cellToCollapse.GridPos.x, cellToCollapse.GridPos.y, cellToCollapse.GridPos.z] = new TileGridCell(newPossibleTiles, cellToCollapse.GridPos, true);
            return new TileGridCell(cellToCollapse); //return une ref
        }

        private void InstantiateObj(TileGridCell cellToInstantiate)
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
        }

        private bool Propagate(TileGridCell collapsedCell) {
            if (collapsedCell.PossibleTiles.Count == 0) return false;
            List<Vector3Int> directions = new List<Vector3Int>
            {
                new Vector3Int(1, 0, 0), //droite
                new Vector3Int(-1, 0, 0), //gauche
                new Vector3Int(0, 1, 0), //haut
                new Vector3Int(0, -1, 0), //bas
                new Vector3Int(0, 0, 1), //devant
                new Vector3Int(0, 0, -1) //derri�re
            };
            for (int i = 0; i < 6; i++)
            {
                Vector3Int neighborPos = new Vector3Int(collapsedCell.GridPos.x, collapsedCell.GridPos.y, collapsedCell.GridPos.z) + directions[i];
                if (!NeighborInGrid(neighborPos)) {
                    continue;
                }
                
                TileGridCell neighbor = _grid[neighborPos.x, neighborPos.y, neighborPos.z];
                if (neighbor.PossibleTiles.Count == 0)
                {
                    return false;
                }
                    
                bool changed = false;
                List<TileStruct> possibleTiles = new List<TileStruct>();
                    
                foreach (TileStruct tile in neighbor.PossibleTiles) {
                    if (collapsedCell.PossibleTiles[0].Neighboors.GetNeighborList(i).Contains(tile.Id)) {
                        possibleTiles.Add(tile);
                    }
                    else {
                        changed = true;
                    }
                }
                neighbor.PossibleTiles = possibleTiles;

                if (!changed) {
                    continue;
                }

                if (!Propagate(neighbor)) {
                    return false;
                }
            }
            return true;
        }

        private bool NeighborInGrid(Vector3Int pos) => pos.x >= 0 && pos.x < _range && pos.y >= 0 && pos.y < _range && pos.z >= 0 && pos.z < _range;
    }
}

