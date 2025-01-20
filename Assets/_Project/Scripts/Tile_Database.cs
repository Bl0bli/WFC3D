using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.WSA;


namespace WFC3D
{
    [CreateAssetMenu(menuName = "DatabaseTile")]
    public class Tile_Database : ScriptableObject
    {
        public List<TileStruct> Tiles = new List<TileStruct>();
        public int ID = 0;

        //public List<TileStruct>[] AddTile(TileStruct tile)
        public void Reset()
        {
            Tiles.Clear();
            TileStruct emptyTile = new TileStruct(null, 0, "-1", "-1", "-1", "-1", "-1", "-1", 0);
            Tiles.Add(emptyTile);
            ID = 1;
        }
        public Neighbors AddTile(TileStruct tile)
        {
            TileStruct newTile = new TileStruct(tile);
            Tiles.Add(newTile);
            ID++;
            foreach (TileStruct t in Tiles) 
            {
                for(int i = 0; i<6; i++) 
                {
                    bool b = CheckNeighboor(t.Faces[i], newTile.Faces[GetOppositeFace(i)]);
                    Debug.Log("Check Neighboor : " + t.Id + " on Face " + i + " and " + newTile.Id + " on Face " + GetOppositeFace(i) + " : " + b);
                    if (b)
                    {
                        //t.Neighboors[i].Add(tile);
                        //tile.Neighboors[i].Add(t);
                       // if (!t.Neighboors.GetNeighbor(i).Contains(tile))
                       // {
                       //     t.Neighboors.GetNeighbor(i).Add(tile);
                       //     tile.Neighboors.GetNeighbor(i).Add(t);
                       // }

                        if (!t.Neighboors.GetNeighborList(i).Contains(newTile.Id))
                        {
                            t.Neighboors.GetNeighborList(i).Add(newTile.Id);
                            Debug.Log("Add Neighboor " + newTile.Id + " to " + t.Id + " on Face " + i);

                        }
                        if(!newTile.Neighboors.GetNeighborList(GetOppositeFace(i)).Contains(t.Id))
                        {
                            newTile.Neighboors.GetNeighborList(GetOppositeFace(i)).Add(t.Id);
                        }
                    }
                }
            }
            //Debug.Log("Add Tile " + tile.Rotation);
            //newTile.SetID(_id);
            //return tile.Neighboors;
            return newTile.Neighboors;
        }

        //public List<TileStruct>[] RemoveTile (TileStruct tile)
        public Neighbors RemoveTile(TileStruct tile)
        {
            foreach (TileStruct t in Tiles)
            {
                if (t.Mesh == tile.Mesh)
                {
                    Tiles.Remove(t);
                    return t.Neighboors;
                }
                for (int i = 0; i < 5; i++)
                {
                    if (CheckNeighboor(t.Faces[i], tile.Faces[GetOppositeFace(i)]))
                    {
                        //t.Neighboors[i].Remove(tile);
                        //tile.Neighboors[i].Remove(t);
                        //t.Neighboors.GetNeighbor(i).Remove(tile);
                        //tile.Neighboors.GetNeighbor(i).Remove(t);
                        t.Neighboors.GetNeighborList(i).Remove(tile.Id);
                        tile.Neighboors.GetNeighborList(GetOppositeFace(i)).Remove(t.Id);
                    }
                }
            }
            return null;
        }
        public List<TileStruct> GetRandomTile(List<TileStruct> _tiles)
        {
            while (_tiles.Count > 1)
            {
                int count = _tiles.Count;
                _tiles.RemoveAt(Random.Range(0, _tiles.Count - 1));
                
            }
            return _tiles;
        }

        public bool CheckNeighboor(string face1, string face2)
        {
            string chara1 = "";
            string chara2 = "";
            string prefixe1 = "";
            string prefixe2 = "";
            //foreach (char c in face1)
            //{
            //    if (char.IsLetter(c))
            //    {
            //        chara1 += c;
            //    }
            //}
            //foreach (char c in face2)
            //{
            //    if(char.IsLetter(c))
            //    {
            //        chara2 += c;
            //    }
            //}
           // Debug.Log("face1 " + face1);
           // Debug.Log("face2 " + face2);
            prefixe1 = TileStruct.GetPrefixe(face1);
            prefixe2 = TileStruct.GetPrefixe(face2);
           //Debug.Log("Prefixe: " + prefixe1);
           //Debug.Log("Prefixe: " + prefixe2);
            chara1 = TileStruct.GetSuffixe(face1);
            chara2 = TileStruct.GetSuffixe(face2);
           // Debug.Log("Suffixe: " + chara1);
           // Debug.Log("Suffixe: " + chara2);
            int num1 = 999;
            int num2 = 999;
            //if (chara1 != "")
            //{
            //    num1 = int.Parse(face1.Remove(face1.Length - 1));
            //}
            //else
            //{
            //    num1 = int.Parse(face1);
            //}
            //if (chara2 != "")
            //{
            //    num2 = int.Parse(face2.Remove(face2.Length - 1));
            //}
            //else
            //{
            //    num2 = int.Parse(face2);
            //}
            //
            //
            num1 = TileStruct.GetIndex(face1);
            num2 = TileStruct.GetIndex(face2);
            //Debug.Log("num1 " + num1);
            //Debug.Log("num2 " + num2);
            //
            return ((num1 == num2 && (chara1 == "s" && chara2 == "s")) || // SYMETRIQUE
                (num1 == num2 && (chara1 == "" || chara2 == "") && (chara1 == "f" || chara2 == "f")) || // ASYMETRIQUE
                (num1 < 0 && num2 < 0)); // BORD
        }
        int GetOppositeFace(int face)
        {
            switch (face)
            {
                case 0: return 1;
                case 1: return 0;
                case 2: return 3;
                case 3: return 2;
                case 4: return 5;
                case 5: return 4;
                default: return -1;
            }
        }
    }
}

