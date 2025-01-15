using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


namespace WFC3D
{
    [CreateAssetMenu(menuName = "DatabaseTile")]
    public class Tile_Database : ScriptableObject
    {
        public List<TileStruct> Tiles;

        public List<List<TileStruct>> AddTile(TileStruct tile)
        {
            foreach (TileStruct t in Tiles)
            {
                if (t.Mesh == tile.Mesh && t.Rotation == tile.Rotation)
                {
                    return null;
                }
                for(int i = 0; i<5; i++) 
                {
                    if(CheckNeighboor(t.Faces[i], tile.Faces[i]))
                    {
                        t.Neighboors[i].Add(tile);
                        tile.Neighboors[i].Add(t);
                    }
                }
            }
            Tiles.Add(tile);
            return tile.Neighboors;
        }

        public List<List<TileStruct>> RemoveTile (TileStruct tile)
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
                    if (CheckNeighboor(t.Faces[i], tile.Faces[i]))
                    {
                        t.Neighboors[i].Remove(tile);
                        tile.Neighboors[i].Remove(t);
                    }
                }
            }
            return null;
        }
        public List<TileStruct> GetRandomTile(List<TileStruct> _tiles)
        {
            int count = _tiles.Count;
            for(int i = 0; i < count - 2 ; i++)
            {
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
            prefixe1 = TileStruct.GetPrefixe(face1);
            prefixe2 = TileStruct.GetPrefixe(face2);
            Debug.Log("Prefixe: " + prefixe1);
            Debug.Log("Prefixe: " + prefixe2);
            chara1 = TileStruct.GetSuffixe(face1);
            chara2 = TileStruct.GetSuffixe(face2);
            Debug.Log("Suffixe: " + chara1);
            Debug.Log("Suffixe: " + chara2);
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
            Debug.Log("Index: " + num1);
            Debug.Log("Index: " + num2);

            return ((num1 == num2 && (chara1 == "s" && chara2 == "s")) || // SYMETRIQUE
                (num1 == num2 && (chara1 == "" || chara2 == "") && (chara1 == "f" || chara2 == "f")) || // ASYMETRIQUE
                (num1 < 0 && num2 < 0)); // BORD
        }
    }
}

