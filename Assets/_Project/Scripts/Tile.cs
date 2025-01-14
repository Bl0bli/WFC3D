using System;
using UnityEngine;

namespace WFC3D
{
    [Serializable]
    public struct Tile
    {
        [field: SerializeField] public Mesh Mesh { get; private set; }
        [field: SerializeField] public string Px { get; private set; }
        [field: SerializeField] public string Nx { get; private set; }
        [field: SerializeField] public string Py { get; private set; }
        [field: SerializeField] public string Ny { get; private set; }
        [field: SerializeField] public string Pz { get; private set; }
        [field: SerializeField] public string Nz { get; private set; }

        Tile(Mesh mesh, string px, string nx, string py, string ny, string pz, string nz)
        {
            Mesh = mesh;
            Px = px;
            Nx = nx;
            Py = py;
            Ny = ny;
            Pz = pz;
            Nz = nz;
        }
    }
}
