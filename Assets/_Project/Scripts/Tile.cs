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

        public static int GetIndex(string ID)
        {
            string digits = "";
            foreach (char c in ID)
            {
                if (char.IsDigit(c))
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
    }

}
