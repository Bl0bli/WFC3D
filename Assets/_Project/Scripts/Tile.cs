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

        [SerializeField] string[] _faces;                            // 0 = right, 1 = left, 2 = up, 3 = down, 4 = front, 5 = back

        [SerializeField] List<TileStruct>[] _neighboors;

        public Mesh Mesh { get => _mesh; }
        public string[] Faces { get => _faces; }
        public List<TileStruct>[] Neighboors { get => _neighboors; }
        public int Rotation { get => _rotation; }

        public TileStruct(Mesh mesh, int rotation, string px, string nx, string py, string ny, string pz, string nz)
        {
            _mesh = mesh;
            _faces = new string[] { px, nx, py, ny, pz, nz};
            _neighboors = new List<TileStruct>[]{ new List<TileStruct>(), new List<TileStruct>(), new List<TileStruct>(), new List<TileStruct>(), new List<TileStruct>(), new List<TileStruct>()};
            _rotation = rotation;
        }

        public void Rotate() {
            _rotation++;
            _rotation %= 4;

            string[] facesCopy = new[] { _faces[5], _faces[4], _faces[2], _faces[3], _faces[0], _faces[1] }; //Rotate faces;
            _faces = facesCopy;
        }

        public static int GetIndex(string ID) {
            string digits = "";
            foreach (char c in ID)
            {
                if (char.IsDigit(c) || c.ToString() == "-")
                {
                    digits += c;
                }
            }
            if(digits == "") return 0;
            return int.Parse(digits);
        }

        public static string GetSuffixe(string ID)
        {
            if (ID.Contains("s")) return "s";
            else if (ID.Contains("f")) return "f";
            for (int i = 0; i < ID.Length; i++)
            {
                char c = ID[i];
                if (c.ToString() == "_")
                {
                    return c + ID[i + 1].ToString();
                }
            }
            return "";
        }
        public static string GetPrefixe(string ID)
        {
            if (ID.Contains("v")) return "v";
            return "";
        }

        public static bool operator==(TileStruct a, TileStruct b) {
            return a._mesh == b._mesh && a.Rotation == b.Rotation;
        }
        public static bool operator !=(TileStruct a, TileStruct b) {
            return !(a == b);
        }

        public override bool Equals(object obj) {
            TileStruct tile = (TileStruct)obj;
            return this == tile;
        }
    }

}
