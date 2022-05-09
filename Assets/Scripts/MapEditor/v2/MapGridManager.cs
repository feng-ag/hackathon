using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{

    public class MapGridManager 
    {
        const int mapWidth = 100;
        const int mapHeight = 100;

        readonly ItemData[,] mapGrid = new ItemData[mapWidth, mapHeight];

        readonly List<ItemData> items = new List<ItemData>();



        public ItemData Query(Pos pos)
        {
            return mapGrid[pos.x, pos.y];
        }


        public bool IsEmbedValid(ItemData itemData, float rotation, Pos pos)
        {

            // TODO
            return false;


        }

    }

}