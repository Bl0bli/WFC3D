using System;
using UnityEngine;
using System.Collections.Generic;

namespace WFC3D
{
    [Serializable]
    public struct TileStruct
    {
        [SerializeField] Mesh _mesh;
        [SerializeField] int _rotation;

        [SerializeField] string[] _faces;

        [SerializeField] List<List<TileStruct>> _neighboors;
        public Mesh Mesh { get => _mesh; }
        public string[] Faces { get => _faces; }
        public List<List<TileStruct>> Neighboors { get => _neighboors; }
        public int Rotation { get => _rotation; }

        TileStruct(Mesh mesh, int rotation)
        {
            _mesh = mesh;
            _faces = new string[6];
            _neighboors = new List<List<TileStruct>>();
            _rotation = rotation;    
        }
    }
   
}
