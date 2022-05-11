using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{

    public class MapStructManager 
    {

        static MapStructManager _Instance;
        public static MapStructManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new MapStructManager();
                }
                return _Instance;
            }
        }

        readonly List<ItemData> items = new List<ItemData>();


        public void AddItem(ItemData itemData)
        {
            items.Add(itemData);
        }


        public void RemoveItem(ItemData itemData)
        {
            items.Remove(itemData);
        }




    }

}