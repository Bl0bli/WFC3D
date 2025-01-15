using UnityEngine;
using UnityEditor;

public class Grid : MonoBehaviour
{
    [SerializeField] int _cellSize = 25;
    [SerializeField] int _gridSize = 50;

    public int CellSize { get => _cellSize; set => _cellSize = value; }
    public int GridSize { get => _gridSize; set => _gridSize = value; }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        for (int j = 0; j <= _gridSize / _cellSize; j++)
        {
            for (int i = 0; i <= _gridSize / _cellSize; i++)
            {
                Handles.DrawAAPolyLine(new Vector3((_gridSize / 2) * -1, (j - (_gridSize / _cellSize) / 2) * _cellSize, (i - (_gridSize / _cellSize) / 2) * _cellSize), new Vector3(_gridSize / 2, (j - (_gridSize / _cellSize) / 2) * _cellSize, (i - (_gridSize / _cellSize) / 2) * _cellSize));
                Handles.DrawAAPolyLine(new Vector3((i - (_gridSize / _cellSize) / 2) * _cellSize, (j - (_gridSize / _cellSize) / 2) * _cellSize, (_gridSize / 2) * -1), new Vector3((i - (_gridSize / _cellSize) / 2) * _cellSize, (j - (_gridSize / _cellSize) / 2) * _cellSize, _gridSize / 2));
            }
            for (int k = 0; k <= _gridSize / _cellSize; k++)
            {
                Handles.DrawAAPolyLine(new Vector3((_gridSize / 2) * -1, (k - (_gridSize / _cellSize) / 2) * _cellSize, (j - (_gridSize / _cellSize) / 2) * _cellSize), new Vector3(_gridSize / 2, (k - (_gridSize / _cellSize) / 2) * _cellSize, (j - (_gridSize / _cellSize) / 2) * _cellSize));
                Handles.DrawAAPolyLine(new Vector3((k - (_gridSize / _cellSize) / 2) * _cellSize, (_gridSize / 2) * -1, (j - (_gridSize / _cellSize) / 2) * _cellSize), new Vector3((k - (_gridSize / _cellSize) / 2) * _cellSize, _gridSize / 2, (j - (_gridSize / _cellSize) / 2) * _cellSize));
            }
        }

    }
}
