using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFC3D
{
    public struct TileGridCell
    {
        public List<TileStruct> PossibleTiles;
        public Vector3Int GridPos;
        public bool Collapsed;

        public TileGridCell(List<TileStruct> possibleTiles, Vector3Int gridPos)
        {
            PossibleTiles = possibleTiles;
            GridPos = gridPos;
            Collapsed = false;
        }
    }
        public class WaveFunctionCollapse : MonoBehaviour
    {
        [SerializeField] Tile_Database _tileDatabase;
        [SerializeField] List<TileGridCell> _CellCollapsed;
        [SerializeField] Grid _grid;
        [SerializeField] int _neighboors = 4;

        TileGridCell[][][] _gridData;
        TileGridCell _current;
        bool finished = false;
        int rangeMAX;

        void Start()
        {
            rangeMAX = _grid.GridSize / _grid.CellSize;
            for (int i = 0; i < _grid.GridSize / rangeMAX; i++) // On pré rempli le table de donnée de la grille par défaut
            {
                for (int j = 0; j < _grid.GridSize / rangeMAX; j++)
                {
                    for (int k = 0; k < _grid.GridSize / rangeMAX; k++)
                    {
                        _gridData[i][j][k] = new TileGridCell(_tileDatabase.Tiles, new Vector3Int(i,j,k));
                    }
                }
            }
            while(!WaveFuncCollapse())
            {
            }
        }

        bool WaveFuncCollapse()
        {

            int x = Random.Range(0, rangeMAX);
            int y = Random.Range(0, rangeMAX);
            int z = Random.Range(0, rangeMAX);

            _current = _gridData[x][y][z];
            _current.PossibleTiles = _tileDatabase.GetRandomTile(_gridData[x][y][z].PossibleTiles);

            List<TileGridCell> _neighboorCells = GetNeighboors();

            //Instantiate(_gridData[x][y][z].PossibleTiles[0].Mesh, new Vector3(x * _grid.GridSize, y * _grid.GridSize, z * _grid.GridSize), Quaternion.identity);

            while (_CellCollapsed.Count - 1 < _grid.GridSize / _grid.CellSize)
            {
                _neighboorCells = GetNeighboors();


                TileGridCell minEnthropyCell = _neighboorCells[0];
                foreach(TileGridCell cell in _neighboorCells)
                {
                    if(cell.Collapsed) continue;

                    UpdatePossibleTilesFromCell(cell.PossibleTiles, _current, _neighboorCells.IndexOf(cell));

                    if (cell.PossibleTiles.Count < minEnthropyCell.PossibleTiles.Count)
                    {
                        minEnthropyCell = cell;
                    }
                }
                
                _current = CollapseCell(_current.PossibleTiles[0], minEnthropyCell, _neighboorCells.IndexOf(minEnthropyCell));

                if(_current.PossibleTiles.Count == 0)
                {
                    Debug.Log("No possible tiles for this cell");
                    return false;
                }
            }

            return true;
        }

        TileGridCell CollapseCell(TileStruct currentTile, TileGridCell CellToCollapse, int direction)
        {
            if(CellToCollapse.Collapsed) return new TileGridCell();

            List<TileStruct> possibleTiles = new List<TileStruct>();

            foreach ( TileStruct t in CellToCollapse.PossibleTiles)
            {
                if (_tileDatabase.CheckNeighboor(t.Faces[direction], currentTile.Faces[direction]))
                {
                    possibleTiles.Add(t);
                }
            }

            CellToCollapse.PossibleTiles = possibleTiles;
            CellToCollapse.Collapsed = true;
            Instantiate(CellToCollapse.PossibleTiles[0].Mesh, CellToCollapse.GridPos, Quaternion.identity);
            _CellCollapsed.Add(CellToCollapse);

            return CellToCollapse;
        }
        void UpdatePossibleTilesFromCell(List<TileStruct> PossibleTiles, TileGridCell Cell, int direction)
        {
            foreach (TileStruct t in PossibleTiles)
            {
                if (!_tileDatabase.CheckNeighboor(t.Faces[direction], Cell.PossibleTiles[0].Faces[direction]))
                {
                    PossibleTiles.Remove(t);
                }
            }
        }
        List<TileGridCell> GetNeighboors()
        {

            List<TileGridCell> neighboorCells = new List<TileGridCell>();

            neighboorCells.Add(_gridData[_current.GridPos.x + 1][_current.GridPos.y][_current.GridPos.z]);
            neighboorCells.Add(_gridData[_current.GridPos.x - 1][_current.GridPos.y][_current.GridPos.z]);
            neighboorCells.Add(_gridData[_current.GridPos.x][_current.GridPos.y + 1][_current.GridPos.z]);
            neighboorCells.Add(_gridData[_current.GridPos.x][_current.GridPos.y - 1][_current.GridPos.z]);
            neighboorCells.Add(_gridData[_current.GridPos.x][_current.GridPos.y][_current.GridPos.z + 1]);
            neighboorCells.Add(_gridData[_current.GridPos.x][_current.GridPos.y][_current.GridPos.z - 1]);

            return neighboorCells;
        }
    }
}

