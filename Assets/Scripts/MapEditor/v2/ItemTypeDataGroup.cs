using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnionCollections.DataEditor;


namespace MapEditor {

    public class ItemTypeDataGroup : ScriptableObject, IEnumerable, IEnumerable<ItemTypeData>
    {
        [NodeElement]
        [SerializeField]
        ItemTypeData[] data = new ItemTypeData[0];


        public ItemTypeData GetTypeData(int type)
        {
            if (type < 0 || type > data.Length)
            {
                return null;
            }

            return data[type];
        }

        public IEnumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator<ItemTypeData> IEnumerable<ItemTypeData>.GetEnumerator()
        {
            return ((IEnumerable<ItemTypeData>)data).GetEnumerator();
        }
    }
}