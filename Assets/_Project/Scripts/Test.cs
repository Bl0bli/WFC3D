using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFC3D;

public class Test : MonoBehaviour
{
    [SerializeField] string tests;
    [SerializeField] string tests2;
    [SerializeField] Tile_Database TileDatabase;
    void Start()
    {
        Debug.Log(TileDatabase.CheckNeighboor(tests, tests2));
    }
}
