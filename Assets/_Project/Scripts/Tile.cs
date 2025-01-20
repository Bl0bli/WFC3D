using System;
using UnityEngine;
using System.Collections.Generic;

namespace WFC3D
{
    [Serializable]
    public class Neighbors
    {
        //[HideInInspector] public List<TileStruct> Right = new List<TileStruct>();
        //[HideInInspector] public List<TileStruct> Left = new List<TileStruct>();
        //[HideInInspector] public List<TileStruct> Up = new List<TileStruct>();
        //[HideInInspector] public List<TileStruct> Down = new List<TileStruct>();
        //[HideInInspector] public List<TileStruct> Front = new List<TileStruct>();
        //[HideInInspector] public List<TileStruct> Back = new List<TileStruct>();

        public List<int> i_Right = new List<int>();
        public List<int> i_Left = new List<int>();
        public List<int> i_Up = new List<int>();
        public List<int> i_Down = new List<int>();
        public List<int> i_Front = new List<int>();
        public List<int> i_Back = new List<int>();

        //public List<TileStruct> GetNeighbor(int face)
        //{
        //    switch (face)
        //    {
        //        case 0:
        //            return Right;
        //        case 1:
        //            return Left;
        //        case 2:
        //            return Up;
        //        case 3:
        //            return Down;
        //        case 4:
        //            return Front;
        //        case 5:
        //            return Back;
        //        default:
        //            return null;
        //    }
        //}

        public List<int> GetNeighborList(int face)
        {
            switch (face)
            {
                case 0:
                    return i_Right;
                case 1:
                    return i_Left;
                case 2:
                    return i_Up;
                case 3:
                    return i_Down;
                case 4:
                    return i_Front;
                case 5:
                    return i_Back;
                default:
                    return null;
            }
        }

    }
    [Serializable]
    public class TileStruct
    {
        [SerializeField] int _id;
        [VisibleInDebugInspector] Mesh _mesh;
        [VisibleInDebugInspector] int _rotation;

        [VisibleInDebugInspector] string[] _faces;                            // 0 = right, 1 = left, 2 = up, 3 = down, 4 = front, 5 = back

        //[SerializeField] List<TileStruct>[] _neighboors;
        [VisibleInDebugInspector] Neighbors _neighboors;

        public Mesh Mesh { get => _mesh; }
        public string[] Faces { get => _faces; }
        //public List<TileStruct>[] Neighboors { get => _neighboors; }
        public Neighbors Neighboors { get => _neighboors; }
        public int Rotation { get => _rotation; }
        public int Id { get => _id; }

        public TileStruct(Mesh mesh, int rotation, string px, string nx, string py, string ny, string pz, string nz, int id)
        {
            _mesh = mesh;
            _faces = new string[] { px, nx, py, ny, pz, nz};
            //_neighboors = new List<TileStruct>[]{ new List<TileStruct>(), new List<TileStruct>(), new List<TileStruct>(), new List<TileStruct>(), new List<TileStruct>(), new List<TileStruct>()};
            _neighboors = new Neighbors();
            _rotation = rotation;
            _id = id;
        }
        public TileStruct(TileStruct tileToClone)
        {
            _mesh = tileToClone.Mesh;
            _neighboors = new Neighbors();
            _faces = tileToClone.Faces;
            _rotation = tileToClone.Rotation;
            _id = tileToClone.Id;
        }

        public void SetID(int id)
        {
            _id = id;
        }
        public void Rotate(int ID) {
            _rotation++;
            _rotation %= 4;
            //Debug.Log("rotation " + _rotation);
            string[] facesCopy = new[] { _faces[5], _faces[4], _faces[2], _faces[3], _faces[0], _faces[1] }; //Rotate faces;
            _faces = facesCopy;
            _id = ID;
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

        public override int GetHashCode()
        {
            return HashCode.Combine(_mesh, _rotation, _faces, _neighboors, Mesh, Faces, Neighboors, Rotation);
        }
    }

}
