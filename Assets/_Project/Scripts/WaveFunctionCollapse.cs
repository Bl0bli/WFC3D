using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WFC3D
{
    public class TileGridCell : MonoBehaviour
    {
        public List<TileStruct> PossibleTiles;
        public Vector3Int GridPos;
        public bool Collapsed;
        public bool Visisted;
        public int IndexInList;
        
        public TileGridCell(List<TileStruct> possibleTiles, Vector3Int gridPos)
        {
            PossibleTiles = new List<TileStruct>(possibleTiles);
            GridPos = gridPos;
            Visisted = false;
            Collapsed = false;
        }
        public TileGridCell(List<TileStruct> possibleTiles, Vector3Int gridPos, int index)
        {
            PossibleTiles = new List<TileStruct>(possibleTiles);
            GridPos = gridPos;
            Visisted = false;
            Collapsed = false;
            IndexInList = index;
        }
        public TileGridCell(List<TileStruct> possibleTiles, Vector3Int gridPos, bool collapsed)
        {
            PossibleTiles = new List<TileStruct>(possibleTiles);
            GridPos = gridPos;
            Visisted = false;
            Collapsed = collapsed;
        }
        public TileGridCell(bool state)
        {
            PossibleTiles = new List<TileStruct>();
            GridPos = new Vector3Int();
            Visisted = false;
            Collapsed = false;
        }
        public TileGridCell(TileGridCell celltoCopy)
        {
            PossibleTiles = new List<TileStruct>(celltoCopy.PossibleTiles);
            GridPos = new Vector3Int(celltoCopy.GridPos.x, celltoCopy.GridPos.y, celltoCopy.GridPos.z);
            Visisted = false;
            Collapsed = false;
        }
        public TileGridCell(List<TileStruct> possibleTiles) {
            PossibleTiles = new List<TileStruct>(possibleTiles);
            GridPos = new Vector3Int(0, 0, 0);
            Visisted = false;
            Collapsed = false;
        }

        public void Collapse() {
            int rnd = Random.Range(0, PossibleTiles.Count - 1);
            TileStruct selectedTile = PossibleTiles[rnd];
            PossibleTiles = new List<TileStruct>();
            PossibleTiles.Add(selectedTile);
            Collapsed = true;
        }
        
        public void UpdatePossibleTilesFromCell(int direction, Tile_Database tileDatabase)
        {
            List<TileStruct> _p = new List<TileStruct>(PossibleTiles);
            foreach (TileStruct t in _p)
            {
                if (!tileDatabase.CheckNeighboor(t.Faces[direction], PossibleTiles[0].Faces[direction]))
                {
                    PossibleTiles.Remove(t);
                }
            }
        }
        public void Remove(TileStruct tile) {
            //List<TileStruct> newPossibleTile = new List<TileStruct>();
            //foreach (TileStruct possibleTile in PossibleTiles) {
            //    if (possibleTile != tile) {
            //        newPossibleTile.Add(possibleTile);
            //    }
            //}
            //
            //PossibleTiles = newPossibleTile;
            PossibleTiles.Remove(tile);
        }
    }
}
    /*public class WaveFunctionCollapse : MonoBehaviour
    {
        [SerializeField] Tile_Database _tileDatabase;
        [SerializeField] List<TileGridCell> _CellCollapsed;
        [SerializeField] Grid _grid;
        [SerializeField] int _neighboors = 4;

        TileGridCell[,,] _gridData;
        TileGridCell _current;
        bool finished = false;
        bool next = false;
        int rangeMAX;
        List<TileStruct> _tiles;

        void Start()
        {
            _tiles = new List<TileStruct>(_tileDatabase.Tiles);
            _CellCollapsed = new List<TileGridCell>();
            rangeMAX = _grid.GridSize / _grid.CellSize;
            //Debug.Log("RANGEMAX = " + rangeMAX);
            _gridData = new TileGridCell[rangeMAX, rangeMAX, rangeMAX];
            for (int i = 0; i < rangeMAX; i++) // On pr� rempli le table de donn�e de la grille par d�faut
            {
                for (int j = 0; j < rangeMAX; j++)
                {
                    for (int k = 0; k < rangeMAX; k++)
                    {
                        //Debug.Log(i + " " + j + " " + k);
                        _gridData[i, j, k] = new TileGridCell(_tiles, new Vector3Int(i,j,k));
                    }
                }
            }
            int c = 0;
            while(!WaveFuncCollapse())
            {
                c++;
                Debug.Log("try: " + c);
            }
        }

        bool WaveFuncCollapse()
        {

            int x = UnityEngine.Random.Range(0, rangeMAX);
            int y = UnityEngine.Random.Range(0, rangeMAX);
            int z = UnityEngine.Random.Range(0, rangeMAX);

            _current = _gridData[x, y, z];
            _current.PossibleTiles = new List<TileStruct>(_tileDatabase.GetRandomTile(_gridData[x, y, z].PossibleTiles));

            List<TileGridCell> _neighboorCells = GetNeighboors();
            Propagate();

            while (_CellCollapsed.Count - 2 < (_grid.GridSize / _grid.CellSize) * (_grid.GridSize / _grid.CellSize) * (_grid.GridSize / _grid.CellSize))
            {
                _neighboorCells = GetNeighboors();

                int minIndex = 0;
                TileGridCell minEnthropyCell = new TileGridCell(_neighboorCells[minIndex]);

               for(int i = 0; i < _neighboorCells.Count; i++)
                {
                    //Debug.Log("countce " + cell.PossibleTiles.Count);
                    if (_neighboorCells[i].Collapsed) continue;

                    

                    if (_neighboorCells[i].PossibleTiles.Count < minEnthropyCell.PossibleTiles.Count && _neighboorCells[i].PossibleTiles.Count > 0)
                    {
                        minIndex = i;
                        minEnthropyCell = _neighboorCells[i];
                    }
                }
                Debug.Log(_current.PossibleTiles.Count + " " + minEnthropyCell.PossibleTiles.Count + " " + minIndex);
                for (int i = 0; i < _neighboorCells.Count; i++)
                {
                    _neighboorCells[i].UpdatePossibleTilesFromCell(_neighboorCells.IndexOf(_neighboorCells[i]), _tileDatabase);
                }
                _current = CollapseCell(_current.PossibleTiles[0], minEnthropyCell, minIndex);
                Propagate();

                if (_current.PossibleTiles.Count == 0)
                {
                    Debug.Log("No possible tiles for this cell");
                    return false;
                }
            }

            return true;
        }

        TileGridCell CollapseCell(TileStruct currentTile, TileGridCell CellToCollapse, int direction)
        {
            if (CellToCollapse.Collapsed)
            {
                Debug.Log(" D�j� Collapsed");
                return new TileGridCell(new List<TileStruct>(), new Vector3Int());
            }

            List<TileStruct> possibleTiles = new List<TileStruct>();
            //Debug.Log("Sizecelltocoll: " + CellToCollapse.PossibleTiles.Count);
            foreach ( TileStruct t in CellToCollapse.PossibleTiles)
            {
                Debug.Log("Check: " + t.Mesh.name + t.Faces);
                if (_tileDatabase.CheckNeighboor(t.Faces[direction], currentTile.Faces[direction]))
                {
                    possibleTiles.Add(t);
                    Debug.Log("Add: " + t.Mesh.name);
                }
            }

            CellToCollapse.PossibleTiles = new List<TileStruct>(possibleTiles);
            CellToCollapse.Collapsed = true;
            if (possibleTiles.Count <= 0) 
            {
                Debug.Log("Pas de Tile Possible, Non Instanci�");
                return new TileGridCell(new List<TileStruct>(), new Vector3Int());
            }
            GameObject go = new GameObject();
            go.AddComponent<MeshFilter>().mesh = CellToCollapse.PossibleTiles[0].Mesh;
            go.AddComponent<MeshRenderer>();
            go.transform.position = CellToCollapse.GridPos * _grid.CellSize;    
            //Instantiate(go, CellToCollapse.GridPos * _grid.CellSize, Quaternion.identity);
            Debug.Log("Instantiate");
            _CellCollapsed.Add(CellToCollapse);
            return CellToCollapse;
        }

        void Propagate()
        {
            Queue<TileGridCell> cellsToUpdate = new Queue<TileGridCell>();
            cellsToUpdate.Enqueue(_current);

            while (cellsToUpdate.Count > 0)
            {
                TileGridCell cell = cellsToUpdate.Dequeue();
                List<TileGridCell> neighboors = GetNeighboors();
                foreach (TileGridCell n in neighboors)
                {
                    if (n.Collapsed) continue;
                    int initialCount = n.PossibleTiles.Count;
                    n.UpdatePossibleTilesFromCell(neighboors.IndexOf(n), _tileDatabase);

                    if (n.PossibleTiles.Count != initialCount)
                    {
                        cellsToUpdate.Enqueue(n);
                    }
                }
            }
        }

            List<TileGridCell> GetNeighboors()
        {

            List<TileGridCell> neighboorCells = new List<TileGridCell>();

            TryAddElement(neighboorCells, _current.GridPos.x + 1, _current.GridPos.y, _current.GridPos.z, rangeMAX);
            TryAddElement(neighboorCells, _current.GridPos.x - 1, _current.GridPos.y, _current.GridPos.z, rangeMAX);
            TryAddElement(neighboorCells, _current.GridPos.x, _current.GridPos.y + 1, _current.GridPos.z, rangeMAX);
            TryAddElement(neighboorCells, _current.GridPos.x, _current.GridPos.y - 1, _current.GridPos.z, rangeMAX);
            TryAddElement(neighboorCells, _current.GridPos.x, _current.GridPos.y, _current.GridPos.z + 1, rangeMAX);
            TryAddElement(neighboorCells, _current.GridPos.x, _current.GridPos.y, _current.GridPos.z - 1, rangeMAX);

            return neighboorCells;
        }

        bool TryAddElement(List<TileGridCell> list, int i1, int i2, int i3, int arraySize)
        {
            if ((i1 < arraySize && i1 >= 0) && (i2 < arraySize && i2 >= 0) && (i3 < arraySize && i3 >= 0))
            {
                //Debug.Log(i1 + " " + i2 + " " + i3 + "Range " + arraySize);
                list.Add(_gridData[i1, i2, i3]);
                return true;
            }
            else
            {
                list.Add(new TileGridCell(false));
            }
            return false;
        }
    }
}*/

